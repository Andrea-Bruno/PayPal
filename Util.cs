using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PayPal
{
    /// <summary>
    /// PayPal Utility Class
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Generates a PayPal payment link for either a single purchase or a subscription.
        /// </summary>
        /// <param name="businessEmail">The email address of the PayPal account that will receive the payment.</param>
        /// <param name="productName">The name or description of the product being purchased.</param>
        /// <param name="amount">The payment amount, formatted to two decimal places.</param>
        /// <param name="currency">The currency code for the payment (e.g., "EUR" for Euros).</param>
        /// <param name="purchaseId">A unique purchase ID for transaction tracking.</param>
        /// <param name="returnUrl">Optional URL to redirect the user after a successful payment.</param>
        /// <param name="cancelUrl">Optional URL to redirect the user if the payment is canceled.</param>
        /// <param name="NoShipping">Indicates whether shipping information is required. Defaults to false (Shipping address is required by default).</param>
        /// <param name="subscriptionPeriod">Optional. Number of days/weeks/months for a subscription. Defaults to 0.</param>
        /// <param name="subscriptionCycleUnit">Optional. Unit of time for billing (e.g., "D", "W", "M", "Y").</param>
        /// <returns>A string representing the full PayPal payment link.</returns>
        public static string GeneratePayPalLink(
            string businessEmail,
            string productName,
            double amount,
            string currency,
            string purchaseId,
            string returnUrl,
            string cancelUrl,
            bool NoShipping = false,
            int subscriptionPeriod = 0,
            BillingCycleUnit subscriptionCycleUnit = BillingCycleUnit.Days)
        {
            string baseUrl;
            if (businessEmail.ToLower().EndsWith("example.com"))
                // Base URL for PayPal Sandbox (used for testing)            
                baseUrl = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            else
                // Base URL for PayPal production (used for live payments)
                baseUrl = "https://www.paypal.com/cgi-bin/webscr";

            // Dictionary for required PayPal payment parameters
            var parameters = new Dictionary<string, string>
                {
                    { "business", businessEmail },  // PayPal account email receiving the payment
                    { "item_name", productName },  // Description of the product being purchased
                    { "amount", amount.ToString("F2", CultureInfo.InvariantCulture) }, // Payment amount with 2 decimal places
                    { "currency_code", currency }, // Currency of the payment (e.g., EUR)
                    { "custom", purchaseId }       // Unique identifier for the purchase (used for tracking)
                };

            // Decide PayPal operation type based on subscriptionDays
            if (subscriptionPeriod > 0)
            {
                // parameters["cmd"] = "_xclick";
                parameters["cmd"] = "_xclick-subscriptions";
                parameters["p3"] = subscriptionPeriod.ToString();
                parameters["t3"] = ((char)subscriptionCycleUnit).ToString();
                parameters["a3"] = amount.ToString("F2", CultureInfo.InvariantCulture); // Subscription amount
            }
            else
            {
                parameters["cmd"] = "_xclick";
            }

            if (NoShipping)
            {
                parameters["no_shipping"] = "1"; // No shipping address required
            }
            else
            {
                parameters["no_shipping"] = "0"; // Shipping address required
            }

            // Add optional redirect URLs if provided
            if (!string.IsNullOrEmpty(returnUrl))
            {
                parameters.Add("return", returnUrl); // URL for redirect after successful payment
            }

            if (!string.IsNullOrEmpty(cancelUrl))
            {
                parameters.Add("cancel_return", cancelUrl); // URL for redirect if payment is canceled
            }

            // Construct the full query string from the parameters
            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
            return $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// Enumeration for the billing cycle unit.
        /// </summary>
        public enum BillingCycleUnit
        {
            /// <summary>
            /// Billing cycle in days.
            /// </summary>
            Days = 'D',

            /// <summary>
            /// Billing cycle in weeks.
            /// </summary>
            Weeks = 'W',

            /// <summary>
            /// Billing cycle in months.
            /// </summary>
            Months = 'M',

            /// <summary>
            /// Billing cycle in years.
            /// </summary>
            Years = 'Y'
        }
    }

}
