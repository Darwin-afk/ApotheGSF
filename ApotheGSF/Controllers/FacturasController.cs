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
        public async Task<IActionResult> Create()
        {
            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                      select new MedicamentosViewModel
                                      {

                                          Codigo = meds.Codigo,
                                          Nombre = meds.Nombre,
                                          Inactivo = (bool)meds.Inactivo,
                                          Cajas = _context.MedicamentosCajas.Where(m => m.MedicamentoId == meds.Codigo).ToList().Count

                                      }).Where(x => x.Inactivo == false).ToListAsync();
            //usar los medicamentos que tengan alguna caja en inventario
            ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

            return View(new FacturaViewModel());
        }

        // POST: Facturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubTotal,Total,Estado,MedicamentosDetalle")] FacturaViewModel viewModel)
        {
            string error = VerificarInventario(ref viewModel);

            //si hubo alguno error al verificial la disponibilidad en el inventario se regresa al view
            if (!error.Equals(""))//si no esta vacio
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                List<FacturaMedicamentosCajas> facturaMedicamentos = new();
                //foreach (var i in viewModel.MedicamentosDetalle)
                //{
                //    facturaMedicamentos.Add(new FacturaMedicamentosCajas
                //    {
                //        CajaId = i.CajaId,
                //        CantidadUnidad = i.CantidadUnidad,
                //        Precio = i.Precio
                //    });
                //}

                Facturas nuevaFactura = new()
                {
                    FechaCreacion = viewModel.FechaCreacion,
                    SubTotal = viewModel.SubTotal,
                    Total = viewModel.Total,
                    Estado = viewModel.Estado,
                    FacturasMedicamentosCajas = facturaMedicamentos,
                    Creado = DateTime.Now,
                    Modificado = DateTime.Now,
                    Inactivo = false
                };

                _context.Add(nuevaFactura);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        private string VerificarInventario(ref FacturaViewModel viewModel)
        {
            List<string> errores = new();
            //por cada detalle de medicamentosDetalle
            foreach (var detalle in viewModel.MedicamentosDetalle)
            {
                //si el tipoCantidad es caja
                if (detalle.TipoCantidad == 1)//caja
                {
                    //por cada cajaId del detalle
                    for (int i = 0; i < detalle.CajasId.Count; i++)
                    {
                        //verificar si esta inactivo
                        if (_context.MedicamentosCajas.Where(mc => mc.CajaId == detalle.CajasId[i]).FirstOrDefault().Inactivo)
                        {
                            //obtener un listo con todos los detalles del mismo medicamento
                            List<MedicamentosDetalle> otrosMedicamentos = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == detalle.NombreMedicamento).ToList();
                            List<int> cajasOcupadas = new();
                            foreach (var otrosDetalle in otrosMedicamentos)
                            {
                                cajasOcupadas.AddRange(otrosDetalle.CajasId);
                            }
                            //obtener un cajaId que no sea igual a algun cajaId del listado
                            int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.MedicamentoId == medicamentoId && mc.Inactivo == false && mc.Detallada == false).ToList();

                            cajas = ExcluirCajas(cajas, cajasOcupadas);

                            if (cajas.Count > 0)
                            {
                                //entonces remplazar el cajaId inactivo por el nuevo cajaId
                                detalle.CajasId[i] = cajas.FirstOrDefault().CajaId;
                            }
                            else
                            {
                                //si no hay algun otro cajaId para remplazarlo se mostrar un error en el view
                                return "No hay existencia suficiente para el medicamento({detalle.DetalleId + 1}).";
                            }
                        }
                    }
                }
                else//unidades
                {
                    //obtiene la lista de cajas del medicamento que no esten inactivas
                    int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                    List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.MedicamentoId == medicamentoId && mc.Inactivo == false).ToList();

                    //por cada cajaId del detalle
                    for (int i = 0; i < detalle.CajasId.Count; i++)
                    {
                        //verificar si esta inactivo
                        if (_context.MedicamentosCajas.Where(mc => mc.CajaId == detalle.CajasId[i]).FirstOrDefault().Inactivo)
                        {
                            //int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                            //List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.MedicamentoId == medicamentoId && mc.Inactivo == false).ToList();

                            //obtiene el detalle en cajas del medicamento
                            var detalleCajas = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == detalle.NombreMedicamento && md.TipoCantidad == 1).FirstOrDefault();
                            //si el detalle no es null
                            if(detalleCajas != null)
                            {
                                //excluye las cajas del detalle en cajas
                                cajas = ExcluirCajas(cajas, detalleCajas.CajasId);
                            }

                            List<int> nuevaCajas = new();
                            int cantidadUsar = 0;

                            //agregar a nuevasCajas cajasId hasta que la cantidadUnidad de las cajas sea mayor o igual a la cantidad del detalle
                            for(int j = 0; j < cajas.Count; j++)
                            {
                                nuevaCajas.Add(cajas[i].CajaId);
                                cantidadUsar += cajas[i].CantidadUnidad;

                                if(cantidadUsar >= detalle.Cantidad)
                                {
                                    detalle.CajasId = nuevaCajas;
                                    break;
                                }
                            }

                            //si se alcanzo el final de la lista sin tener la cantidad suficiente de unidades
                            if(cantidadUsar < detalle.Cantidad)
                            {
                                return "No hay existencia suficiente para el medicamento({detalle.DetalleId + 1}).";
                            }

                        }
                    }
                    //verificar si la cantidad de las cajas usadas es mayor o igual a la cantidad del detalle
                    int cantidadUsada = 0;
                    foreach(var caja in cajas)
                    {
                        cantidadUsada += caja.CantidadUnidad;
                    }

                    if(cantidadUsada < detalle.Cantidad)
                    {
                        //sino, se reasigna

                        List<int> nuevaCajas = new();
                        int cantidadUsar = 0;

                        //agregar a nuevasCajas cajasId hasta que la cantidadUnidad de las cajas sea mayor o igual a la cantidad del detalle
                        for (int j = 0; j < cajas.Count; j++)
                        {
                            nuevaCajas.Add(cajas[j].CajaId);
                            cantidadUsar += cajas[j].CantidadUnidad;

                            if (cantidadUsar >= detalle.Cantidad)
                            {
                                detalle.CajasId = nuevaCajas;
                                break;
                            }
                        }

                        //si se alcanzo el final de la lista sin tener la cantidad suficiente de unidades
                        if (cantidadUsar < detalle.Cantidad)
                        {
                            return "No hay existencia suficiente para el medicamento({detalle.DetalleId + 1}).";
                        }
                    }
                }

            }
            return "";
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
            List<int>? cajasUsar = null;
            List<int>? cajasUsadas = null;
            bool existente = false;
            int detalleId = 0;
            Medicamentos? medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId && m.Inactivo == false).Include(m => m.MedicamentosCajas).FirstOrDefaultAsync();

            if (medicamento == null)//si despues de entrar el medicamento pasa a estar inactivo
            {
                return Json(GenerarPartialView(true, "Medicamento Invalido", viewModel));
            }

            if (Cantidad <= 0)//cantidad invalida
            {
                return Json(GenerarPartialView(true, "Cantidad Invalida", viewModel));
            }

            if (viewModel.MedicamentosDetalle.Any(md => md.NombreMedicamento == medicamento.Nombre))//medicamento existente
            {
                //cajasUsar = a las cajas usadas por el medicamento con el mismo tipoMedicamento
                var detalleUsar = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == TipoCantidad).FirstOrDefault();
                if (detalleUsar != null)
                    cajasUsar = detalleUsar.CajasId.ToList();
                //cajasUsadas = a las cajas usadas por el medicamento con diferente tipoMedicamento
                var detalleUsadas = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad != TipoCantidad).FirstOrDefault();
                if (detalleUsadas != null)
                    cajasUsadas = detalleUsadas.CajasId.ToList();

                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
            }
            else//medicamento nuevo
            {
                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, TipoCantidad, Cantidad, cajasUsar, cajasUsadas, existente, detalleId));
                }
            }
        }

        private ResultadoAjax AgregarCaja(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int>? cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId)
        {
            List<MedicamentosCajas> CajasSinDetallar = medicamento.MedicamentosCajas.Where(mc => mc.Detallada == false && mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            if (cajasUsadas != null)//excluir las cajas ya usadas por el mismo medicamento en unidades
            {
                CajasSinDetallar = ExcluirCajas(CajasSinDetallar, cajasUsadas);
            }

            if (cajasUsar != null)//si se agregara mas a el detalle existente excluir las ya usadas
            {
                CajasSinDetallar = ExcluirCajas(CajasSinDetallar, cajasUsar);

                //obtener el detalle Id de la caja
                detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().DetalleId;

                existente = true;
            }

            if (cantidad > CajasSinDetallar.Count)//si se quiere agregar mas cajas de las existentes
            {
                return GenerarPartialView(true, "Cantidad Superior a la existente", viewModel);
            }

            //si no sean agregado cantidades en cajas del medicamento
            if (cajasUsar == null)
            {
                cajasUsar = new();
            }

            for (int i = 0; i < cantidad; i++)
            {
                cajasUsar.Add(CajasSinDetallar[i].CajaId);
            }

            viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);

            return GenerarPartialView(false, "", viewModel);
        }

        private ResultadoAjax AgregarUnidades(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int>? cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId)
        {
            int cantidadUsar = 0;
            int cantidadCajas = 0;
            ResultadoAjax resultado = new ResultadoAjax();
            MedicamentosCajas? cajaDetallada = null;
            List<MedicamentosCajas> cajas = medicamento.MedicamentosCajas.Where(mc => mc.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            //si se agregaran suficientes unidades para hacer cajas
            while (cantidad >= medicamento.UnidadesCaja)
            {
                //agregar la cantidad de cajas que se agregar y se disminuira de la cantidad de unidades
                cantidadCajas++;
                cantidad -= medicamento.UnidadesCaja;
            }

            //si hay cajas que agregar en vez de unidades
            if (cantidadCajas > 0)
            {
                //obtener el detalle Id de la caja
                if (cajasUsadas != null)
                {
                    detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().DetalleId;
                }

                //se invierte cajasUsar y cajasUsadas porque se cambiar el tipo de agregar
                resultado = AgregarCaja(viewModel, medicamento, 1, cantidadCajas, cajasUsadas, cajasUsar, existente, detalleId);
                if (resultado.error)
                {
                    return GenerarPartialView(resultado.error, resultado.mensaje, resultado.viewModel);
                }

                //poner como cajas usadas las del viewModel
                var detalleNuevo = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault();
                if (detalleNuevo != null)
                    cajasUsadas = detalleNuevo.CajasId;
            }

            //si se agregaron cajas y ya no quedan unidades que agregar
            if (cantidad == 0)
            {
                return GenerarPartialView(resultado.error, resultado.mensaje, resultado.viewModel);
            }

            //si cajasUsar no es null
            if (cajasUsar != null)
            {
                existente = true;

                //obtener el detalle anterior del mismo medicamento con el mismo TipoCantidad
                MedicamentosDetalle detalleAnterior = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == tipoCantidad).FirstOrDefault();

                //si la suma de la cantidad del detalle anterior mas la cantidad agregar es => a las unidades por medicamento
                if (detalleAnterior.Cantidad + cantidad >= medicamento.UnidadesCaja)
                {
                    existente = false;

                    //obtener el detalle Id de la caja
                    if (cajasUsadas != null)
                    {
                        detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().DetalleId;
                    }

                    //se agregar otra caja
                    cantidad -= medicamento.UnidadesCaja - detalleAnterior.Cantidad;
                    viewModel = AgregarCaja(viewModel, medicamento, 1, 1, cajasUsadas, cajasUsar, existente, detalleId).viewModel;

                    //se eliminar el detalle anterior
                    viewModel.MedicamentosDetalle.RemoveAt(detalleAnterior.DetalleId);

                    //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
                    for (int i = detalleAnterior.DetalleId; i < viewModel.MedicamentosDetalle.Count; i++)
                    {
                        viewModel.MedicamentosDetalle[i].DetalleId = i;
                    }
                }
                //para que se modifique el mismo detalle
                detalleId = detalleAnterior.DetalleId;
                //obtener ultima caja del detalle anterior
                MedicamentosCajas ultimaCaja = cajas.Where(mc => mc.CajaId == detalleAnterior.CajasId.Last()).FirstOrDefault();
                //cantidadUsar = cantidad caja - cantidad del detalle anterior
                cantidadUsar = ultimaCaja.CantidadUnidad - detalleAnterior.Cantidad;
            }
            else
            {
                cajasUsar = new();

                cajaDetallada = cajas.Where(mc => mc.Detallada == true).FirstOrDefault();

                if (cajaDetallada != null)
                {
                    cajasUsar.Add(cajaDetallada.CajaId);
                    cantidadUsar += cajaDetallada.CantidadUnidad;

                    cajas.Remove(cajaDetallada);
                }
            }

            //si se agregaron cajas y ya no quedan unidades que agregar
            if (cantidad == 0)
            {
                return GenerarPartialView(false, "", viewModel);
            }

            if (cajasUsadas != null)//excluir las cajas ya usadas por el mismo medicamento en unidades
            {
                cajas = ExcluirCajas(cajas, cajasUsadas);
            }

            if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
            {
                viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                return GenerarPartialView(false, "", viewModel);
            }

            //si la cantidad que se pide es mayor a la que se usara
            for (int i = 0; i < cajas.Count; i++)
            {
                cajasUsar.Add(cajas[i].CajaId);
                cantidadUsar += cajas[i].CantidadUnidad;

                if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
                {
                    viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                    return GenerarPartialView(false, "", viewModel);
                }
            }

            return GenerarPartialView(true, "Cantidad Superior a la existente", viewModel);
        }

       private FacturaViewModel AgregarDetalle(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, bool existente, int detalleId)
        {
            if (existente == true)
            {
                viewModel.MedicamentosDetalle[detalleId].CajasId = cajasUsar;
                viewModel.MedicamentosDetalle[detalleId].Cantidad += cantidad;
                viewModel.MedicamentosDetalle[detalleId].Total = viewModel.MedicamentosDetalle[detalleId].Precio * viewModel.MedicamentosDetalle[detalleId].Cantidad;
            }
            else
            {
                MedicamentosDetalle detalle = new()
                {
                    DetalleId = viewModel.MedicamentosDetalle.Count(),
                    CajasId = cajasUsar,
                    NombreMedicamento = medicamento.Nombre,
                    TipoCantidad = tipoCantidad,
                    Cantidad = cantidad,
                    Precio = medicamento.PrecioUnidad
                };

                if (tipoCantidad == 1)//cajas
                    detalle.Precio = medicamento.PrecioUnidad * medicamento.UnidadesCaja;

                detalle.Total = detalle.Precio * cantidad;

                viewModel.MedicamentosDetalle.Add(detalle);
            }

            return viewModel;
        }

        private List<MedicamentosCajas> ExcluirCajas(List<MedicamentosCajas> cajas, List<int> cajasExcluir)
        {
            for (int i = 0; i < cajas.Count; i++)
            {
                if (cajasExcluir.Contains(cajas[i].CajaId))
                {
                    cajas.Remove(cajas[i]);
                    i--;
                }
            }

            return cajas;
        }

        private ResultadoAjax GenerarPartialView(bool error, string mensaje, FacturaViewModel viewModel)
        {
            ModelState.Clear();// para quitar el input anterior
            PartialViewResult partialViewResult = PartialView("MedicamentosDetalles", viewModel);
            string viewContent = ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            return new ResultadoAjax()
            {
                error = error,
                mensaje = mensaje,
                partial = viewContent,
                viewModel = viewModel
            };
        }

        private string ConvertViewToString(ControllerContext controllerContext, PartialViewResult pvr, ICompositeViewEngine _viewEngine)
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
        public IActionResult RemoverMedicamento([Bind("MedicamentosDetalle")] FacturaViewModel viewModel, int RemoverId)
        {
            viewModel.MedicamentosDetalle.RemoveAt(RemoverId);

            //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
            for (int i = RemoverId; i < viewModel.MedicamentosDetalle.Count; i++)
            {
                viewModel.MedicamentosDetalle[i].DetalleId = i;
            }

            ModelState.Clear();// para quitar el input anterior
            return PartialView("MedicamentosDetalles", viewModel);
        }
    }
}
