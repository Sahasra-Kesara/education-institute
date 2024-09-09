using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class StudentDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Grade { get; set; } = "";

        public IFormFile? ImageFile { get; set; }
    }
}
