using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class UserAccount
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastPasswordUpdate { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
