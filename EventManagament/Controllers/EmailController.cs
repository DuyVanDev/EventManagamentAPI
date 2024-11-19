using EventManagament.Service;
using EventManagament.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace EventManagament.Controllers
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

        // POST api/email
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.ToEmail) || string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Missing required fields");
            }

            await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Body);
            return Ok("Email sent successfully");
        }
    }
}
