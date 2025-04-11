using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Student
{
    [Authorize]
    public class EvaluateCourseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EvaluateCourseModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dropdown for courses the student is enrolled in.
        public SelectList CourseOptions { get; set; }

        [BindProperty]
        public EvaluationInputModel NewEvaluation { get; set; }

        public class EvaluationInputModel
        {
            [Required(ErrorMessage = "Please select a course.")]
            public int CourseId { get; set; }

            [Required(ErrorMessage = "Please provide a rating.")]
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
            public int Rating { get; set; }

            public string Feedback { get; set; }
        }

        public async Task OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            // Load courses the student is enrolled in (you may customize this query)
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.CourseId)
                .ToListAsync();

            var courses = await _context.Courses
                .Where(c => enrolledCourseIds.Contains(c.CourseId))
                .ToListAsync();

            CourseOptions = new SelectList(courses, "CourseId", "CourseName");

            // Initialize NewEvaluation if null.
            NewEvaluation ??= new EvaluationInputModel();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            int studentId = GetCurrentStudentId();

            var evaluation = new StudentEvaluation
            {
                StudentId = studentId,
                CourseId = NewEvaluation.CourseId,
                Rating = NewEvaluation.Rating,
                Feedback = NewEvaluation.Feedback,
                CreatedAt = DateTime.Now
            };

            _context.StudentEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Thank you for your feedback!";
            return RedirectToPage();
        }

        // Replace this with your actual logic to get the student ID; for demonstration, using a hard-coded value.
        private int GetCurrentStudentId()
        {
             return int.Parse(User.FindFirst("StudentId")?.Value);
        }
    }
}
