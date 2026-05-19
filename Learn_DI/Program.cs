using Learn_DI;
using Microsoft.Extensions.DependencyInjection;

// 1 .NET Service Provider Statement
var services = new ServiceCollection();


// 2. Write service 
services.AddTransient<INotificationService, GmailNotification>();
services.AddTransient<INotificationService, SmsNotificationService>();
services.AddTransient<IPaymentService, VISAPayment>();
services.AddTransient<OrderManager>();

// 3. build server
var serviceProvider = services.BuildServiceProvider();

// 4. start, .NET creat alone and injection object.
var orderManager = serviceProvider.GetRequiredService<OrderManager>();

Console.WriteLine("--- Starting Checkout Process ---");
orderManager.Checkout(150.00m);
Console.WriteLine("--- Process Finished ---");



