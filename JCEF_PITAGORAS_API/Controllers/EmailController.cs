using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiCoreMongoDb.Models;
using WebApiCoreMongoDb.Services;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Email")]
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;
        public EmailController(IEmailSender emailSender, IHostingEnvironment env)
        {
            _emailSender = emailSender;
        }

        [HttpGet]
        public string Get()
        {
            return "retorno com sucesso";
        }

        [HttpPost]
        public async Task<IActionResult> EnviarEmail([FromBody]Email mail)
        {
            try
            {                
                await _emailSender.SendEmailAsync(mail.email, mail.assunto, mail.mensagem);

                return Ok(new { mensagem = "E-mail enviado com sucesso" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("FaleConosco")]
        public async Task<IActionResult> EnviarEmailFaleConosco([FromBody]Email mail)
        {
            try
            {
                await _emailSender.SendEmailFaleConoscoAsync(mail.nome, mail.email, mail.assunto, mail.mensagem);

                return Ok(new { mensagem = "E-mail enviado com sucesso" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}