using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Pages.Admin
{
    public class ViewClashingModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewClashingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // The list of timetable entries to display
        public List<Timetable> TimetableEntries { get; set; } = new();

        // The filter for student ID
        public int? StudentId { get; set; }

        // Filter to only show conflicts
        public bool ShowConflictsOnly { get; set; }

        public async Task<IActionResult> OnGetAsync(int? studentId, bool showConflictsOnly = false)
        {
            ShowConflictsOnly = showConflictsOnly;
            StudentId = studentId;

            // Get all entries (or filter by student ID if provided)
            var query = _context.Timetables
                .Include(e => e.Course)
                .AsQueryable();

            if (studentId.HasValue)
            {
                // Filter by StudentId and Enrollment status
                query = query
                    .Where(t => _context.Enrollments
                        .Any(e => e.StudentId == studentId && e.CourseId == t.CourseId && e.Status == EnrollmentStatus.Enrolled));
            }

            var allEntries = await query.ToListAsync();

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
