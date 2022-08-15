﻿using System;
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

            List<Proveedores> listado = new List<Proveedores>();
            if (search == 1 || (search == 0 && !string.IsNullOrWhiteSpace(sortExpression)))
            {
                listado = await _context.Proveedores.Where(filtro.ToString()).ToListAsync();
            }

            if (listado.Count == 0 && search == 1)
                _notyf.Information("No hay proveedores existentes");

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

            var proveedor = await _context.Proveedores.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
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
                string error = ValidarDatos(proveedor);

                if (error != "")
                {
                    _notyf.Error(error);
                    return View(proveedor);
                }

                proveedor.Creado = DateTime.Now;
                proveedor.CreadoNombreUsuario = _user.GetUserName();
                proveedor.Modificado = DateTime.Now;
                proveedor.ModificadoNombreUsuario = _user.GetUserName();
                proveedor.Inactivo = false;
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });

            }
            return View(proveedor);
        }

        private string ValidarDatos(Proveedores proveedor)
        {
            //obtener lista de proveedores
            List<Proveedores> proveedores = _context.Proveedores.Where(p => p.Inactivo == false && p.Codigo != proveedor.Codigo).ToList();
            //si la lista no es null
            if (proveedores != null)
            {
                //por cada elemento de la lista verificar repeticion de datos
                foreach (var item in proveedores)
                {
                    //nombre
                    if (item.Nombre.ToUpper() == proveedor.Nombre.ToUpper())
                    {
                        return "Este proveedor ya existe";
                    }

                    //rnc
                    if (item.RNC == proveedor.RNC)
                    {
                        return "RNC existente";
                    }

                    //telefono 1 y fax
                    if (proveedor.Telefono1 == proveedor.Fax)
                    {
                        return "Teléfono 1 y Fax no pueden ser iguales";
                    }

                    //telefono 1
                    if (item.Telefono1 == proveedor.Telefono1 || item.Fax == proveedor.Telefono1)
                    {
                        return "Teléfono 1 existente";
                    }
                    //fax
                    if (item.Telefono1 == proveedor.Fax || item.Fax == proveedor.Fax)
                    {
                        return "Fax existente";
                    }

                    //telefono 2 viewModel
                    if(proveedor.Telefono2 != null)
                    {
                        if(proveedor.Telefono1 == proveedor.Telefono2)
                        {
                            return "Teléfono 1 y Teléfono 2 no pueden ser iguales";
                        }

                        if (proveedor.Fax == proveedor.Telefono2)
                        {
                            return "Teléfono 2 y Fax no pueden ser iguales";
                        }

                        if (item.Telefono1 == proveedor.Telefono2 || item.Fax == proveedor.Telefono2)
                        {
                            return "Teléfono 2 existente";
                        }

                        if(item.Telefono2 != null)
                        {
                            if(item.Telefono2 == proveedor.Telefono2)
                            {
                                return "Teléfono 2 existente";
                            }
                        }
                    }
                    //telefono 2 database
                    if(item.Telefono2 != null)
                    {
                        if(item.Telefono2 == proveedor.Telefono1)
                        {
                            return "Teléfono 1 existente";
                        }

                        if (item.Telefono2 == proveedor.Fax)
                        {
                            return "Fax existente";
                        }
                    }
                    

                    //email
                    if (item.Email == proveedor.Email)
                    {
                        return "Email existente";
                    }
                }
            }

            //validar email
            if (!proveedor.Email.IsValidEmail())
            {
                return "Email invalido";
            }

            return "";
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            
            if (id == null || _context.Proveedores == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
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
                    string error = ValidarDatos(proveedor);

                    if (error != "")
                    {
                        _notyf.Error(error);
                        return View(proveedor);
                    }

                    _context.Update(proveedor);
                    proveedor.Modificado = DateTime.Now;
                    proveedor.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Entry(proveedor).Property(c => c.Creado).IsModified = false;
                    _context.Entry(proveedor).Property(c => c.CreadoNombreUsuario).IsModified = false;
                    _context.Entry(proveedor).Property(c => c.Inactivo).IsModified = false;
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
                _notyf.Custom("Se ha guardado exitosamente!!!", 5, "#17D155", "fas fa-check");
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

            var proveedor = await _context.Proveedores.Where(p => p.Codigo == id && p.Inactivo == false).FirstOrDefaultAsync();
            if (proveedor == null)
            {
                return NotFound();
            }
            
            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteConfirmed(int Codigo)
        {
            if (_context.Proveedores == null)
            {
                _notyf.Error("No se ha podido eliminar");
                return false;
            }

            var proveedor = await _context.Proveedores.FindAsync(Codigo);
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
            return true;
        }

        private bool ProveedorExists(int id)
        {
          return (_context.Proveedores?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
