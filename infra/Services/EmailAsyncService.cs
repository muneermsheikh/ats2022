using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using core.Entities.EmailandSMS;
using core.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace infra.Services
{
    public class EmailAsyncService
    {

        //static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            string token = (string) e.UserState;
            var errorList = new List<string>();
            if (e.Cancelled)
            {
                 errorList.Add("[{0}] Send canceled, " + token);
            }
            if (e.Error != null)
            {
                 errorList.Add((token + "," + e.Error.ToString()));      //(Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            } else
            {
                //Console.WriteLine("Message sent.");
            }
            //mailSent = true;
        }

        public async Task SendEmailAsync(EmailMessage msg)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(msg.SenderUserName, "idealsoln58@gmail.com"));
            mailMessage.To.Add(new MailboxAddress(msg.RecipientUserName, msg.RecipientEmailAddress));
            mailMessage.Subject = msg.Subject;
            mailMessage.Body = new TextPart("html")        //new TextPart("plain")
            {
                Text = msg.Content
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 587, true);    //port 465 for plain text
                smtpClient.Authenticate("idealsoln58@gmail.com", "ideaL*058");
                
                //await smtpClient.SendAsync(mailMessage);  // smtpClient.Send(mailMessage);
                await smtpClient.SendAsync(mailMessage);
                smtpClient.Disconnect(true);
            }

            /*
            // Command-line argument must be the SMTP host.
            SmtpClient client = new SmtpClient(args[0]);
            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("jane@contoso.com",
               "Jane " + (char)0xD8+ " Clayton",
            System.Text.Encoding.UTF8);
            // Set destinations for the email message.
            MailAddress to = new MailAddress("ben@contoso.com");
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = "This is a test email message sent by an application. ";
            // Include some non-ASCII characters in body and subject.
            string someArrows = new string(new char[] {'\u2190', '\u2191', '\u2192', '\u2193'});
            message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding =  System.Text.Encoding.UTF8;
            message.Subject = "test message 1" + someArrows;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            client.SendCompleted += new
            SendCompletedEventHandler(SendCompletedCallback);
            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(message, userState);
            Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
            string answer = Console.ReadLine();
            // If the user canceled the send, and mail hasn't been sent yet,
            // then cancel the pending operation.
            if (answer.StartsWith("c") && mailSent == false)
            {
                client.SendAsyncCancel();
            }
            // Clean up.
            message.Dispose();
            Console.WriteLine("Goodbye.");
            */
        }
    }
}