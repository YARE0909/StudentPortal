using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public enum EnrollmentStatus { Enrolled, Dropped }

    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime EnrollmentDate { get; set; }

        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

        // Navigation properties
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
