using System;
using Xunit;
using SyllabusApp.Services;

namespace SyllabusApp.Tests
{
    /// <summary>
    /// Unit tests for NotificationService
    /// Tests the SMS and Email notification simulation logic
    /// </summary>
    public class NotificationServiceTests
    {
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationService = new NotificationService();
        }

        [Fact]
        public void NotifyHOD_WithSECourse_NotifiesKayaOguz()
        {
            // Arrange
            string courseCode = "SE 307";

            // Act
            var result = _notificationService.NotifyHOD(courseCode, "UPDATED", "Test User", "Test message");

            // Assert
            Assert.Contains("Kaya", result);
        }

        [Fact]
        public void NotifyHOD_WithSEPrefix_CaseInsensitive()
        {
            // Arrange - lowercase
            string courseCode = "se 101";

            // Act
            var result = _notificationService.NotifyHOD(courseCode, "CREATED", "Author", "Message");

            // Assert
            Assert.Contains("Kaya", result);
        }

        [Fact]
        public void NotifyHOD_WithCECourse_NotifiesKayaOguz()
        {
            // Arrange
            string courseCode = "CE 221";

            // Act
            var result = _notificationService.NotifyHOD(courseCode, "UPDATED", "Test User", "Test message");

            // Assert
            Assert.Contains("Kaya", result);
        }

        [Fact]
        public void NotifyHOD_WithOtherCourse_NotifiesGeneralAdmin()
        {
            // Arrange
            string courseCode = "MATH 101";

            // Act
            var result = _notificationService.NotifyHOD(courseCode, "UPDATED", "Math Prof", "Updated math syllabus");

            // Assert
            Assert.Contains("General Admin", result);
        }

        [Fact]
        public void NotifyHOD_ReturnsConfirmationMessage()
        {
            // Arrange
            string courseCode = "SE 307";

            // Act
            var result = _notificationService.NotifyHOD(courseCode, "UPDATED", "Author", "Message");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Notification sent", result);
            Assert.Contains("SMS", result);
            Assert.Contains("Email", result);
        }
    }
}
