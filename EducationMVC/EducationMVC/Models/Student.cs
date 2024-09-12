using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class Student
    {
        internal object Subject;

        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Grade { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";


        public DateTime JoinDate { get; set; }
    }
}
