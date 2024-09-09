using System;
using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class EnrollmentDto
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }
    }
}