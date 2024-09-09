using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class TeacherDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Subject { get; set; } = "";

        public IFormFile? ImageFile { get; set; }
    }
}
