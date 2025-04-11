using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Admin
{
    public class ManageTimetableModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageTimetableModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Timetable> TimetableEntries { get; set; } = new();
        public SelectList CourseOptions { get; set; }

        [BindProperty]
        public NewTimetableEntryInputModel NewEntry { get; set; }

        public class NewTimetableEntryInputModel
        {
            [Required]
            public int CourseId { get; set; }

            [Required]
            public DayOfWeekEnum DayOfWeek { get; set; }

            [Required]
            public TimeSpan StartTime { get; set; }

            [Required]
            public TimeSpan EndTime { get; set; }

            [StringLength(50)]
            public string RoomNumber { get; set; }
        }

        public async Task OnGetAsync()
        {
            CourseOptions = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            TimetableEntries = await _context.Timetables
                                    .Include(t => t.Course)
                                    .ToListAsync();
            NewEntry ??= new NewTimetableEntryInputModel();
        }

        public async Task<IActionResult> OnPostAddEntryAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var entry = new Timetable
            {
                CourseId = NewEntry.CourseId,
                DayOfWeek = NewEntry.DayOfWeek,
                StartTime = NewEntry.StartTime,
                EndTime = NewEntry.EndTime,
                RoomNumber = NewEntry.RoomNumber
            };

            _context.Timetables.Add(entry);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteEntryAsync(int id)
        {
            var entry = await _context.Timetables.FindAsync(id);
            if (entry != null)
            {
                _context.Timetables.Remove(entry);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
