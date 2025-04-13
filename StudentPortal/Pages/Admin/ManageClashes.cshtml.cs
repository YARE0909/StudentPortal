using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Admin
{
    public class ManageClashesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageClashesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TimetableConflictReport> ClashReports { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch all timetable conflict reports with student details
            ClashReports = await _context.TimetableConflictReports
                .Include(t => t.Student) // To include the student's name
                .OrderByDescending(t => t.DateSubmitted)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int reportId)
        {
            var report = await _context.TimetableConflictReports.FindAsync(reportId);
            if (report == null)
            {
                return NotFound();
            }

            // Mark the clash report as noted
            report.IsNoted = true;
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Clash has been acknowledged.";
            return RedirectToPage();
        }
    }
}
