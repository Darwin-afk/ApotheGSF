using System;
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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ApotheGSF.Controllers
{
    public class FacturasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;
        private ICompositeViewEngine _viewEngine;

        public FacturasController(AppDbContext context,
                                  IHttpContextAccessor accessor,
                                  ICompositeViewEngine viewEngine)
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _viewEngine = viewEngine;
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
            List<int> cajasUsar = new List<int>();
            List<int> cajasUsadas = new List<int>();
            Medicamentos? medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId && m.Inactivo == false).Include(m => m.MedicamentosCajas).FirstOrDefaultAsync();

            if (medicamento == null)//si despues de entrar el medicamento pasa a estar inactivo
            {
                return Json(GenerarPartialView(true, "Medicamento Invalido", viewModel));
            }

            if (Cantidad <= 0)//cantidad invalida
            {
                return Json(GenerarPartialView(true, "Cantidad Invalida", viewModel));
            }

            if (viewModel.MedicamentosDetalle.Count() > 0)//medicamento existente
            {
                foreach (var item in viewModel.MedicamentosDetalle)//obtener la posicion en la lista del medicamento existente pero con tipo cantidad diferente
                {
                    if (item.NombreMedicamento.Equals(medicamento.Nombre))
                    {
                        foreach(var cajaId in item.CajasId)
                        {
                            if (!cajasUsadas.Contains(cajaId))//si la caja no se ha verificado ya, para que asi no existan duplicados
                            {
                                cajasUsadas.Add(cajaId);
                            }
                        }
                    }
                }

                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas));
                }

            }
            else//medicamento nuevo
            {
                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas));
                }
            }
        }

        public ResultadoAjax AgregarCaja(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, List<int> cajasUsadas)
        {
            List<MedicamentosCajas> CajasSinDetallar = medicamento.MedicamentosCajas.Where(mc => mc.Detallada == false && mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            if (cajasUsadas.Count > 0)//excluir las cajas ya usadas por el mismo medicamento
            {
                for (int i = 0; i < CajasSinDetallar.Count; i++)
                {
                    if (cajasUsadas.Contains(CajasSinDetallar[i].CajaId))
                    {
                        CajasSinDetallar.Remove(CajasSinDetallar[i]);
                        i--;
                    }
                }
            }

            if (cantidad > CajasSinDetallar.Count)//si se quiere agregar mas cajas de las existentes
            {
                return GenerarPartialView(true, "Cantidad Superior a la Existente", viewModel);
            }


            for (int i = 0; i < cantidad; i++)
            {
                cajasUsar.Add(CajasSinDetallar[i].CajaId);
            }

            viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar);

            return GenerarPartialView(false, "", viewModel);
        }

        public ResultadoAjax AgregarUnidades(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, List<int> cajasUsadas)
        {
            int cantidadUsar = 0;
            int cantidadUsada = 0;
            int cantidadExtra = 0;
            MedicamentosCajas? cajaDetallada = null;
            List<MedicamentosCajas> cajas = medicamento.MedicamentosCajas.Where(mc => mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            if (cajasUsadas.Count > 0)
            {
                //--cantidadUsada = suma de las cantidades de todas las cajas
                foreach(var caja in cajas)
                {
                    if (cajasUsadas.Contains(caja.CajaId))
                        cantidadUsada += caja.CantidadUnidad;
                }

                //--cantidadUsada -= cada cantidad en los detalles
                var detallesAnteriores = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre);
                foreach (var detalle in detallesAnteriores)
                {
                    if(detalle.TipoCantidad == 1)
                        cantidadUsada -= detalle.CantidadUnidad * medicamento.UnidadesCaja;
                    else
                        cantidadUsada -= detalle.CantidadUnidad;
                }
                //--si cantidadUsada > 0 al final, se usara la ultima caja del detalle con tipo cantiadad 2(unidad) para el nuevo detalle
                if (cantidadUsada > 0)
                {
                    cajasUsar.Add(detallesAnteriores.Where(da => da.TipoCantidad == 2).Last().CajasId.Last());
                    cantidadUsar += cantidadUsada;
                }

                for (int i = 0; i < cajas.Count; i++)//excluir las cajas ya usadas
                {
                    if (cajasUsadas.Contains(cajas[i].CajaId))
                    {
                        cajas.Remove(cajas[i]);
                        i--;
                    }
                }
            }
            else
            {
                cajaDetallada = _context.MedicamentosCajas.Where(mc => mc.Detallada == true && mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).FirstOrDefault();
                if (cajaDetallada != null)
                {
                    cajasUsar.Add(cajaDetallada.CajaId);
                    cantidadUsar += cajaDetallada.CantidadUnidad;

                    cajas.Remove(cajaDetallada);


                }
            }

            if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
            {
                viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar);
                return GenerarPartialView(false, "", viewModel);
            }

            //si la cantidad que se pide es mayor a la que se usara
            for (int i = 0; i < cajas.Count; i++)
            {
                cajasUsar.Add(cajas[i].CajaId);
                cantidadUsar += cajas[i].CantidadUnidad;

                if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
                {
                    viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar);
                    return GenerarPartialView(false, "", viewModel);
                }
            }

            return GenerarPartialView(true, "Cantidad Superior a la Existente", viewModel);
        }

        public FacturaViewModel AgregarDetalle(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar)
        {
            MedicamentosDetalle detalle = new()
            {
                DetalleId = viewModel.MedicamentosDetalle.Count(),
                CajasId = cajasUsar,
                NombreMedicamento = medicamento.Nombre,
                TipoCantidad = tipoCantidad,
                CantidadUnidad = cantidad,
                Precio = medicamento.PrecioUnidad
            };

            if (tipoCantidad == 1)//cajas
                detalle.Precio = medicamento.PrecioUnidad * medicamento.UnidadesCaja;

            detalle.Total = detalle.Precio * cantidad;

            viewModel.MedicamentosDetalle.Add(detalle);


            return viewModel;
        }

        public ResultadoAjax GenerarPartialView(bool error, string mensaje, FacturaViewModel viewModel)
        {
            PartialViewResult partialViewResult = PartialView("MedicamentosDetalles", viewModel);
            string viewContent = ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            return new ResultadoAjax()
            {
                error = error,
                mensaje = mensaje,
                partial = viewContent,
            };
        }

        public string ConvertViewToString(ControllerContext controllerContext, PartialViewResult pvr, ICompositeViewEngine _viewEngine)
        {
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = _viewEngine.FindView(controllerContext, pvr.ViewName, false);
                ViewContext viewContext = new ViewContext(controllerContext, vResult.View, pvr.ViewData, pvr.TempData, writer, new HtmlHelperOptions());

                vResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoverMedicamento([Bind("MedicamentosDetalle")] FacturaViewModel viewModel, int RemoverId)
        {
            viewModel.MedicamentosDetalle.RemoveAt(RemoverId);

            //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
            for (int i = RemoverId; i < viewModel.MedicamentosDetalle.Count; i++)
            {
                viewModel.MedicamentosDetalle[i].DetalleId = i;
            }

            return PartialView("MedicamentosDetalles", viewModel);
        }
    }
}
