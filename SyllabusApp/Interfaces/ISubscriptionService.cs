using System.Collections.Generic;
using SyllabusApp.Models;

namespace SyllabusApp.Interfaces
{
    /// <summary>
    /// Interface for managing user subscriptions to course notifications.
    /// Demonstrates: Interface Segregation Principle (ISP) from SOLID
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Gets all subscriptions for a specific user.
        /// </summary>
        List<Subscription> GetUserSubscriptions(string userEkoid);

        /// <summary>
        /// Gets all users who should be notified about changes to a course.
        /// Matches course code against subscription patterns.
        /// </summary>
        List<Subscription> GetSubscribersForCourse(string courseCode);

        /// <summary>
        /// Adds a new subscription for a user.
        /// </summary>
        void Subscribe(Subscription subscription);

        /// <summary>
        /// Removes a subscription.
        /// </summary>
        void Unsubscribe(string subscriptionId);

        /// <summary>
        /// Gets all subscriptions in the system (for admin purposes).
        /// </summary>
        List<Subscription> GetAllSubscriptions();
    }
}
