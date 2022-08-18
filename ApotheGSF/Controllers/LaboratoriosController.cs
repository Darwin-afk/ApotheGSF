using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ApotheGSF.Clases;
using System.Text;
using System.Linq.Dynamic.Core;
using ReflectionIT.Mvc.Paging;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ApotheGSF.Controllers
{
    [Authorize(Roles = "Administrador, Comprador")]
    public class LaboratoriosController : Controller
    {

        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly INotyfService _notyf;


        public LaboratoriosController(AppDbContext context,
                             IHttpContextAccessor accessor,
                             INotyfService notyf
            )
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _notyf = notyf;
        }

        // GET: Laboratorios
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

            List<Laboratorios> listado = new List<Laboratorios>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await _context.Laboratorios.Where(filtro.ToString()).ToListAsync();
            }

            if (listado.Count == 0 && search == 1)
                _notyf.Information("No hay laboratorios existentes");

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "Nombre" : sortExpression;
            var model = PagingList.Create(listado, 3, pageindex, sortExpression, "");
            model.RouteValue = new RouteValueDictionary {
                            { "filter", filter}
            };
            model.Action = "Index";

            return model != null ?
                View(model) :
                Problem("Entity set 'ApplicationDbContext.Laboratorios'  is null.");
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Laboratorios == null)
            {
                return NotFound();
            }

            var laboratorio = await _context.Laboratorios.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
            if (laboratorio == null)
            {
                return NotFound();
            }

            return View(laboratorio);
        }

        // GET: Laboratorios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Laboratorios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,RNC,Telefono1,Telefono2,Fax,Direccion,Email,TerminosdePago")] Laboratorios laboratorio)
        {
            if (ModelState.IsValid)
            {
                string error = ValidarDatos(laboratorio);

                if (error != "")
                {
                    _notyf.Error(error);
                    return View(laboratorio);
                }

                laboratorio.Creado = DateTime.Now;
                laboratorio.CreadoNombreUsuario = _user.GetUserName();
                laboratorio.Modificado = DateTime.Now;
                laboratorio.ModificadoNombreUsuario = _user.GetUserName();
                laboratorio.Inactivo = false;
                _context.Add(laboratorio);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });

            }
            return View(laboratorio);
        }

        private string ValidarDatos(Laboratorios laboratorio)
        {
            //obtener lista de proveedores
            List<Laboratorios> laboratorios = _context.Laboratorios.Where(p => p.Inactivo == false && p.Codigo != laboratorio.Codigo).ToList();
            //si la lista no es null
            if (laboratorios != null)
            {
                //por cada elemento de la lista verificar repeticion de datos
                foreach (var item in laboratorios)
                {
                    //nombre
                    if (item.Nombre.ToUpper() == laboratorio.Nombre.ToUpper())
                    {
                        return "Estelaboratorio ya existe";
                    }

                    //rnc
                    if (item.RNC == laboratorio.RNC)
                    {
                        return "RNC existente";
                    }

                    //telefono 1 y fax
                    if (laboratorio.Telefono1 == laboratorio.Fax)
                    {
                        return "Teléfono 1 y Fax no pueden ser iguales";
                    }

                    //telefono 1
                    if (item.Telefono1 == laboratorio.Telefono1 || item.Fax == laboratorio.Telefono1)
                    {
                        return "Teléfono 1 existente";
                    }
                    //fax
                    if (item.Telefono1 == laboratorio.Fax || item.Fax == laboratorio.Fax)
                    {
                        return "Fax existente";
                    }

                    //telefono 2 viewModel
                    if (laboratorio.Telefono2 != null)
                    {
                        if (laboratorio.Telefono1 == laboratorio.Telefono2)
                        {
                            return "Teléfono 1 y Teléfono 2 no pueden ser iguales";
                        }

                        if (laboratorio.Fax == laboratorio.Telefono2)
                        {
                            return "Teléfono 2 y Fax no pueden ser iguales";
                        }

                        if (item.Telefono1 == laboratorio.Telefono2 || item.Fax == laboratorio.Telefono2)
                        {
                            return "Teléfono 2 existente";
                        }

                        if (item.Telefono2 != null)
                        {
                            if (item.Telefono2 == laboratorio.Telefono2)
                            {
                                return "Teléfono 2 existente";
                            }
                        }
                    }
                    //telefono 2 database
                    if (item.Telefono2 != null)
                    {
                        if (item.Telefono2 == laboratorio.Telefono1)
                        {
                            return "Teléfono 1 existente";
                        }

                        if (item.Telefono2 == laboratorio.Fax)
                        {
                            return "Fax existente";
                        }
                    }


                    //email
                    if (item.Email == laboratorio.Email)
                    {
                        return "Email existente";
                    }
                }
            }

            //validar email
            if (!laboratorio.Email.IsValidEmail())
            {
                return "Email invalido";
            }

            return "";
        }

        // GET: Laboratorios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Laboratorios == null)
            {
                return NotFound();
            }

            var laboratorio = await _context.Laboratorios.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
            if (laboratorio == null)
            {
                return NotFound();
            }

            return View(laboratorio);
        }

        // POST: Laboratorios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,Nombre,RNC,Telefono1,Telefono2,Fax,Direccion,Email,TerminosdePago")] Laboratorios laboratorio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error = ValidarDatos(laboratorio);

                    if (error != "")
                    {
                        _notyf.Error(error);
                        return View(laboratorio);
                    }

                    _context.Update(laboratorio);
                    laboratorio.Modificado = DateTime.Now;
                    laboratorio.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Entry(laboratorio).Property(c => c.Creado).IsModified = false;
                    _context.Entry(laboratorio).Property(c => c.CreadoNombreUsuario).IsModified = false;
                    _context.Entry(laboratorio).Property(c => c.Inactivo).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaboratorioExists(laboratorio.Codigo))
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
            return View(laboratorio);
        }

        // GET: Laboratios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Laboratorios == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Laboratorios.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Laboratorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteConfirmed(int Codigo)
        {
            if (_context.Laboratorios == null)
            {
                _notyf.Error("No se ha podido eliminar");
                return false;
            }

            var laboratorio = await _context.Laboratorios.FindAsync(Codigo);
            if (laboratorio != null)
            {
                _context.Laboratorios.Update(laboratorio);
                laboratorio.Modificado = DateTime.Now;
                laboratorio.ModificadoNombreUsuario = _user.GetUserName();
                laboratorio.Inactivo = true;
                _context.Entry(laboratorio).Property(c => c.Creado).IsModified = false;
                _context.Entry(laboratorio).Property(c => c.CreadoNombreUsuario).IsModified = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private bool LaboratorioExists(int id)
        {
            return (_context.Laboratorios?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}