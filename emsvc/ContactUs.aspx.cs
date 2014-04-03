using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace emsvc
{
    public partial class ContactUs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SendMail()
        {
            // Gmail Address from where you send the mail
            // http://eufuckingreka.wordpress.com/2012/11/13/sending-smtp-messages-through-office-365-and-system-net-mail-smtpclient-specify-both-from-address-and-from-name/
            //const string fromAddress = "max@electromorph.co.uk";
            //const string fromName = "Max Davies";
            string fromAddress = ConfigurationManager.AppSettings.Get("mailFromAddress");
            string fromName = ConfigurationManager.AppSettings.Get("mailFromName");
            // any address where the email will be sending
            const string fromPassword = "--AppSpecificPassword--";
            // Passing the values and make a email formate to display
            var message = new MailMessage();
            message.Body = Comments.Text;
            message.From = new MailAddress(fromAddress, fromName);
            message.Subject = YourSubject.Text;
            message.To.Add(new MailAddress(YourEmail.Text));

            // smtp settings
            var smtp = new SmtpClient();
            {
                smtp.Host = "podxxxxx.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
                smtp.Timeout = 20000;
            }
            // Passing values to smtp object
            smtp.Send(message);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //here on button click what will done 
            try
            {
                SendMail();
                DisplayMessage.Text = "Thank you - your message has been sent.";
                DisplayMessage.Visible = true;
                YourSubject.Text = string.Empty;
                YourEmail.Text = string.Empty;
                YourName.Text = string.Empty;
                Comments.Text = string.Empty;
            }
            catch (Exception)
            {
                DisplayMessage.Text = "There was a problem sending your email.";
                DisplayMessage.Visible = true;
            }

        }
    }
}