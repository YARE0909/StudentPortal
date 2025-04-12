using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;
namespace StudentPortal.Pages.Admin
{
    public class TimetableMatchingcshtmlModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TimetableMatchingcshtmlModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Timetable> TimetableEntries { get; set; } = new();
        public bool ShowConflictsOnly { get; set; }

        public async Task<IActionResult> OnGetAsync(bool showConflictsOnly = false)
        {
            ShowConflictsOnly = showConflictsOnly;

            // Get all entries (or filter to current student if needed)
            var allEntries = await _context.Timetables
                .Include(e => e.Course)
                .ToListAsync();

            if (ShowConflictsOnly)
            {
                // Filter only time slots that have > 1 entry (conflicts)
                TimetableEntries = allEntries
                    .GroupBy(e => new { e.DayOfWeek, e.StartTime, e.EndTime })
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .ToList();
            }
            else
            {
                TimetableEntries = allEntries;
            }

            return Page();
        }
    }
}
