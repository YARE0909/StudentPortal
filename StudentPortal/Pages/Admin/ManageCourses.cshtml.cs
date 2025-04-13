using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class ManageCoursesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<Course> Courses { get; set; } = new();

        // Bind the new course input
        [BindProperty]
        public NewCourseInputModel NewCourse { get; set; }

        public class NewCourseInputModel
        {
            [Required]
            [StringLength(20)]
            public string CourseCode { get; set; }

            [Required]
            [StringLength(100)]
            public string CourseName { get; set; }

            public string Description { get; set; }

            [Required]
            [Range(1, 10, ErrorMessage = "Credit hours must be between 1 and 10.")]
            public int CreditHours { get; set; }

            [Required]
            [StringLength(100)]
            public string Department { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Course cost must be positive.")]
            public int CourseCost { get; set; }
        }

        public ManageCoursesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Courses = await _context.Courses.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddCourseAsync()
        {
            if (!ModelState.IsValid)
            {
                Courses = await _context.Courses.ToListAsync();
                return Page();
            }

            //  Check for duplicate CourseCode
            var exists = await _context.Courses.AnyAsync(c => c.CourseCode == NewCourse.CourseCode);
            if (exists)
            {
                ModelState.AddModelError("NewCourse.CourseCode", "A course with this code already exists.");
                Courses = await _context.Courses.ToListAsync();
                return Page();
            }

            var course = new Course
            {
                CourseCode = NewCourse.CourseCode,
                CourseName = NewCourse.CourseName,
                Description = NewCourse.Description,
                CreditHours = NewCourse.CreditHours,
                Department = NewCourse.Department,
                CourseCost = NewCourse.CourseCost,
                CreatedAt = DateTime.Now,
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            //  Add success message
            TempData["SuccessMessage"] = "Course added successfully!";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // Refresh the page after deletion
        }

    }
}
