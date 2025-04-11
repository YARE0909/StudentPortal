// ManagePayments.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class ManagePaymentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public List<Payment> Payments { get; set; } = new();

        public ManagePaymentsModel(ApplicationDbContext context) => _context = context;

        public async Task OnGetAsync()
        {
            Payments = await _context.Payments
                .Include(p => p.Student)
                .ToListAsync();
        }
    }
}