using System.Collections.Generic;
using SyllabusApp.Models;

namespace SyllabusApp.Interfaces
{
    public interface ISyllabusService
    {
        List<Syllabus> GetAllSyllabi();
        Syllabus GetSyllabus(string courseCode);
        void CreateSyllabus(Syllabus syllabus);
        void UpdateSyllabus(Syllabus syllabus, string commitMessage, User currentUser);
        void DeleteSyllabus(string courseCode, User currentUser);
        List<SyllabusCommit> GetHistory(string courseCode);
        void EnsureSeedData();
        SyllabusCommit GetCommit(string courseCode, string commitId);
    }
}