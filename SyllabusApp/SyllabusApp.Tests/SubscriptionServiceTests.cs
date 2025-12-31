using System;
using Xunit;
using SyllabusApp.Models;

namespace SyllabusApp.Tests
{
    /// <summary>
    /// Unit tests for SubscriptionService and Subscription model
    /// Tests the notification subscription system
    /// </summary>
    public class SubscriptionServiceTests
    {
        [Fact]
        public void Subscription_MatchesCourse_ExactMatch()
        {
            var sub = new Subscription("user1", "User", "SE 307");

            Assert.True(sub.MatchesCourse("SE 307"));
            Assert.True(sub.MatchesCourse("se 307")); // Case insensitive
            Assert.False(sub.MatchesCourse("SE 308"));
            Assert.False(sub.MatchesCourse("CE 221"));
        }

        [Fact]
        public void Subscription_MatchesCourse_WildcardPrefix()
        {
            var sub = new Subscription("user1", "User", "SE*");

            Assert.True(sub.MatchesCourse("SE 307"));
            Assert.True(sub.MatchesCourse("SE 101"));
            Assert.True(sub.MatchesCourse("SE 999"));
            Assert.False(sub.MatchesCourse("CE 221"));
            Assert.False(sub.MatchesCourse("MATH 101"));
        }

        [Fact]
        public void Subscription_MatchesCourse_AllWildcard()
        {
            var sub = new Subscription("user1", "User", "*");

            Assert.True(sub.MatchesCourse("SE 307"));
            Assert.True(sub.MatchesCourse("CE 221"));
            Assert.True(sub.MatchesCourse("MATH 101"));
            Assert.True(sub.MatchesCourse("ANY COURSE"));
        }

        [Fact]
        public void Subscription_HasDefaultValues()
        {
            var sub = new Subscription();

            Assert.NotNull(sub.Id);
            Assert.True(sub.NotifyByEmail); // Default is true
            Assert.False(sub.NotifyBySMS); // Default is false
        }

        [Fact]
        public void FakeUserDatabase_ValidateUser_ReturnsUser()
        {
            var user = FakeUserDatabase.ValidateUser("kaya.oguz");

            Assert.NotNull(user);
            Assert.Equal("kaya.oguz", user.EKOID);
            Assert.Equal("Instructor", user.Role);
        }

        [Fact]
        public void FakeUserDatabase_ValidateUser_CaseInsensitive()
        {
            var user1 = FakeUserDatabase.ValidateUser("KAYA.OGUZ");
            var user2 = FakeUserDatabase.ValidateUser("Kaya.Oguz");

            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.Equal(user1.FullName, user2.FullName);
        }

        [Fact]
        public void FakeUserDatabase_ValidateUser_InvalidReturnsNull()
        {
            var user = FakeUserDatabase.ValidateUser("invalid.user");
            Assert.Null(user);
        }

        [Fact]
        public void FakeUserDatabase_CreateUser_Instructor()
        {
            var info = FakeUserDatabase.ValidateUser("kaya.oguz");
            var user = FakeUserDatabase.CreateUser(info);

            Assert.NotNull(user);
            Assert.IsType<Instructor>(user);
            Assert.Equal("Instructor", user.Role);
        }

        [Fact]
        public void FakeUserDatabase_CreateUser_Student()
        {
            var info = FakeUserDatabase.ValidateUser("sinan.sener");
            var user = FakeUserDatabase.CreateUser(info);

            Assert.NotNull(user);
            Assert.IsType<Student>(user);
            Assert.Equal("Student", user.Role);
        }
    }
}
