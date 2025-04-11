using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class Enquiry
    {
        [Key]
        public int EnquiryId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(255)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public string Response { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
