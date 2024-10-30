using AK.Data;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<WeatherForecastService>();

builder.WebHost.UseElectron(args);
builder.Services.AddElectron();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.StartAsync();

// Open the Electron-Window here
await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
{
    Width = 1600,//1920,
    Height = 900,//1080,
    Fullscreenable = true,
    Fullscreen = true,
    AcceptFirstMouse = true,
    Focusable = true,
    Kiosk = true,
    Resizable = false,
    TitleBarStyle = TitleBarStyle.hidden,
});

app.WaitForShutdown();
