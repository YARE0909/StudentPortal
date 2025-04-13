// ViewModels/StudentStatementViewModel.cs
using StudentPortal.Models;

namespace StudentPortal.ViewModels
{
    public class StudentStatementViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();

        public decimal TotalFinalAmount => Invoices.Sum(i => i.FinalAmount);
        public decimal TotalPaid => Invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.FinalAmount);
        public decimal TotalDue => Invoices.Where(i => i.Status == InvoiceStatus.Pending).Sum(i => i.FinalAmount);
    }
}
