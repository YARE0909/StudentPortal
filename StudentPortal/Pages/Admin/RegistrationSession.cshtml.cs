using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System;
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

        public void OnGet()
        {
            var period = _context.RegistrationPeriods.FirstOrDefault(r => r.Id == 1);

            if (period != null)
            {
                IsActive = period.IsActive;
            }
            else
            {
                IsActive = false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Try to get the existing record (assumes only one)
                var existingPeriod = await _context.RegistrationPeriods.FirstOrDefaultAsync();

                if (existingPeriod != null)
                {
                    // Update existing entry
                    existingPeriod.IsActive = IsActive;
                }
                else
                {
                    // Create new entry if none exists
                    var newPeriod = new RegistrationPeriod
                    {
                        IsActive = IsActive
                    };
                    _context.RegistrationPeriods.Add(newPeriod);
                }

                // Save changes
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Registration session has been {(IsActive ? "opened" : "closed")}.";
                return RedirectToPage();
            }

            return Page();
        }

    }

}

