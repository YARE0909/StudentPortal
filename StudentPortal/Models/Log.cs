using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(255)]
        public string Action { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Timestamp { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
