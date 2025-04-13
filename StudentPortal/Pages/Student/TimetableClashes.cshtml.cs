using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Pages.Student
{
    public class TimetableClashesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TimetableClashesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TimetableConflictReport> TimetableClashes { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Fetch all timetable conflict reports from the database
            TimetableClashes = await _context.TimetableConflictReports
                .Include(t => t.Student)  // Optionally include related student info
                .OrderByDescending(t => t.DateSubmitted)
                .ToListAsync();
        }
    }
}
