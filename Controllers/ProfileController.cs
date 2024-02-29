using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNet.Identity;
using Arkenea_new.Data;
using Arkenea_new.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Arkenea_new.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ApplicationDbContext _dbContext;

        public ProfileController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;

        }
        public async Task<IActionResult> Profile()
        {
            string userId = User.Identity.GetUserId();
            var userProfile = await _dbContext.UserProfiles.Include(p => p.Address).FirstOrDefaultAsync(p => p.UserID == userId);

            if (userProfile == null)
            {
                ViewBag.Message = "No profile is present. Please update one.";
                return View("NoProfile");
            }
            else
            {
                ViewBag.UserProfile = userProfile;
            }
            return View();
        }

        public IActionResult EditProfileDetails()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfileDetails(UserProfileModel profileModel)
        {
            string userId = User.Identity.GetUserId();
            profileModel.UserID = userId;
            if (!ModelState.IsValid)
            {
                return View(profileModel);
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.FirstName = profileModel.FirstName;
                await _dbContext.SaveChangesAsync();
            }
            

            if (profileModel.ResumeFile != null && profileModel.ResumeFile.Length > 0)
            {
                // Save resume file
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + profileModel.ResumeFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileModel.ResumeFile.CopyToAsync(stream);
                }
                profileModel.Resume = uniqueFileName;
            }

            var existingProfile = await _dbContext.UserProfiles.Include(p => p.Address).FirstOrDefaultAsync(p => p.UserID == userId);

            if (existingProfile == null)
            {
                // Create a new profile if it doesn't exist
                profileModel.UserID = userId;
                _dbContext.UserProfiles.Add(profileModel);
            }
            else
            {
                // Update the existing profile
                existingProfile.FirstName = profileModel.FirstName;
                existingProfile.LastName = profileModel.LastName;
                existingProfile.Username = profileModel.Username;
                existingProfile.PhoneNumber = profileModel.PhoneNumber;
                existingProfile.ProfilePhoto = profileModel.ProfilePhoto;

                // Update resume file if uploaded
                if (!string.IsNullOrEmpty(profileModel.Resume))
                {
                    existingProfile.Resume = profileModel.Resume;
                }

                // Update address if it exists
                if (existingProfile.Address != null && profileModel.Address != null)
                {
                    existingProfile.Address.City = profileModel.Address.City;
                    existingProfile.Address.State = profileModel.Address.State;
                    existingProfile.Address.Pincode = profileModel.Address.Pincode;
                }
                else if (existingProfile.Address == null && profileModel.Address != null)
                {
                    // Add address if it doesn't exist
                    profileModel.Address.Id = 0; // Ensure a new address is added
                    existingProfile.Address = profileModel.Address;
                }
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Profile");
        }


        public async Task<IActionResult> DownloadResume()
        {
            string userId = User.Identity.GetUserId();
            var userProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (userProfile == null || string.IsNullOrEmpty(userProfile.Resume))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", userProfile.Resume);
            return PhysicalFile(filePath, "application/octet-stream", Path.GetFileName(filePath));
        }

    }
}
