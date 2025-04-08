using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class PaymentHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int PaymentId { get; set; }

        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; }

        [StringLength(255)]
        public string ReceiptUrl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Payment Payment { get; set; }
    }
}
