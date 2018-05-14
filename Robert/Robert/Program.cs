using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using Sappworks.Stocks;
using Sappworks.Stocks.ETrade;

namespace Robert
{
    class Program
    {
        static OAuthStockbrokerServiceInterface stockBrokerService;

        // this token identifies your program
        static Sappworks.Stocks.OAuthToken SandboxConsumerToken =
            new Sappworks.Stocks.OAuthToken
            {
                Token = "bbfe44dd53be1875b18eaf784bc4cc91",
                Secret = "d9f864c5acaedc2d1deb431f708c6e06"
            };

        static void Main(string[] args)
        {
            stockBrokerService = new ETradeClient(SandboxConsumerToken);

            // authenticate
            OAuthProcess();

            // get account
            var account = stockBrokerService.GetAccount();

            // get some other quotes
            var quotes = stockBrokerService.GetQuotes(new[] { "YHOO", "TSLA", "XOM" });

            foreach (Quote quote in quotes)
            {
                Console.WriteLine(quote.Symbol + Environment.NewLine + quote.Price + Environment.NewLine);
                Console.WriteLine("*******************************");
            }
                
            Console.ReadKey();
 
            ExitPrompt();
        }

        static void OAuthProcess()
        {
            // the goal is to obtain the access token
            // the access token authenticates every request
            // it will expire after 2 hours idle and 12am pacific time 

            // send the user to log in and return to your app with the verification key
            try
            {
                Process.Start(stockBrokerService.GetUserAuthorizationUrl());
            }
            catch (OAuthGetRequestTokenException ex)
            {
                Complain(ex);
            }

            string verificationKey;

            //do
            {
                Console.Write("Enter verification key: ");

                verificationKey = Console.ReadLine();
            }
            //while (verificationKey != null);

            try
            {
                // now that we have the verification key, exchange it for the access token
                stockBrokerService.GetAccessToken(verificationKey);
            }
            catch (OAuthGetAccessTokenException ex)
            {
                Complain(ex);
            }
        }

        static void ExitPrompt(bool error = false)
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Environment.Exit(error ? 1 : 0);
        }

        static void Complain(AuthenticationException ex)
        {
            Console.WriteLine(ex.RequestUri);
            Console.WriteLine();
            Console.WriteLine("Authorization Headers:" + Environment.NewLine);
            Console.WriteLine(string.Join(Environment.NewLine + "\t", ex.AuthorizationHeaders));
            Console.WriteLine();
            Console.WriteLine("Problem: " + ex.Problem);
            Console.WriteLine();
            Console.WriteLine("Problem advice: " + ex.ProblemAdvice);
            Console.WriteLine();
            Console.WriteLine("Parameters absent: " + string.Join(", ", ex.ParametersAbsent));
            Console.WriteLine();
            Console.WriteLine("Parameters rejected: " + string.Join(", ", ex.ParametersRejected));

            ExitPrompt(true);
        }
    }
}
