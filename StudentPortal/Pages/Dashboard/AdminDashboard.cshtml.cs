using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Security.Claims;

namespace StudentPortal.Pages.Dashboard
{
    [Authorize(Roles = "Admin,Staff")] // Only authorized admins or staff can access this page
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string AdminEmail { get; set; }
        public int StudentCount { get; set; }
        public int CourseCount { get; set; }
        public int PaymentCount { get; set; }
        public int EnrollmentCount { get; set; }
        public int EnquiryCount { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task OnGetAsync()
        {
            // Assumes that the admin's email is stored in the Name claim
            AdminEmail = User.Identity.Name;
            StudentCount = await _context.Students.CountAsync();
            CourseCount = await _context.Courses.CountAsync();
            PaymentCount = await _context.Payments.CountAsync();
            EnrollmentCount = await _context.Enrollments.CountAsync();
            EnquiryCount = await _context.Enquiries.CountAsync();
            InvoiceCount = await _context.Invoices.CountAsync();
            TotalRevenue = await _context.Payments.SumAsync(p => p.Amount);
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Account/AdminLogin");
        }
    }
}
