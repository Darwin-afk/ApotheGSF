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
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ApotheGSF.Controllers
{
    public class MedicamentosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly INotyfService _notyf;

        /*CONFIGURACIÓN SMTP:
    ---------------------------------------------------------
    * OUTLOOK -->
    servidor SMTP: smtp-mail.outlook.com
    puerto: 587
    ---------------------------------------------------------
    * GMAIL -->
    servidor SMTP: smtp.gmail.com
    puerto: 465 (SSL); 587 (TLS)
    ---------------------------------------------------------
    * YAHOO! -->
    servidor SMTP: smtp.mail.yahoo.com
    puerto: 25 ó 265
       */

        private string[] microsoftDomains = new string[] { "@outlook", "@live", "@hotmail" };
        private int[] microsoftPorts = new int[] { 587 };

        private string[] googleDomains = new string[] { "@gmail" };
        private int[] googlePorts = new int[] { 587, 487 };

        private string[] yahooDomains = new string[] { "@yahoo" };
        private int[] yahooPorts = new int[] { 25, 265 };

        private string correEmisor = new string("ApotheGSF@outlook.com");

        public MedicamentosController(AppDbContext context,
                             IHttpContextAccessor accessor,
                             INotyfService notyf
            )
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _notyf = notyf;
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
                                     Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo && m.CantidadUnidad > 0 && m.Inactivo == false).ToList().Count,

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
                                          ProveedoresId = provMed.CodigoProveedor,
                                          MedicamentosId = provMed.CodigoMedicamento
                                      }
                                      select new Proveedores
                                      {
                                          Nombre = p.Nombre
                                      }
                                      ).Select(x => x.Nombre).ToList())

                                 }).Where(filtro.ToString()).ToListAsync();
            }
            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Nombre" : sortExpression;
            var model = PagingList.Create(listado, 3, pageindex, sortExpression, "");
            model.RouteValue = new RouteValueDictionary {
                            { "filter", filter}
            };
            model.Action = "Index";

            if (string.IsNullOrWhiteSpace(filter))
            {
                ViewBag.Filtro = "";
            }
            else
                ViewBag.Filtro = filter;


            return model != null ?
                View(model) :
                Problem("Entity set 'ApplicationDbContext.ApplicationUser'  is null.");
            ;
        }

        // GET: Medicamentos/Details/5
        public async Task<IActionResult> Details(int? id, int pageindex = 1)
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
                                              ProveedoresId = provMed.CodigoProveedor,
                                              MedicamentosId = provMed.CodigoMedicamento
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

            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == medicamento.Codigo && m.CantidadUnidad > 0 && m.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            for (int i = 0; i < cajas.Count; i++)
                cajas[i].NumeroCaja = i + 1;

            var inventario = PagingList.Create(cajas, 5, pageindex, "Codigo", "");

            inventario.Action = "Details";

            ViewBag.Inventario = inventario;

            //ViewBag.Inventario = (List<MedicamentosCajas>)_context.MedicamentosCajas.Where(m => m.CodigoMedicamento == medicamento.Codigo && m.CantidadUnidad > 0 && m.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();
            return View(medicamento);
        }

        // GET: Medicamentos/Create
        public IActionResult Create()
        {
            ViewBag.CodigoProveedores = new MultiSelectList(_context.Proveedores.Where(p => p.Inactivo == false), "Codigo", "Nombre");
            return View(new MedicamentosViewModel());
        }

        // POST: Medicamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Categoria,Sustancia,Concentracion,UnidadesCaja,Costo,PrecioUnidad,Indicaciones,Dosis,CodigosProveedores")] MedicamentosViewModel viewModel)
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
                    Inactivo = false,
                    EnvioPendiente = false
                };

                _context.Medicamentos.Add(newMedicamentos);

                foreach (var item in viewModel.CodigosProveedores)
                {
                    ProveedorMedicamentos proveedorMedicamentos = new()
                    {
                        CodigoProveedor = item

                    };

                    newMedicamentos.ProveedoresMedicamentos.Add(proveedorMedicamentos);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores.Where(p => p.Inactivo == false), "Codigo", "Nombre");
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

            medicamentos.CodigosProveedores = await (from provMeds in _context.ProveedoresMedicamentos
                                               .Where(x => x.CodigoMedicamento == medicamentos.Codigo)
                                               .AsNoTracking()
                                                     join proveedores in _context.Proveedores on provMeds
                                                    .CodigoProveedor equals proveedores.Codigo
                                                     select proveedores.Codigo).ToListAsync();

            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores.Where(p => p.Inactivo == false), "Codigo", "Nombre", medicamentos.CodigosProveedores);

            return View(medicamentos);
        }

        // POST: Medicamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,Nombre,Categoria,Sustancia,Concentracion,Costo,PrecioUnidad,UnidadesCaja,Indicaciones,Dosis,CodigosProveedores")] MedicamentosViewModel viewModel)
        {

            ModelState.Remove("NombreProveedor");

            if (ModelState.IsValid)
            {
                try
                {
                    var editmedicamento = await _context.Medicamentos.Include(x => x.ProveedoresMedicamentos).
                                       FirstOrDefaultAsync(y => y.Codigo == viewModel.Codigo);

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
                        if (!viewModel.CodigosProveedores.Contains(proveedor.CodigoProveedor))
                        {
                            editmedicamento.ProveedoresMedicamentos.Remove(proveedor);
                        }
                    }

                    foreach (var newProveedorId in viewModel.CodigosProveedores)
                    {
                        if (!editmedicamento.ProveedoresMedicamentos.Any(x => x.CodigoProveedor == newProveedorId))
                        {
                            var nuevoProv = new ProveedorMedicamentos
                            {
                                CodigoProveedor = newProveedorId

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

            ViewBag.ProveedoresId = new MultiSelectList(_context.Proveedores.Where(p => p.Inactivo == false), "Codigo", "Nombre", viewModel.CodigosProveedores);
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
                                         Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count,

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
                                              ProveedoresId = provMed.CodigoProveedor,
                                              MedicamentosId = provMed.CodigoMedicamento
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
            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == id).Include(m => m.MedicamentosCajas.Where(mc => mc.Inactivo == false)).FirstOrDefaultAsync();
            if (medicamento != null)
            {
                _context.Medicamentos.Update(medicamento);
                medicamento.Modificado = DateTime.Now;  
                medicamento.ModificadoNombreUsuario = _user.GetUserName();
                medicamento.Inactivo = true;
                _context.Entry(medicamento).Property(c => c.Creado).IsModified = false;
                _context.Entry(medicamento).Property(c => c.CreadoNombreUsuario).IsModified = false;

                foreach(var caja in medicamento.MedicamentosCajas)
                {
                    _context.MedicamentosCajas.Update(caja);
                    caja.Inactivo = true;
                }
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
                                                                  Cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == meds.Codigo).ToList().Count,

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
                                                                       ProveedoresId = provMed.CodigoProveedor,
                                                                       MedicamentosId = provMed.CodigoMedicamento
                                                                   }
                                                                   select new Proveedores
                                                                   {
                                                                       Nombre = p.Nombre
                                                                   }
                                                                   ).Select(x => x.Nombre).ToList())

                                                              }).Where(filtro.ToString()).ToListAsync();

            return new ViewAsPdf("ReporteInventario", medicamentos);
        }

        public IActionResult EnviarCorreo(int codigoMedicamento)
        {
            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == codigoMedicamento).FirstOrDefault();

            CorreoViewModel correo = new CorreoViewModel()
            {
                NombreMedicamento = medicamento.Nombre
            };

            //selectList con los proveedores del medicamento
            List<int> codigosProveedores = _context.ProveedoresMedicamentos.Where(pm => pm.CodigoMedicamento == medicamento.Codigo).Select(pm => pm.CodigoProveedor).ToList();

            ViewBag.ProveedoresId = new SelectList(_context.Proveedores.Where(p => codigosProveedores.Contains(p.Codigo) && p.Inactivo == false).ToList(), "Codigo", "Nombre");

            return View(correo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarCorreo(CorreoViewModel correo)
        {
            var smtpClient = ConfigurarSmtpClient();
            var mensaje = GenerarCorreo(correo);

            smtpClient.Send(mensaje);

            //hacer que no aparezca en notificaciones ya que se pidio reabastecimiento
            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Nombre == correo.NombreMedicamento && m.Inactivo == false).FirstOrDefault();
            _context.Update(medicamento);
            medicamento.EnvioPendiente = true;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private SmtpClient ConfigurarSmtpClient()
        {
            (string host, int port) = ObtenerHost($"{correEmisor}");
            var smtpClient = new SmtpClient(host, port);

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential($"{correEmisor}", "Red321_0");
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            return smtpClient;
        }

        private (string host, int port) ObtenerHost(string correo)
        {
            if (correo.Contains(microsoftDomains, ignoreCase: true))
            {
                return ("smtp-mail.outlook.com", microsoftPorts[0]);

            }
            else if (correo.Contains(googleDomains, ignoreCase: true))
            {
                return ("smtp.gmail.com", googlePorts[0]);

            }
            else if (correo.Contains(yahooDomains, ignoreCase: true))
            {
                return ("smtp.mail.yahoo.com", yahooPorts[0]);
            }

            throw new InvalidOperationException("No hay un host configurado para este correo");
        }

        private MailMessage GenerarCorreo(CorreoViewModel correo)
        {
            Proveedores proveedor = _context.Proveedores.Where(p => p.Codigo == correo.CodigoProveedor).FirstOrDefault();

            var mail = new MailMessage();
            mail.From = new MailAddress($"{correEmisor}", "Botica Popular", Encoding.UTF8);
            mail.To.Add(new MailAddress($"{proveedor.Email}"));

            mail.Subject = "Reabastecer Inventario";
            mail.SubjectEncoding = Encoding.UTF8;

            mail.Body = $"Se solicitan {correo.Cajas} cajas de {correo.NombreMedicamento}.";
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;

            return mail;
        }


    }
}
