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

        [BindProperty]
        public List<int> SelectedCourseIds { get; set; } = new();


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

        public async Task<IActionResult> OnPostEnrollAsync()
        {
            int studentId = GetCurrentStudentId();

            // Get how many courses the student is already enrolled in
            var alreadyEnrolledCount = await _context.Enrollments
                .CountAsync(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled);

            if (SelectedCourseIds == null || !SelectedCourseIds.Any())
            {
                TempData["Message"] = "Please select at least one course to enroll.";
                return RedirectToPage();
            }

            int remainingSlots = 5 - alreadyEnrolledCount;

            if (SelectedCourseIds.Count > remainingSlots)
            {
                TempData["Message"] = $"You can only enroll in {remainingSlots} more course(s).";
                return RedirectToPage();
            }

            // Enroll in selected courses
            foreach (var courseId in SelectedCourseIds)
            {
                bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e =>
                    e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Enrolled);

                if (!alreadyEnrolled)
                {
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

                    _context.Enrollments.Add(enrollment);
                    _context.AddDropHistories.Add(historyEntry);
                }
            }

            // Generate Invoice (if needed)
            decimal totalAmount = 0;
            foreach (var courseId in SelectedCourseIds)
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course != null)
                {
                    totalAmount += course.CourseCost; // Using CourseCost for invoice calculation
                }
            }

            var invoice = new Invoice
            {
                StudentId = studentId,
                AmountDue = totalAmount,
                FinalAmount = totalAmount,
                Adjustment = 0,
                Status = InvoiceStatus.Pending,
                IssueDate = DateTime.Now
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(); // Save invoice to get InvoiceId

            TempData["Message"] = "Successfully enrolled in selected courses and invoice generated!";
            return RedirectToPage();
        }




        private int GetCurrentStudentId()
        {
            var claim = User.FindFirst("StudentId")?.Value;
            return int.Parse(claim);
        }
    }
}
