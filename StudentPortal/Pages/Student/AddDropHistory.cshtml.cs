using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Student
{
    [Authorize]
    public class AddDropHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddDropHistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List to store the add/drop history for the student
        public List<AddDropHistory> HistoryItems { get; set; } = new();

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            HistoryItems = await _context.AddDropHistories
                                .Where(h => h.StudentId == studentId)
                                .Include(h => h.Course)
                                .OrderByDescending(h => h.ActionDate)
                                .ToListAsync();
        }

        // For testing purposes, this returns a hard-coded value.
        // In production, retrieve the student ID from User.Claims.
        private int GetCurrentStudentId()
        {
            var claim = User.FindFirst("StudentId")?.Value;
            return int.Parse(claim);
        }
    }
}
