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
using System.Text;
using ReflectionIT.Mvc.Paging;
using System.Linq.Dynamic.Core;
using Rotativa.AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ApotheGSF.Controllers
{
    [Authorize(Roles = "Administrador, Vendedor")]
    public class FacturasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;
        private ICompositeViewEngine _viewEngine;
        private static List<MedicamentosDetalle>? detallesEdit;
        private readonly INotyfService _notyf;
        private readonly UserManager<AppUsuario> _userManager;

        public FacturasController(AppDbContext context,
                                  IHttpContextAccessor accessor,
                                  ICompositeViewEngine viewEngine,
                                  INotyfService notyf,
                                  UserManager<AppUsuario> userManager)
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _viewEngine = viewEngine;
            _notyf = notyf;
            _userManager = userManager;
        }

        // GET: Facturas
        public async Task<IActionResult> Index(DateTime desde, DateTime hasta, string Mensaje = "", int pageindex = 1, string sortExpression = "", int search = 0)
        {
            if (Mensaje != "")
            {
                _notyf.Custom(Mensaje, 5, "#17D155", "fas fa-check");
            }

            StringBuilder filtro = new StringBuilder(" Inactivo == false ");

            if (desde > default(DateTime))
                filtro.AppendFormat("  && Creado >= DateTime({0},{1},{2},{3},{4},{5})", desde.Year, desde.Month, desde.Day, desde.Hour, desde.Minute, desde.Second);

            if (hasta > default(DateTime))
                filtro.AppendFormat("  && Creado <= DateTime({0},{1},{2},{3},{4},{5})", hasta.Year, hasta.Month, hasta.Day, hasta.Hour, hasta.Minute, hasta.Second);

            List<Facturas> listado = new List<Facturas>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await _context.Facturas.Where(filtro.ToString()).ToListAsync();
            }

            if (listado.Count == 0 && search == 1)
                _notyf.Information("No hay facturas existentes");

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Codigo" : sortExpression;//verificar para fecha
            var model = PagingList.Create(listado, 3, pageindex, sortExpression, "");
            model.RouteValue = new RouteValueDictionary {
                            { "desde", desde},
                {"hasta", hasta }
            };

            model.Action = "Index";

            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;

            return model != null ?
                View(model) :
                Problem("Entity set 'ApplicationDbContext.Facturas' is null.");
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await (from f in _context.Facturas
                               .AsNoTracking()
                               .AsQueryable()
                                 select new FacturaViewModel
                                 {
                                     Codigo = f.Codigo,
                                     SubTotal = f.SubTotal,
                                     Total = f.Total,
                                     Creado = f.Creado,
                                     CreadoNombreUsuario = f.CreadoNombreUsuario,
                                     Inactivo = f.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            factura.MedicamentosDetalle = ObtenerDetalles(factura);

            return View(factura);
        }

        private List<MedicamentosDetalle> ObtenerDetalles(FacturaViewModel factura)
        {
            //obtener listado de facturamedicamentocaja con facturaId == factura.codigo
            List<FacturaMedicamentosCajas> facturasCajas = _context.FacturasMedicamentosCajas.Where(fmv => fmv.CodigoFactura == factura.Codigo).ToList();
            //crear listado de detalle
            List<MedicamentosDetalle> listaDetalle = new();
            //obtener listado de cajas
            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.ToList();
            MedicamentosCajas caja;

            do
            {
                //crear un medicamentoDetalle con el primer elemento de facturasCajas
                MedicamentosDetalle detalle = new MedicamentosDetalle()
                {
                    CodigoDetalle = listaDetalle.Count,
                    CodigoCaja = facturasCajas[0].CodigoCaja,
                    TipoCantidad = facturasCajas[0].TipoCantidad,
                    Cantidad = facturasCajas[0].CantidadUnidad,
                    Precio = facturasCajas[0].Precio
                };

                caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
                detalle.NombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;
                detalle.NombreLaboratorio = _context.Laboratorios.Where(l => l.Codigo == caja.CodigoLaboratorio).FirstOrDefault().Nombre;
                detalle.Total = detalle.Cantidad * detalle.Precio;

                //excluir ese elemento de facturasCajas
                facturasCajas.RemoveAt(0);

                //obtener el nombreMedicamento del primer elemento de facturasCajas si quedan otras facturasCajas
                if (facturasCajas.Count > 0)
                {
                    caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
                    string nombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;
                    string nombreLaboratorio = _context.Laboratorios.Where(l => l.Codigo == caja.CodigoLaboratorio).FirstOrDefault().Nombre;

                    //mientras el nombreMedicamento y tipoCantidad sean igual al detalle
                    while (nombreMedicamento == detalle.NombreMedicamento && facturasCajas[0].TipoCantidad == detalle.TipoCantidad)
                    {
                        listaDetalle.Add(detalle);
                        detalle = new MedicamentosDetalle()
                        {
                            CodigoDetalle = listaDetalle.Count,
                            CodigoCaja = facturasCajas[0].CodigoCaja,
                            TipoCantidad = facturasCajas[0].TipoCantidad,
                            Cantidad = facturasCajas[0].CantidadUnidad,
                            Precio = facturasCajas[0].Precio
                        };

                        detalle.NombreMedicamento = nombreMedicamento;
                        detalle.NombreLaboratorio = nombreLaboratorio;
                        detalle.Total = detalle.Cantidad * detalle.Precio;

                        //excluir ese elemento de facturasCajas
                        facturasCajas.RemoveAt(0);

                        if (facturasCajas.Count == 0)
                            break;
                    }
                }

                listaDetalle.Add(detalle);

            } while (facturasCajas.Count > 0);

            return listaDetalle;
        }

        // GET: Facturas/Create
        public async Task<IActionResult> Create()
        {
            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await _context.Medicamentos.Where(m => m.Inactivo == false).ToListAsync();

            foreach (var medicamento in medicamentos)
            {
                medicamento.MedicamentosCajas = await _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo).ToArrayAsync();
            }

            if (medicamentos == null)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            if (medicamentos.Any(m => m.MedicamentosCajas.Count > 0))
            {
                //usar los medicamentos que tengan alguna caja en inventario
                ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.MedicamentosCajas.Count > 0), "Codigo", "Nombre");


                detallesEdit = new();

                return View(new FacturaViewModel());
            }

            _notyf.Information("Es necesario tener inventario");
            return RedirectToAction("Index", "Home");
        }

        // POST: Facturas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("SubTotal,Total,MedicamentosDetalle")] FacturaViewModel viewModel)
        {
            if (viewModel.MedicamentosDetalle.Count == 0)
            {
                _notyf.Error("Se necesita agregar algun medicamento");
                return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
            }

            string error = VerificarInventario(ref viewModel);

            //si hubo alguno error al verificar la disponibilidad en el inventario se regresa al view
            if (!error.Equals(""))//si no esta vacio
            {
                _notyf.Error(error);
                return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await CrearFactura(viewModel);
                }
                catch (Exception e)
                {
                    _notyf.Error(e.Message);
                    return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                }
                int ultimaFactura = _context.Facturas.OrderByDescending(f => f.Codigo).First().Codigo;
                return Json(new ResultadoFactura() { resultado = true, codigofactura = ultimaFactura, mensaje = "Se ha facturado exitosamente!!" });
            }
            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await _context.Medicamentos.Where(m => m.Inactivo == false).ToListAsync();

            foreach (var item in medicamentos)
            {
                item.Cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == item.Codigo && mc.CantidadUnidad > 0 && mc.Inactivo == false).ToList().Count;
            }

            //usar los medicamentos que tengan alguna caja en inventario
            ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

            _notyf.Error("Invalido");
            return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
        }

        private string VerificarInventario(ref FacturaViewModel viewModel)
        {
            //List<string> errores = new();
            //por cada detalle de medicamentosDetalle
            foreach (var detalle in viewModel.MedicamentosDetalle)
            {
                //si el tipoCantidad es caja
                if (detalle.TipoCantidad == 1)//caja
                {
                    //verificar si esta inactivo
                    if (_context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault().Inactivo)
                    {
                        //-----------revisar despues
                        return $"La caja #{detalle.CodigoCaja} ya no existe";
                    }
                }
                else//unidades
                {
                    //-----------revisar despues
                    //verificar si esta inactivo
                    if (_context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault().Inactivo)
                    {
                        return $"La caja #{detalle.CodigoCaja} ya no existe";
                    }
                }
            }
            return "";
        }

        private async Task<bool> CrearFactura(FacturaViewModel viewModel)
        {
            //actualizar los medicamentos cajas por cada detalle
            foreach (var detalle in viewModel.MedicamentosDetalle)
            {
                if (detalle.TipoCantidad == 1)//si el detalle es tipo caja
                {
                    MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault();

                    caja.CantidadUnidad = 0;

                    _context.Update(caja);
                }
                else //si el detalle es tipo unidad
                {

                    //se guarda en una variable la cantidad del detalle
                    var cantidadUsada = detalle.Cantidad;

                    //obtener la caja del cajaId
                    MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault();

                    //si la cantidad de la caja es mayor a la cantidad usada
                    if (caja.CantidadUnidad > cantidadUsada)
                    {
                        caja.CantidadUnidad -= cantidadUsada;
                        caja.Detallada = true;
                    }
                    else if (caja.CantidadUnidad == cantidadUsada)
                    {
                        caja.CantidadUnidad = 0;
                    }
                    else//caja.CantidadUnidad < cantidadUsada
                    {
                        cantidadUsada -= caja.CantidadUnidad;

                        caja.CantidadUnidad = 0;
                    }
                    _context.Update(caja);
                }
            }

            //agregar la nueva factura
            Facturas nuevaFactura = new()
            {
                SubTotal = viewModel.SubTotal,
                Total = viewModel.Total,
                Creado = DateTime.Now,
                CreadoNombreUsuario = _user.GetUserName(),
                Inactivo = false
            };

            _context.Facturas.Add(nuevaFactura);

            foreach (var detalle in viewModel.MedicamentosDetalle)
            {
                FacturaMedicamentosCajas facturaCaja;

                if (detalle.TipoCantidad == 1)
                {
                    facturaCaja = new()
                    {
                        CodigoCaja = detalle.CodigoCaja,
                        TipoCantidad = 1,
                        CantidadUnidad = 1,
                        Precio = detalle.Precio,
                    };

                    nuevaFactura.FacturasMedicamentosCajas.Add(facturaCaja);
                }
                else
                {

                    int cantidadUsada = detalle.Cantidad;

                    facturaCaja = new()
                    {
                        CodigoCaja = detalle.CodigoCaja,
                        TipoCantidad = 2,
                        Precio = detalle.Precio,
                    };

                    MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault();

                    if (caja.CantidadUnidad > cantidadUsada)
                    {
                        facturaCaja.CantidadUnidad = detalle.Cantidad;
                        cantidadUsada -= caja.CantidadUnidad;
                    }
                    else
                    {
                        facturaCaja.CantidadUnidad = cantidadUsada;
                    }

                    nuevaFactura.FacturasMedicamentosCajas.Add(facturaCaja);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await (from f in _context.Facturas
                               .AsNoTracking()
                               .AsQueryable()
                                 select new FacturaViewModel
                                 {
                                     Codigo = f.Codigo,
                                     SubTotal = f.SubTotal,
                                     Total = f.Total,
                                     Inactivo = f.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            var listaDetalle = ObtenerDetalles(factura);

            factura.MedicamentosDetalle = listaDetalle;

            detallesEdit = listaDetalle;

            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await _context.Medicamentos.Where(m => m.Inactivo == false).ToListAsync();

            foreach (var medicamento in medicamentos)
            {
                medicamento.MedicamentosCajas = await _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo).ToArrayAsync();
            }

            if (medicamentos == null)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            if (medicamentos.Any(m => m.MedicamentosCajas.Count > 0))
            {
                //usar los medicamentos que tengan alguna caja en inventario
                ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.MedicamentosCajas.Count > 0), "Codigo", "Nombre");

                return View(factura);
            }

            _notyf.Information("Es necesario tener inventario");
            return RedirectToAction("Index", "Home");
        }

        // POST: Facturas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,SubTotal,Total,MedicamentosDetalle")] FacturaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.MedicamentosDetalle.Count == 0)
                    {
                        _notyf.Error("Se necesita agregar algun medicamento");
                        return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                    }

                    string error = VerificarInventario(ref viewModel);

                    //si hubo alguno error al verificar la disponibilidad en el inventario se regresa al view
                    if (!error.Equals(""))//si no esta vacio
                    {
                        _notyf.Error(error);
                        return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                    }

                    //buscar la factura anterior
                    Facturas facturaAnterior = _context.Facturas.Where(f => f.Codigo == viewModel.Codigo && f.Inactivo == false).Include(f => f.FacturasMedicamentosCajas).FirstOrDefault();
                    //desactivar la factura anterior
                    _context.Update(facturaAnterior);
                    facturaAnterior.Inactivo = true;

                    //devolver la cantidad de cajas usadas
                    foreach (var facturaCaja in facturaAnterior.FacturasMedicamentosCajas)
                    {
                        //obtener la caja del cajaId
                        MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == facturaCaja.CodigoCaja).FirstOrDefault();

                        if (facturaCaja.TipoCantidad == 1)//caja
                        {
                            //obtener el medicamento de la caja.MedicamentoId
                            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault();

                            caja.CantidadUnidad = medicamento.UnidadesCaja;
                        }
                        else//unidades
                        {
                            caja.CantidadUnidad += facturaCaja.CantidadUnidad;
                        }

                        //update a la caja
                        _context.Update(caja);
                        _context.Remove(facturaCaja);
                    }

                    await CrearFactura(viewModel);
                }
                catch (Exception e)
                {
                    _notyf.Error(e.Message);
                    return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                }
                int ultimaFactura = _context.Facturas.OrderByDescending(f => f.Codigo).First().Codigo;
                return Json(new ResultadoFactura() { resultado = true, codigofactura = ultimaFactura, mensaje = "Se ha guardado la factura exitosamente!!" });
            }

            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await _context.Medicamentos.Where(m => m.Inactivo == false).ToListAsync();

            foreach (var item in medicamentos)
            {
                item.Cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == item.Codigo && mc.CantidadUnidad > 0 && mc.Inactivo == false).ToList().Count;
            }

            //usar los medicamentos que tengan alguna caja en inventario
            ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

            return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await (from f in _context.Facturas
                               .AsNoTracking()
                               .AsQueryable()
                                 select new FacturaViewModel
                                 {
                                     Codigo = f.Codigo,
                                     SubTotal = f.SubTotal,
                                     Total = f.Total,
                                     Creado = f.Creado,
                                     CreadoNombreUsuario = f.CreadoNombreUsuario,
                                     Inactivo = f.Inactivo
                                 }).Where(x => x.Codigo == id && x.Inactivo == false).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            factura.MedicamentosDetalle = ObtenerDetalles(factura);

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteConfirmed(FacturaViewModel viewModel)
        {
            return false;

            if (_context.Facturas == null)
            {
                _notyf.Error("No se ha podido eliminar");
                return false;
            }

            var factura = await _context.Facturas.FindAsync(viewModel.Codigo);
            if (factura != null)
            {
                factura.Inactivo = true;
                _context.Update(factura);

                int codigoCaja = 0;

                //por cada detalle en medicamentosDetalle
                foreach (var detalle in viewModel.MedicamentosDetalle)
                {
                    List<int> codigosCajas = new();

                    //si la caja no fue abierta
                    if (detalle.Abierto == false)
                    {
                        if (detalle.TipoCantidad == 1)//si es caja
                        {
                            MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigoCaja).FirstOrDefault();

                            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault();

                            _context.Update(caja);
                            caja.CantidadUnidad = medicamento.UnidadesCaja;
                            caja.Detallada = false;
                        }
                        else//si es unidades
                        {
                            int cantidadDevolver = detalle.Cantidad;

                            MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == codigoCaja).FirstOrDefault();

                            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault();

                            _context.Update(caja);

                            if (caja.CantidadUnidad + cantidadDevolver <= medicamento.UnidadesCaja)
                            {
                                caja.CantidadUnidad += cantidadDevolver;
                            }
                            else
                            {
                                caja.CantidadUnidad += medicamento.UnidadesCaja - cantidadDevolver;
                                cantidadDevolver -= medicamento.UnidadesCaja - cantidadDevolver;
                            }

                            if (caja.CantidadUnidad == medicamento.UnidadesCaja)
                            {
                                caja.Detallada = false;
                            }
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }


        private bool FacturaExists(int id)
        {
            return (_context.Facturas?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarMedicamento(FacturaViewModel viewModel, int MedicamentoId, int LaboratorioId, int TipoCantidad, int Cantidad)
        {
            List<int>? cajasUsadas = new();
            bool existente = false;
            int detalleId = 0;
            Medicamentos? medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId && m.Inactivo == false).FirstOrDefaultAsync();

            medicamento.MedicamentosCajas = await _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo).ToArrayAsync();

            if (MedicamentoId == 0)
            {
                _notyf.Warning("Seleccione un medicamento");
                return Json(GenerarPartialView(true, viewModel));
            }

            if (LaboratorioId == 0)
            {
                _notyf.Warning("Seleccione un laboratorio");
                return Json(GenerarPartialView(true, viewModel));
            }

            if (medicamento == null)//si despues de entrar el medicamento pasa a estar inactivo
            {
                _notyf.Warning("Medicamento Invalido");
                return Json(GenerarPartialView(true, viewModel));
            }

            if (Cantidad <= 0)//cantidad invalida
            {
                _notyf.Warning("Cantidad Invalida");
                return Json(GenerarPartialView(true, viewModel));
            }

            if (medicamento.Detallable == false && TipoCantidad == 2)//si el medicamento no es detallable
            {
                _notyf.Warning("Este medicamento no es detallable");
                return Json(GenerarPartialView(true, viewModel));
            }


            if (viewModel.MedicamentosDetalle.Any(md => md.CodigoMedicamento == medicamento.Codigo))//medicamento existente
            {
                //por cada detalle
                foreach (var detalle in viewModel.MedicamentosDetalle)
                {
                    //si tiene el mismo codigo medicamento y codigo laboratorio
                    if (detalle.CodigoMedicamento == medicamento.Codigo && detalle.CodigoLaboratorio == LaboratorioId)
                    {
                        //agrega su caja  a cajas usadas
                        cajasUsadas.Add(detalle.CodigoCaja);
                    }
                }

                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, LaboratorioId, TipoCantidad, Cantidad, cajasUsadas));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, LaboratorioId, TipoCantidad, Cantidad, cajasUsadas));
                }
            }
            else//medicamento nuevo
            {
                if (TipoCantidad == 1)//cajas
                {
                    return Json(AgregarCaja(viewModel, medicamento, LaboratorioId, TipoCantidad, Cantidad, cajasUsadas));
                }
                else//unidades
                {
                    return Json(AgregarUnidades(viewModel, medicamento, LaboratorioId, TipoCantidad, Cantidad, cajasUsadas));
                }
            }
        }

        private ResultadoAjax AgregarCaja(FacturaViewModel viewModel, Medicamentos medicamento, int laboratorioId, int tipoCantidad, int cantidad, List<int>? cajasUsadas)
        {
            //nueva lista de cajasSinDetallar
            List<MedicamentosCajas> cajasSinDetallar = new();
            List<int> cajasUsar = new();
            //si detallesEdit tiene elementos
            if (detallesEdit.Count > 0)
            {
                foreach (var detalle in detallesEdit)
                {
                    if (detalle.CodigoMedicamento == medicamento.Codigo && detalle.CodigoLaboratorio == laboratorioId && detalle.TipoCantidad == 1)
                    {
                        cajasSinDetallar.Add(new MedicamentosCajas()
                        {
                            Codigo = detalle.CodigoCaja
                        });
                    }
                }
            }

            cajasSinDetallar.AddRange(medicamento.MedicamentosCajas.Where(mc => mc.CodigoLaboratorio == laboratorioId && mc.Detallada == false && mc.Inactivo == false && mc.CantidadUnidad > 0).OrderBy(mc => mc.FechaVencimiento).ToList());

            if (cajasUsadas != null)//excluir las cajas ya usadas
            {
                cajasSinDetallar = ExcluirCajas(cajasSinDetallar, cajasUsadas);
            }

            if (cantidad > cajasSinDetallar.Count)//si se quiere agregar mas cajas de las existentes
            {
                _notyf.Warning("Cantidad Superior a la existente");
                return GenerarPartialView(true, viewModel);
            }

            for (int i = 0; i < cantidad; i++)
            {
                cajasUsar.Add(cajasSinDetallar[i].Codigo);
            }

            viewModel = AgregarDetalle(viewModel, medicamento, laboratorioId, tipoCantidad, cantidad, cajasUsar);

            return GenerarPartialView(false, viewModel);
        }

        private ResultadoAjax AgregarUnidades(FacturaViewModel viewModel, Medicamentos medicamento, int laboratorioId, int tipoCantidad, int cantidad, List<int>? cajasUsadas)
        {
            List<int> cajasUsar = new();
            int cantidadUsar = 0;
            int cantidadCajas = 0;
            int cantidadAgregar = 0;
            ResultadoAjax resultado = new ResultadoAjax();
            MedicamentosCajas? cajaDetallada = null;

            List<MedicamentosCajas> cajas = new();
            //si detallesEdit tiene elementos
            if (detallesEdit.Count > 0)
            {
                foreach (var detalle in detallesEdit)
                {
                    if (detalle.CodigoMedicamento == medicamento.Codigo && detalle.CodigoLaboratorio == laboratorioId && detalle.TipoCantidad == 2)
                    {
                        if (detalle.Cantidad > medicamento.UnidadesCaja)
                        {
                            cantidadAgregar = medicamento.UnidadesCaja;
                            detalle.Cantidad -= medicamento.UnidadesCaja;
                        }
                        else
                            cantidadAgregar = detalle.Cantidad;

                        //se agregan las cajas necesarias
                        cajas.Add(new MedicamentosCajas()
                        {
                            Codigo = detalle.CodigoCaja,
                            CantidadUnidad = cantidadAgregar,
                            Detallada = true
                        });
                    }
                }
            }

            //agregar las cantidades de las cajas usadas en el create de la factura
            foreach (var medicamentoCaja in medicamento.MedicamentosCajas.Where(mc => mc.CodigoLaboratorio == laboratorioId && mc.Inactivo == false && mc.CantidadUnidad > 0).OrderBy(mc => mc.FechaVencimiento).ToList())
            {
                if (cajas.Any(c => c.Codigo == medicamentoCaja.Codigo))
                {
                    cajas.Where(c => c.Codigo == medicamentoCaja.Codigo).First().CantidadUnidad += medicamentoCaja.CantidadUnidad;
                }
                else
                    cajas.Add(medicamentoCaja);
            }

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
                resultado = AgregarCaja(viewModel, medicamento, laboratorioId, 1, cantidadCajas, cajasUsadas);
                if (resultado.error)
                {
                    return GenerarPartialView(resultado.error, resultado.viewModel);
                }
            }

            //si se agregaron cajas y ya no quedan unidades que agregar
            if (cantidad == 0)
            {
                return GenerarPartialView(resultado.error, resultado.viewModel);
            }
            if (viewModel.MedicamentosDetalle.Any(md => md.CodigoMedicamento == medicamento.Codigo && md.CodigoLaboratorio == laboratorioId && md.TipoCantidad == 2))
            {
                //obtener la lista de detalles anteriores del mismo medicamento con el mismo TipoCantidad
                List<MedicamentosDetalle> detallesAnteriores = viewModel.MedicamentosDetalle.Where(md => md.CodigoMedicamento == medicamento.Codigo && md.CodigoLaboratorio == laboratorioId && md.TipoCantidad == 2).ToList();

                var cantidaAnterior = 0;
                foreach (var detalle in detallesAnteriores)
                {
                    cantidaAnterior += detalle.Cantidad;
                }

                //si la suma de la cantidad del detalle anterior mas la cantidad agregar es => a las unidades por medicamento
                if (cantidaAnterior + cantidad >= medicamento.UnidadesCaja)
                {
                    //se agregar otra caja
                    cantidad -= medicamento.UnidadesCaja - cantidaAnterior;
                    viewModel = AgregarCaja(viewModel, medicamento, laboratorioId, 1, 1, cajasUsadas).viewModel;

                    //se eliminan los detalles anteriores
                    foreach (var detalle in detallesAnteriores)
                    {
                        viewModel.MedicamentosDetalle.RemoveAt(detalle.CodigoDetalle);

                        //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
                        for (int i = detalle.CodigoDetalle; i < viewModel.MedicamentosDetalle.Count; i++)
                        {
                            viewModel.MedicamentosDetalle[i].CodigoDetalle = i;
                        }
                    }
                }
                //obtener ultima caja del detalle anterior
                MedicamentosCajas ultimaCaja = cajas.Where(mc => mc.Codigo == detallesAnteriores.Last().CodigoCaja).FirstOrDefault();
                //MedicamentosCajas ultimaCaja = cajas.Where(mc => mc.Codigo == detalleAnterior.CodigoCaja).FirstOrDefault();
                //cantidadUsar = cantidad caja - cantidad del detalle anterior
                cantidadUsar = ultimaCaja.CantidadUnidad - cantidaAnterior;
            }
            else
            {
                cajaDetallada = cajas.Where(mc => mc.Detallada == true).FirstOrDefault();

                if (cajaDetallada != null)
                {
                    cajasUsar.Add(cajaDetallada.Codigo);
                    cantidadUsar += cajaDetallada.CantidadUnidad;

                    cajas.Remove(cajaDetallada);
                }
            }

            //si se agregaron cajas y ya no quedan unidades que agregar
            if (cantidad == 0)
            {
                return GenerarPartialView(false, viewModel);
            }

            if (cajasUsadas != null)//excluir las cajas ya usadas por el mismo medicamento en cajas
            {
                cajas = ExcluirCajas(cajas, cajasUsadas);
            }

            if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
            {
                viewModel = AgregarDetalle(viewModel, medicamento, laboratorioId, tipoCantidad, cantidad, cajasUsar);
                return GenerarPartialView(false, viewModel);
            }

            //si la cantidad que se pide es mayor a la que se usara
            for (int i = 0; i < cajas.Count; i++)
            {
                cajasUsar.Add(cajas[i].Codigo);
                cantidadUsar += cajas[i].CantidadUnidad;

                if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
                {
                    viewModel = AgregarDetalle(viewModel, medicamento, laboratorioId, tipoCantidad, cantidad, cajasUsar);
                    return GenerarPartialView(false, viewModel);
                }
            }

            _notyf.Warning("Cantidad Superior a la existente");
            return GenerarPartialView(true, viewModel);
        }

        private FacturaViewModel AgregarDetalle(FacturaViewModel viewModel, Medicamentos medicamento, int laboratorioId, int tipoCantidad, int cantidad, List<int> cajasUsar)
        {
            var laboratorio = _context.Laboratorios.Where(l => l.Codigo == laboratorioId).FirstOrDefault();

            int cantidadUsar;

            foreach (var codigo in cajasUsar)
            {
                if (tipoCantidad == 1)
                {
                    cantidadUsar = 1;
                    cantidad--;
                }
                else
                {
                    //caja de cajasUsar
                    MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == codigo).FirstOrDefault();
                    //si la cantidad de la caja es menor o igual a la cantidad a agregar se agregar
                    if(caja.CantidadUnidad <= cantidad)
                    {
                        cantidadUsar = cantidad;
                    }
                    else
                    {
                        //si no se agrega la cantidad de cajas y resta a la cantidad
                        cantidadUsar = caja.CantidadUnidad;
                        cantidad -= caja.CantidadUnidad;
                    }

                }

                MedicamentosDetalle detalle = new()
                {
                    CodigoDetalle = viewModel.MedicamentosDetalle.Count(),
                    CodigoCaja = codigo,
                    CodigoMedicamento = medicamento.Codigo,
                    CodigoLaboratorio = laboratorioId,
                    NombreMedicamento = medicamento.Nombre,
                    NombreLaboratorio = laboratorio.Nombre,
                    TipoCantidad = tipoCantidad,
                    Cantidad = cantidadUsar,
                    Precio = medicamento.MedicamentosCajas.Where(mc => mc.Codigo == codigo).First().PrecioUnidad
                };

                if (tipoCantidad == 1)//cajas
                    detalle.Total = detalle.Precio * detalle.Cantidad;

                viewModel.MedicamentosDetalle.Add(detalle);
            }

            return viewModel;
        }

        private List<MedicamentosCajas> ExcluirCajas(List<MedicamentosCajas> cajas, List<int> cajasExcluir)
        {
            if (cajas != null && cajasExcluir != null)
            {
                for (int i = 0; i < cajas.Count; i++)
                {
                    if (cajasExcluir.Contains(cajas[i].Codigo))
                    {
                        cajas.Remove(cajas[i]);
                        i--;
                    }
                }
            }

            return cajas;
        }

        private ResultadoAjax GenerarPartialView(bool error, FacturaViewModel viewModel)
        {
            ModelState.Clear();// para quitar el input anterior
            PartialViewResult partialViewResult = PartialView("MedicamentosDetalles", viewModel);
            string viewContent = ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            return new ResultadoAjax()
            {
                error = error,
                partial = viewContent,
                viewModel = viewModel,
                subtotal = CalcularSubTotal(viewModel)
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
        public ActionResult RemoverMedicamento(FacturaViewModel viewModel, int RemoverId)
        {
            viewModel.MedicamentosDetalle.RemoveAt(RemoverId);

            //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
            for (int i = RemoverId; i < viewModel.MedicamentosDetalle.Count; i++)
            {
                viewModel.MedicamentosDetalle[i].CodigoDetalle = i;
            }

            return Json(GenerarPartialView(false, viewModel));
        }

        private float CalcularSubTotal(FacturaViewModel viewModel)
        {
            float subTotal = 0;

            foreach (var detalle in viewModel.MedicamentosDetalle)
            {
                subTotal += detalle.Total;
            }

            return subTotal;
        }

        public async Task<IActionResult> ReporteFacturas(string desde, string hasta)
        {
            DateTime Desde = DateTime.Parse(desde);
            DateTime Hasta = DateTime.Parse(hasta);

            StringBuilder filtro = new StringBuilder(" Inactivo == false ");

            if (Desde > default(DateTime))
                filtro.AppendFormat("  && Creado >= DateTime({0},{1},{2},{3},{4},{5})", Desde.Year, Desde.Month, Desde.Day, Desde.Hour, Desde.Minute, Desde.Second);

            if (Hasta > default(DateTime))
                filtro.AppendFormat("  && Creado <= DateTime({0},{1},{2},{3},{4},{5})", Hasta.Year, Hasta.Month, Hasta.Day, Hasta.Hour, Hasta.Minute, Hasta.Second);

            List<Facturas> facturas = await _context.Facturas.Where(filtro.ToString()).ToListAsync();

            return new ViewAsPdf("ReporteFacturas", facturas);
        }

        public async Task<IActionResult> ReporteFactura(int CodigoFactura)
        {
            var factura = await (from f in _context.Facturas
                               .AsNoTracking()
                               .AsQueryable()
                                 select new FacturaViewModel
                                 {
                                     Codigo = f.Codigo,
                                     SubTotal = f.SubTotal,
                                     Total = f.Total,
                                 }).Where(x => x.Codigo == CodigoFactura).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            factura.MedicamentosDetalle = ObtenerDetalles(factura);

            //obtener nombre del usuario
            AppUsuario usuario = await _userManager.GetUserAsync(User);

            return new ViewAsPdf("ReporteFactura", factura, new ViewDataDictionary(this.ViewData) { { "Nombre", usuario?.Nombre} })
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A6
            };
        }
    }
}
