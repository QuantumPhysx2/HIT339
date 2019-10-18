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
    public class ScheduleMembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ScheduleMembersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ScheduleMembers
        [Authorize(Policy = "RequiresAdmin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ScheduleMembers.ToListAsync());
        }

        // GET: ScheduleMembers/Details/5
        [Authorize(Policy = "RequiresAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleMembers = await _context.ScheduleMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleMembers == null)
            {
                return NotFound();
            }

            return View(scheduleMembers);
        }

        // GET: ScheduleMembers/Create
        [Authorize(Policy = "RequiresAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScheduleMembers/Create
        [Authorize(Policy = "RequiresAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ScheduleId,MemberEmail")] ScheduleMembers scheduleMembers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheduleMembers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scheduleMembers);
        }

        // GET: ScheduleMembers/Edit/5
        [Authorize(Policy = "RequiresAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleMembers = await _context.ScheduleMembers.FindAsync(id);
            if (scheduleMembers == null)
            {
                return NotFound();
            }
            return View(scheduleMembers);
        }

        // POST: ScheduleMembers/Edit/5
        [Authorize(Policy = "RequiresAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ScheduleId,MemberEmail")] ScheduleMembers scheduleMembers)
        {
            if (id != scheduleMembers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleMembers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleMembersExists(scheduleMembers.Id))
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
            return View(scheduleMembers);
        }

        // GET: ScheduleMembers/Delete/5
        [Authorize(Policy = "RequiresMember")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleMembers = await _context.ScheduleMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scheduleMembers == null)
            {
                return NotFound();
            }

            return View(scheduleMembers);
        }

        // POST: ScheduleMembers/Delete/5
        [ActionName("Delete")]
        [Authorize(Policy = "RequiresMember")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleMembers = await _context.ScheduleMembers.FindAsync(id);
            _context.ScheduleMembers.Remove(scheduleMembers);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Schedules");
        }

        // GET /ScheduleMembers/Enrollments/5
        [Authorize(Policy = "RequiresMember")]
        public IActionResult MyEnrollments()
        {
            // GET Username
            var user = _userManager.GetUserName(User);

            // Query db records that match Username
            var schedules = _context.ScheduleMembers.Where(m => m.MemberEmail == user);

            return View("MyEnrollments", schedules);
        }

        private bool ScheduleMembersExists(int id)
        {
            return _context.ScheduleMembers.Any(e => e.Id == id);
        }
    }
}
