using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortal.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Pages.Payments
{
    [Authorize]
    public class MakePaymentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MakePaymentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // Drop-down list for Payment Methods.
        public List<SelectListItem> PaymentMethodOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                return Unauthorized(); // Handles when student is not logged in
            }

            int studentId = int.Parse(studentIdClaim);

            var invoice = await _context.Invoices
                .Where(i => i.StudentId == studentId && i.Status == InvoiceStatus.Pending)
                .OrderByDescending(i => i.IssueDate)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                ModelState.AddModelError(string.Empty, "No pending invoice found.");
                return Page(); // Return with error
            }

            Input = new InputModel
            {
                Amount = invoice.AmountDue
            };

            PaymentMethodOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
        new SelectListItem { Value = "DebitCard", Text = "Debit Card" },
        new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
        new SelectListItem { Value = "Cash", Text = "Cash" }
    };

            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the payment methods on validation failure
                PaymentMethodOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
                    new SelectListItem { Value = "DebitCard", Text = "Debit Card" },
                    new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
                    new SelectListItem { Value = "Cash", Text = "Cash" }
                };
                return Page();
            }

            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (studentIdClaim == null)
                return Unauthorized();

            int studentId = int.Parse(studentIdClaim);

            // Retrieve the latest pending invoice for the student
            var invoice = await _context.Invoices
                .Where(i => i.StudentId == studentId && i.Status == InvoiceStatus.Pending)
                .OrderByDescending(i => i.IssueDate)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                // Handle case where no pending invoice exists
                ModelState.AddModelError(string.Empty, "No pending invoice found.");
                return Page();
            }

            if (Input.Amount > invoice.AmountDue)
            {
                // Handle payment exceeding the amount due
                ModelState.AddModelError(string.Empty, "The payment amount cannot exceed the amount due.");
                return Page();
            }

            var payment = new Payment
            {
                StudentId = studentId,
                Amount = Input.Amount,
                PaymentMethod = Enum.Parse<PaymentMethod>(Input.PaymentMethod),
                PaymentDate = DateTime.UtcNow
            };

            // Save the payment
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Update the invoice status and adjust the final amount
            invoice.AmountDue -= Input.Amount;
            if (invoice.AmountDue == 0)
            {
                invoice.Status = InvoiceStatus.Paid;
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("/Payments/PaymentHistory");
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Currency)]
            public decimal Amount { get; set; }

            [Required]
            public string PaymentMethod { get; set; }
        }
    }
}
