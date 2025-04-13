using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class CourseEvaluationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentEvaluation[] CourseEvaluations { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public CourseEvaluationsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            var query = _context.StudentEvaluations
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e => e.Course.CourseCode.Contains(SearchTerm));
            }

            CourseEvaluations = query.ToArray();
        }
    }
}
