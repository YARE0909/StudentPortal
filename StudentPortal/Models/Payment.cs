using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public enum PaymentMethod { CreditCard, DebitCard, BankTransfer, Cash }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
