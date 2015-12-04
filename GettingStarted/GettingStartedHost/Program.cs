using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using GettingStartedLib;
using System.ServiceModel.Description;
    

namespace GettingStartedHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // Step 1 Create a URI to serve as the base address.
            Uri baseAddress = new Uri("http://10.211.55.3:8000/GettingStarted/");
            //Uri baseAddress = new UriBuilder(Uri.UriSchemeHttp, Environment.MachineName, -1, "/GettingStarted/").Uri;

            // Step 2 Create a ServiceHost instance
            ServiceHost selfHost = new ServiceHost(typeof(CalculatorService), baseAddress);

            try
            {
                // Step 3 Add a service endpoint.
                //var sh = selfHost.AddServiceEndpoint(typeof(ICalculator), new WSHttpBinding(), "GettingStartedLib.CalculatorService");
                //sh.Behaviors.Add(new XmlRpcEndpointBehavior());
                
                
                // Step 4 Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                
                selfHost.Description.Behaviors.Add(smb);
                
                // Step 5 Start the service.
                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}