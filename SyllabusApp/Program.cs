using System.Globalization;
using SyllabusApp.Interfaces;
using SyllabusApp.Services;
using System.Diagnostics; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Register services with Dependency Injection
// Order matters: SubscriptionService must be registered before NotificationService
builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();

// NotificationService now uses SubscriptionService for subscriber-based notifications
builder.Services.AddSingleton<INotificationService>(provider => 
{
    var subscriptionService = provider.GetRequiredService<ISubscriptionService>();
    return new NotificationService(subscriptionService);
});

builder.Services.AddSingleton<ISyllabusService, SyllabusManager>();

var cultureInfo = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var app = builder.Build();

if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Syllabus}/{action=Index}/{id?}");


var url = "http://localhost:5001";
app.Urls.Add(url);


app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        
        Console.WriteLine("Tarayıcı otomatik açılamadı. Linke tıklayınız: " + url);
    }
});

app.Run();