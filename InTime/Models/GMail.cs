using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InTime.Models
{
    public class GMail
    {
        private const string GmailUsername = "servmaillevizlauzon@gmail.com";
        private const string GmailPassword = "admin123*";
        public static string GmailHost { get; set; }
        public static int GmailPort { get; set; }
        public static bool GmailSSL { get; set; }
        public static string ToEmail { get; set; }
        [Required(ErrorMessage = "Vous devez absolument indiquer un sujet.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Vous ne pouvez pas envoyer un message vide.")]
        public string Body { get; set; }
        public bool IsHtml { get; set; }

        static GMail()
        {
            GmailHost = "smtp.gmail.com";
            GmailPort = 587; //Port 25, 465 et 587 
            GmailSSL = true;
            ToEmail = "marcmarcmarc123@hotmail.com";
        }

        GMail (string sujet, string contenu, bool Html)
        {
            Subject = sujet;
            Body = contenu;
            IsHtml = Html;
        }


        public void Send()
        {
            SmtpClient stmp = new SmtpClient();
            stmp.Host = GmailHost;
            stmp.Port = GmailPort;
            stmp.EnableSsl = GmailSSL;
            stmp.DeliveryMethod = SmtpDeliveryMethod.Network;
            stmp.UseDefaultCredentials = false;
            stmp.Credentials = new NetworkCredential(GmailUsername, GmailPassword);

            using (var message = new MailMessage(GmailUsername, ToEmail))
            {
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsHtml;
                stmp.Send(message);
            }

        }


    }
}
