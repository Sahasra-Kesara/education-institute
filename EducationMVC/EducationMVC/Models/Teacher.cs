using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Subject { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        public DateTime JoinDate { get; set; }
    }
}
