﻿using System.Threading.Tasks;

namespace WebApiCoreMongoDb.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailFaleConoscoAsync(string nome, string email, string subject, string message);
    }
}
