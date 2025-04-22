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
                .ThenInclude(a => a.Ambiente)
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
            var ambientes = await _context.DataAmbiente.ToListAsync();

            // Pasar los registros a la vista
            ViewBag.Globals = globals;
            ViewBag.Especificos = especificos;
            ViewBag.Respacademico = respacademico;
            ViewBag.Respoperador = respoperador;
            ViewBag.Ambientes = ambientes;

            // Devolver un modelo Principal vacío para el formulario
            return View(new Principal { AmbienteA = new List<AmbienteA>() });
        }

        // POST: Calendario/Create
        // POST: Calendario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Principal principal)
        {
            Console.WriteLine($"Cantidad de bloques de AmbienteA: {principal.AmbienteA?.Count}");

            // Remover propiedades de navegación del modelo para evitar problemas de validación
            ModelState.Remove("Global");
            ModelState.Remove("Especifico");
            ModelState.Remove("RespAcademico");
            ModelState.Remove("RespOperador");

            // También quitar las propiedades de navegación de AmbienteA
            if (principal.AmbienteA != null)
            {
                for (int i = 0; i < principal.AmbienteA.Count; i++)
                {
                    ModelState.Remove($"AmbienteA[{i}].Ambiente");
                    ModelState.Remove($"AmbienteA[{i}].Principal");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si hay ambientes para procesar
                    if (principal.AmbienteA != null)
                    {
                        // Filtrar ambientes inválidos (puede haber entradas vacías)
                        principal.AmbienteA = principal.AmbienteA
                            .Where(a => a.AmbienteId > 0)
                            .ToList();

                        // Validación de conflictos de horario
                        if (principal.AmbienteA != null && principal.AmbienteA.Any())
                        {
                            foreach (var nuevoAmbiente in principal.AmbienteA)
                            {
                                var fecha = nuevoAmbiente.Fecha.Date;

                                var conflictos = await _context.DataAmbienteA
                                    .Where(a => a.AmbienteId == nuevoAmbiente.AmbienteId && a.Fecha.Date == fecha)
                                    .ToListAsync();

                                foreach (var existente in conflictos)
                                {
                                    if (nuevoAmbiente.HoraInicio < existente.HoraFin && nuevoAmbiente.HoraFin > existente.HoraInicio)
                                    {
                                        var nombreAmbiente = await _context.DataAmbiente
                                            .Where(a => a.Id == nuevoAmbiente.AmbienteId)
                                            .Select(a => a.NomAmb)
                                            .FirstOrDefaultAsync();

                                        ModelState.AddModelError("", $"El ambiente '{nombreAmbiente}' ya está reservado el {fecha:dd/MM/yyyy} de {existente.HoraInicio:hh\\:mm} a {existente.HoraFin:hh\\:mm}.");
                                    }
                                }
                            }
                        }

                        // Si hay errores, no continuamos
                        if (!ModelState.IsValid)
                        {
                            throw new Exception("Conflictos de horarios detectados.");
                        }

                        // Preparar los datos de fecha/hora
                        foreach (var ambiente in principal.AmbienteA)
                        {
                            ambiente.Fecha = DateTime.SpecifyKind(ambiente.Fecha, DateTimeKind.Utc);
                            // Limpiar referencias circulares
                            ambiente.Principal = null;
                            ambiente.Ambiente = null;
                        }
                    }

                    // Limpiar otras referencias para evitar problemas de tracking
                    principal.Global = null;
                    principal.Especifico = null;
                    principal.RespAcademico = null;
                    principal.RespOperador = null;

                    // Añadir y guardar el registro principal con sus ambientes
                    _context.DataPrincipal.Add(principal);

                    // Preparar los datos de fecha/hora
                    foreach (var ambiente in principal.AmbienteA)
                    {
                        var horaInicio = ambiente.HoraInicio;
                        var horaFin = ambiente.HoraFin;
                        var fecha = ambiente.Fecha;

                        ambiente.Codigo = $"{ambiente.AmbienteId:D2}{horaInicio.Hours:D2}{horaInicio.Minutes:D2}{horaFin.Hours:D2}{horaFin.Minutes:D2}{fecha.Day:D2}{fecha.Month:D2}{fecha.Year % 100:D2}";
                    }



                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
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
                // Log de errores de validación
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error en '{state.Key}': {error.ErrorMessage}");
                    }
                }
            }

            // Si llegamos aquí, algo falló; recargar datos para los dropdowns
            ViewBag.Globals = await _context.DataGlobal.ToListAsync();
            ViewBag.Especificos = await _context.DataEspecifico.ToListAsync();
            ViewBag.Respacademico = await _context.DataRespAcademico.ToListAsync();
            ViewBag.Respoperador = await _context.DataRespOperador.ToListAsync();
            ViewBag.Ambientes = await _context.DataAmbiente.ToListAsync();

            return View(principal);
        }

        // GET: Calendario/GetEvents
        public async Task<IActionResult> GetEvents()
        {
            // Obtener todos los registros principales con sus ambientes
            var principales = await _context.DataPrincipal
                .Include(p => p.Global)
                .Include(p => p.Especifico)
                .Include(p => p.RespAcademico)
                .Include(p => p.RespOperador)
                .Include(p => p.AmbienteA)
                    .ThenInclude(a => a.Ambiente)
                .ToListAsync();

            // Transformar los datos al formato que espera FullCalendar
            var events = new List<object>();

            foreach (var principal in principales)
            {
                if (principal.AmbienteA != null)
                {
                    foreach (var ambiente in principal.AmbienteA)
                    {
                        // Combinar fecha y hora para crear fechas de inicio y fin
                        var startDate = ambiente.Fecha.Add(ambiente.HoraInicio);
                        var endDate = ambiente.Fecha.Add(ambiente.HoraFin);

                        events.Add(new
                        {
                            id = ambiente.Id,
                            title = $"{principal.Global?.NomGlo} - {ambiente.Ambiente?.NomAmb} - {ambiente.Codigo}",
                            codigo = ambiente.Codigo,
                            start = startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = endDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                            description = $"Específico: {principal.Especifico?.NomEsp}\nResponsable Académico: {principal.RespAcademico?.NomAcad}\nResponsable Operador: {principal.RespOperador?.NomOpe}",
                            backgroundColor = "#007bff", // Color azul por defecto, puedes personalizarlo
                            borderColor = "#0066cc",
                            textColor = "#ffffff"
                        });
                    }
                }
            }

            return Json(events);
        }
    }
}