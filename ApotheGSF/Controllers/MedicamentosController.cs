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
using Microsoft.AspNetCore.Authorization;

namespace ApotheGSF.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(string filter, string Mensaje = "", int pageindex = 1, string sortExpression = "", int search = 0)
        {
            if (Mensaje != "")
            {
                _notyf.Custom(Mensaje, 5, "#17D155", "fas fa-check");
            }

            StringBuilder filtro = new StringBuilder(" Inactivo == false ");
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<Medicamentos> listado = new List<Medicamentos>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await _context.Medicamentos.Where(filtro.ToString()).ToListAsync();

                foreach (var item in listado)
                {
                    item.Cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == item.Codigo && mc.CantidadUnidad > 0 && mc.Inactivo == false).ToList().Count;
                }
            }

            if (listado.Count == 0 && search == 1)
                _notyf.Information("No hay medicamentos existentes");

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
        public async Task<IActionResult> Details(int? id, int pageindex = 1, string Mensaje = "")
        {

            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            if (Mensaje != "")
            {
                _notyf.Custom(Mensaje, 5, "#17D155", "fas fa-check");
            }

            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == id && m.Inactivo == false).FirstOrDefaultAsync();

            if (medicamento == null)
            {
                return NotFound();
            }

            List<MedicamentosCajas> cajas = _context.MedicamentosCajas.Where(m => m.CodigoMedicamento == medicamento.Codigo && m.CantidadUnidad > 0 && m.Inactivo == false).OrderBy(mc => mc.FechaVencimiento).ToList();

            //for (int i = 0; i < cajas.Count; i++)
            //    cajas[i].NumeroCaja = i + 1;

            var inventario = PagingList.Create(cajas, 5, pageindex, "FechaVencimiento", "");

            inventario.Action = "Details";

            ViewBag.Inventario = inventario;

            return View(medicamento);
        }

        // GET: Medicamentos/Create
        [Authorize(Roles = "Administrador, Comprador")]
        public IActionResult Create()
        {
            return View(new Medicamentos());
        }

        // POST: Medicamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,NombreCientifico,Categoria,Sustancia,Concentracion,UnidadesCaja,Reorden,Detallable")] Medicamentos medicamento)
        {
            if (ModelState.IsValid)
            {
                string error = ValidarDatos(medicamento);

                if (error != "")
                {
                    _notyf.Error(error);
                    return View(medicamento);
                }

                medicamento.Creado = DateTime.Now;
                medicamento.CreadoNombreUsuario = _user.GetUserName();
                medicamento.Modificado = DateTime.Now;
                medicamento.ModificadoNombreUsuario = _user.GetUserName();
                medicamento.Inactivo = false;
                medicamento.EnvioPendiente = false;

                _context.Medicamentos.Add(medicamento);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });
            }
            return View(medicamento);
        }

        private string ValidarDatos(Medicamentos medicamento)
        {
            if (medicamento.UnidadesCaja <= 0)
            {
                return "Las unidades de una caja deben ser mayor a 0";
            }

            if (medicamento.Reorden <= 0)
            {
                return "El reorden debe ser mayor a 0";
            }

            return "";
        }

        // GET: Medicamentos/Edit/5
        [Authorize(Roles = "Administrador, Comprador")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == id && m.Inactivo == false).FirstOrDefaultAsync();

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
        public async Task<IActionResult> Edit([Bind("Codigo,Nombre,NombreCientifico,Categoria,Sustancia,Concentracion,UnidadesCaja,Reorden,Detallable")] Medicamentos medicamento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error = ValidarDatos(medicamento);

                    if (error != "")
                    {
                        _notyf.Error(error);
                        return View(medicamento);
                    }

                    _context.Update(medicamento);
                    medicamento.Modificado = DateTime.Now;
                    medicamento.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Entry(medicamento).Property(c => c.Creado).IsModified = false;
                    _context.Entry(medicamento).Property(c => c.CreadoNombreUsuario).IsModified = false;
                    _context.Entry(medicamento).Property(c => c.Inactivo).IsModified = false;
                    _context.Entry(medicamento).Property(c => c.EnvioPendiente).IsModified = false;

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
                _notyf.Custom("Se ha guardado exitosamente!!!", 5, "#17D155", "fas fa-check");
                return RedirectToAction(nameof(Index));
            }

            return View(medicamento);
        }

        // GET: Medicamentos/Delete/5
        [Authorize(Roles = "Administrador, Comprador")]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Medicamentos == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == id && m.Inactivo == false).FirstOrDefaultAsync();

            medicamento.Cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == medicamento.Codigo && mc.CantidadUnidad > 0 && mc.Inactivo == false).ToList().Count;

            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        // POST: Medicamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteConfirmed(int Codigo)
        {
            if (_context.Medicamentos == null)
            {
                _notyf.Error("No se ha podido eliminar");
                return false;
            }
            var medicamento = await _context.Medicamentos.Where(m => m.Codigo == Codigo).Include(m => m.MedicamentosCajas.Where(mc => mc.Inactivo == false)).FirstOrDefaultAsync();
            if (medicamento != null)
            {
                _context.Medicamentos.Update(medicamento);
                medicamento.Modificado = DateTime.Now;
                medicamento.ModificadoNombreUsuario = _user.GetUserName();
                medicamento.Inactivo = true;
                _context.Entry(medicamento).Property(c => c.Creado).IsModified = false;
                _context.Entry(medicamento).Property(c => c.CreadoNombreUsuario).IsModified = false;

                foreach (var caja in medicamento.MedicamentosCajas)
                {
                    _context.MedicamentosCajas.Update(caja);
                    caja.Inactivo = true;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private bool MedicamentoExists(int id)
        {
            return (_context.Medicamentos?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<int> ObtenerUnidades(int CodigoMedicamento)
        {
            if(CodigoMedicamento == 0)
            {
                return 0;
            }

            Medicamentos medicamento = await _context.Medicamentos.Where(m => m.Codigo == CodigoMedicamento).FirstOrDefaultAsync();

            return medicamento.UnidadesCaja;
        }

        [Authorize(Roles = "Administrador, Comprador")]
        public async Task<IActionResult> ReporteInventario(string filter)
        {
            StringBuilder filtro = new StringBuilder(" Inactivo == false ");

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<Medicamentos> medicamentos = await _context.Medicamentos.Where(filtro.ToString()).ToListAsync();

            foreach (var item in medicamentos)
            {
                item.Cajas = _context.MedicamentosCajas.Where(mc => mc.CodigoMedicamento == item.Codigo && mc.CantidadUnidad > 0 && mc.Inactivo == false).ToList().Count;
            }

            return new ViewAsPdf("ReporteInventario", medicamentos);
        }

        public IActionResult EnviarCorreo(int codigoMedicamento)
        {
            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == codigoMedicamento).FirstOrDefault();

            CorreoViewModel correo = new CorreoViewModel()
            {
                NombreMedicamento = medicamento.Nombre
            };

            ViewBag.ProveedoresId = new SelectList(_context.Laboratorios.Where(p => p.Inactivo == false).ToList(), "Codigo", "Nombre");

            return View(correo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarCorreo([Bind("NombreMedicamento,CodigoLaboratorio,Cajas")] CorreoViewModel correo)
        {
            if(correo.CodigoLaboratorio <= 0)
            {
                _notyf.Error("Debe seleccionar algun laboratorio");

                Medicamentos _medicamento = _context.Medicamentos.Where(m => m.Nombre == correo.NombreMedicamento).FirstOrDefault();

                ViewBag.ProveedoresId = new SelectList(_context.Laboratorios.Where(p => p.Inactivo == false).ToList(), "Codigo", "Nombre");

                return View(correo);
            }

            if (correo.Cajas <= 0)
            {
                _notyf.Error("La cantidad de cajas debe ser mayor a 0");

                Medicamentos _medicamento = _context.Medicamentos.Where(m => m.Nombre == correo.NombreMedicamento).FirstOrDefault();

                ViewBag.ProveedoresId = new SelectList(_context.Laboratorios.Where(p => p.Inactivo == false).ToList(), "Codigo", "Nombre");

                return View(correo);
            }

            var smtpClient = ConfigurarSmtpClient();
            var mensaje = GenerarCorreo(correo);

            smtpClient.Send(mensaje);

            //hacer que no aparezca en notificaciones ya que se pidio reabastecimiento
            Medicamentos medicamento = _context.Medicamentos.Where(m => m.Nombre == correo.NombreMedicamento && m.Inactivo == false).FirstOrDefault();
            _context.Update(medicamento);
            medicamento.EnvioPendiente = true;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home", new { Mensaje = "Se ha enviado exitosamente!!!" });
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
            Laboratorios laboratorio = _context.Laboratorios.Where(p => p.Codigo == correo.CodigoLaboratorio).FirstOrDefault();

            var mail = new MailMessage();
            mail.From = new MailAddress($"{correEmisor}", "Botica Popular", Encoding.UTF8);
            mail.To.Add(new MailAddress($"{laboratorio.Email}"));

            mail.Subject = "Reabastecer Inventario";
            mail.SubjectEncoding = Encoding.UTF8;

            mail.Body = $"Saludos {laboratorio.Nombre}, <br/><br/> La Botica Popular de la Iglesia Santa Rosa de Lima, " +
                $"solicita {correo.Cajas} cajas de {correo.NombreMedicamento}.<br/> Por favor enviar las cantidades necesarias lo mas pronto posible." +
                $"<br/> <br/> Se despide cordialmente la administración. <br/> <br/> " +
                $"<img src='https://i.pinimg.com/564x/4e/b9/b7/4eb9b70dee1f41c59dc790b67d6b498b.jpg' alt='Portrait1'  />";

            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;

            return mail;
        }


    }
}
