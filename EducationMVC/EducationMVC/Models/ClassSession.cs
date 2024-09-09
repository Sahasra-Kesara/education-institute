using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class ClassSession
    {
        [Key]
        [MaxLength(50)]
        public string SessionID { get; set; } = "";

        [MaxLength(50)]
        public string CourseID { get; set; } = "";

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        [Range(1, int.MaxValue)]
        public int Duration { get; set; } // Duration in minutes
    }
}
