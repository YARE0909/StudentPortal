using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Student
{
    public class EnquiriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EnquiriesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Enquiry> PastEnquiries { get; set; } = new();

        [BindProperty]
        public EnquiryInputModel Input { get; set; }

        public class EnquiryInputModel
        {
            [Required(ErrorMessage = "Subject is required.")]
            [StringLength(255)]
            public string Subject { get; set; }

            [Required(ErrorMessage = "Message is required.")]
            public string Message { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            PastEnquiries = await _context.Enquiries
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            Input = new EnquiryInputModel();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            int studentId = GetCurrentStudentId();

            var enquiry = new Enquiry
            {
                StudentId = studentId,
                Subject = Input.Subject,
                Message = Input.Message,
                Response = "",
                CreatedAt = DateTime.Now
            };

            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your enquiry has been submitted successfully!";
            return RedirectToPage();
        }

        private int GetCurrentStudentId()
        {
             return int.Parse(User.FindFirst("StudentId")?.Value);
        }
    }
}
