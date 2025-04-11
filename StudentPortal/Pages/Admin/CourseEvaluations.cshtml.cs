using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class CourseEvaluationsModel : PageModel
    {
        public ApplicationDbContext _context { get; set; }
        
        public StudentEvaluation[] CourseEvaluations { get; set; }

        public CourseEvaluationsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGetAsync()
        {
            // Fetch all course evaluations from the database
            CourseEvaluations = _context.StudentEvaluations
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToArray();
        }
    }
}
