using Microsoft.AspNetCore.Mvc;
using SyllabusApp.Interfaces;
using SyllabusApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SyllabusApp.Controllers
{
    public class SyllabusController : Controller
    {
        private readonly ISyllabusService _service;
        private readonly INotificationService _notifier;
        private readonly ISubscriptionService _subscriptionService;
        
        // Note: Static user is a simplification for this demo.
        // In production, use session-based authentication.
        private static User _currentUser = null;
        private static UserInfo _currentUserInfo = null;

        public SyllabusController(
            ISyllabusService service, 
            INotificationService notifier,
            ISubscriptionService subscriptionService)
        {
            _service = service;
            _notifier = notifier;
            _subscriptionService = subscriptionService;
        }

        #region Authentication (OAuth Simulation)

        public IActionResult Login() => View();
        
        /// <summary>
        /// OAuth/EKOID Login Simulation
        /// Validates EKOID against the fake user database.
        /// </summary>
        [HttpPost]
        public IActionResult Login(string ekoid, string password)
        {
            _service.EnsureSeedData();
            
            // Validate EKOID using FakeUserDatabase (OAuth simulation)
            var userInfo = FakeUserDatabase.ValidateUser(ekoid);
            
            if (userInfo == null)
            {
                TempData["LoginError"] = $"Invalid EKOID: '{ekoid}'. Please check and try again.";
                return RedirectToAction("Login");
            }

            // Create user from UserInfo (Factory pattern)
            _currentUser = FakeUserDatabase.CreateUser(userInfo);
            _currentUserInfo = userInfo;

            return RedirectToAction("Index");
        }

        public IActionResult Logout() 
        { 
            _currentUser = null; 
            _currentUserInfo = null;
            return RedirectToAction("Login"); 
        }

        #endregion

        #region Syllabus CRUD Operations

        public IActionResult Index()
        {
            if (_currentUser == null) return RedirectToAction("Login");
            ViewBag.User = _currentUser;
            ViewBag.UserInfo = _currentUserInfo;
            return View(_service.GetAllSyllabi());
        }

        public IActionResult Details(string id)
        {
            if (_currentUser == null) return RedirectToAction("Login");
            ViewBag.User = _currentUser;
            ViewBag.UserInfo = _currentUserInfo;
            
            var syllabus = _service.GetSyllabus(id);
            if (syllabus == null) return RedirectToAction("Index");
            
            return View(syllabus);
        }

        [HttpPost]
        public IActionResult UpdateInline(Syllabus model, string commitMessage)
        {
            if (_currentUser?.Role != "Instructor") return Unauthorized();

            // Null checks and cleanup
            if (model.WeeklyPlan == null) model.WeeklyPlan = new List<WeeklyItem>();
            if (model.Assessments == null) model.Assessments = new List<AssessmentItem>();
            if (model.WorkloadTable == null) model.WorkloadTable = new List<WorkloadItem>();
            if (model.LearningOutcomes == null) model.LearningOutcomes = new List<string>();
            if (model.Textbooks == null) model.Textbooks = new List<string>();
            if (model.SuggestedReadings == null) model.SuggestedReadings = new List<string>();
            if (model.ProgramCompetencies == null) model.ProgramCompetencies = new List<ProgramCompetencyItem>();

            model.LearningOutcomes = model.LearningOutcomes.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            model.Textbooks = model.Textbooks.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            model.SuggestedReadings = model.SuggestedReadings.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            model.WeeklyPlan = model.WeeklyPlan.Where(x => !string.IsNullOrWhiteSpace(x.Topics)).ToList();
            model.ProgramCompetencies = model.ProgramCompetencies.Where(x => !string.IsNullOrWhiteSpace(x.Description)).ToList();

            // Update syllabus (this also triggers notifications to subscribers)
            _service.UpdateSyllabus(model, commitMessage, _currentUser);
            
            // Get notified subscribers for display
            var subscribers = _subscriptionService.GetSubscribersForCourse(model.CourseCode);
            string notificationMsg = subscribers.Any() 
                ? $"Notified {subscribers.Count} subscriber(s): {string.Join(", ", subscribers.Select(s => s.UserName).Distinct())}"
                : "No subscribers to notify.";

            TempData["NotifyMessage"] = notificationMsg;

            return RedirectToAction("Details", new { id = model.CourseCode });
        }

        public IActionResult Create()
        {
            if (_currentUser?.Role != "Instructor") return Unauthorized();
            ViewBag.User = _currentUser;
            return View(new Syllabus());
        }

        [HttpPost]
        public IActionResult Create(Syllabus model)
        {
            if (_currentUser?.Role != "Instructor") return Unauthorized();
            _service.CreateSyllabus(model);
            return RedirectToAction("Details", new { id = model.CourseCode });
        }

        public IActionResult Delete(string id)
        {
            if (_currentUser?.Role != "Instructor") return Unauthorized();
            _service.DeleteSyllabus(id, _currentUser);
            return RedirectToAction("Index");
        }

        #endregion

        #region Version History

        public IActionResult History(string id)
        {
            if (_currentUser == null) return RedirectToAction("Login");
            var history = _service.GetHistory(id);
            
            // Students only see yearly snapshots (Archive view)
            if (_currentUser.Role == "Student")
            {
                history = history.GroupBy(x => x.Timestamp.Year)
                    .Select(g => g.OrderByDescending(x => x.Timestamp).First())
                    .OrderByDescending(x => x.Timestamp).ToList();
            }
            
            ViewBag.CourseCode = id;
            ViewBag.User = _currentUser;
            return View(history);
        }

        public IActionResult ViewCommit(string courseCode, string commitId)
        {
            if (_currentUser == null) return RedirectToAction("Login");
            ViewBag.User = _currentUser;
            var commit = _service.GetCommit(courseCode, commitId);
            if (commit == null) return NotFound();
            var oldSyllabus = System.Text.Json.JsonSerializer.Deserialize<Syllabus>(commit.SnapshotJson);
            ViewBag.IsHistory = true;
            ViewBag.CommitDate = commit.Timestamp;
            ViewBag.CommitAuthor = commit.AuthorName;
            return View("Details", oldSyllabus);
        }

        #endregion

        #region Subscription Management

        /// <summary>
        /// View and manage notification subscriptions
        /// </summary>
        public IActionResult Subscriptions()
        {
            if (_currentUser == null) return RedirectToAction("Login");
            ViewBag.User = _currentUser;
            ViewBag.UserInfo = _currentUserInfo;
            
            var userSubscriptions = _subscriptionService.GetUserSubscriptions(_currentUser.EKOID);
            var allCourses = _service.GetAllSyllabi().Select(s => s.CourseCode).ToList();
            
            ViewBag.AllCourses = allCourses;
            return View(userSubscriptions);
        }

        [HttpPost]
        public IActionResult Subscribe(string pattern, bool notifyEmail, bool notifySMS)
        {
            if (_currentUser == null) return RedirectToAction("Login");
            
            var subscription = new Subscription(
                _currentUser.EKOID,
                _currentUser.FullName,
                pattern,
                notifyEmail,
                notifySMS
            );
            
            _subscriptionService.Subscribe(subscription);
            TempData["SubscriptionMessage"] = $"Successfully subscribed to '{pattern}'";
            
            return RedirectToAction("Subscriptions");
        }

        [HttpPost]
        public IActionResult Unsubscribe(string subscriptionId)
        {
            if (_currentUser == null) return RedirectToAction("Login");
            
            _subscriptionService.Unsubscribe(subscriptionId);
            TempData["SubscriptionMessage"] = "Subscription removed.";
            
            return RedirectToAction("Subscriptions");
        }

        #endregion
    }
}