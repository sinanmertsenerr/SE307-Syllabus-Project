using System;
using System.Collections.Generic;
using System.Linq;
using SyllabusApp.Interfaces;
using SyllabusApp.Models;

namespace SyllabusApp.Services
{
    /// <summary>
    /// Handles SMS and Email notification simulations.
    /// 
    /// Demonstrates:
    /// - Open/Closed Principle: Extended to support subscriptions without modifying core logic
    /// - Dependency Injection: Uses ISubscriptionService for subscriber lookup
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ISubscriptionService _subscriptionService;

        // Parameterless constructor for backward compatibility
        public NotificationService()
        {
            _subscriptionService = null; // Will use legacy HOD logic
        }

        // Constructor with subscription service (Dependency Injection)
        public NotificationService(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public string NotifyHOD(string courseCode, string action, string authorName, string commitMessage)
        {
            var notifiedUsers = new List<string>();

            // If subscription service is available, use it
            if (_subscriptionService != null)
            {
                var subscribers = _subscriptionService.GetSubscribersForCourse(courseCode);
                
                foreach (var sub in subscribers)
                {
                    if (sub.NotifyBySMS)
                    {
                        SendSMS(sub.UserName, GetPhoneForUser(sub.UserEKOID), courseCode, action, authorName);
                    }
                    
                    if (sub.NotifyByEmail)
                    {
                        SendEmail(sub.UserName, GetEmailForUser(sub.UserEKOID), courseCode, action, authorName, commitMessage);
                    }
                    
                    notifiedUsers.Add(sub.UserName);
                }

                if (subscribers.Any())
                {
                    return $"Notification sent to {string.Join(", ", notifiedUsers.Distinct())} via SMS and Email.";
                }
            }

            // Fallback to legacy HOD logic if no subscription service or no subscribers
            return NotifyLegacyHOD(courseCode, action, authorName, commitMessage);
        }

        /// <summary>
        /// Legacy notification logic - notifies hardcoded HOD based on course prefix.
        /// Kept for backward compatibility.
        /// </summary>
        private string NotifyLegacyHOD(string courseCode, string action, string authorName, string commitMessage)
        {
            string hodName = "General Admin";
            string hodPhone = "+90 555 000 00 00";
            string hodEmail = "admin@ieu.edu.tr";

            if (courseCode.StartsWith("SE", StringComparison.OrdinalIgnoreCase))
            {
                hodName = "Doc. Dr. Kaya Oguz";
                hodPhone = "+90 555 999 88 77";
                hodEmail = "kaya.oguz@ieu.edu.tr";
            }
            else if (courseCode.StartsWith("CE", StringComparison.OrdinalIgnoreCase))
            {
                hodName = "Doc. Dr. Kaya Oguz";
                hodPhone = "+90 555 999 88 77";
                hodEmail = "kaya.oguz@ieu.edu.tr";
            }

            SendSMS(hodName, hodPhone, courseCode, action, authorName);
            SendEmail(hodName, hodEmail, courseCode, action, authorName, commitMessage);

            return $"Notification sent to {hodName} via SMS and Email.";
        }

        /// <summary>
        /// Simulates sending an SMS notification.
        /// In a real system, this would integrate with the university's SMS gateway.
        /// </summary>
        private void SendSMS(string recipientName, string phone, string courseCode, string action, string authorName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SMS SERVICE] To: {phone} ({recipientName}) | Msg: {courseCode} has been {action} by {authorName}.");
            Console.ResetColor();
        }

        /// <summary>
        /// Simulates sending an Email notification.
        /// In a real system, this would integrate with the university's email service.
        /// </summary>
        private void SendEmail(string recipientName, string email, string courseCode, string action, string authorName, string commitMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[EMAIL SERVICE] To: {email} ({recipientName})");
            Console.WriteLine($"                Subject: Syllabus Update Alert: {courseCode}");
            Console.WriteLine($"                Body: Course {courseCode} has been {action} by {authorName}.");
            Console.WriteLine($"                Details: {commitMessage}");
            Console.ResetColor();
        }

        /// <summary>
        /// Gets phone number for a user (simulated lookup).
        /// </summary>
        private string GetPhoneForUser(string ekoid)
        {
            var user = FakeUserDatabase.ValidateUser(ekoid);
            if (user == null) return "+90 555 000 00 00";

            // Simulated phone numbers based on user
            return user.Role == "Instructor" ? "+90 555 999 88 77" : "+90 555 111 22 33";
        }

        /// <summary>
        /// Gets email for a user.
        /// </summary>
        private string GetEmailForUser(string ekoid)
        {
            var user = FakeUserDatabase.ValidateUser(ekoid);
            return user?.Email ?? "notification@ieu.edu.tr";
        }
    }
}