using System.ComponentModel.DataAnnotations;

namespace EducationMVC.Models
{
    public class PaymentDto
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "";  // Example: "Credit Card", "Cash"
    }
}
