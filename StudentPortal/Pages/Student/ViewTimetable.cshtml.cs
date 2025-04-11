using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Student
{
    [Authorize]
    public class ViewTimetableModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewTimetableModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Assuming that students see only the timetable entries for courses in which they are enrolled.
        // You'll need to filter based on the current student's ID.
        public List<Timetable> TimetableEntries { get; set; } = new();

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            // Get timetable entries for courses the student is enrolled in.
            // Adjust the query if necessary to match your relationships.
            TimetableEntries = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Include(e => e.Course)
                .SelectMany(e => _context.Timetables.Where(t => t.CourseId == e.CourseId))
                .Include(t => t.Course)
                .ToListAsync();
        }

        // Replace with your real logic to get the student ID from claims.
        private int GetCurrentStudentId()
        {
            var claim = User.FindFirst("StudentId")?.Value;
            return int.Parse(claim);
        }
    }
}
