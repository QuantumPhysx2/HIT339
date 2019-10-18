using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tennis339.Data;
using Tennis339.ViewModels;

namespace Tennis339.Controllers
{
    [Authorize(Policy = "RequiresAdmin")]
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdministratorController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Coach.ToListAsync());
        }

        public IActionResult Details()
        {
            // GET all roles
            var roles = _roleManager.Roles;

            // Parse 'roles' to view
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new role
                IdentityRole _identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Create a new role
                IdentityResult _identityResult = await _roleManager.CreateAsync(_identityRole);
                if (_identityResult.Succeeded)
                {
                    return RedirectToAction("Details", "Administrator");
                }

                // Loop through any error found in the result
                foreach (IdentityError _identityError in _identityResult.Errors)
                {
                    // Return back the error + description
                    ModelState.AddModelError("", _identityError.Description);
                }
            }
            // Parse back 'CreateRoleViewModel' to use in the view
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // GET role
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            
            // Create an instance of the View Model inputs
            var model = new EditRoleViewModel
            {
                // Set input values with db values
                Id = role.Id,
                RoleName = role.Name
            };

            // Loop through all users in database
            foreach (var user in _userManager.Users)
            {
                // Check if user is in current role
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    // Add to a list to return back
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            // GET role
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();
            }
            else
            {
                // Read from 'AspNetUserRoles.Name' database
                role.Name = model.RoleName;

                // Update the current role
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Details");
                }

                // Loop through possible errors
                foreach (var error in result.Errors)
                {
                    // Return it back to the page
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.Roles.FirstOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // GET current role
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Delete the role
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("Details");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        public async Task<IActionResult> Modify(string id)
        {
            // Store the ID from GET request
            ViewBag.id = id;

            // Find associated role ID with GET id
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Create a list of the user roles
            var model = new List<UserRoleViewModel>();

            // Loop through database
            foreach (var user in _userManager.Users)
            {
                // Create an instance of the Users
                var userRoleViewModel = new UserRoleViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName
                };

                // Check if user matches current role
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                // Add to current view model
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Modify(List<UserRoleViewModel> model, string id)
        {
            // Compare current role ID with role ID in 'AspNetRoles' database
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            for (int userCount = 0; userCount < model.Count; userCount++)
            {
                // GET current user Id
                var currentUser = await _userManager.FindByIdAsync(model[userCount].Id);

                IdentityResult _identityResult = null;

                // Check role status of current user
                if (model[userCount].IsSelected && !(await _userManager.IsInRoleAsync(currentUser, role.Name)))
                {
                    // Add user to current role
                    _identityResult = await _userManager.AddToRoleAsync(currentUser, role.Name);
                }
                else if (!(model[userCount].IsSelected) && await _userManager.IsInRoleAsync(currentUser, role.Name))
                {
                    // Remove user from current role
                    _identityResult = await _userManager.RemoveFromRoleAsync(currentUser, role.Name);
                }
            }

            return RedirectToAction("Edit", new { Id = id });
        }
    }
}
