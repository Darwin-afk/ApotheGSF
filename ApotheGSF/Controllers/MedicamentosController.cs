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
using System.Text;
using ReflectionIT.Mvc.Paging;
using System.Linq.Dynamic.Core;
using Rotativa;
using Rotativa.AspNetCore;

namespace ApotheGSF.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;

        public MedicamentosController(AppDbContext context,
                             IHttpContextAccessor accessor
            )
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        // GET: Medicamentos
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sortExpression = "", int search = 0)
        {
            StringBuilder filtro = new StringBuilder(" Inactivo == false ");
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<MedicamentosViewModel> listado = new List<MedicamentosViewModel>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                 select new MedicamentosViewModel
                                 {

                                     Codigo = meds.Codigo,
                                     Nombre = meds.Nombre,
                                     Categoria = meds.Categoria,
                                     Sustancia = meds.Sustancia,
                                     Concentracion = meds.Concentracion,
                                     UnidadesCaja = meds.UnidadesCaja,
                                     Costo = meds.Costo,
                                     PrecioUnidad = meds.PrecioUnidad,
                                     Indicaciones = meds.Indicaciones,
                                     Dosis = meds.Dosis,
                                     Inactivo = (bool)meds.Inactivo,
                                     Cajas = _context.MedicamentosCajas.Where(m => m.MedicamentoId == meds.Codigo).ToList().Count,

                                     NombreProveedor = string.Join(", ",
                                     (from p in _context.Proveedores
                                      .AsNoTracking()
                                      join provMed in _context.ProveedoresMedicamentos
                                      on new
                                      {
                                          ProveedoresId = p.Codigo,
                                          MedicamentosId = meds.Codigo
                                      } equals new
                                      {
                                          ProveedoresId = provMed.ProveedoresId,
                                          MedicamentosId = provMed.MedicamentosId
                                      }
                                      select new Proveedores
                                      {
                                          Nombre = p.Nombre
                                      }
                                      ).Select(x => x.Nombre).ToList())

                                 }).Where(filtro.ToString()).ToListAsync();

                listado = listado.Where(x => x.Inactivo == false).ToList();
            }
            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Nombre" : sortExpression;
            var model = PagingList.Create(listado, 3, pageindex, sortExpression, "");
            model.RouteValue = new RouteValueDictionary {
                            { "filter", filter}
            };
            model.Action = "Index";

            return model != null ?
                View(model) :
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
                                     select new MedicamentosViewModel
                                     {

                                         Codigo = meds.Codigo,
                                         Nombre = meds.Nombre,
                                         Categoria = meds.Categoria,
                                         Sustancia = meds.Sustancia,
                                         Concentracion = meds.Concentracion,
                                         UnidadesCaja = meds.UnidadesCaja,
                                         Costo = meds.Costo,
                                         PrecioUnidad = meds.PrecioUnidad,
                                         Indicaciones = meds.Indicaciones,
                                         Dosis = meds.Dosis,

                                         NombreProveedor = string.Join(", ",
                                         (from p in _context.Proveedores
                                          .AsNoTracking()
                                          join provMed in _context.ProveedoresMedicamentos
                                          on new
                                          {
                                              ProveedoresId = p.Codigo,
                                              MedicamentosId = meds.Codigo
                                          } equals new
                                          {
                                              ProveedoresId = provMed.ProveedoresId,
                                              MedicamentosId = provMed.MedicamentosId
                                          }
                                          select new Proveedores
                                          {
                                              Nombre = p.Nombre
                                          }
                                          ).Select(x => x.Nombre).ToList())

                                     }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            if (medicamento == null)
            {
                return NotFound();
            }

            ViewBag.Inventario = (List<MedicamentosCajas>)_context.MedicamentosCajas.Where(m => m.MedicamentoId == medicamento.Codigo).ToList();
            return View(medicamento);
        }

        // GET: Medicamentos/Create
        public IActionResult Create()
        {
            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores, "Codigo", "Nombre");
            return View();
        }

        // POST: Medicamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Categoria,Sustancia,Concentracion,UnidadesCaja,Costo,PrecioUnidad,Indicaciones,Dosis,ProveedoresId")] MedicamentosViewModel viewModel)
        {
            ModelState.Remove("NombreProveedor");

            if (ModelState.IsValid)
            {
                Medicamentos newMedicamentos = new()
                {
                    Nombre = viewModel.Nombre,
                    Categoria = viewModel.Categoria,
                    Sustancia = viewModel.Sustancia,
                    Concentracion = viewModel.Concentracion,
                    UnidadesCaja = viewModel.UnidadesCaja,
                    Costo = viewModel.Costo,
                    PrecioUnidad = viewModel.PrecioUnidad,
                    Indicaciones = viewModel.Indicaciones,
                    Dosis = viewModel.Dosis,
                    Creado = DateTime.Now,
                    CreadoNombreUsuario = _user.GetUserName(),
                    Modificado = DateTime.Now,
                    ModificadoNombreUsuario = _user.GetUserName(),
                    Inactivo = false

                };

                _context.Medicamentos.Add(newMedicamentos);

                foreach (var item in viewModel.ProveedoresId)
                {
                    ProveedorMedicamentos proveedorMedicamentos = new()
                    {
                        ProveedoresId = item

                    };

                    newMedicamentos.ProveedoresMedicamentos.Add(proveedorMedicamentos);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores, "Codigo", "Nombre");
            return View(viewModel);
        }

        // GET: Medicamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamentos = await (from meds in _context.Medicamentos
                                      select new MedicamentosViewModel
                                      {

                                          Codigo = meds.Codigo,
                                          Nombre = meds.Nombre,
                                          Categoria = meds.Categoria,
                                          Sustancia = meds.Sustancia,
                                          Concentracion = meds.Concentracion,
                                          UnidadesCaja = meds.UnidadesCaja,
                                          Costo = meds.Costo,
                                          PrecioUnidad = meds.PrecioUnidad,
                                          Indicaciones = meds.Indicaciones,
                                          Dosis = meds.Dosis
                                      }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            if (medicamentos == null)
            {
                return NotFound();
            }

            medicamentos.ProveedoresId = await (from provMeds in _context.ProveedoresMedicamentos
                                               .Where(x => x.MedicamentosId == medicamentos.Codigo)
                                               .AsNoTracking()
                                                join proveedores in _context.Proveedores on provMeds
                                               .ProveedoresId equals proveedores.Codigo
                                                select proveedores.Codigo).ToListAsync();

            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores, "Codigo", "Nombre", medicamentos.ProveedoresId);

            return View(medicamentos);
        }

        // POST: Medicamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,Nombre,Categoria,Sustancia,Concentracion,Costo,PrecioUnidad,UnidadesCaja,Indicaciones,Dosis,ProveedoresId")] MedicamentosViewModel viewModel)
        {
            if (id != viewModel.Codigo)
            {
                return NotFound();
            }

            ModelState.Remove("NombreProveedor");

            if (ModelState.IsValid)
            {
                try
                {
                    var editmedicamento = await _context.Medicamentos.Include(x => x.ProveedoresMedicamentos).
                                       FirstOrDefaultAsync(y => y.Codigo == id);

                    editmedicamento.Nombre = viewModel.Nombre;
                    editmedicamento.Categoria = viewModel.Categoria;
                    editmedicamento.Sustancia = viewModel.Sustancia;
                    editmedicamento.Concentracion = viewModel.Concentracion;
                    editmedicamento.UnidadesCaja = viewModel.UnidadesCaja;
                    editmedicamento.Costo = viewModel.Costo;
                    editmedicamento.PrecioUnidad = viewModel.PrecioUnidad;
                    editmedicamento.Indicaciones = viewModel.Indicaciones;
                    editmedicamento.Dosis = viewModel.Dosis;
                    editmedicamento.Modificado = DateTime.Now;
                    editmedicamento.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Update(editmedicamento);


                    foreach (var proveedor in editmedicamento.ProveedoresMedicamentos.ToList())
                    {
                        if (!viewModel.ProveedoresId.Contains(proveedor.ProveedoresId))
                        {
                            editmedicamento.ProveedoresMedicamentos.Remove(proveedor);
                        }
                    }

                    foreach (var newProveedorId in viewModel.ProveedoresId)
                    {
                        if (!editmedicamento.ProveedoresMedicamentos.Any(x => x.ProveedoresId == newProveedorId))
                        {
                            var nuevoProv = new ProveedorMedicamentos
                            {
                                ProveedoresId = newProveedorId

                            };
                            editmedicamento.ProveedoresMedicamentos.Add(nuevoProv);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentoExists(viewModel.Codigo))
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

            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores, "Codigo", "Nombre", viewModel.ProveedoresId);
            return View(viewModel);
        }

        // GET: Medicamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }


            var medicamento = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                     select new MedicamentosViewModel
                                     {

                                         Codigo = meds.Codigo,
                                         Nombre = meds.Nombre,
                                         Categoria = meds.Categoria,
                                         Sustancia = meds.Sustancia,
                                         Concentracion = meds.Concentracion,
                                         UnidadesCaja = meds.UnidadesCaja,
                                         Costo = meds.Costo,
                                         PrecioUnidad = meds.PrecioUnidad,
                                         Indicaciones = meds.Indicaciones,
                                         Dosis = meds.Dosis,
                                         Cajas = _context.MedicamentosCajas.Where(m => m.MedicamentoId == meds.Codigo).ToList().Count,

                                         NombreProveedor = string.Join(", ",
                                         (from p in _context.Proveedores
                                          .AsNoTracking()
                                          join provMed in _context.ProveedoresMedicamentos
                                          on new
                                          {
                                              ProveedoresId = p.Codigo,
                                              MedicamentosId = meds.Codigo
                                          } equals new
                                          {
                                              ProveedoresId = provMed.ProveedoresId,
                                              MedicamentosId = provMed.MedicamentosId
                                          }
                                          select new Proveedores
                                          {
                                              Nombre = p.Nombre
                                          }
                                          ).Select(x => x.Nombre).ToList())

                                     }).Where(x => x.Codigo == id).FirstOrDefaultAsync();


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
                _context.Medicamentos.Update(medicamento);
                medicamento.Modificado = DateTime.Now;
                medicamento.ModificadoNombreUsuario = _user.GetUserName();
                medicamento.Inactivo = true;
                _context.Entry(medicamento).Property(c => c.Creado).IsModified = false;
                _context.Entry(medicamento).Property(c => c.CreadoNombreUsuario).IsModified = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentoExists(int id)
        {
            return (_context.Medicamentos?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ReporteInventario(string filter)
        {
            StringBuilder filtro = new StringBuilder(" Inactivo == false ");

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<MedicamentosViewModel> medicamentos = await (from meds in _context.Medicamentos
                                                              .AsNoTracking()
                                                              .AsQueryable()
                                                              select new MedicamentosViewModel
                                                              {

                                                                  Codigo = meds.Codigo,
                                                                  Nombre = meds.Nombre,
                                                                  Categoria = meds.Categoria,
                                                                  Sustancia = meds.Sustancia,
                                                                  Concentracion = meds.Concentracion,
                                                                  UnidadesCaja = meds.UnidadesCaja,
                                                                  Costo = meds.Costo,
                                                                  PrecioUnidad = meds.PrecioUnidad,
                                                                  Indicaciones = meds.Indicaciones,
                                                                  Dosis = meds.Dosis,
                                                                  Inactivo = (bool)meds.Inactivo,
                                                                  Cajas = _context.MedicamentosCajas.Where(m => m.MedicamentoId == meds.Codigo).ToList().Count,

                                                                  NombreProveedor = string.Join(", ",
                                                                  (from p in _context.Proveedores
                                                                   .AsNoTracking()
                                                                   join provMed in _context.ProveedoresMedicamentos
                                                                   on new
                                                                   {
                                                                       ProveedoresId = p.Codigo,
                                                                       MedicamentosId = meds.Codigo
                                                                   } equals new
                                                                   {
                                                                       ProveedoresId = provMed.ProveedoresId,
                                                                       MedicamentosId = provMed.MedicamentosId
                                                                   }
                                                                   select new Proveedores
                                                                   {
                                                                       Nombre = p.Nombre
                                                                   }
                                                                   ).Select(x => x.Nombre).ToList())

                                                              }).Where(filtro.ToString()).ToListAsync();

            return new ViewAsPdf("ReporteInventario", medicamentos);
        }
    }
}
