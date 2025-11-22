using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeanScene.Web.Data;
using BeanScene.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace BeanScene.Web.Controllers
{
    [Authorize(Roles = "Admin,Staff,Member")]
    public class ReservationsController : Controller
    {
        private readonly BeanSceneContext _context;

        public ReservationsController(BeanSceneContext context)
        {
            _context = context;
        }

        // GET: Reservations
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            var beanSceneContext = _context.Reservations.Include(r => r.Sitting);
            return View(await beanSceneContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Staff,Member")]
        public IActionResult Confirmation()
        {
            return View();
        }


        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Admin,Staff,Member")]
        public IActionResult Create()
        {
            // Generate time slots from 8:00 AM to 8:00 PM in 30-minute increments
            var open = new TimeSpan(8, 0, 0);
            var close = new TimeSpan(20, 0, 0);

            var timeSlots = new List<SelectListItem>();

            for (var t = open; t <= close; t = t.Add(TimeSpan.FromMinutes(30)))
            {
                var text = DateTime.Today.Add(t).ToString("h:mm tt"); // Display format: 8:00 AM
                var value = t.ToString(@"hh\:mm");                    // Stored value: 08:00

                timeSlots.Add(new SelectListItem { Text = text, Value = value });
            }

            ViewBag.StartTimeSlots = timeSlots;

            var sittings = _context.SittingSchedules
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.SittingScheduleId,
                    Display = $"{s.Stype} ({s.StartTime:hh\\:mm} – {s.EndTime:hh\\:mm})"
                })
                .ToList();

            ViewData["SittingId"] = new SelectList(sittings, "SittingScheduleId", "Display");

            var model = new Reservation();

            if (User.IsInRole("Member"))
            {
                model.ReservationSource = "Online";
            }

            return View(model);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff,Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,SittingId,FirstName,LastName,Email,Phone,StartTime,Duration,NumOfGuests,ReservationSource,Notes,CreatedAt")] Reservation reservation)
        {
            // Members booking type always set to "Online" 
            if (User.IsInRole("Member"))
            {
                reservation.ReservationSource = "Online";
            }

            // Decide which sitting (Breakfast / Lunch / Dinner) based on StartTime
            var timeOfDay = reservation.StartTime.TimeOfDay;   // e.g. 09:30, 13:00, 18:15

            string sittingType;

            if (timeOfDay >= new TimeSpan(8, 0, 0) && timeOfDay < new TimeSpan(12, 0, 0))
            {
                // 8:00 AM – 11:59 AM
                sittingType = "Breakfast";
            }
            else if (timeOfDay >= new TimeSpan(12, 0, 0) && timeOfDay < new TimeSpan(16, 0, 0))
            {
                // 12:00 PM – 3:59 PM
                sittingType = "Lunch";
            }
            else if (timeOfDay >= new TimeSpan(16, 0, 0) && timeOfDay <= new TimeSpan(20, 0, 0))
            {
                // 4:00 PM – 8:00 PM
                sittingType = "Dinner";
            }
            else
            {
                // Outside any sitting range – show a validation error
                ModelState.AddModelError("StartTime", "We only accept reservations between 8:00 AM and 8:00 PM.");
                ViewData["SittingId"] = new SelectList(_context.SittingSchedules, "SittingScheduleId", "SittingScheduleId", reservation.SittingId);
                return View(reservation);
            }

            // Look up the SittingSchedule row with that type
            var sitting = await _context.SittingSchedules
                .FirstOrDefaultAsync(s => s.Stype == sittingType);

            if (sitting == null)
            {
                // If your seeding/data is wrong and there is no Breakfast/Lunch/Dinner row
                ModelState.AddModelError("", $"No sitting found for type '{sittingType}'.");
                ViewData["SittingId"] = new SelectList(_context.SittingSchedules, "SittingScheduleId", "SittingScheduleId", reservation.SittingId);
                return View(reservation);
            }

            // Assign the correct SittingId based on the time
            reservation.SittingId = sitting.SittingScheduleId;

            if (ModelState.IsValid)
            {
                reservation.Status = "Pending";
                reservation.CreatedAt = DateTime.UtcNow;

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Confirmation));
            }

            ViewData["SittingId"] = new SelectList(_context.SittingSchedules, "SittingScheduleId", "SittingScheduleId", reservation.SittingId);
            return View(reservation);
        }



        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["SittingId"] = new SelectList(_context.SittingSchedules, "SittingScheduleId", "SittingScheduleId", reservation.SittingId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,SittingId,FirstName,LastName,Email,Phone,StartTime,Duration,NumOfGuests,ReservationSource,Notes,Status,CreatedAt")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
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
            ViewData["SittingId"] = new SelectList(_context.SittingSchedules, "SittingScheduleId", "SittingScheduleId", reservation.SittingId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Sitting)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
