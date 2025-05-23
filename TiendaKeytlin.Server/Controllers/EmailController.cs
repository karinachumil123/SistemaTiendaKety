using Microsoft.AspNetCore.Mvc;
using TiendaKeytlin.Server.Models;
using TiendaKeytlin.Server.Services;

namespace TiendaKeytlin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _emailService.SendEmailAsync(emailModel);
                return Ok("Correo enviado exitosamente.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Error en los datos de entrada: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error enviando correo: {ex.Message}");
            }
        }
    }
}