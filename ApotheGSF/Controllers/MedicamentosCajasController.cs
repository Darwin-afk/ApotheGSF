﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using ApotheGSF.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ApotheGSF.Clases;

namespace ApotheGSF.Controllers
{
    [Authorize]
    public class MedicamentosCajasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;
        private readonly ClaimsPrincipal _user;

        public MedicamentosCajasController(AppDbContext context, 
                                           INotyfService notyf,
                                           IHttpContextAccessor accessor)
        {
            _context = context;
            _notyf = notyf;
            _user = accessor.HttpContext.User;
        }

        // GET: MedicamentosCajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await (from mc in _context.MedicamentosCajas
                                           .AsNoTracking()
                                           .AsQueryable()
                                           join m in _context.Medicamentos on mc.CodigoMedicamento equals m.Codigo
                                           select new MedicamentosCajasViewModel
                                           {
                                               CodigoCaja = mc.Codigo,
                                               CodigoMedicamento = mc.CodigoMedicamento,
                                               NombreMedicamento = m.Nombre,
                                               CantidadUnidad = mc.CantidadUnidad,
                                               FechaAdquirido = mc.FechaAdquirido,
                                               FechaVencimiento = mc.FechaVencimiento,
                                               Detallada = mc.Detallada,
                                               Creado = mc.Creado,
                                               CreadoNombreUsuario = mc.CreadoNombreUsuario,
                                               Modificado = mc.Modificado,
                                               ModificadoNombreUsuario = mc.ModificadoNombreUsuario
                                           }).Where(x => x.CodigoCaja == id).FirstOrDefaultAsync();

            if (medicamentosCajas == null)
            {
                return NotFound();
            }

            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Create
        [Authorize(Roles = "Administrador, Comprador")]
        public IActionResult Create()
        {
            List<Medicamentos> medicamentos = _context.Medicamentos.Where(p => p.Inactivo == false).ToList();

            if (medicamentos == null)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            if(medicamentos.Count == 0)
            {
                _notyf.Information("Es necesario tener algun medicamento");
                return RedirectToAction("Index", "Home");
            }

            ViewData["MedicamentosId"] = new SelectList(medicamentos, "Codigo", "Nombre");
            return View();
        }

        // POST: MedicamentosCajas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoMedicamento,Cajas,FechaAdquirido,FechaVencimiento")] MedicamentosCajasViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                //validar la cantidad
                if (viewModel.Cajas <= 0)
                {
                    _notyf.Error("Cantidad de cajas debe ser mayor a 0");
                    ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
                    return View(viewModel);
                }

                //validar las fechas
                if (viewModel.FechaAdquirido > DateTime.Now)
                {
                    _notyf.Error("Fecha adquirido invalida");
                    ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
                    return View(viewModel);
                }

                if (viewModel.FechaVencimiento < DateTime.Now.AddMonths(1))
                {
                    _notyf.Error("fecha vencimiento invalida");
                    ViewData["medicamentosid"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "codigo", "nombre");
                    return View(viewModel);
                }

                Medicamentos medicamento = _context.Medicamentos.Where(m => m.Codigo == viewModel.CodigoMedicamento).FirstOrDefault();

                if(medicamento.EnvioPendiente == true)
                {
                    _context.Update(medicamento);
                    medicamento.EnvioPendiente = false;
                }

                for (int i = 0; i < viewModel.Cajas; i++)
                {
                    MedicamentosCajas medicamentoCaja = new();
                    medicamentoCaja.CantidadUnidad = await _context.Medicamentos.Where(m => m.Codigo == viewModel.CodigoMedicamento)
                                                                                .Select(m => m.UnidadesCaja)
                                                                                .FirstOrDefaultAsync();
                    medicamentoCaja.CodigoMedicamento = viewModel.CodigoMedicamento;
                    medicamentoCaja.FechaAdquirido = viewModel.FechaAdquirido;
                    medicamentoCaja.FechaVencimiento = viewModel.FechaVencimiento;
                    medicamentoCaja.Detallada = false;
                    medicamentoCaja.Inactivo = false;
                    medicamentoCaja.Creado = DateTime.Now;
                    medicamentoCaja.CreadoNombreUsuario = _user.GetUserName();
                    medicamentoCaja.Modificado = DateTime.Now;
                    medicamentoCaja.ModificadoNombreUsuario = _user.GetUserName();

                    _context.Add(medicamentoCaja);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { Mensaje = "Se ha guardado exitosamente!!!" });
            }
            ViewData["MedicamentosId"] = new SelectList(_context.Medicamentos.Where(m => m.Inactivo == false), "Codigo", "Nombre");
            return View(viewModel);
        }

        // GET: MedicamentosCajas/Edit/5
        [Authorize(Roles = "Administrador, Comprador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await _context.MedicamentosCajas.FindAsync(id);

            if (medicamentosCajas == null)
            {
                return NotFound();
            }
            ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;

            return View(medicamentosCajas);
        }

        // POST: MedicamentosCajas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Codigo,CodigoMedicamento,CantidadUnidad,FechaAdquirido,FechaVencimiento")] MedicamentosCajas medicamentosCajas)
        {
            //obtener el medicamento de la caja
            Medicamentos medicamento = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault();
            //si cantidadUnidad es mayor  unidadesCaja del medicamento se regresa el error
            if (medicamentosCajas.CantidadUnidad > medicamento.UnidadesCaja)
            {
                _notyf.Error("Cantidad Superior a la Valida");
                ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;
                return View(medicamentosCajas);
            }

            if (ModelState.IsValid)
            {
                //validar la cantidad
                if (medicamentosCajas.CantidadUnidad <= 0)
                {
                    _notyf.Error("Cantidad de cajas debe ser mayor a 0");
                    ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;
                    return View(medicamentosCajas);
                }

                //validar las fechas
                if (medicamentosCajas.FechaAdquirido > DateTime.Now)
                {
                    _notyf.Error("Fecha adquirido invalida");
                    ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;
                    return View(medicamentosCajas);
                }

                if (medicamentosCajas.FechaVencimiento < DateTime.Now.AddMonths(1))
                {
                    _notyf.Error("Fecha vencimiento invalida");
                    ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;
                    return View(medicamentosCajas);
                }

                try
                {
                    _context.Update(medicamentosCajas);
                    medicamentosCajas.Modificado = DateTime.Now;
                    medicamentosCajas.ModificadoNombreUsuario = _user.GetUserName();
                    _context.Entry(medicamentosCajas).Property(c => c.Creado).IsModified = false;
                    _context.Entry(medicamentosCajas).Property(c => c.CreadoNombreUsuario).IsModified = false;
                    _context.Entry(medicamentosCajas).Property(c => c.Inactivo).IsModified = false;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentosCajasExists(medicamentosCajas.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Medicamentos", new { id = medicamentosCajas.CodigoMedicamento });
            }
            ViewData["MedicamentosId"] = _context.Medicamentos.Where(x => x.Codigo == medicamentosCajas.CodigoMedicamento).FirstOrDefault().Nombre;
            return View(medicamentosCajas);
        }

        // GET: MedicamentosCajas/Delete/5
        [Authorize(Roles = "Administrador, Comprador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MedicamentosCajas == null)
            {
                return NotFound();
            }

            var medicamentosCajas = await (from mc in _context.MedicamentosCajas
                                           .AsNoTracking()
                                           .AsQueryable()
                                           join m in _context.Medicamentos on mc.CodigoMedicamento equals m.Codigo
                                           select new MedicamentosCajasViewModel
                                           {
                                               CodigoCaja = mc.Codigo,
                                               CodigoMedicamento = mc.CodigoMedicamento,
                                               NombreMedicamento = m.Nombre,
                                               CantidadUnidad = mc.CantidadUnidad,
                                               FechaAdquirido = mc.FechaAdquirido,
                                               FechaVencimiento = mc.FechaVencimiento,
                                               Detallada = mc.Detallada,
                                               Creado = mc.Creado,
                                               CreadoNombreUsuario = mc.CreadoNombreUsuario,
                                               Modificado = mc.Modificado,
                                               ModificadoNombreUsuario = mc.ModificadoNombreUsuario
                                           }).Where(x => x.CodigoCaja == id).FirstOrDefaultAsync();

            if (medicamentosCajas == null)
            {
                return NotFound();
            }

            return View(medicamentosCajas);
        }

        // POST: MedicamentosCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CodigoCaja)
        {
            if (_context.MedicamentosCajas == null)
            {
                return Problem("Entity set 'AppDbContext.MedicamentosCajas'  is null.");
            }
            var medicamentosCajas = await _context.MedicamentosCajas.FindAsync(CodigoCaja);
            if (medicamentosCajas != null)
            {
                _context.MedicamentosCajas.Update(medicamentosCajas);
                medicamentosCajas.Inactivo = true;
            }

            await _context.SaveChangesAsync();
            return Json(medicamentosCajas.CodigoMedicamento);
        }

        private bool MedicamentosCajasExists(int id)
        {
            return (_context.MedicamentosCajas?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
