using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calendario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Calendario.Models.Data;

namespace Calendario.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly CalendarioContext _context;

        public CalendarioController(CalendarioContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var principales = await _context.DataPrincipal
                .Include(p => p.Global)
                .Include(p => p.Especifico)
                .Include(p => p.RespAcademico)
                .Include(p => p.RespOperador)
                .Include(p => p.AmbienteA)
                .ToListAsync();
            return View(principales);
        }
        public IActionResult Calendario()
        {
            return View();
        }

        // GET: Calendario/Create
        public async Task<IActionResult> Create()
        {
            // Cargar los registros de Global para el primer dropdown
            var globals = await _context.DataGlobal.ToListAsync();

            // Cargar los registros de Especifico para el segundo dropdown
            var especificos = await _context.DataEspecifico.ToListAsync();

            var respacademico = await _context.DataRespAcademico.ToListAsync();

            var respoperador = await _context.DataRespOperador.ToListAsync();

            // Pasar los registros a la vista
            ViewBag.Globals = globals;
            ViewBag.Especificos = especificos;
            ViewBag.Respacademico = respacademico;
            ViewBag.Respoperador = respoperador;

            // Devolver un modelo Principal vacío para el formulario
            return View(new Principal { AmbienteA = new List<AmbienteA>() });
        }

        // POST: Calendario/Create
        // POST: Calendario/Create
        [HttpPost]
        public async Task<IActionResult> Create(Principal principal)
        {
            // Log the form data
            Console.WriteLine($"Form submission - GlobalId: {principal.GlobalId}");
            Console.WriteLine($"Form submission - EspecificoId: {principal.EspecificoId}");
            Console.WriteLine($"Form submission - RespAcademicoId: {principal.RespAcademicoId}");
            Console.WriteLine($"Form submission - RespOperadorId: {principal.RespOperadorId}");

            // Clear navigation properties as we only need the IDs
            ModelState.Remove("Global");
            ModelState.Remove("Especifico");
            ModelState.Remove("RespAcademico");
            ModelState.Remove("RespOperador");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set navigation properties to null to avoid validation issues
                    principal.Global = null;
                    principal.Especifico = null;
                    principal.RespAcademico = null;
                    principal.RespOperador = null;

                    // Convertir todas las fechas a UTC
                    if (principal.AmbienteA != null && principal.AmbienteA.Count > 0)
                    {
                        foreach (var ambienteA in principal.AmbienteA)
                        {
                            // Convertir fecha a UTC
                            ambienteA.Fecha = DateTime.SpecifyKind(ambienteA.Fecha, DateTimeKind.Utc);
                        }
                    }

                    // First save the Principal record
                    _context.DataPrincipal.Add(principal);
                    await _context.SaveChangesAsync();

                    

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Registro detallado del error
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        Console.WriteLine($"Inner Stack trace: {ex.InnerException.StackTrace}");
                    }

                    ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error in '{key}': {error.ErrorMessage}");
                    }
                }
            }

            // If we got here, something failed; reload data for dropdowns
            ViewBag.Globals = await _context.DataGlobal.ToListAsync();
            ViewBag.Especificos = await _context.DataEspecifico.ToListAsync();
            ViewBag.Respacademico = await _context.DataRespAcademico.ToListAsync();
            ViewBag.Respoperador = await _context.DataRespOperador.ToListAsync();

            return View(principal);
        }
    }
}