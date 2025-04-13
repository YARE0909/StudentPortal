using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortal.Pages.Admin
{
    public class RegistrationSessionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistrationSessionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public bool IsActive { get; set; }

        // OnGet is used to check the current registration period status
        public void OnGet()
        {
            var period = _context.RegistrationPeriods.FirstOrDefault(r => r.IsActive);
            IsActive = period != null && period.IsActive;
        }

        // OnPost is used to handle the form submission for opening/closing the session
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var existingPeriod = await _context.RegistrationPeriods.FirstOrDefaultAsync();

                // If an existing period is found, update it. Otherwise, create a new one.
                if (existingPeriod != null)
                {
                    existingPeriod.IsActive = IsActive;
                }
                else
                {
                    var newPeriod = new RegistrationPeriod
                    {
                        IsActive = IsActive
                    };
                    _context.RegistrationPeriods.Add(newPeriod);
                }

                // If the session is opened, reset the enrollment flags for all students
                if (IsActive)
                {
                    var students = await _context.Students.ToListAsync();
                    foreach (var student in students)
                    {
                        student.HasEnrolledThisSession = false; // Reset enrollment status
                    }
                    _context.Students.UpdateRange(students);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Set a success message
                TempData["Message"] = $"Registration session has been {(IsActive ? "opened" : "closed")}.";
                return RedirectToPage(); // Redirect to refresh the page and show the message
            }

            return Page(); // Return the current page in case of validation failure
        }
    }
}
