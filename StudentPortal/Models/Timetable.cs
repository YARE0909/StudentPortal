using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public enum DayOfWeekEnum { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

    public class Timetable
    {
        [Key]
        public int TimetableId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DayOfWeekEnum DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(50)]
        public string RoomNumber { get; set; }

        // Navigation property
        public Course Course { get; set; }
    }
}
