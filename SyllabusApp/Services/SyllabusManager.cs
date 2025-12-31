using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using SyllabusApp.Models;
using SyllabusApp.Interfaces;

namespace SyllabusApp.Services
{
    public class SyllabusManager : ISyllabusService
    {
        
        private readonly string _pathData = Path.Combine(AppContext.BaseDirectory, "Data_Syllabi");
        private readonly string _pathCommits = Path.Combine(AppContext.BaseDirectory, "Data_Commits");
        private readonly INotificationService _notifier;

        public SyllabusManager(INotificationService notifier)
        {
            _notifier = notifier;
            if (!Directory.Exists(_pathData)) Directory.CreateDirectory(_pathData);
            if (!Directory.Exists(_pathCommits)) Directory.CreateDirectory(_pathCommits);
        }

        public void EnsureSeedData()
        {
            if (!File.Exists(Path.Combine(_pathData, "SE 307.json")))
            {
                var s = new Syllabus { CourseCode = "SE 307", CourseName = "Concepts of Object-Oriented Programming", Semester = "Fall/Spring", TheoryHours = 2, LabHours = 2, LocalCredit = 3, ECTS = 7, Prerequisites = "CE 221", Language = "English", CourseType = "Elective", CourseLevel = "First Cycle", TeachingMethods = "Discussion, Project, Lab", CoordinatorEKOID = "Dr. Kutluhan Erol", Lecturer = "Doc. Dr. Kaya Oguz", Assistant = "Hamza Cekirdek", Objectives = "OOP concepts with C#", Description = "Deep dive into OOP.", LearningOutcomes = new List<string> { "Understand OOP", "Apply Inheritance" }, Textbooks = new List<string> { "C# 10 and .NET 6" }, SuggestedReadings = new List<string> { "Clean Code" } };
                s.WeeklyPlan.Add(new WeeklyItem { WeekNumber=1, Topics="Intro", Preparation="Ch1" });
                s.Assessments.Add(new AssessmentItem { Activity="Midterm", Count=1, Percentage=30 });
                s.WorkloadTable.Add(new WorkloadItem { Activity="Lectures", Count=14, Duration=2 });
                CreateSyllabus(s);
            }
        }
        public List<Syllabus> GetAllSyllabi() {
            var list = new List<Syllabus>();
            foreach(var f in Directory.GetFiles(_pathData, "*.json")) list.Add(JsonSerializer.Deserialize<Syllabus>(File.ReadAllText(f)));
            return list;
        }
        public Syllabus GetSyllabus(string code) {
            string p = Path.Combine(_pathData, code + ".json");
            return File.Exists(p) ? JsonSerializer.Deserialize<Syllabus>(File.ReadAllText(p)) : null;
        }
        public void CreateSyllabus(Syllabus s) => SaveJson(s);
        
        
        public void UpdateSyllabus(Syllabus s, string msg, User u) {
            var old = GetSyllabus(s.CourseCode);
            if(old != null) {
                var c = new SyllabusCommit { CommitID = Guid.NewGuid().ToString(), Timestamp = DateTime.Now, AuthorName = u.FullName, Message = msg, SnapshotJson = JsonSerializer.Serialize(old) };
                File.WriteAllText(Path.Combine(_pathCommits, $"{s.CourseCode}_{c.Timestamp.Ticks}.json"), JsonSerializer.Serialize(c));
            }
            SaveJson(s);
            
            
            _notifier.NotifyHOD(s.CourseCode, "UPDATED", u.FullName, msg);
        }

        public void DeleteSyllabus(string code, User u) {
            string p = Path.Combine(_pathData, code + ".json");
            if(File.Exists(p)) {
                var old = GetSyllabus(code);
                var c = new SyllabusCommit { CommitID = "DEL_"+Guid.NewGuid().ToString(), Timestamp = DateTime.Now, AuthorName = u.FullName, Message = "DELETED", SnapshotJson = JsonSerializer.Serialize(old) };
                File.WriteAllText(Path.Combine(_pathCommits, $"{code}_DELETED.json"), JsonSerializer.Serialize(c));
                File.Delete(p);
                
                
                _notifier.NotifyHOD(code, "DELETED", u.FullName, "Course deleted permanently.");
            }
        }
        public List<SyllabusCommit> GetHistory(string code) {
            var list = new List<SyllabusCommit>();
            foreach(var f in Directory.GetFiles(_pathCommits, $"{code}_*.json")) list.Add(JsonSerializer.Deserialize<SyllabusCommit>(File.ReadAllText(f)));
            return list.OrderByDescending(x => x.Timestamp).ToList();
        }
        public SyllabusCommit GetCommit(string code, string id) {
            foreach(var f in Directory.GetFiles(_pathCommits, $"{code}_*.json")) {
                var c = JsonSerializer.Deserialize<SyllabusCommit>(File.ReadAllText(f));
                if(c.CommitID == id) return c;
            }
            return null;
        }
        private void SaveJson(Syllabus s) => File.WriteAllText(Path.Combine(_pathData, s.CourseCode + ".json"), JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true }));
    }
}