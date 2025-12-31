using Xunit;
using SyllabusApp.Models;
using System.Collections.Generic;

namespace SyllabusApp.Tests
{
    /// <summary>
    /// Unit tests for User Models - Testing OOP Concepts
    /// Demonstrates: Inheritance, Polymorphism, Abstract Classes
    /// </summary>
    public class UserModelTests
    {
        [Fact]
        public void Instructor_IsUser()
        {
            var instructor = new Instructor("101", "Dr. Test");
            Assert.IsAssignableFrom<User>(instructor);
        }

        [Fact]
        public void Student_IsUser()
        {
            var student = new Student("202", "Student Test");
            Assert.IsAssignableFrom<User>(student);
        }

        [Fact]
        public void Instructor_HasCorrectRole()
        {
            var instructor = new Instructor("101", "Test Instructor");
            Assert.Equal("Instructor", instructor.Role);
        }

        [Fact]
        public void Student_HasCorrectRole()
        {
            var student = new Student("202", "Test Student");
            Assert.Equal("Student", student.Role);
        }

        [Fact]
        public void DifferentUserTypes_HaveDifferentRoles()
        {
            User instructor = new Instructor("001", "Same Name");
            User student = new Student("002", "Same Name");

            Assert.Equal("Instructor", instructor.Role);
            Assert.Equal("Student", student.Role);
            Assert.NotEqual(instructor.Role, student.Role);
        }

        [Fact]
        public void UserList_CanContainDifferentUserTypes()
        {
            var users = new List<User>
            {
                new Instructor("101", "Instructor 1"),
                new Student("201", "Student 1"),
                new Instructor("102", "Instructor 2"),
                new Student("202", "Student 2")
            };

            Assert.Equal(4, users.Count);
            Assert.Equal(2, users.FindAll(u => u.Role == "Instructor").Count);
            Assert.Equal(2, users.FindAll(u => u.Role == "Student").Count);
        }

        [Fact]
        public void Instructor_InheritsUserProperties()
        {
            var instructor = new Instructor("instructor_001", "Prof. John Doe");
            Assert.Equal("instructor_001", instructor.EKOID);
            Assert.Equal("Prof. John Doe", instructor.FullName);
        }

        [Fact]
        public void Student_InheritsUserProperties()
        {
            var student = new Student("student_001", "Jane Smith");
            Assert.Equal("student_001", student.EKOID);
            Assert.Equal("Jane Smith", student.FullName);
        }
    }
}
