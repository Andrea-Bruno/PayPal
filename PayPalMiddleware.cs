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
    /// <summary>
    /// Middleware to handle PayPal Instant Payment Notification (IPN) messages.
    /// This middleware is designed to handle PayPal IPN messages by verifying their authenticity
    /// with PayPal and invoking the provided callback action when a valid "Completed" payment is
    /// detected. It is important to configure PayPal IPN notifications to send the notification to your website/IP address.
    /// </summary>
    public class PayPalIpnMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<Dictionary<string, string>> _onPaymentCompletes;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalIpnMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next middleware in the HTTP request pipeline. This delegate is invoked to pass control
        /// to the subsequent middleware after the current middleware has completed its processing.
        /// </param>
        /// <param name="onPaymentCompletes">
        /// A callback action that is invoked when a PayPal Instant Payment Notification (IPN) with a
        /// "Completed" payment status is successfully verified. The callback receives a dictionary
        /// containing the parsed IPN data as key-value pairs, which can be used to process the payment
        /// (e.g., updating order status, sending confirmation emails, etc.).
        /// </param>
        public PayPalIpnMiddleware(RequestDelegate next, Action<Dictionary<string, string>> onPaymentCompletes)
        {
            _onPaymentCompletes = onPaymentCompletes;
            _next = next; // The next middleware in the pipeline
        }

        /// <summary>
        /// Asynchronously processes the incoming HTTP request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the incoming request is targeting the IPN endpoint
            if (context.Request.Method == "POST" && context.Request.Headers["User-Agent"].ToString().Contains("PayPal IPN"))
            {
                // Read the content of the request body (IPN notification)
                using var reader = new StreamReader(context.Request.Body);
                var ipnMessage = await reader.ReadToEndAsync();

                // Prepare the verification message to send back to PayPal
                var verificationMessage = "cmd=_notify-validate&" + ipnMessage;

                // Send the verification request to PayPal
                using var client = new HttpClient();

                string baseUrl;
                if (Debugger.IsAttached)
                    // Base URL for PayPal Sandbox (used for testing)            
                    baseUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
                else
                    // Base URL for PayPal production (used for live payments)
                    baseUrl = "https://www.paypal.com/cgi-bin/webscr";

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
