using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class ManageInvoicesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Invoice> Invoices { get; set; } = new();

        // For dropdown (students)
        public SelectList StudentOptions { get; set; }

        [BindProperty]
        public NewInvoiceInputModel NewInvoice { get; set; }

        public class NewInvoiceInputModel
        {
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
        }

        public ManageInvoicesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostGenerateInvoiceAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var invoice = new Invoice
            {
                StudentId = NewInvoice.StudentId,
                AmountDue = NewInvoice.AmountDue,
                Adjustment = NewInvoice.Adjustment,
                FinalAmount = NewInvoice.FinalAmount,
                IssueDate = DateTime.Now,
                Status = InvoiceStatus.Pending
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                invoice.Status = status;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Invoices = await _context.Invoices
                .Include(i => i.Student)
                .ToListAsync();

            StudentOptions = new SelectList(await _context.Students.ToListAsync(), "StudentId", "FirstName");

            NewInvoice ??= new NewInvoiceInputModel();
        }
    }
}
