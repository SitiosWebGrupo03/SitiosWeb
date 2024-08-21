using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

namespace SitiosWeb.Controllers
{
    public class RebajosController : Controller
    {
        private readonly Tiusr22plProyectoContext _context;

        public RebajosController(Tiusr22plProyectoContext context)
        {
            _context = context;
        }

        // GET: Rebajos
        public async Task<IActionResult> Index()
        {
            var tiusr22plProyectoContext = _context.Rebajos.Include(r => r.IdColaboradorNavigation).Include(r => r.IdTipoRebajoNavigation).Include(r => r.IdValidadorNavigation).Include(r => r.InconsistenciaNavigation);
            return View(await tiusr22plProyectoContext.ToListAsync());
        }


        // GET: Rebajos/Create
        public IActionResult Create()
        {
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo");
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion");
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia");
            return View();
        }

        public async Task<IActionResult> aplicarRebajo(string colab, DateOnly fecha, int idtipoinconsistencia, int IdTipoRebajo) 
        {

            var colaborador = _context.Colaboradores
                .FirstOrDefault(c => c.Identificacion == colab);

            var rebajo = new Rebajos
            {
                IdColaborador = colab,
                IdValidador = Request.Cookies["Id"],
                FechaRebajo = fecha,
                Inconsistencia = idtipoinconsistencia,
                IdTipoRebajo = IdTipoRebajo,
                Aprobacion = true
            };

            _context.Rebajos.Add(rebajo);
            await _context.SaveChangesAsync();

            string nombre = colaborador.Nombre;
            string correo = colaborador.Correo;

            EnviarCorreo(correo, "Rebajo aplicado", $"Estimado {nombre} se le ha aplicado un rebajo, por informacion consulte a su jefatura");

            return View("~/Views/Rebajos/Index.cshtml");

        }

        private void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("calderonmora6@gmail.com", "qsre xvxi yyvt flyw"),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("calderonmora6@gmail.com"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(destinatario);

                smtpClient.Send(mailMessage);
                Console.WriteLine("Correo enviado con éxito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRebajo,IdColaborador,IdValidador,FechaRebajo,Inconsistencia,IdTipoRebajo,Aprobacion")] Rebajos rebajos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rebajos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        // GET: Rebajos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rebajos = await _context.Rebajos.FindAsync(id);
            if (rebajos == null)
            {
                return NotFound();
            }
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        // POST: Rebajos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRebajo,IdColaborador,IdValidador,FechaRebajo,Inconsistencia,IdTipoRebajo,Aprobacion")] Rebajos rebajos)
        {
            if (id != rebajos.IdRebajo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rebajos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RebajosExists(rebajos.IdRebajo))
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
            ViewData["IdColaborador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdColaborador);
            ViewData["IdTipoRebajo"] = new SelectList(_context.TiposRebajos, "IdTipoRebajo", "IdTipoRebajo", rebajos.IdTipoRebajo);
            ViewData["IdValidador"] = new SelectList(_context.Colaboradores, "Identificacion", "Identificacion", rebajos.IdValidador);
            ViewData["Inconsistencia"] = new SelectList(_context.Inconsistencias, "IdInconsistencia", "IdInconsistencia", rebajos.Inconsistencia);
            return View(rebajos);
        }

        public IActionResult HistoricoRebajos()
        {
            dynamic result=0;
            if (User.IsInRole("COLABORADOR")) 
            {
                result = _context.Rebajos.Where(r => r.IdColaborador == Request.Cookies["Id"])
                    .Include(r => r.IdTipoRebajoNavigation)
                    .ToList();
            }
            else if (User.IsInRole("JEFATURA")) 
            { 
                ViewBag.Nombres = _context.Colaboradores.ToList();
                result = _context.Rebajos.Where(r => r.IdColaboradorNavigation.IdPuestoNavigation.IdDepartamentoNavigation.IdDepartamento == int.Parse(Request.Cookies["IDDepartamento"]))
                                        .Include(r => r.IdTipoRebajoNavigation)

                    .ToList();
            }
            else if (User.IsInRole("SUPERVISOR"))
            {
                ViewBag.Nombres = _context.Colaboradores.ToList();
                result = _context.Rebajos
                    .Include(r => r.IdTipoRebajoNavigation)
                    .ToList();
               
            }
            
            return View(result);
        }
        private bool RebajosExists(int id)
        {
            return _context.Rebajos.Any(e => e.IdRebajo == id);
        }

    }
}
