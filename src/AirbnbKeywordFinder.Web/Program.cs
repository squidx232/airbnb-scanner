using System.Diagnostics;
using System.Runtime.InteropServices;
using AirbnbKeywordFinder.Core.Extensions;
using AirbnbKeywordFinder.Web.Components;

// Hide console window on Windows (only in Release)
#if !DEBUG
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    var handle = GetConsoleWindow();
    ShowWindow(handle, 0); // 0 = SW_HIDE
}
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers(); // Add API controllers
builder.Services.AddAirbnbKeywordFinder();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers(); // Map API endpoints
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Auto-launch browser
var url = "http://localhost:5000";
if (args.Length > 0 && args[0].StartsWith("--urls="))
{
    url = args[0].Replace("--urls=", "").Split(';').First();
}

Task.Run(async () =>
{
    await Task.Delay(1500); // Wait for server to start
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else
        {
            Process.Start("xdg-open", url);
        }
    }
    catch { /* Ignore browser launch errors */ }
});

app.Run();
