namespace SyllabusApp.Interfaces
{
    public interface INotificationService
    {
       
        string NotifyHOD(string courseCode, string action, string authorName, string commitMessage);
    }
}