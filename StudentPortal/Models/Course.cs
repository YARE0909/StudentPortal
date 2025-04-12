using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        public string Description { get; set; }

        [Required]
        public int CreditHours { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        public int CourseCost { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Timetable> Timetables { get; set; } // This links to Timetable
    }
}
