using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public enum InvoiceStatus { Pending, Paid, Cancelled }

    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal AmountDue { get; set; }

        [DataType(DataType.Currency)]
        public decimal Adjustment { get; set; } = 0;

        [Required]
        [DataType(DataType.Currency)]
        public decimal FinalAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

        public DateTime IssueDate { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
