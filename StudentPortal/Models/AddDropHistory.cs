using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public enum ActionType { Added, Dropped }

    public class AddDropHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public ActionType ActionType { get; set; }

        public DateTime ActionDate { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
    