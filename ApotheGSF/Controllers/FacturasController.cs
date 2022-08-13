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

        public FacturasController(AppDbContext context,
                                  IHttpContextAccessor accessor,
                                  ICompositeViewEngine viewEngine,
                                  INotyfService notyf)
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _viewEngine = viewEngine;
            _notyf = notyf;
        }

        // GET: Facturas
        public async Task<IActionResult> Index(DateTime desde, DateTime hasta, int pageindex = 1, string sortExpression = "", int search = 0)
        {
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

            for (int i = 0; i < listado.Count; i++)
                listado[i].Posicion = i + 1;

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Creado" : sortExpression;//verificar para fecha
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
                                     Modificado = f.Modificado,
                                     ModificadoNombreUsuario = f.ModificadoNombreUsuario
                                 }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

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
                    CodigosCajas = new List<int>() { facturasCajas[0].CodigoCaja },
                    TipoCantidad = facturasCajas[0].TipoCantidad,
                    Cantidad = facturasCajas[0].CantidadUnidad,
                    Precio = facturasCajas[0].Precio
                };

                caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
                detalle.NombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;

                //excluir ese elemento de facturasCajas
                facturasCajas.RemoveAt(0);

                //obtener el nombreMedicamento del primer elemento de facturasCajas si quedan otras facturasCajas
                if (facturasCajas.Count > 0)
                {
                    caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
                    string nombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;

                    //mientras el nombreMedicamento y tipoCantidad sean igual al detalle
                    while (nombreMedicamento == detalle.NombreMedicamento && facturasCajas[0].TipoCantidad == detalle.TipoCantidad)
                    {
                        //add(cajaId, cantidad)
                        detalle.CodigosCajas.Add(facturasCajas[0].CodigoCaja);
                        detalle.Cantidad += facturasCajas[0].CantidadUnidad;

                        //excluir ese elemento de facturasCajas
                        facturasCajas.RemoveAt(0);

                        if (facturasCajas.Count == 0)
                            break;
                    }
                }

                //al final del mientras calcular el total del detalle
                detalle.Total = detalle.Cantidad * detalle.Precio;

                listaDetalle.Add(detalle);

            } while (facturasCajas.Count > 0);

            return listaDetalle;
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
                                          Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count

                                      }).Where(x => x.Inactivo == false).ToListAsync();

            if (medicamentos == null)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            if(medicamentos.Any(m => m.Cajas > 0))
            {
                //usar los medicamentos que tengan alguna caja en inventario
                ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

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
            if(viewModel.MedicamentosDetalle.Count == 0)
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
                    //actualizar los medicamentos cajas por cada detalle
                    foreach (var detalle in viewModel.MedicamentosDetalle)
                    {
                        if (detalle.TipoCantidad == 1)//si el detalle es tipo caja
                        {
                            //por cada cajaId
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                //obtener la caja del cajaId
                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

                                //la cantidad de la caja es cero
                                caja.CantidadUnidad = 0;

                                _context.Update(caja);
                            }
                        }
                        else //si el detalle es tipo unidad
                        {
                            //se guarda en una variable la cantidad del detalle
                            var cantidadUsada = detalle.Cantidad;
                            //por cada cajaId
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                //obtener la caja del cajaId
                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

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
                    }

                    //agregar la nueva factura
                    Facturas nuevaFactura = new()
                    {
                        SubTotal = viewModel.SubTotal,
                        Total = viewModel.Total,
                        Creado = DateTime.Now,
                        CreadoNombreUsuario = _user.GetUserName(),
                        Modificado = DateTime.Now,
                        ModificadoNombreUsuario = _user.GetUserName(),
                        Inactivo = false
                    };

                    _context.Facturas.Add(nuevaFactura);

                    foreach (var detalle in viewModel.MedicamentosDetalle)
                    {
                        FacturaMedicamentosCajas facturaCaja;

                        if (detalle.TipoCantidad == 1)
                        {
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                facturaCaja = new()
                                {
                                    CodigoCaja = cajaId,
                                    TipoCantidad = 1,
                                    CantidadUnidad = 1,
                                    Precio = detalle.Precio,
                                };

                                nuevaFactura.FacturasMedicamentosCajas.Add(facturaCaja);
                            }
                        }
                        else
                        {
                            int cantidadUsada = detalle.Cantidad;

                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                facturaCaja = new()
                                {
                                    CodigoCaja = cajaId,
                                    TipoCantidad = 2,
                                    Precio = detalle.Precio,
                                };

                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

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
                    }

                    await _context.SaveChangesAsync();
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
            var medicamentos = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                      select new MedicamentosViewModel
                                      {

                                          Codigo = meds.Codigo,
                                          Nombre = meds.Nombre,
                                          Inactivo = (bool)meds.Inactivo,
                                          Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count

                                      }).Where(x => x.Inactivo == false).ToListAsync();
            //usar los medicamentos que tengan alguna caja en inventario
            ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

            _notyf.Error("Invalido");
            return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
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
                    for (int i = 0; i < detalle.CodigosCajas.Count; i++)
                    {
                        //verificar si esta inactivo
                        if (_context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigosCajas[i]).FirstOrDefault().Inactivo)
                        {
                            //obtener un listo con todos los detalles del mismo medicamento
                            List<MedicamentosDetalle> otrosMedicamentos = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == detalle.NombreMedicamento).ToList();
                            List<int> cajasOcupadas = new();
                            foreach (var otrosDetalle in otrosMedicamentos)
                            {
                                cajasOcupadas.AddRange(otrosDetalle.CodigosCajas);
                            }
                            //obtener un cajaId que no sea igual a algun cajaId del listado
                            int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamentoId && mc.Inactivo == false && mc.Detallada == false).ToList();

                            cajas = ExcluirCajas(cajas, cajasOcupadas);

                            if (cajas.Count > 0)
                            {
                                //entonces remplazar el cajaId inactivo por el nuevo cajaId
                                detalle.CodigosCajas[i] = cajas.FirstOrDefault().Codigo;
                            }
                            else
                            {
                                //si no hay algun otro cajaId para remplazarlo se mostrar un error en el view
                                return $"No hay existencia suficiente para el medicamento({detalle.CodigoDetalle + 1}).";
                            }
                        }
                    }
                }
                else//unidades
                {
                    //obtiene la lista de cajas del medicamento que no esten inactivas
                    int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                    List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamentoId && mc.Inactivo == false).ToList();
                    MedicamentosDetalle detalleCajas;

                    //por cada cajaId del detalle
                    for (int i = 0; i < detalle.CodigosCajas.Count; i++)
                    {
                        //verificar si esta inactivo
                        if (_context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigosCajas[i]).FirstOrDefault().Inactivo)
                        {
                            //int medicamentoId = _context.Medicamentos.Where(m => m.Nombre == detalle.NombreMedicamento).FirstOrDefault().Codigo;
                            //List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(mc => mc.MedicamentoId == medicamentoId && mc.Inactivo == false).ToList();

                            //obtiene el detalle en cajas del medicamento
                            detalleCajas = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == detalle.NombreMedicamento && md.TipoCantidad == 1).FirstOrDefault();
                            //si el detalle no es null
                            if (detalleCajas != null)
                            {
                                //excluye las cajas del detalle en cajas
                                cajas = ExcluirCajas(cajas, detalleCajas.CodigosCajas);
                            }

                            List<int> nuevaCajas = new();
                            int cantidadUsar = 0;

                            //agregar a nuevasCajas cajasId hasta que la cantidadUnidad de las cajas sea mayor o igual a la cantidad del detalle
                            for (int j = 0; j < cajas.Count; j++)
                            {
                                nuevaCajas.Add(cajas[i].Codigo);
                                cantidadUsar += cajas[i].CantidadUnidad;

                                if (cantidadUsar >= detalle.Cantidad)
                                {
                                    detalle.CodigosCajas = nuevaCajas;
                                    break;
                                }
                            }

                            //si se alcanzo el final de la lista sin tener la cantidad suficiente de unidades
                            if (cantidadUsar < detalle.Cantidad)
                            {
                                return $"No hay existencia suficiente para el medicamento({detalle.CodigoDetalle + 1}).";
                            }

                        }
                    }

                    //obtiene el detalle en cajas del medicamento
                    detalleCajas = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == detalle.NombreMedicamento && md.TipoCantidad == 1).FirstOrDefault();
                    //si el detalle no es null
                    if (detalleCajas != null)
                    {
                        //excluye las cajas del detalle en cajas
                        cajas = ExcluirCajas(cajas, detalleCajas.CodigosCajas);
                    }

                    //verificar si la cantidad de las cajas usadas es mayor o igual a la cantidad del detalle
                    int cantidadUsada = 0;
                    foreach (var caja in cajas)
                    {
                        cantidadUsada += caja.CantidadUnidad;
                    }

                    if (cantidadUsada < detalle.Cantidad)
                    {
                        //sino, se reasigna

                        List<int> nuevaCajas = new();
                        int cantidadUsar = 0;

                        //agregar a nuevasCajas cajasId hasta que la cantidadUnidad de las cajas sea mayor o igual a la cantidad del detalle
                        for (int j = 0; j < cajas.Count; j++)
                        {
                            nuevaCajas.Add(cajas[j].Codigo);
                            cantidadUsar += cajas[j].CantidadUnidad;

                            if (cantidadUsar >= detalle.Cantidad)
                            {
                                detalle.CodigosCajas = nuevaCajas;
                                break;
                            }
                        }

                        //si se alcanzo el final de la lista sin tener la cantidad suficiente de unidades
                        if (cantidadUsar < detalle.Cantidad)
                        {
                            return $"No hay existencia suficiente para el medicamento({detalle.CodigoDetalle + 1}).";
                        }
                    }
                }

            }
            return "";
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
                                 }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            //obtener listado de facturamedicamentocaja con facturaId == factura.codigo
            List<FacturaMedicamentosCajas> facturasCajas = _context.FacturasMedicamentosCajas.Where(fmv => fmv.CodigoFactura == factura.Codigo).ToList();
            //crear listado de detalle
            List<MedicamentosDetalle> listaDetalle = new();
            //obtener listado de cajas
            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.ToList();
            MedicamentosCajas caja;
            string nombreMedicamento;

            //agregar el primer facturasCajas a listaDetalle
            MedicamentosDetalle detalle = new MedicamentosDetalle()
            {
                CodigoDetalle = listaDetalle.Count,
                CodigosCajas = new List<int>() { facturasCajas[0].CodigoCaja },
                TipoCantidad = facturasCajas[0].TipoCantidad,
                Cantidad = facturasCajas[0].CantidadUnidad,
                Precio = facturasCajas[0].Precio
            };

            caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
            detalle.NombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;
            detalle.Total = detalle.Cantidad * detalle.Precio;
            //quitar ese elemento de facturasCajas
            facturasCajas.RemoveAt(0);

            listaDetalle.Add(detalle);

            while (facturasCajas.Count > 0)
            {
                //si algun elemento de listaDetalle tiene el mismo nombre medicamento y tipo cantidad que el primer facturasCajas
                caja = cajas.Where(mc => mc.Codigo == facturasCajas[0].CodigoCaja).FirstOrDefault();
                nombreMedicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault().Nombre;

                if(listaDetalle.Any(ld => ld.NombreMedicamento == nombreMedicamento && ld.TipoCantidad == facturasCajas[0].TipoCantidad))
                {
                    //se agrega el codigoCaja y la cantidad al listaDetalle
                    MedicamentosDetalle agregarDetalle = listaDetalle.Where(ld => ld.NombreMedicamento == nombreMedicamento && ld.TipoCantidad == facturasCajas[0].TipoCantidad).First();

                    agregarDetalle.CodigosCajas.Add(facturasCajas[0].CodigoCaja);
                    agregarDetalle.Cantidad += facturasCajas[0].CantidadUnidad;
                    agregarDetalle.Total = agregarDetalle.Cantidad * agregarDetalle.Precio;

                    listaDetalle[agregarDetalle.CodigoDetalle] = agregarDetalle;

                    facturasCajas.RemoveAt(0);
                }
                else
                {
                    //sino, se agrega un nuevo listaDetalle
                    detalle = new MedicamentosDetalle()
                    {
                        CodigoDetalle = listaDetalle.Count,
                        CodigosCajas = new List<int>() { facturasCajas[0].CodigoCaja },
                        TipoCantidad = facturasCajas[0].TipoCantidad,
                        Cantidad = facturasCajas[0].CantidadUnidad,
                        Precio = facturasCajas[0].Precio
                    };
                    detalle.NombreMedicamento = nombreMedicamento;

                    //excluir ese elemento de facturasCajas
                    facturasCajas.RemoveAt(0);

                    detalle.Total = detalle.Cantidad * detalle.Precio;

                    listaDetalle.Add(detalle);
                }
            }

            factura.MedicamentosDetalle = listaDetalle;

            detallesEdit = listaDetalle;

            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                      select new MedicamentosViewModel
                                      {

                                          Codigo = meds.Codigo,
                                          Nombre = meds.Nombre,
                                          Inactivo = (bool)meds.Inactivo,
                                          Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count

                                      }).Where(x => x.Inactivo == false).ToListAsync();

            if (medicamentos == null)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            if (medicamentos.Any(m => m.Cajas > 0))
            {
                //usar los medicamentos que tengan alguna caja en inventario
                ViewData["MedicamentosId"] = new SelectList(medicamentos.Where(m => m.Cajas > 0), "Codigo", "Nombre");

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

                    await _context.SaveChangesAsync();

                    string error = VerificarInventario(ref viewModel);

                    //si hubo alguno error al verificar la disponibilidad en el inventario se regresa al view
                    if (!error.Equals(""))//si no esta vacio
                    {
                        _notyf.Error(error);
                        return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                    }

                    //actualizar los medicamentos cajas por cada detalle
                    foreach (var detalle in viewModel.MedicamentosDetalle)
                    {
                        if (detalle.TipoCantidad == 1)//si el detalle es tipo caja
                        {
                            //por cada cajaId
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                //obtener la caja del cajaId
                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

                                //la cantidad de la caja es cero
                                caja.CantidadUnidad = 0;

                                _context.Update(caja);
                            }
                        }
                        else //si el detalle es tipo unidad
                        {
                            //se guarda en una variable la cantidad del detalle
                            var cantidadUsada = detalle.Cantidad;
                            //por cada cajaId
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                //obtener la caja del cajaId
                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

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
                    }

                    //agregar la nueva factura
                    Facturas nuevaFactura = new()
                    {
                        SubTotal = viewModel.SubTotal,
                        Total = viewModel.Total,
                        Creado = DateTime.Now,
                        CreadoNombreUsuario = _user.GetUserName(),
                        Modificado = DateTime.Now,
                        ModificadoNombreUsuario = _user.GetUserName(),
                        Inactivo = false
                    };

                    _context.Facturas.Add(nuevaFactura);

                    foreach (var detalle in viewModel.MedicamentosDetalle)
                    {
                        FacturaMedicamentosCajas facturaCaja;

                        if (detalle.TipoCantidad == 1)
                        {
                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                facturaCaja = new()
                                {
                                    CodigoCaja = cajaId,
                                    TipoCantidad = 1,
                                    CantidadUnidad = 1,
                                    Precio = detalle.Precio,
                                };

                                nuevaFactura.FacturasMedicamentosCajas.Add(facturaCaja);
                            }
                        }
                        else
                        {
                            int cantidadUsada = detalle.Cantidad;

                            foreach (var cajaId in detalle.CodigosCajas)
                            {
                                facturaCaja = new()
                                {
                                    CodigoCaja = cajaId,
                                    TipoCantidad = 2,
                                    Precio = detalle.Precio,
                                };

                                MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == cajaId).FirstOrDefault();

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
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _notyf.Error(e.Message);
                    return Json(new ResultadoFactura() { resultado = false, codigofactura = 0 });
                }
                int ultimaFactura = _context.Facturas.OrderByDescending(f => f.Codigo).First().Codigo;
                return Json(new ResultadoFactura() { resultado = true, codigofactura = ultimaFactura, mensaje = "Se ha editado la factura exitosamente!!" });
            }

            //obtener lista de medicamentos que no esten inactivos
            var medicamentos = await (from meds in _context.Medicamentos
                               .AsNoTracking()
                               .AsQueryable()
                                      select new MedicamentosViewModel
                                      {

                                          Codigo = meds.Codigo,
                                          Nombre = meds.Nombre,
                                          Inactivo = (bool)meds.Inactivo,
                                          Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count

                                      }).Where(x => x.Inactivo == false).ToListAsync();
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
                                     Modificado = f.Modificado,
                                     ModificadoNombreUsuario = f.ModificadoNombreUsuario
                                 }).Where(x => x.Codigo == id).FirstOrDefaultAsync();

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

                //por cada detalle en medicamentosDetalle
                foreach (var detalle in viewModel.MedicamentosDetalle)
                {
                    //si la caja no fue abierta
                    if (detalle.Abierto == false)
                    {
                        if (detalle.TipoCantidad == 1)//si es caja
                        {
                            if (detalle.CantidadAbierto <= detalle.Cantidad)
                            {
                                //foreach (var codigoCaja in detalle.CodigosCajas)
                                for (int i = 0; i < detalle.CantidadAbierto; i++)
                                {
                                    MedicamentosCajas caja = _context.MedicamentosCajas.Where(mc => mc.Codigo == detalle.CodigosCajas[i]).FirstOrDefault();

                                    Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == caja.CodigoMedicamento).FirstOrDefault();

                                    _context.Update(caja);
                                    caja.CantidadUnidad = medicamento.UnidadesCaja;
                                }
                            }
                            else
                            {
                                //mensaje de error
                            }
                        }
                        else//si es unidades
                        {
                            int cantidadDevolver = detalle.Cantidad;

                            foreach (var codigoCaja in detalle.CodigosCajas)
                            {
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
        public async Task<ActionResult> AgregarMedicamento(FacturaViewModel viewModel, int MedicamentoId, int TipoCantidad, int Cantidad)
        {
            List<int>? cajasUsar = null;
            List<int>? cajasUsadas = null;
            bool existente = false;
            int detalleId = 0;
            Medicamentos? medicamento = await _context.Medicamentos.Where(m => m.Codigo == MedicamentoId && m.Inactivo == false).Include(m => m.MedicamentosCajas).FirstOrDefaultAsync();

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

            if (viewModel.MedicamentosDetalle.Any(md => md.NombreMedicamento == medicamento.Nombre))//medicamento existente
            {
                //cajasUsar = a las cajas usadas por el medicamento con el mismo tipoMedicamento
                var detalleUsar = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == TipoCantidad).FirstOrDefault();
                if (detalleUsar != null)
                    cajasUsar = detalleUsar.CodigosCajas.ToList();
                //cajasUsadas = a las cajas usadas por el medicamento con diferente tipoMedicamento
                var detalleUsadas = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad != TipoCantidad).FirstOrDefault();
                if (detalleUsadas != null)
                    cajasUsadas = detalleUsadas.CodigosCajas.ToList();

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

        private ResultadoAjax AgregarCaja(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int>? cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId, bool UnidadToCaja = false)
        {
            //nueva lista de cajasSinDetallar
            List<MedicamentosCajas> cajasSinDetallar = new();
            //si detallesEdit tiene elementos
            if(detallesEdit.Count > 0)
            {
                foreach(var detalle in detallesEdit)
                {
                    if(detalle.NombreMedicamento == medicamento.Nombre && detalle.TipoCantidad == 1)
                    {
                        foreach(var codigoCaja in detalle.CodigosCajas)
                        {
                            //se agregan las cajas necesarias
                            cajasSinDetallar.Add(new MedicamentosCajas()
                            {
                                Codigo = codigoCaja
                            });
                        }
                    }
                }
            }

            cajasSinDetallar.AddRange(medicamento.MedicamentosCajas.Where(mc => mc.Detallada == false && mc.Inactivo == false && mc.CantidadUnidad > 0).OrderBy(mc => mc.FechaVencimiento).ToList());

            if (cajasUsadas != null && UnidadToCaja == false)//excluir las cajas ya usadas por el mismo medicamento en unidades, al menos que sea llevar las unidades a caja
            {
                cajasSinDetallar = ExcluirCajas(cajasSinDetallar, cajasUsadas);
            }

            if (cajasUsar != null)//si se agregara mas a el detalle existente excluir las ya usadas
            {
                cajasSinDetallar = ExcluirCajas(cajasSinDetallar, cajasUsar);

                //obtener el detalle Id de la caja
                detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().CodigoDetalle;

                existente = true;
            }

            if (cantidad > cajasSinDetallar.Count)//si se quiere agregar mas cajas de las existentes
            {
                _notyf.Warning("Cantidad Superior a la existente");
                return GenerarPartialView(true, viewModel);
            }

            //si no sean agregado cantidades en cajas del medicamento
            if (cajasUsar == null)
            {
                cajasUsar = new();
            }

            for (int i = 0; i < cantidad; i++)
            {
                cajasUsar.Add(cajasSinDetallar[i].Codigo);
            }

            viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);

            return GenerarPartialView(false, viewModel);
        }

        private ResultadoAjax AgregarUnidades(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int>? cajasUsar, List<int>? cajasUsadas, bool existente, int detalleId)
        {
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
                    if (detalle.NombreMedicamento == medicamento.Nombre && detalle.TipoCantidad == 2)
                    {
                        foreach (var codigoCaja in detalle.CodigosCajas)
                        {
                            if(detalle.Cantidad > medicamento.UnidadesCaja)
                            {
                                cantidadAgregar = medicamento.UnidadesCaja;
                                detalle.Cantidad -= medicamento.UnidadesCaja;
                            }
                            else
                                cantidadAgregar = detalle.Cantidad;

                            //se agregan las cajas necesarias
                            cajas.Add(new MedicamentosCajas()
                            {
                                Codigo = codigoCaja,
                                CantidadUnidad = cantidadAgregar,
                                Detallada = true
                            });
                        }
                    }
                }
            }

            foreach(var medicamentoCaja in medicamento.MedicamentosCajas.Where(mc => mc.Inactivo == false && mc.CantidadUnidad > 0).OrderBy(mc => mc.FechaVencimiento).ToList())
            {
                if(cajas.Any(c => c.Codigo == medicamentoCaja.Codigo))
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
                //obtener el detalle Id de la caja
                if (cajasUsadas != null)
                {
                    detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().CodigoDetalle;
                }

                //se invierte cajasUsar y cajasUsadas porque se cambiar el tipo de agregar
                resultado = AgregarCaja(viewModel, medicamento, 1, cantidadCajas, cajasUsadas, cajasUsar, existente, detalleId);
                if (resultado.error)
                {
                    return GenerarPartialView(resultado.error, resultado.viewModel);
                }

                //poner como cajas usadas las del viewModel
                var detalleNuevo = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault();
                if (detalleNuevo != null)
                    cajasUsadas = detalleNuevo.CodigosCajas;
            }

            //si se agregaron cajas y ya no quedan unidades que agregar
            if (cantidad == 0)
            {
                return GenerarPartialView(resultado.error, resultado.viewModel);
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
                        detalleId = viewModel.MedicamentosDetalle.Where(md => md.NombreMedicamento == medicamento.Nombre && md.TipoCantidad == 1).FirstOrDefault().CodigoDetalle;
                    }

                    //se agregar otra caja
                    cantidad -= medicamento.UnidadesCaja - detalleAnterior.Cantidad;
                    viewModel = AgregarCaja(viewModel, medicamento, 1, 1, cajasUsadas, cajasUsar, existente, detalleId, true).viewModel;

                    //se eliminar el detalle anterior
                    viewModel.MedicamentosDetalle.RemoveAt(detalleAnterior.CodigoDetalle);

                    //Para que los detallesId que estaban despues del elemento eleminado no pierdan continuidad
                    for (int i = detalleAnterior.CodigoDetalle; i < viewModel.MedicamentosDetalle.Count; i++)
                    {
                        viewModel.MedicamentosDetalle[i].CodigoDetalle = i;
                    }
                }
                //para que se modifique el mismo detalle
                detalleId = detalleAnterior.CodigoDetalle;
                //obtener ultima caja del detalle anterior
                MedicamentosCajas ultimaCaja = cajas.Where(mc => mc.Codigo == detalleAnterior.CodigosCajas.Last()).FirstOrDefault();
                //cantidadUsar = cantidad caja - cantidad del detalle anterior
                cantidadUsar = ultimaCaja.CantidadUnidad - detalleAnterior.Cantidad;
            }
            else
            {
                cajasUsar = new();

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
                viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                return GenerarPartialView(false, viewModel);
            }

            //si la cantidad que se pide es mayor a la que se usara
            for (int i = 0; i < cajas.Count; i++)
            {
                cajasUsar.Add(cajas[i].Codigo);
                cantidadUsar += cajas[i].CantidadUnidad;

                if (cantidad <= cantidadUsar)//si la cantidad a usar es suficiente para lo que se pide
                {
                    viewModel = AgregarDetalle(viewModel, medicamento, tipoCantidad, cantidad, cajasUsar, existente, detalleId);
                    return GenerarPartialView(false, viewModel);
                }
            }

            _notyf.Warning("Cantidad Superior a la existente");
            return GenerarPartialView(true, viewModel);
        }

        private FacturaViewModel AgregarDetalle(FacturaViewModel viewModel, Medicamentos medicamento, int tipoCantidad, int cantidad, List<int> cajasUsar, bool existente, int detalleId)
        {
            if (existente == true)
            {
                viewModel.MedicamentosDetalle[detalleId].CodigosCajas = cajasUsar;
                viewModel.MedicamentosDetalle[detalleId].Cantidad += cantidad;
                viewModel.MedicamentosDetalle[detalleId].Total = viewModel.MedicamentosDetalle[detalleId].Precio * viewModel.MedicamentosDetalle[detalleId].Cantidad;
            }
            else
            {
                MedicamentosDetalle detalle = new()
                {
                    CodigoDetalle = viewModel.MedicamentosDetalle.Count(),
                    CodigosCajas = cajasUsar,
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
            if(cajas != null && cajasExcluir != null)
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

            return new ViewAsPdf("ReporteFactura", factura)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A6
            };
        }
    }
}
