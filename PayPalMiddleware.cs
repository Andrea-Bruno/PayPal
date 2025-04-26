using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PayPal
{
    public class PayPalIpnMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<Dictionary<string, string>> _onPaymentCompletes;

        // Constructor to initialize the middleware
        public PayPalIpnMiddleware(RequestDelegate next, Action<Dictionary<string, string>> onPaymentCompletes)
        {
            _onPaymentCompletes = onPaymentCompletes;
            _next = next; // The next middleware in the pipeline
        }

        // Main method to handle incoming requests
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the incoming request is targeting the IPN endpoint
            if (context.Request.Path.StartsWithSegments("/ipn")) // Ensure "/ipn" is the configured endpoint in PayPal
            {
                // Read the content of the request body (IPN notification)
                using var reader = new StreamReader(context.Request.Body);
                var ipnMessage = await reader.ReadToEndAsync();

                // Prepare the verification message to send back to PayPal
                var verificationMessage = "cmd=_notify-validate&" + ipnMessage;

                // Send the verification request to PayPal
                using var client = new HttpClient();

#if DEBUG
                // Base URL for test (SandBox)
                string baseUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
#else
                // Base URL for PayPal payments
                string baseUrl = "https://www.paypal.com/cgi-bin/webscr";
#endif
                var response = await client.PostAsync(baseUrl, new StringContent(verificationMessage, Encoding.UTF8, "application/x-www-form-urlencoded"));

                // Read PayPal's verification response
                var verificationResponse = await response.Content.ReadAsStringAsync();

                // Check if PayPal confirms the IPN notification as valid
                if (verificationResponse == "VERIFIED")
                {
                    // Parse the IPN message content into key-value pairs for processing
                    var formData = ParseFormData(ipnMessage); // Helper method to parse the notification data
                    if (formData.ContainsKey("payment_status") && formData["payment_status"] == "Completed")
                    {
                        _onPaymentCompletes.Invoke(formData);
                    }
                }
                else if (verificationResponse == "INVALID")
                {
                    // Handle cases where the IPN notification is deemed invalid
                    Debug.WriteLine("Invalid IPN notification received.");
                }

                // Respond to PayPal with HTTP 200 to acknowledge receipt of the notification
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("OK");
            }
            else
            {
                // Pass the request to the next middleware if it's not targeting the IPN endpoint
                await _next(context);
            }
        }

        // Helper method to parse form data into a dictionary
        private Dictionary<string, string> ParseFormData(string formData)
        {
            var result = new Dictionary<string, string>();
            var pairs = formData.Split('&'); // Split the form data into key-value pairs

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    result[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]); // Decode and store key-value pairs
                }
            }

            return result; // Return the parsed data
        }
    }

}
