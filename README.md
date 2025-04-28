# 🚀 Simple and easy library to accept credit card payments via PayPal in your application

🔥 **Accept Payments in Minutes, Not Days!**  

PayPal.Easy is a game-changer for developers in search of a hassle-free solution to integrate credit card payments into their .NET web applications. The library is designed with simplicity in mind, eliminating the need to grapple with complex APIs or boilerplate code. Developers can now focus on their app’s core logic, knowing that PayPal.Easy takes care of the heavy lifting in payment processing.

With just one method call, PayPal.Easy allows you to generate customized PayPal or card payment links. These links can be tailored to include specific amounts, items, shipping details, and more, giving developers complete control and flexibility. Furthermore, the library supports the entire PayPal ecosystem, ensuring compatibility with credit and debit cards, PayPal balance, Venmo, and other payment methods.

The library also introduces auto-triggered events that notify developers of successful or canceled payments, enabling seamless order fulfillment and status updates. Whether you're handling digital products, automating subscription billing, or capturing buyer addresses for shipping, PayPal.Easy simplifies the process to the core. Within just ten lines of code, developers can implement e-commerce payments, reducing the time and effort involved in integration.

This library is not just a tool—it’s a gateway to offering a smooth and secure payment experience to end users. PayPal.Easy empowers developers to deliver frictionless checkout processes while maximizing efficiency and reliability in their applications. For anyone who has struggled with integrating payment APIs in the past, PayPal.Easy is the ultimate solution to say goodbye to those challenges.

---

## 🌟 **Why Developers Love PayPal.Easy**  
✅ **Single-Method Simplicity**  
Generate PayPal/card payment links with **1 method call** – customize amounts, items, shipping, and more via parameters.  

✅ **Zero Boilerplate Code**  
Focus on your app’s logic, not payment APIs. Minimal setup, maximal results.  

✅ **Auto-Triggered Events**  
Instant notifications for `PaymentSuccess` or `PaymentCanceled` events. Perfect for order fulfillment!  

✅ **Seamless Shipping**  
Automatically capture buyer addresses for physical goods.  

✅ **Full PayPal Ecosystem**  
Support credit/debit cards, PayPal Balance, Venmo, and more. 

📈 **Easy Implementation**
- ".NET PayPal Library with 1-Click Integration"

- "Credit Card Processing for C# Developers"

- "Automate E-commerce Payments in 10 Lines of Code"

- "Shipping Address Capture for .NET Apps"

🚀 Use Cases
- Digital Products: Instant access after payment

- E-commerce: Auto-capture shipping addresses

- Subscriptions: Recurring billing made simple

- Donations: Support causes with 3-line integration

---

## 📖 **Introduction**
**PayPal.Easy** is a .NET library designed to simplify PayPal integration. With just a few steps, you can accept payments, handle payment notifications, and automate processes like order management and shipping address capture.

---

## 👉 **Tutorial**

This is a minimalist, distraction-free example of a website with an online purchase of a service, and an event notification of the purchase completion.
We recommend that you start by looking at this example: [Tutorial on GitHub](https://github.com/Andrea-Bruno/Blazor-Auto-GUI-generator-samples/tree/master/PayPalIntegration)
___
## 🔧 **PayPalIpnMiddleware**
The `PayPalIpnMiddleware` allows you to handle PayPal Instant Payment Notifications (IPN). This middleware verifies incoming notifications and triggers an event when a payment is successfully completed.

### How to Configure IPN Notifications
1. Log in to your PayPal account.
2. Navigate to **Account Settings > Instant Payment Notifications (IPN)**.
3. Set the notification URL to point to your endpoint (e.g., `https://yourdomain.com`).
4. Save the changes.

For more details, refer to the [official PayPal documentation](https://developer.paypal.com/docs/api-basics/notifications/ipn/).

### Middleware Usage Example
Here is an example of how to use the `PayPalIpnMiddleware` in your application:

```csharp
    var app = builder.Build();

    // user for PayPal IPN validation:
    // in your program.cs file you can use this code to set an event (example: OnPaymentCompleted) that will be executed at every payment.
    app.UseMiddleware<PayPal.PayPalIpnMiddleware>(Events.OnPaymentCompleted);
```
### Event Handling

You can handle the payment success event by subscribing to the `OnPaymentCompleted` event. This allows you to perform actions like updating order status, sending confirmation emails, etc.

### Example Event Handler
Here is an example of how to handle the payment success event:
```csharp
public class Events
{
	static internal void OnPaymentCompleted(Dictionary<string, string> instantPaymentNotificationData)
        {
            // If the payment is completed, extract transaction details
            var transactionId = instantPaymentNotificationData["txn_id"]; // Transaction ID from PayPal
            var id = instantPaymentNotificationData["custom"]; // Custom field set in the payment link
            var amount = instantPaymentNotificationData["mc_gross"]; // Gross amount of the transaction
            Debug.WriteLine($"Payment completed. Transaction ID: {transactionId}, Custom ID: {id}, Amount: {amount}");            
        }
}
```

---

## 💳 **Generate Payment Links with GeneratePayPalLink**
The `GeneratePayPalLink` method allows you to create a PayPal payment link for single purchases or subscriptions.

### Key Parameters
- **businessEmail**: The email address of the PayPal account receiving the payment.
- **productName**: The name or description of the product.
- **amount**: The payment amount.
- **currency**: The currency code (e.g., "EUR").
- **purchaseId**: A unique ID for tracking the transaction.
- **returnUrl**: The URL to redirect the user after a successful payment.
- **cancelUrl**: The URL to redirect the user if the payment is canceled.

### Example Usage
Here is an example of how to generate a payment link:

```csharp

        var paypalLink = PayPal.Util.GeneratePayPalLink(Settings.PayPalBusinessEmail, description, CostInEuro, "EUR", id, true, returnUrl, cancelUrl);
        Redirect = new Uri(paypalLink);
```

---

## 📈 **SEO-Optimized Benefits**
- ".NET PayPal Library with 1-Click Integration"
- "Credit Card Processing for C# Developers"
- "Automate E-commerce Payments in 10 Lines of Code"
- "Shipping Address Capture for .NET Apps"

---

## 🚀 **Use Cases**
- **Digital Products**: Instant access after payment.
- **E-commerce**: Auto-capture shipping addresses.
- **Subscriptions**: Recurring billing made simple.
- **Donations**: Support causes with 3-line integration.

---

## 🌐 **SEO Keywords**
.NET payment integration, PayPal SDK C#, credit card processing library, generate payment links, minimal code payments, automate e-commerce, capture shipping address, developer-friendly PayPal, event-driven payments.

---

💡 **Why Wait?**
"Spent 3 days on payment APIs? PayPal library does it in 3 minutes. Your users deserve frictionless checkout – give it to them."

🌐 **SEO Keywords**
.NET payment integration, PayPal SDK C#, credit card processing library, generate payment links, minimal code payments, automate e-commerce, capture shipping address, developer-friendly PayPal, event-driven payments

