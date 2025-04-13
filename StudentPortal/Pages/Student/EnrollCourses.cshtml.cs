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
        public bool IsRegistrationOpen { get; set; } // Flag to check if registration is open

        [BindProperty]
        public List<int> SelectedCourseIds { get; set; } = new();

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            // Check if registration period is open
            var registrationPeriod = await _context.RegistrationPeriods
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync();

            IsRegistrationOpen = registrationPeriod != null && registrationPeriod.IsActive;

            EnrolledCourses = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Include(e => e.Course)
                .Select(e => e.Course)
                .ToListAsync();

            if (!IsRegistrationOpen)
            {
                TempData["Message"] = "The registration session is closed. You cannot enroll in courses.";
                return; // End method if registration is closed
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

            // Check if registration period is open
            var registrationPeriod = await _context.RegistrationPeriods
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync();

            if (registrationPeriod == null || !registrationPeriod.IsActive)
            {
                TempData["Message"] = "The registration session is closed. You cannot enroll in courses.";
                return RedirectToPage();
            }

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

            // Begin transaction for atomicity
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
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

                    // Calculate the total amount
                    decimal totalAmount = 0;
                    foreach (var courseId in SelectedCourseIds)
                    {
                        var course = await _context.Courses.FindAsync(courseId);
                        if (course != null)
                        {
                            totalAmount += course.CourseCost;
                        }
                    }

                    // Check if there is an existing invoice for the student
                    var existingInvoice = await _context.Invoices
                        .Where(i => i.StudentId == studentId && i.Status == InvoiceStatus.Pending)
                        .FirstOrDefaultAsync();

                    if (existingInvoice != null)
                    {
                        // Update the existing invoice
                        existingInvoice.AmountDue += totalAmount;
                        existingInvoice.FinalAmount += totalAmount;
                        _context.Invoices.Update(existingInvoice);
                    }
                    else
                    {
                        // Create a new invoice if none exists
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

                    // Save changes within the transaction
                    await _context.SaveChangesAsync();

                    // Commit the transaction to make the changes permanent
                    await transaction.CommitAsync();

                    TempData["Message"] = "Successfully enrolled in selected courses and invoice updated!";
                    return RedirectToPage();
                }
                catch (Exception)
                {
                    // Rollback transaction if something goes wrong
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

