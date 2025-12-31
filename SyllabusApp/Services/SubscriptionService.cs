using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SyllabusApp.Interfaces;
using SyllabusApp.Models;

namespace SyllabusApp.Services
{
    /// <summary>
    /// Manages user subscriptions to course notifications.
    /// Stores subscriptions in a JSON file (simulating a database).
    /// 
    /// Demonstrates:
    /// - Single Responsibility Principle: Only handles subscription logic
    /// - Dependency Inversion: Implements ISubscriptionService interface
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        private readonly string _subscriptionsFilePath;
        private List<Subscription> _subscriptions;

        public SubscriptionService()
        {
            _subscriptionsFilePath = Path.Combine(AppContext.BaseDirectory, "Data_Subscriptions.json");
            LoadSubscriptions();
        }

        private void LoadSubscriptions()
        {
            if (File.Exists(_subscriptionsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_subscriptionsFilePath);
                    _subscriptions = JsonSerializer.Deserialize<List<Subscription>>(json) ?? new List<Subscription>();
                }
                catch
                {
                    _subscriptions = new List<Subscription>();
                }
            }
            else
            {
                // Initialize with default subscriptions (HOD for SE and CE courses)
                _subscriptions = new List<Subscription>
                {
                    new Subscription("kaya.oguz", "Doç. Dr. Kaya Oğuz", "SE*", true, true),
                    new Subscription("kaya.oguz", "Doç. Dr. Kaya Oğuz", "CE*", true, true),
                };
                SaveSubscriptions();
            }
        }

        private void SaveSubscriptions()
        {
            var json = JsonSerializer.Serialize(_subscriptions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_subscriptionsFilePath, json);
        }

        public List<Subscription> GetUserSubscriptions(string userEkoid)
        {
            return _subscriptions
                .Where(s => s.UserEKOID.Equals(userEkoid, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Subscription> GetSubscribersForCourse(string courseCode)
        {
            return _subscriptions
                .Where(s => s.MatchesCourse(courseCode))
                .ToList();
        }

        public void Subscribe(Subscription subscription)
        {
            if (subscription == null) return;

            // Check for duplicate (same user, same pattern)
            var existing = _subscriptions.FirstOrDefault(s => 
                s.UserEKOID.Equals(subscription.UserEKOID, StringComparison.OrdinalIgnoreCase) &&
                s.CourseCodePattern.Equals(subscription.CourseCodePattern, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                // Update existing subscription
                existing.NotifyByEmail = subscription.NotifyByEmail;
                existing.NotifyBySMS = subscription.NotifyBySMS;
            }
            else
            {
                // Add new subscription
                _subscriptions.Add(subscription);
            }

            SaveSubscriptions();
        }

        public void Unsubscribe(string subscriptionId)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
            if (subscription != null)
            {
                _subscriptions.Remove(subscription);
                SaveSubscriptions();
            }
        }

        public List<Subscription> GetAllSubscriptions()
        {
            return _subscriptions.ToList();
        }
    }
}
