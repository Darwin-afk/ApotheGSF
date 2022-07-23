﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;
using System.Security.Claims;
using ApotheGSF.Clases;

namespace ApotheGSF.Controllers
{
    public class FacturasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;

        public FacturasController(AppDbContext context,
                                  IHttpContextAccessor accessor)
        {
            _context = context;
            _user = accessor.HttpContext.User;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            return _context.Facturas != null ?
                        View(await _context.Facturas.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Facturas'  is null.");
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            /*
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (factura == null)
            {
                return NotFound();
            }
            */
            return View();
        }

        // GET: Facturas/Create
        public IActionResult Create()
        {
            ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
            //FacturaViewModel factura = new FacturaViewModel()
            //{
            //    MedicamentosDetalle = new List<MedicamentosDetalle>()
            //    {
            //        new MedicamentosDetalle
            //        {
            //            DetalleId = 3,
            //            CajasId = new List<int>()
                        
            //        }
            //    }
            //};
            //return View(factura);
            return View(new FacturaViewModel());
        }

        // POST: Facturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Bind("Codigo,FechaCreacion,SubTotal,Total,Estado,Medicamentos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public List<MedicamentosDetalle> Create([Bind("Codigo,FechaCreacion,SubTotal,Total,Estado,MedicamentosDetalle")] FacturaViewModel viewModel)
        {
            return viewModel.MedicamentosDetalle;
            //if (ModelState.IsValid)
            //{
            //    List<FacturaMedicamentosCajas> facturaMedicamentos = new();
            //    //foreach (var i in viewModel.MedicamentosDetalle)
            //    //{
            //    //    facturaMedicamentos.Add(new FacturaMedicamentosCajas
            //    //    {
            //    //        CajaId = i.CajaId,
            //    //        CantidadUnidad = i.CantidadUnidad,
            //    //        Precio = i.Precio
            //    //    });
            //    //}

            //    Facturas nuevaFactura = new()
            //    {
            //        FechaCreacion = viewModel.FechaCreacion,
            //        SubTotal = viewModel.SubTotal,
            //        Total = viewModel.Total,
            //        Estado = viewModel.Estado,
            //        FacturasMedicamentosCajas = facturaMedicamentos,
            //        Creado = DateTime.Now,
            //        Modificado = DateTime.Now,
            //        Inactivo = false
            //    };

            //    _context.Add(nuevaFactura);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(viewModel);
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            /*
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await (from f in _context.Facturas
                                 .AsNoTracking()
                                 .AsQueryable()
                                 .Include(fmc => fmc.FacturasMedicamentosCajas)
                                 join fm in _context.FacturasMedicamentosCajas on f.Codigo equals fm.FacturaId
                                 select new FacturaViewModel
                                 {
                                     Codigo = f.Codigo,
                                     FechaCreacion = f.FechaCreacion,
                                     SubTotal = f.SubTotal,
                                     Total = f.Total,
                                     Estado = f.Estado,
                                     Medicamentos = (List<FacturaMedicamentosCajas>)f.FacturasMedicamentosCajas
                                 }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }
            */
            return View();
        }

        // POST: Facturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo")] Facturas factura)
        {
            if (id != factura.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.Codigo))
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
            return View(factura);
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            /*
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (factura == null)
            {
                return NotFound();
            }
            */
            return View();
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facturas == null)
            {
                return Problem("Entity set 'AppDbContext.Facturas'  is null.");
            }
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
            return (_context.Facturas?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarMedicamento([Bind("MedicamentosDetalle")] FacturaViewModel viewModel, int MedicamentoId, int TipoCantidad, int Cantidad)
        {
            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId).Include(m => m.MedicamentosCajas).FirstOrDefaultAsync();
            MedicamentosDetalle detalle;
            var medicamentosCajas = medicamento.MedicamentosCajas.Where(mc => mc.CantidadUnidad != 0).ToList();
            var medicamentosCajasSinDetalle = medicamentosCajas.Where(mc => mc.Detallada == false).ToList();
            var medicamentosCajasConDetalle = medicamentosCajas.Where(mc => mc.Detallada == true).ToList();

            if (TipoCantidad == 1)//Cajas
            {
                //En caso de que se quiera agregar mas cajas de lo que se tenga en inventario
                if (Cantidad > medicamentosCajasSinDetalle.Count())
                {
                    return PartialView("MedicamentosDetalles", viewModel);
                }

                detalle = new()
                {
                    DetalleId = viewModel.MedicamentosDetalle.Count(),
                    CajasId = new List<int>(),
                    NombreMedicamento = medicamento.Nombre,
                    TipoCantidad = TipoCantidad,
                    Cantidad = Cantidad,
                    Precio = medicamento.PrecioUnidad * medicamento.UnidadesCaja,
                    Total = medicamento.PrecioUnidad * medicamento.UnidadesCaja * Cantidad
                };

                for (int i = 0; i < Cantidad; i++)
                {
                    detalle.CajasId.Add(medicamentosCajasSinDetalle[i].CajaId);
                }

                viewModel.MedicamentosDetalle.Add(detalle);
                return PartialView("MedicamentosDetalles", viewModel);
            }
            else//Unidades
            {
                //En caso de que se quiera agregar mas unidades de lo que se puede tener en una caja
                if (Cantidad > medicamento.UnidadesCaja)
                {
                    return PartialView("MedicamentosDetalles", viewModel);
                }

                foreach (var item in medicamentosCajasConDetalle)
                {
                    if (item.CantidadUnidad >= Cantidad)
                    {
                        detalle = new()
                        {
                            DetalleId = viewModel.MedicamentosDetalle.Count(),
                            CajasId = new() { item.CajaId },
                            NombreMedicamento = medicamento.Nombre,
                            TipoCantidad = TipoCantidad,
                            Cantidad = Cantidad,
                            Precio = medicamento.PrecioUnidad,
                            Total = medicamento.PrecioUnidad * Cantidad
                        };
                        viewModel.MedicamentosDetalle.Add(detalle);
                        return PartialView("MedicamentosDetalles", viewModel);
                    }
                }

                if (medicamentosCajasSinDetalle.Count() > 0)
                {
                    detalle = new()
                    {
                        DetalleId = viewModel.MedicamentosDetalle.Count(),
                        CajasId = new() { medicamentosCajasSinDetalle[0].CajaId },
                        NombreMedicamento = medicamento.Nombre,
                        TipoCantidad = TipoCantidad,
                        Cantidad = Cantidad,
                        Precio = medicamento.PrecioUnidad,
                        Total = medicamento.PrecioUnidad * Cantidad
                    };
                }
                else
                {
                    detalle = new() { Cantidad = 0 };
                }

                viewModel.MedicamentosDetalle.Add(detalle);
                return PartialView("MedicamentosDetalles", viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverMedicamento([Bind("MedicamentosDetalle")] FacturaViewModel viewModel, int RemoverId)
        {
            viewModel.MedicamentosDetalle.RemoveAt(RemoverId);

            //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
            for(int i = RemoverId; i < viewModel.MedicamentosDetalle.Count; i++)
            {
                viewModel.MedicamentosDetalle[i].DetalleId = i;
            }

            return PartialView("MedicamentosDetalles", viewModel);
        }
    }
}
