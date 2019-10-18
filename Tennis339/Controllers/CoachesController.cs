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
    public class CoachesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoachesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Coaches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coach.ToListAsync());
        }

        // GET: Coaches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coach = await _context.Coach.FirstOrDefaultAsync(m => m.Id == id);

            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // GET: Coaches/Create
        [Authorize(Policy = "RequiresCoach")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coaches/Create
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Biography,PhotoUrl")] Coach coach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coach);
        }

        // GET: Coaches/Edit/5
        [Authorize(Policy = "RequiresCoach")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coach = await _context.Coach.FindAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        // POST: Coaches/Edit/5
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Biography,PhotoUrl")] Coach coach)
        {
            if (id != coach.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachExists(coach.Id))
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
            return View(coach);
        }

        // GET: Coaches/Delete/5
        [Authorize(Policy = "RequiresCoach")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coach = await _context.Coach.FirstOrDefaultAsync(m => m.Id == id);

            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        // POST: Coaches/Delete/5
        [ActionName("Delete")]
        [Authorize(Policy = "RequiresCoach")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coach = await _context.Coach.FindAsync(id);
            _context.Coach.Remove(coach);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET Coaches/MySchedules/
        [Authorize(Policy = "RequiresCoach")]
        public IActionResult MySchedules()
        {
            // GET current User
            var user = _userManager.GetUserName(User);

            // Query db to match all records with same CoachEmail
            var schedules = _context.Schedule.Where(m => m.CoachEmail == user);

            return View("MySchedules", schedules);
        }

        // GET Coaches/Schedules/5
        [Authorize(Policy = "RequiresCoach")]
        public IActionResult Schedules(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // Query db to find matching ID based on GET Id
            var member = _context.ScheduleMembers.Where(m => m.ScheduleId == id);

            return View("Schedules", member);
        }

        private bool CoachExists(int id)
        {
            return _context.Coach.Any(e => e.Id == id);
        }
    }
}
