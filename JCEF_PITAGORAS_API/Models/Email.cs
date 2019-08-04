using System.ComponentModel.DataAnnotations;

namespace WebApiCoreMongoDb.Models
{
    public class Email
    {
        public string nome { get; set; }
        public string email { get; set; }
        public string assunto { get; set; }

        public string mensagem { get; set; }

        public System.Net.Mail.MailAddress Remetente { get; set; }
        public System.Net.Mail.MailAddress Destinatario { get; set; }
    }
}
