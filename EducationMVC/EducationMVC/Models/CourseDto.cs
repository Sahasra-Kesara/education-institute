namespace EducationMVC.Models
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string CourseName { get; set; } = "";

        public string CourseDescription { get; set; } = "";

        public int TeacherId { get; set; }

        public string TeacherName { get; set; } = "";  // Additional property to show the Teacher's name

        public string Schedule { get; set; } = "";

        public int DurationInWeeks { get; set; }
    }
}
