using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentPortal.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        public void OnGet()
        {
            PaymentMethodOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "CreditCard", Text = "Credit Card" },
                new SelectListItem { Value = "DebitCard", Text = "Debit Card" },
                new SelectListItem { Value = "BankTransfer", Text = "Bank Transfer" },
                new SelectListItem { Value = "Cash", Text = "Cash" }
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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

            var payment = new Payment
            {
                StudentId = studentId,
                Amount = Input.Amount,
                PaymentMethod = Enum.Parse<PaymentMethod>(Input.PaymentMethod),
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
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
