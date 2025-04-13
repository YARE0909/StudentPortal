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
            int? studentId = null;

            // Check if the studentId filter is provided in the query string
            if (Request.Query.ContainsKey("studentId") &&
                int.TryParse(Request.Query["studentId"], out var parsedStudentId))
            {
                studentId = parsedStudentId;
            }

            // Load enrollments, possibly filtered by studentId
            await LoadDataAsync(studentId);
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

            // Create the new enrollment record
            var enrollment = new Enrollment
            {
                StudentId = NewEnrollment.StudentId,
                CourseId = NewEnrollment.CourseId,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);

            // Log the action in AddDropHistory
            var historyEntry = new AddDropHistory
            {
                StudentId = NewEnrollment.StudentId,
                CourseId = NewEnrollment.CourseId,
                ActionType = ActionType.Added,
                ActionDate = DateTime.Now
            };
            _context.AddDropHistories.Add(historyEntry);

            // Update the invoice for the student
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.StudentId == NewEnrollment.StudentId);

            if (invoice != null)
            {
                // Get the course cost and add it to the AmountDue
                var courseCost = await _context.Courses
                    .Where(c => c.CourseId == NewEnrollment.CourseId)
                    .Select(c => c.CourseCost)
                    .FirstOrDefaultAsync();

                invoice.AmountDue += courseCost;
                invoice.FinalAmount = invoice.AmountDue; // Update FinalAmount
                _context.Invoices.Update(invoice);
            }
            else
            {
                // If no invoice exists, create a new one
                var newInvoice = new Invoice
                {
                    StudentId = NewEnrollment.StudentId,
                    AmountDue = await _context.Courses
                        .Where(c => c.CourseId == NewEnrollment.CourseId)
                        .Select(c => c.CourseCost)
                        .FirstOrDefaultAsync(),
                    FinalAmount = 0, // Initialize FinalAmount as needed
                    Status = InvoiceStatus.Pending, // Adjust status as needed
                    IssueDate = DateTime.Now
                };

                _context.Invoices.Add(newInvoice);
            }

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

                // Log the action in AddDropHistory
                var historyEntry = new AddDropHistory
                {
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    ActionType = ActionType.Added,
                    ActionDate = DateTime.Now
                };
                _context.AddDropHistories.Add(historyEntry);

                // Update the invoice for the student
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.StudentId == enrollment.StudentId);

                if (invoice != null)
                {
                    // Get the course cost and add it to the AmountDue
                    var courseCost = await _context.Courses
                        .Where(c => c.CourseId == enrollment.CourseId)
                        .Select(c => c.CourseCost)
                        .FirstOrDefaultAsync();

                    invoice.AmountDue += courseCost;
                    invoice.FinalAmount = invoice.AmountDue; // Update FinalAmount
                    _context.Invoices.Update(invoice);
                }
                else
                {
                    // If no invoice exists, create a new one
                    var newInvoice = new Invoice
                    {
                        StudentId = enrollment.StudentId,
                        AmountDue = await _context.Courses
                            .Where(c => c.CourseId == enrollment.CourseId)
                            .Select(c => c.CourseCost)
                            .FirstOrDefaultAsync(),
                        FinalAmount = 0, // Initialize FinalAmount as needed
                        Status = InvoiceStatus.Pending, // Adjust status as needed
                        IssueDate = DateTime.Now
                    };

                    _context.Invoices.Add(newInvoice);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }




        public async Task<IActionResult> OnPostDropAsync(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Course)  // Include the course to get the course cost
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment != null)
            {
                // Step 1: Create an AddDropHistory entry to log the action
                var historyEntry = new AddDropHistory
                {
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    ActionType = ActionType.Dropped,
                    ActionDate = DateTime.Now
                };
                _context.AddDropHistories.Add(historyEntry);

                // Step 2: Remove the enrollment record
                _context.Enrollments.Remove(enrollment);

                // Step 3: Update the Invoice (minus course cost)
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.StudentId == enrollment.StudentId);

                if (invoice != null)
                {
                    var courseCost = enrollment.Course.CourseCost;

                    // Subtract the course cost from the AmountDue
                    invoice.AmountDue = invoice.AmountDue - courseCost;

                    // Ensure the FinalAmount does not go below zero
                    invoice.FinalAmount = Math.Max(0, invoice.AmountDue);

                    // Save the changes to the invoice
                    _context.Invoices.Update(invoice);
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        private async Task LoadDataAsync(int? studentId = null)
        {
            var enrollmentsQuery = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            // Filter by studentId if provided
            if (studentId.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.StudentId == studentId.Value);
            }

            // Fetch the enrollments based on the filtered query
            Enrollments = await enrollmentsQuery.ToListAsync();

            // Load student and course options for the dropdowns
            StudentOptions = new SelectList(
                await _context.Students
                    .Select(s => new
                    {
                        s.StudentId,
                        Display = s.StudentId + " - " + s.FirstName + " " + s.LastName
                    }).ToListAsync(),
                "StudentId", "Display");

            CourseOptions = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");

            // Initialize the NewEnrollment property if it's null
            NewEnrollment ??= new NewEnrollmentInputModel();
        }
    }
}
