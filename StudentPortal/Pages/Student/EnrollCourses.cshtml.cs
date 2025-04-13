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
        public bool IsRegistrationOpen { get; set; }

        [BindProperty]
        public List<int> SelectedCourseIds { get; set; } = new();

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            var student = await _context.Students.FindAsync(studentId);

            var registrationPeriod = await _context.RegistrationPeriods
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync();

            IsRegistrationOpen = registrationPeriod != null && registrationPeriod.IsActive && student != null && !student.HasEnrolledThisSession;

            EnrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            if (!IsRegistrationOpen)
            {
                TempData["Message"] = "You have already enrolled for this session or registration is closed.";
                return;
            }

            var allCourses = await _context.Courses.ToListAsync();

            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.CourseId)
                .ToListAsync();

            AvailableCourses = allCourses.Where(c => !enrolledCourseIds.Contains(c.CourseId)).ToList();
        }

        public async Task<IActionResult> OnPostEnrollAsync()
        {
            int studentId = GetCurrentStudentId();

            var student = await _context.Students.FindAsync(studentId);

            var registrationPeriod = await _context.RegistrationPeriods
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync();

            if (registrationPeriod == null || !registrationPeriod.IsActive || student == null || student.HasEnrolledThisSession)
            {
                TempData["Message"] = "You have already enrolled for this session or registration is closed.";
                return RedirectToPage();
            }

            if (SelectedCourseIds == null || !SelectedCourseIds.Any())
            {
                TempData["Message"] = "Please select at least one course to enroll.";
                return RedirectToPage();
            }

            // Check the current number of enrolled courses
            var currentEnrollmentsCount = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .CountAsync();

            if (currentEnrollmentsCount + SelectedCourseIds.Count > 5)
            {
                TempData["Message"] = "You cannot enroll in more than 5 courses at a time.";
                return RedirectToPage();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
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

                    // Total course fees
                    decimal totalAmount = 0;
                    foreach (var courseId in SelectedCourseIds)
                    {
                        var course = await _context.Courses.FindAsync(courseId);
                        if (course != null)
                        {
                            totalAmount += course.CourseCost;
                        }
                    }

                    var existingInvoice = await _context.Invoices
                        .Where(i => i.StudentId == studentId && i.Status == InvoiceStatus.Pending)
                        .FirstOrDefaultAsync();

                    if (existingInvoice != null)
                    {
                        existingInvoice.AmountDue += totalAmount;
                        existingInvoice.FinalAmount += totalAmount;
                        _context.Invoices.Update(existingInvoice);
                    }
                    else
                    {
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
                    }

                    // Mark student as enrolled for this session
                    student.HasEnrolledThisSession = true;
                    _context.Students.Update(student);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Message"] = "Successfully enrolled in selected courses and invoice updated!";
                    return RedirectToPage();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["Message"] = "An error occurred while processing your enrollment. Please try again.";
                    return RedirectToPage();
                }
            }
        }

        private int GetCurrentStudentId()
        {
            var claim = User.FindFirst("StudentId")?.Value;
            return int.Parse(claim);
        }
    }
}
