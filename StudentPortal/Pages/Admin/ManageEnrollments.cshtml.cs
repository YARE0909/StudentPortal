using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class ManageEnrollmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public List<Enrollment> Enrollments { get; set; } = new();

        public SelectList StudentOptions { get; set; }
        public SelectList CourseOptions { get; set; }

        [BindProperty]
        public NewEnrollmentInputModel NewEnrollment { get; set; }

        public class NewEnrollmentInputModel
        {
            [Required]
            public int StudentId { get; set; }

            [Required]
            public int CourseId { get; set; }
        }

        public ManageEnrollmentsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostEnrollAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var exists = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == NewEnrollment.StudentId &&
                e.CourseId == NewEnrollment.CourseId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Student is already enrolled in this course.");
                await LoadDataAsync();
                return Page();
            }

            var enrollment = new Enrollment
            {
                StudentId = NewEnrollment.StudentId,
                CourseId = NewEnrollment.CourseId,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);

            var historyEntry = new AddDropHistory
            {
                StudentId = NewEnrollment.StudentId,
                CourseId = NewEnrollment.CourseId,
                ActionType = ActionType.Added,
                ActionDate = DateTime.Now
            };
            _context.AddDropHistories.Add(historyEntry);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostReEnrollAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null && enrollment.Status == EnrollmentStatus.Dropped)
            {
                enrollment.Status = EnrollmentStatus.Enrolled;
                enrollment.EnrollmentDate = DateTime.Now;

                var historyEntry = new AddDropHistory
                {
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    ActionType = ActionType.Added,
                    ActionDate = DateTime.Now
                };
                _context.AddDropHistories.Add(historyEntry);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }



        public async Task<IActionResult> OnPostDropAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                var historyEntry = new AddDropHistory
                {
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    ActionType = ActionType.Dropped,
                    ActionDate = DateTime.Now
                };
                _context.AddDropHistories.Add(historyEntry);

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        private async Task LoadDataAsync()
        {
            Enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            StudentOptions = new SelectList(
    await _context.Students
        .Select(s => new
        {
            s.StudentId,
            Display = s.StudentId + " - " + s.FirstName + " " + s.LastName
        }).ToListAsync(),
    "StudentId", "Display");

            CourseOptions = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");

            NewEnrollment ??= new NewEnrollmentInputModel();
        }
    }
}
