using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Student
{
    [Authorize]
    public class EnrollCoursesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EnrollCoursesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> AvailableCourses { get; set; } = new();

        public List<Course> EnrolledCourses { get; set; } = new();

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            var allCourses = await _context.Courses.ToListAsync();

            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.CourseId)
                .ToListAsync();

            AvailableCourses = allCourses.Where(c => !enrolledCourseIds.Contains(c.CourseId)).ToList();

            EnrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostEnrollAsync(int courseId)
        {
            int studentId = GetCurrentStudentId();

            bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Enrolled);

            if (alreadyEnrolled)
            {
                TempData["Message"] = "You are already enrolled in that course.";
                return RedirectToPage();
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = EnrollmentStatus.Enrolled
            };

            var historyEntry = new AddDropHistory
            {
                StudentId = studentId,
                CourseId = courseId,
                ActionType = ActionType.Added,
                ActionDate = DateTime.Now
            };
            _context.AddDropHistories.Add(historyEntry);


            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Successfully enrolled in the course!";
            return RedirectToPage();
        }

        private int GetCurrentStudentId()
        {
            var claim = User.FindFirst("StudentId")?.Value;
            return int.Parse(claim);
        }
    }
}
