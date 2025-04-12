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
            public int StartTime { get; set; }  // Store as an hour (e.g., 8 for 8 AM)

            [Required]
            public int Duration { get; set; }  // Duration in hours

            [StringLength(50)]
            public string RoomNumber { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Populate course options
            CourseOptions = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");

            // Load timetable entries along with related courses
            TimetableEntries = await _context.Timetables
                                            .Include(t => t.Course)
                                            .ToListAsync();

            // Initialize NewEntry for the form
            NewEntry ??= new NewTimetableEntryInputModel();
        }

        public async Task<IActionResult> OnPostAddEntryAsync()
        {
            // Validate if the form is not valid
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Validate that end time is after start time
            var endTime = NewEntry.StartTime + NewEntry.Duration;

            // Ensure the class doesn't end after 6 PM
            if (endTime > 18)
            {
                ModelState.AddModelError("NewEntry.Duration", "Class cannot end after 6 PM.");
                await OnGetAsync();
                return Page();
            }

            // Create new timetable entry and add it to the database
            var entry = new Timetable
            {
                CourseId = NewEntry.CourseId,
                DayOfWeek = NewEntry.DayOfWeek,
                StartTime = TimeSpan.FromHours(NewEntry.StartTime),
                EndTime = TimeSpan.FromHours(endTime),
                RoomNumber = NewEntry.RoomNumber
            };

            _context.Timetables.Add(entry);
            await _context.SaveChangesAsync();

            // Add a success message to TempData
            TempData["SuccessMessage"] = "Timetable entry added successfully!";

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteEntryAsync(int id)
        {
            // Find the timetable entry to delete
            var entry = await _context.Timetables.FindAsync(id);
            if (entry != null)
            {
                // Remove the entry from the database
                _context.Timetables.Remove(entry);
                await _context.SaveChangesAsync();

                // Add a success message to TempData
                TempData["SuccessMessage"] = "Timetable entry deleted successfully!";
            }

            return RedirectToPage();
        }
    }
}
