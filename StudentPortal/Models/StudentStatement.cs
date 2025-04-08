using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class StudentStatement
    {
        [Key]
        public int StatementId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalFees { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal PaidAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal BalanceDue { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        // Navigation property
        public Student Student { get; set; }
    }
}
