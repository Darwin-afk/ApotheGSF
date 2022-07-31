using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;

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

            var medicamentosCajas = await (from mc in _context.MedicamentosCajas
                                           .AsNoTracking()
                                           .AsQueryable()
                                           join m in _context.Medicamentos on mc.MedicamentoId equals m.Codigo
                                           select new MedicamentosCajasViewModel
                                           {
                                               CajaId = mc.CajaId,
                                               MedicamentoId = mc.MedicamentoId,
                                               NombreMedicamento = m.Nombre,
                                               CantidadUnidad = mc.CantidadUnidad,
                                               FechaAdquirido = mc.FechaAdquirido,
                                               FechaVencimiento = mc.FechaVencimiento,
                                               Detallada = mc.Detallada
                                           }).Where(x => x.CajaId == id).FirstOrDefaultAsync();

            if (medicamentosCajas == null)
            {
                return NotFound();
            }

            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Create
        public IActionResult Create()
        {
            ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
            return View();
        }

        // POST: MedicamentosCajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicamentoId,Cajas,FechaAdquirido,FechaVencimiento")] MedicamentosCajasViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var unidades = await _context.Medicamentos.Where(m => m.Codigo == viewModel.MedicamentoId)
                                                                            .Select(m => m.UnidadesCaja)
                                                                            .FirstOrDefaultAsync();

                for (int i = 0; i < viewModel.Cajas; i++)
                {
                    MedicamentosCajas medicamentoCaja = new();
                    medicamentoCaja.CantidadUnidad = await _context.Medicamentos.Where(m => m.Codigo == viewModel.MedicamentoId)
                                                                                .Select(m => m.UnidadesCaja)
                                                                                .FirstOrDefaultAsync();
                    medicamentoCaja.MedicamentoId = viewModel.MedicamentoId;
                    medicamentoCaja.FechaAdquirido = viewModel.FechaAdquirido;
                    medicamentoCaja.FechaVencimiento = viewModel.FechaVencimiento;
                    medicamentoCaja.Detallada = false;
                    medicamentoCaja.Inactivo = false;

                    _context.Add(medicamentoCaja);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
            return View(viewModel);
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
            ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.MedicamentoId).FirstOrDefault().Nombre;

            return View(medicamentosCajas);
        }

        // POST: MedicamentosCajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CajaId,MedicamentoId,CantidadUnidad,FechaAdquirido,FechaVencimiento")] MedicamentosCajas medicamentosCajas)
        {
            if (id != medicamentosCajas.CajaId)
            {
                return NotFound();
            }

            //obtener el medicamento de la caja
            Medicamentos medicamento = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.MedicamentoId).FirstOrDefault();
            //si cantidadUnidad es mayor  unidadesCaja del medicamento se regresa el error
            if (medicamentosCajas.CantidadUnidad > medicamento.UnidadesCaja)
            {
                ModelState.AddModelError("", "Cantidad Superior a la Valida");
            }

            if (ModelState.IsValid && medicamentosCajas.CantidadUnidad <= medicamento.UnidadesCaja)
            {


                try
                {
                    _context.Update(medicamentosCajas);
                    _context.Entry(medicamentosCajas).Property(m => m.Detallada).IsModified = false;
                    _context.Entry(medicamentosCajas).Property(m => m.Inactivo).IsModified = false;
                    _context.Entry(medicamentosCajas).Property(m => m.MedicamentoId).IsModified = false;
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
                return RedirectToAction("Details", "Medicamentos", new { id = medicamentosCajas.MedicamentoId });
            }
            ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.MedicamentoId).FirstOrDefault().Nombre;
            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await (from mc in _context.MedicamentosCajas
                                           .AsNoTracking()
                                           .AsQueryable()
                                           join m in _context.Medicamentos on mc.MedicamentoId equals m.Codigo
                                           select new MedicamentosCajasViewModel
                                           {
                                               CajaId = mc.CajaId,
                                               MedicamentoId = mc.MedicamentoId,
                                               NombreMedicamento = m.Nombre,
                                               CantidadUnidad = mc.CantidadUnidad,
                                               FechaAdquirido = mc.FechaAdquirido,
                                               FechaVencimiento = mc.FechaVencimiento,
                                               Detallada = mc.Detallada
                                           }).Where(x => x.CajaId == id).FirstOrDefaultAsync();

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
                _context.MedicamentosCajas.Update(medicamentosCajas);
                medicamentosCajas.Inactivo = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Medicamentos", new { id = medicamentosCajas.MedicamentoId });
        }

        private bool MedicamentosCajasExists(int id)
        {
            return (_context.MedicamentosCajas?.Any(e => e.CajaId == id)).GetValueOrDefault();
        }
    }
}
