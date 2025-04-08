using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
