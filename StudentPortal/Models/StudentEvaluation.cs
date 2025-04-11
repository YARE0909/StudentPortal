using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class StudentEvaluation
    {
        [Key]
        public int EvaluationId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string Feedback { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
