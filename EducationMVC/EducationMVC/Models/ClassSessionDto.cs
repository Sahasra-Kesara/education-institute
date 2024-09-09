namespace EducationMVC.Models
{
    public class ClassSessionDto
    {
        public string SessionID { get; set; } = "";

        public string CourseID { get; set; } = "";

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public int Duration { get; set; } // Duration in minutes
    }
}
