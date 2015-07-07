using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Net.Http.Headers;
using System.Configuration;
using System.Reflection;
using SendGrid;
using log4net;
using log4net.Config;


namespace SmartSheetWindowsService
{
    /// <summary>
    /// This represents an entity that performs the actual business logic.
    /// </summary>
    internal class SmartSheetLogicLayer : ISmartSheetLogicLayer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Runs the business logic here.
        /// </summary>
        public void Run()
        {
            Log.Info("Running the Async HTTP_GET task...");
            Task t = new Task(HTTP_GET);
            t.Start();
            


        }


        static async void HTTP_GET()
        {

            //this FileDirectory should be externalized and maybe the sheets will be obsoleted since using Linq
            string token = ConfigurationManager.AppSettings["APIAccessToken"];
            string fileNameFormat = ConfigurationManager.AppSettings["fileNameFormat"];
            string fileName;
            string fileDirectory = ConfigurationManager.AppSettings["fileDirectory"];
            //string emailDestination1 = ConfigurationManager.AppSettings["emailDestination1"];
            //string emailDestination2 = ConfigurationManager.AppSettings["emailDestination2"];
            string emailSource = ConfigurationManager.AppSettings["emailSource"];
            string emailUsername = ConfigurationManager.AppSettings["emailUsername"];
            string emailPass = ConfigurationManager.AppSettings["emailPass"];
            string fileType = ConfigurationManager.AppSettings["fileType"];


            //our base url.
            var TARGETURL = " https://api.smartsheet.com/1.1/sheet/";
            
            //SendGrid object
            var mailMessage = new SendGridMessage();
           
            mailMessage.From = new MailAddress(emailSource, "SmartSheet Download Service");
            mailMessage.Subject = "Error in SmartSheet Download Service";



            ////hold all of the sheet information from the App Config.....All Sheet Keys must start with "Sheet_" in the config.
            string[] repositorySheets = ConfigurationManager.AppSettings.AllKeys
                             .Where(key => key.StartsWith("Sheet_"))
                             .Select(key => ConfigurationManager.AppSettings[key])
                             .ToArray();

            string[] repositoryEmailAddresses = ConfigurationManager.AppSettings.AllKeys
                 .Where(key => key.StartsWith("emailRecipient"))
                 .Select(key => ConfigurationManager.AppSettings[key])
                 .ToArray();




            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://127.0.0.1:8888"),
                UseProxy = false,
            };


            // ... Use HttpClient.            
            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(fileType));

            //this is a big call and should have some exception handling or be broken into methods.
            foreach (string sheet in repositorySheets)
            {

                HttpResponseMessage response = await client.GetAsync(TARGETURL + sheet);
                HttpContent content = response.Content;

                // ... Check Status Code                                
               Console.WriteLine("GET: + " + TARGETURL + sheet);
               var currentSheet = sheet;
               Log.Info("Processing sheetId " + currentSheet);
               
               Console.WriteLine("Response Status Code: {0} received for {1}", (int)response.StatusCode, currentSheet);

               if((int)response.StatusCode != 200)
                {
                    foreach (string address in repositoryEmailAddresses)
                    {                        
                        mailMessage.AddTo(address);                       
                    }

                    
                    mailMessage.Text = string.Format("Something failed for Worksheet ID " + currentSheet + " at {0}. Status received: {1}.  The failing call was {2}", DateTime.Now, response.StatusCode, TARGETURL + currentSheet);
                    Log.Info(mailMessage.Text);
                    //Console.WriteLine(mailMessage.Text);
                    //Need to fire off an email .... hopefully Async in nature.
                    SendAsync(mailMessage, emailUsername, emailPass);
                }

                // ... Read the string.
                string result = await content.ReadAsStringAsync();


                if (sheet != string.Empty)
                {

                    Console.WriteLine("Exporting " + sheet);
                    fileName = string.Format(fileNameFormat, sheet, DateTime.Now);
                    Console.WriteLine("Saved to " + fileDirectory + "\\" + fileName + Environment.NewLine);
                    var fileStream = new FileStream(fileDirectory + "\\" + fileName, FileMode.Create, FileAccess.Write, FileShare.None);

                    content.CopyToAsync(fileStream).ContinueWith(
                      (copyTask) =>
                      {
                          fileStream.Close();
                      });
                }
                

            }
        }

        private static async void SendAsync(SendGridMessage message, String username, String password)
        {
            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential(username, password);

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            try
            {
                Log.Info("SendAsync.DeliverAsync Called...");
                await transportWeb.DeliverAsync(message);               
                Log.Info("Mail with message " + message.Text + " was sent");

                Console.WriteLine("SendAsync Complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);               
                Log.Info("Send Mail exception caught...");
                Log.Info("Mail with message " + ex.Message + "was not delivered as expected.");
                Log.Info("Check your SendGrid and Domain credentials");
                
            }
        }


        /// <summary>
        /// Disposes resources not being used any more.
        /// </summary>
        public void Dispose()
        {
            Log.Info("Dispose called...");
        }
    }
}