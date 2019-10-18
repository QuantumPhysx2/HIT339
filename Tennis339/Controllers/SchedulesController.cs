using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tennis339.Data;
using Tennis339.Models;

namespace Tennis339.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SchedulesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(string searchString)
        {
            var coaches = from m in _context.Schedule
                          select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                coaches = coaches.Where(m => m.CoachEmail.Contains(searchString));
            }

            return View(await coaches.ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        [Authorize(Policy = "RequiresCoach")]
        public IActionResult Create()
        {
            // Create an instance of the 'Coach' db and store as a list
            IEnumerable<Coach> model = _context.Coach.ToList();

            // Create an instance of the view model
            ViewModels.ScheduleViewModel viewModel = new ViewModels.ScheduleViewModel
            {
                Coach = model
            };

            // Return multiple models to 'Create.cshtml' view
            return View(viewModel);
        }

        // POST: Schedules/Create
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,When,Description,CoachEmail,Location")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        [Authorize(Policy = "RequiresCoach")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Edit/5
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,When,Description,CoachEmail,Location")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        [Authorize(Policy = "RequiresCoach")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [ActionName("Delete")]
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Schedules/Enroll/5
        public async Task<IActionResult> Enroll(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // GET: id
            var schedule = await _context.Schedule.FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Enroll
        [ActionName("Enroll")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollConfirmed(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                if (schedule.When < DateTime.Now)
                {
                    // Prevent enrollment after due date
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var user = _userManager.GetUserName(User);

                    // Create a new database record
                    ScheduleMembers member = new ScheduleMembers
                    {
                        ScheduleId = schedule.Id,
                        MemberEmail = user
                    };

                    // Add a new Context change
                    _context.Add(member);

                    // Store the Enrollment
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
