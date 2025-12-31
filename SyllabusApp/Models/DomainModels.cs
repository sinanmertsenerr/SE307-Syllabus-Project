using System;
using System.Collections.Generic;

namespace SyllabusApp.Models
{
   
    public abstract class User
    {
        public string EKOID { get; set; }
        public string FullName { get; set; }
        public string Role { get; protected set; }
        public User(string ekoid, string name) { EKOID = ekoid; FullName = name; }
    }
    public class Instructor : User { public Instructor(string id, string name) : base(id, name) { Role = "Instructor"; } }
    public class Student : User { public Student(string id, string name) : base(id, name) { Role = "Student"; } }

    
    public class Syllabus
    {
        
        public string CourseCode { get; set; }
        public string CourseName { get; set; } 
        
        
        public string Semester { get; set; }
        public int TheoryHours { get; set; }
        public int LabHours { get; set; }
        public int LocalCredit { get; set; }
        public int ECTS { get; set; }

        
        public string Prerequisites { get; set; }
        public string Language { get; set; } = "English";
        public string CourseType { get; set; }
        public string CourseLevel { get; set; }
        public string TeachingMethods { get; set; } 
        
        
        public string CoordinatorEKOID { get; set; } 
        public string Lecturer { get; set; } 
        public string Assistant { get; set; } 

        
        public string Objectives { get; set; }
        public string Description { get; set; }
        
        
        public string SustainableDevelopmentGoals { get; set; } 

        
        public List<string> LearningOutcomes { get; set; } = new List<string>();
        public List<WeeklyItem> WeeklyPlan { get; set; } = new List<WeeklyItem>();
        public List<string> Textbooks { get; set; } = new List<string>();
        public List<string> SuggestedReadings { get; set; } = new List<string>();

        
        public List<AssessmentItem> Assessments { get; set; } = new List<AssessmentItem>();
        public List<WorkloadItem> WorkloadTable { get; set; } = new List<WorkloadItem>();
        
        
        public List<ProgramCompetencyItem> ProgramCompetencies { get; set; } = new List<ProgramCompetencyItem>();

        public Syllabus() {}
    }

    
    public class WeeklyItem { public int WeekNumber { get; set; } public string Topics { get; set; } public string Preparation { get; set; } }
    
    public class AssessmentItem { public string Activity { get; set; } public int Count { get; set; } public int Percentage { get; set; } }
    
    public class WorkloadItem { public string Activity { get; set; } public int Count { get; set; } public int Duration { get; set; } public int Workload => Count * Duration; }
    
    
    public class ProgramCompetencyItem 
    { 
        public int Id { get; set; } 
        public string Description { get; set; } 
        public int Level { get; set; } 
    }

    public class SyllabusCommit 
    { 
        public string CommitID { get; set; } 
        public DateTime Timestamp { get; set; } 
        public string AuthorName { get; set; } 
        public string Message { get; set; } 
        public string SnapshotJson { get; set; } 
    }

    // ========================================
    // OAuth/EKOID Simulation - Fake User Database
    // ========================================
    
    /// <summary>
    /// Represents a user record in the simulated university database.
    /// In a real system, this would come from the university's OAuth/LDAP service.
    /// </summary>
    public class UserInfo
    {
        public string EKOID { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "Instructor" or "Student"
        public string Email { get; set; }
        public string Department { get; set; }

        public UserInfo() { }
        public UserInfo(string ekoid, string name, string role, string email, string department = "Computer Engineering")
        {
            EKOID = ekoid;
            FullName = name;
            Role = role;
            Email = email;
            Department = department;
        }
    }

    /// <summary>
    /// Simulates the university's user database.
    /// In a real implementation, this would connect to the OAuth/EKOID service.
    /// This demonstrates the principle of simulating external services for development.
    /// </summary>
    public static class FakeUserDatabase
    {
        public static Dictionary<string, UserInfo> Users { get; } = new Dictionary<string, UserInfo>
        {
            // Instructors
            { "kaya.oguz", new UserInfo("kaya.oguz", "Doç. Dr. Kaya Oğuz", "Instructor", "kaya.oguz@ieu.edu.tr", "Software Engineering") },
            { "kutluhan.erol", new UserInfo("kutluhan.erol", "Dr. Kutluhan Erol", "Instructor", "kutluhan.erol@ieu.edu.tr", "Computer Engineering") },
            { "hamza.cekirdek", new UserInfo("hamza.cekirdek", "Arş. Gör. Hamza Çekirdek", "Instructor", "hamza.cekirdek@ieu.edu.tr", "Software Engineering") },
            { "test.instructor", new UserInfo("test.instructor", "Test Instructor", "Instructor", "test.instructor@ieu.edu.tr", "Computer Engineering") },
            
            // Students
            { "sinan.sener", new UserInfo("sinan.sener", "Sinan Mert Şener", "Student", "sinan.sener@std.ieu.edu.tr", "Software Engineering") },
            { "ali.veli", new UserInfo("ali.veli", "Ali Veli", "Student", "ali.veli@std.ieu.edu.tr", "Computer Engineering") },
            { "test.student", new UserInfo("test.student", "Test Student", "Student", "test.student@std.ieu.edu.tr", "Computer Engineering") },
        };

        /// <summary>
        /// Simulates OAuth token validation and user lookup.
        /// Returns null if user not found (invalid EKOID).
        /// </summary>
        public static UserInfo ValidateUser(string ekoid)
        {
            if (string.IsNullOrWhiteSpace(ekoid)) return null;
            
            // Normalize EKOID (lowercase, trim)
            var normalizedId = ekoid.Trim().ToLowerInvariant();
            
            return Users.TryGetValue(normalizedId, out var user) ? user : null;
        }

        /// <summary>
        /// Creates a User object from UserInfo (Factory pattern demonstration)
        /// </summary>
        public static User CreateUser(UserInfo info)
        {
            if (info == null) return null;
            
            return info.Role == "Instructor" 
                ? new Instructor(info.EKOID, info.FullName) 
                : new Student(info.EKOID, info.FullName);
        }
    }

    // ========================================
    // Subscription System - For Notifications
    // ========================================

    /// <summary>
    /// Represents a user's subscription to course notifications.
    /// Users can subscribe to specific courses or course patterns (e.g., "SE*" for all SE courses).
    /// </summary>
    public class Subscription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserEKOID { get; set; }
        public string UserName { get; set; }
        public string CourseCodePattern { get; set; } // "SE 307", "SE*", "CE*", "*" for all
        public bool NotifyByEmail { get; set; } = true;
        public bool NotifyBySMS { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Subscription() { }
        
        public Subscription(string userEkoid, string userName, string pattern, bool email = true, bool sms = false)
        {
            UserEKOID = userEkoid;
            UserName = userName;
            CourseCodePattern = pattern;
            NotifyByEmail = email;
            NotifyBySMS = sms;
        }

        /// <summary>
        /// Checks if this subscription matches a given course code.
        /// Supports wildcards: "SE*" matches "SE 307", "SE 101", etc.
        /// </summary>
        public bool MatchesCourse(string courseCode)
        {
            if (string.IsNullOrEmpty(CourseCodePattern) || CourseCodePattern == "*")
                return true;

            if (CourseCodePattern.EndsWith("*"))
            {
                var prefix = CourseCodePattern.TrimEnd('*');
                return courseCode.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            return courseCode.Equals(CourseCodePattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}