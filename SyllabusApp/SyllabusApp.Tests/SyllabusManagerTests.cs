using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using Moq;
using SyllabusApp.Models;
using SyllabusApp.Services;
using SyllabusApp.Interfaces;

namespace SyllabusApp.Tests
{
    /// <summary>
    /// Unit tests for SyllabusManager - Testing CRUD operations
    /// Demonstrates: Unit Testing, Mocking concepts
    /// </summary>
    public class SyllabusManagerTests
    {
        private readonly Mock<INotificationService> _mockNotifier;

        public SyllabusManagerTests()
        {
            _mockNotifier = new Mock<INotificationService>();
            _mockNotifier.Setup(n => n.NotifyHOD(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .Returns("Notification sent.");
        }

        [Fact]
        public void Syllabus_DefaultConstructor_InitializesLists()
        {
            var syllabus = new Syllabus();

            Assert.NotNull(syllabus.LearningOutcomes);
            Assert.NotNull(syllabus.WeeklyPlan);
            Assert.NotNull(syllabus.Textbooks);
            Assert.NotNull(syllabus.SuggestedReadings);
            Assert.NotNull(syllabus.Assessments);
            Assert.NotNull(syllabus.WorkloadTable);
            Assert.NotNull(syllabus.ProgramCompetencies);
        }

        [Fact]
        public void Syllabus_CanSetBasicProperties()
        {
            var syllabus = new Syllabus
            {
                CourseCode = "SE 307",
                CourseName = "Concepts of Object-Oriented Programming",
                Semester = "Fall",
                TheoryHours = 2,
                LabHours = 2,
                LocalCredit = 3,
                ECTS = 7
            };

            Assert.Equal("SE 307", syllabus.CourseCode);
            Assert.Equal("Concepts of Object-Oriented Programming", syllabus.CourseName);
            Assert.Equal("Fall", syllabus.Semester);
            Assert.Equal(2, syllabus.TheoryHours);
            Assert.Equal(2, syllabus.LabHours);
            Assert.Equal(3, syllabus.LocalCredit);
            Assert.Equal(7, syllabus.ECTS);
        }

        [Fact]
        public void Syllabus_DefaultLanguage_IsEnglish()
        {
            var syllabus = new Syllabus();
            Assert.Equal("English", syllabus.Language);
        }

        [Fact]
        public void WorkloadItem_CalculatesWorkload()
        {
            var workload = new WorkloadItem
            {
                Activity = "Lectures",
                Count = 14,
                Duration = 2
            };
            Assert.Equal(28, workload.Workload);
        }

        [Fact]
        public void SyllabusCommit_StoresAllProperties()
        {
            var commit = new SyllabusCommit
            {
                CommitID = "abc-123",
                Timestamp = DateTime.Now,
                AuthorName = "Kaya Oğuz",
                Message = "Updated learning outcomes",
                SnapshotJson = "{\"CourseCode\":\"SE 307\"}"
            };

            Assert.Equal("abc-123", commit.CommitID);
            Assert.Equal("Kaya Oğuz", commit.AuthorName);
            Assert.Equal("Updated learning outcomes", commit.Message);
            Assert.Contains("SE 307", commit.SnapshotJson);
        }

        [Fact]
        public void Syllabus_CanAddLearningOutcomes()
        {
            var syllabus = new Syllabus();
            syllabus.LearningOutcomes.Add("Understand OOP concepts");
            syllabus.LearningOutcomes.Add("Apply inheritance");
            syllabus.LearningOutcomes.Add("Implement polymorphism");

            Assert.Equal(3, syllabus.LearningOutcomes.Count);
            Assert.Contains("Understand OOP concepts", syllabus.LearningOutcomes);
        }

        [Fact]
        public void Syllabus_AssessmentPercentages_CanSumTo100()
        {
            var syllabus = new Syllabus();
            syllabus.Assessments.Add(new AssessmentItem { Activity = "Midterm", Count = 1, Percentage = 30 });
            syllabus.Assessments.Add(new AssessmentItem { Activity = "Labs", Count = 5, Percentage = 20 });
            syllabus.Assessments.Add(new AssessmentItem { Activity = "Project", Count = 1, Percentage = 15 });
            syllabus.Assessments.Add(new AssessmentItem { Activity = "Final", Count = 1, Percentage = 35 });

            int totalPercentage = 0;
            foreach (var a in syllabus.Assessments)
            {
                totalPercentage += a.Percentage;
            }

            Assert.Equal(100, totalPercentage);
        }
    }
}
