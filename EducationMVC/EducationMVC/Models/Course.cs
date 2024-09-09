using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string CourseName { get; set; } = "";

        [MaxLength(1000)]
        public string CourseDescription { get; set; } = "";

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }  // Navigation Property

        [Required]
        [MaxLength(50)]
        public string Schedule { get; set; } = ""; // Example: "Mon-Wed-Fri 9:00 AM - 11:00 AM"

        [Required]
        public int DurationInWeeks { get; set; } // Course Duration in weeks
    }
}
