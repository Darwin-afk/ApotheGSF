using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;

namespace ApotheGSF.Controllers
{
    public class MedicamentosCajasController : Controller
    {
        private readonly AppDbContext _context;

        public MedicamentosCajasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MedicamentosCajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await _context.MedicamentosCajas
                .FirstOrDefaultAsync(m => m.CajaId == id);
            if (medicamentosCajas == null)
            {
                return NotFound();
            }

            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Create
        public IActionResult Create()
        {
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos, "Codigo", "Nombre");
            return View();
        }

        // POST: MedicamentosCajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CantidadUnidad,FechaAdquirido,FechaVencimiento,Detallada")] MedicamentosCajas medicamentosCajas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicamentosCajas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos, "Codigo", "Nombre");
            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await _context.MedicamentosCajas.FindAsync(id);
            if (medicamentosCajas == null)
            {
                return NotFound();
            }
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos, "Codigo", "Nombre", medicamentosCajas.MedicamentosId);
            return View(medicamentosCajas);
        }

        // POST: MedicamentosCajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CajaId,CantidadUnidad,FechaAdquirido,FechaVencimiento,Detallada")] MedicamentosCajas medicamentosCajas)
        {
            if (id != medicamentosCajas.CajaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicamentosCajas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentosCajasExists(medicamentosCajas.CajaId))
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
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos, "Codigo", "Nombre", medicamentosCajas.MedicamentosId);
            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await _context.MedicamentosCajas
                .FirstOrDefaultAsync(m => m.CajaId == id);
            if (medicamentosCajas == null)
            {
                return NotFound();
            }

            return View(medicamentosCajas);
        }

        // POST: MedicamentosCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MedicamentosCajas == null)
            {
                return Problem("Entity set 'AppDbContext.MedicamentosCajas'  is null.");
            }
            var medicamentosCajas = await _context.MedicamentosCajas.FindAsync(id);
            if (medicamentosCajas != null)
            {
                _context.MedicamentosCajas.Remove(medicamentosCajas);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentosCajasExists(int id)
        {
          return (_context.MedicamentosCajas?.Any(e => e.CajaId == id)).GetValueOrDefault();
        }
    }
}
