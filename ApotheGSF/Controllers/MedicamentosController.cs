using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using System.Security.Claims;
using ApotheGSF.Clases;
using ApotheGSF.ViewModels;

namespace ApotheGSF.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;

        public MedicamentosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Medicamentos
        public async Task<IActionResult> Index()
        {
        

            var lista = await (from meds in _context.Medicamentos
                              .AsNoTracking()
                              .AsQueryable()
                               join m in _context.ProveedoresMedicamentos on meds.Codigo equals m.MedicamentosId
                               join p in _context.Proveedores  on m.ProveedoresId equals p.Codigo
                               select new Medicamentos
                               {
                                   Codigo = meds.Codigo,
                                   Nombre = meds.Nombre,
                                   NombreProveedor = p.Nombre,
                                   Marca = meds.Marca,
                                   Categoria = meds.Categoria,
                                   PrecioUnidad = meds.PrecioUnidad
                                   
                               }).ToListAsync();

            return lista != null ?
                View(lista) :
                Problem("Entity set 'ApplicationDbContext.ApplicationUser'  is null.");
                ;
        }

        // GET: Medicamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamento = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                               join m in _context.ProveedoresMedicamentos on meds.Codigo equals m.MedicamentosId
                               join p in _context.Proveedores on m.ProveedoresId equals p.Codigo
                               select new Medicamentos
                               {
                                   Codigo = meds.Codigo,
                                   Nombre = meds.Nombre,
                                   NombreProveedor = p.Nombre,
                                   Marca = meds.Marca,
                                   Categoria = meds.Categoria,
                                   PrecioUnidad = meds.PrecioUnidad,
                                   CreadoNombreUsuario = meds.CreadoNombreUsuario,
                                   ModificadoNombreUsuario = meds.ModificadoNombreUsuario
                               }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            return View(medicamento);
        }

        // GET: Medicamentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Marca,Categoria,Sustancia,UnidadesPorCaja,Concentracion,Costo,PrecioUnidad,Indicaciones,Dosis")] Medicamentos medicamento)
        {
            if (ModelState.IsValid)
            {
                medicamento.Creado = DateTime.Now;
                medicamento.CreadoNombreUsuario = _user.GetUserName();
                medicamento.Modificado = DateTime.Now;
                medicamento.ModificadoNombreUsuario = _user.GetUserName();
                _context.Add(medicamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicamento);
        }

        // GET: Medicamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null)
            {
                return NotFound();
            }
            return View(medicamento);
        }

        // POST: Medicamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo")] Medicamentos medicamento)
        {
            if (id != medicamento.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentoExists(medicamento.Codigo))
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
            return View(medicamento);
        }

        // GET: Medicamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamentos
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        // POST: Medicamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Medicamentos == null)
            {
                return Problem("Entity set 'AppDbContext.Medicamentos'  is null.");
            }
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento != null)
            {
                _context.Medicamentos.Remove(medicamento);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentoExists(int id)
        {
          return (_context.Medicamentos?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
