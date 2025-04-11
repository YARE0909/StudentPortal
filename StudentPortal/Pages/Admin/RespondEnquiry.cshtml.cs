using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Admin
{
    public class RespondEnquiryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RespondEnquiryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Response is required.")]
        public string Response { get; set; }

        // The enquiry we are responding to.
        public Enquiry Enquiry { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Enquiry = await _context.Enquiries.Include(e => e.Student).FirstOrDefaultAsync(e => e.EnquiryId == id);
            if (Enquiry == null)
            {
                return NotFound();
            }
            // Pre-populate the response if one already exists
            Response = Enquiry.Response;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                Enquiry = await _context.Enquiries.Include(e => e.Student).FirstOrDefaultAsync(e => e.EnquiryId == id);
                return Page();
            }

            var enquiryToUpdate = await _context.Enquiries.FindAsync(id);
            if (enquiryToUpdate == null)
            {
                return NotFound();
            }

            // Update the Response
            enquiryToUpdate.Response = Response;
            _context.Enquiries.Update(enquiryToUpdate);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Response submitted successfully!";
            return RedirectToPage("/Admin/ManageEnquiries");
        }
    }
}
