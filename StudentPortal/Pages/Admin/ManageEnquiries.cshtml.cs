using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Admin
{
    public class ManageEnquiriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ManageEnquiriesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all enquiries with related student information
        public List<Enquiry> Enquiries { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load all enquiries (you might filter by a date range, etc., if desired)
            Enquiries = await _context.Enquiries
                .Include(e => e.Student)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
    }
}
