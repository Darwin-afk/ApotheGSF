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
            List<int>? cajasUsadas = null;
            bool existente = false;
            int detalleId = 0;
            Medicamentos? medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId && m.Inactivo == false).Include(m => m.MedicamentosCajas).FirstOrDefaultAsync();

            if (medicamento == null)//si despues de entrar el medicamento pasa a estar inactivo
            {
                return Json(GenerarPartialView(true, "Medicamento Invalido", ref viewModel));
            }

            if (Cantidad <= 0)//cantidad invalida
            {
                return Json(GenerarPartialView(true, "Cantidad Invalida", ref viewModel));
            }

            if (viewModel.MedicamentosDetalle.Count() > 0)//medicamento existente
            {
                foreach (var item in viewModel.MedicamentosDetalle)//obtener la posicion en la lista del medicamento existente
                {
                    if (item.NombreMedicamento.Equals(medicamento.Nombre))
                    {
                        detalleId = item.DetalleId;
                    }
                }

                if (viewModel.MedicamentosDetalle[detalleId].TipoCantidad == TipoCantidad)//se agregara mas cantidad al detalle existente
                {
                    existente = true;

                    cajasUsar = viewModel.MedicamentosDetalle[detalleId].CajasId;

                    foreach (var item in viewModel.MedicamentosDetalle)//obtener la posicion en la lista del medicamento existente pero con tipo cantidad diferente
                    {
                        if (item.NombreMedicamento.Equals(medicamento.Nombre) && item.TipoCantidad != TipoCantidad)
                        {
                            cajasUsadas = item.CajasId;

                        }
                    }

                    if (TipoCantidad == 1)//cajas
                    {
                        return Json(AgregarCaja(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                    }
                    else//unidades
                    {
                        return Json(AgregarUnidades(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                    }
                }
                else//se creara el mismo medicamento pero con tipo cantidad diferente
                {
                    cajasUsadas = viewModel.MedicamentosDetalle[detalleId].CajasId;

                    if (TipoCantidad == 1)//cajas
                    {
                        return Json(AgregarCaja(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                    }
                    else//unidades
                    {
                        return Json(AgregarUnidades(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                    }
                }
            }
            else//medicamento nuevo
            {
                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(ref viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
            }
        }

        public ResultadoAjax AgregarCaja(ref FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId)
        {
            List<MedicamentosCajas> CajasSinDetallar = medicamento.MedicamentosCajas.Where(mc => mc.Detallada == false && mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            if (cajasUsadas != null)//excluir las cajas ya usadas por el mismo medicamento en unidades
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

            if (cajasUsar.Count > 0)//si se agregara mas a el detalle existente excluir las ya usadas
            {
                for (int i = 0; i < CajasSinDetallar.Count; i++)
                {
                    if (cajasUsar.Contains(CajasSinDetallar[i].CajaId))
                    {
                        CajasSinDetallar.Remove(CajasSinDetallar[i]);
                        i--;
                    }
                }
            }

            if (cantidad > CajasSinDetallar.Count)//si se quiere agregar mas cajas de las existentes
            {
                return GenerarPartialView(true, "Cantidad Superior a la existente", ref viewModel);
            }


            for (int i = 0; i < cantidad; i++)
            {
                cajasUsar.Add(CajasSinDetallar[i].CajaId);
            }

            viewModel = AgregarDetalle(ref viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);

            return GenerarPartialView(false, "", ref viewModel);
        }

        public ResultadoAjax AgregarUnidades(ref FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId)
        {
            int cantidadUsar = 0;
            int cantidadExtra = 0;
            MedicamentosCajas? cajaDetallada = null;
            List<MedicamentosCajas> Cajas = medicamento.MedicamentosCajas.Where(mc => mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            if (cajasUsar.Count > 0)//si se va a agregar mas cantidad al mismo medicamento
            {
                cantidadExtra = Cajas.Where(c => c.CajaId == cajasUsar.Last()).FirstOrDefault().CantidadUnidad - viewModel.MedicamentosDetalle[detalleId].CantidadUnidad;
                if (cantidadExtra != 0)
                    cantidadUsar = cantidadExtra;

                foreach (var item in Cajas)
                {
                    if (cajasUsar.Contains(item.CajaId))
                        cantidadUsar += item.CantidadUnidad;
                }

                for (int i = 0; i < Cajas.Count; i++)
                {
                    if (cajasUsar.Contains(Cajas[i].CajaId))
                    {
                        Cajas.Remove(Cajas[i]);
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

                    Cajas.Remove(cajaDetallada);

                    
                }

                if (cajasUsadas != null)
                {
                    for (int i = 0; i < Cajas.Count; i++)//excluir las cajas ya usadas por el mismo medicamento en cajas
                    {
                        if (cajasUsadas.Contains(Cajas[i].CajaId))
                        {
                            Cajas.Remove(Cajas[i]);
                            i--;
                        }
                    }
                }
            }

            if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
            {
                viewModel = AgregarDetalle(ref viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                return GenerarPartialView(false, "", ref viewModel);
            }
            //si la cantidad que se pide es mayor a la que se usara

            for (int i = 0; i < Cajas.Count; i++)
            {
                cajasUsar.Add(Cajas[i].CajaId);
                cantidadUsar += Cajas[i].CantidadUnidad;

                if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
                {
                    viewModel = AgregarDetalle(ref viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                    return GenerarPartialView(false, "", ref viewModel);
                }
            }

            return GenerarPartialView(true, "Cantidad Superior a la existente", ref viewModel);
        }

        public FacturaViewModel AgregarDetalle(ref FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, bool existente, int detalleId)
        {
            if (existente == true)
            {
                viewModel.MedicamentosDetalle[detalleId].CajasId = cajasUsar;
                viewModel.MedicamentosDetalle[detalleId].CantidadUnidad += cantidad;
                viewModel.MedicamentosDetalle[detalleId].Total = viewModel.MedicamentosDetalle[detalleId].Precio * viewModel.MedicamentosDetalle[detalleId].CantidadUnidad;
            }
            else
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
            }

            return viewModel;
        }

        public ResultadoAjax GenerarPartialView(bool error, string mensaje, ref FacturaViewModel viewModel)
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
