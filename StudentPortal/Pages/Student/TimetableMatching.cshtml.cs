using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Pages.Student
{
    public class TimetableMatchingModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TimetableMatchingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> EnrolledCourses { get; set; } = new();

        [BindProperty]
        public TimetableConflictInputModel Input { get; set; }

        public class TimetableConflictInputModel
        {
            [Required(ErrorMessage = "Preferred Course is required.")]
            public string PreferredCourse { get; set; }

            [Required(ErrorMessage = "Preferred Day is required.")]
            public string PreferredDay { get; set; }

            [Required(ErrorMessage = "Preferred Time Slot is required.")]
            public string PreferredTimeSlot { get; set; }

            [StringLength(500)]
            public string Remarks { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            // Fetch the student's enrolled courses
            EnrolledCourses = await _context.Enrollments
                                            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                                            .Select(e => e.Course)
                                            .ToListAsync();

            Input = new TimetableConflictInputModel();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            int studentId = GetCurrentStudentId();

            // Create a new timetable conflict report
            var conflictReport = new TimetableConflictReport
            {
                StudentId = studentId,
                PreferredCourse = Input.PreferredCourse,
                PreferredDay = Input.PreferredDay,
                PreferredTimeSlot = Input.PreferredTimeSlot,
                Remarks = Input.Remarks,
            };

            _context.TimetableConflictReports.Add(conflictReport);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your timetable conflict report has been submitted successfully!";
            return RedirectToPage();
        }

        private int GetCurrentStudentId()
        {
            // Ensure you are correctly extracting the logged-in student's ID
            return int.Parse(User.FindFirst("StudentId")?.Value);
        }
    }
}
