using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StudentPortal.Pages.Payments
{
    [Authorize]
    public class PaymentHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PaymentHistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Payment> Payments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the logged-in student’s ID from the claims.
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (studentIdClaim == null)
                return Unauthorized();

            int studentId = int.Parse(studentIdClaim);

            // Retrieve payments for the student, ordered by PaymentDate descending.
            Payments = await _context.Payments
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return Page();
        }
    }
}
