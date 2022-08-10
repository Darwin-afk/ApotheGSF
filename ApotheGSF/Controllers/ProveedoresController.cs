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
    public class ProveedoresController : Controller
    {
       
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly INotyfService _notyf;


        public ProveedoresController(AppDbContext context,
                             IHttpContextAccessor accessor,
                             INotyfService notyf
            )
        {
            _context = context;
            _user = accessor.HttpContext.User;
            _notyf = notyf;
        }

        // GET: Proveedores
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sortExpression = "", int search = 0)
        {
            StringBuilder filtro = new StringBuilder(" Inactivo == false ");
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filtro.AppendFormat("  && (Nombre.ToUpper().Contains(\"{0}\")) ", filter.ToUpper());
            }

            List<Proveedores> listado = new List<Proveedores>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await _context.Proveedores.Where(filtro.ToString()).ToListAsync();
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
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
        
            if (id == null || _context.Proveedores == null)
            {
                return NotFound();
            }

            var proveedor = _context.Proveedores
                .FirstOrDefault();
            if (proveedor == null)
            {
                return NotFound();
            }
           
            return View(proveedor);
        }

        // GET: Proveedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,RNC,Telefono1,Telefono2,Fax,Direccion,Email,TerminosdePago")] Proveedores proveedor)
        {
            if (ModelState.IsValid)
            {
                //obtener lista de proveedores
                List<Proveedores> proveedores = _context.Proveedores.Where(p => p.Inactivo == false).ToList();
                //si la lista no es null
                if (proveedores != null)
                {
                    //por cada elemento de la lista
                    foreach (var _proveedor in proveedores)
                    {
                        //se verifica si tiene el mismo nombre que el medicamento que se quiere crear
                        if (_proveedor.Nombre.ToUpper() == proveedor.Nombre.ToUpper())
                        {
                            //si lo tiene regresa error
                            _notyf.Error("Este proveedor ya existe");
                            return View(proveedor);
                        }

                    }
                }

                proveedor.Creado = DateTime.Now;
                proveedor.CreadoNombreUsuario = _user.GetUserName();
                proveedor.Modificado = DateTime.Now;
                proveedor.ModificadoNombreUsuario = _user.GetUserName();
                proveedor.Inactivo = false;
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
                
            }
            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null || _context.Proveedores == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            
            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,Nombre,RNC,Telefono1,Telefono2,Fax,Direccion,Email,TerminosdePago")] Proveedores proveedor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //obtener lista de proveedores
                    List<Proveedores> proveedores = _context.Proveedores.Where(p => p.Inactivo == false && p.Codigo != proveedor.Codigo).ToList();
                    //si la lista no es null
                    if (proveedores != null)
                    {
                        //por cada elemento de la lista
                        foreach (var _proveedor in proveedores)
                        {
                            //se verifica si tiene el mismo nombre que el medicamento que se quiere crear
                            if (_proveedor.Nombre.ToUpper() == proveedor.Nombre.ToUpper())
                            {
                                //si lo tiene regresa error
                                _notyf.Error("Este proveedor ya existe");
                                return View(proveedor);
                            }

                        }
                    }

                    proveedor.Modificado = DateTime.Now;
                    proveedor.ModificadoNombreUsuario = _user.GetUserName();
                    proveedor.Inactivo = false;
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.Codigo))
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
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            
            if (id == null || _context.Proveedores == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (proveedor == null)
            {
                return NotFound();
            }
            
            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Proveedores == null)
            {
                return Problem("Entity set 'AppDbContext.Proveedores'  is null.");
            }



            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Update(proveedor);
                proveedor.Modificado = DateTime.Now;
                proveedor.ModificadoNombreUsuario = _user.GetUserName();
                proveedor.Inactivo = true;
                _context.Entry(proveedor).Property(c => c.Creado).IsModified = false;
                _context.Entry(proveedor).Property(c => c.CreadoNombreUsuario).IsModified = false;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
          return (_context.Proveedores?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
