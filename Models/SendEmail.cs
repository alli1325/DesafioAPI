using System;
using System.Net;
using System.Net.Mail;

namespace DesafioAPI.Models
{
    public static class SendEmail
    {
        public static void Send(string email, string user)
        {
            MailMessage emailMessage = new MailMessage();
            try{
                //Configurando SMTP
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Timeout = 60 * 60; 
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("gft.desafioapi@gmail.com","desafioapi");
                
                //Configurando email enviado
                emailMessage.From = new MailAddress("gft.desafioapi@gmail.com", "Desafio API");
                emailMessage.Body = $"O usuário {user} acabou de acessar a aplicação Desafio_API";
                emailMessage.Subject = "Aviso de acesso";
                emailMessage.IsBodyHtml = true;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.To.Add(email);

                smtpClient.Send(emailMessage);

            }catch{
                return;
            }
        }
    }
}