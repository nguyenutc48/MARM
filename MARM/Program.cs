using ElectronNET.API;
using ElectronNET.API.Entities;
using MARM.Data;
using MARM.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source=database.db");
});

builder.Services.AddSingleton<DeviceStateManager>();
builder.Services.AddSingleton<ITargetConnectStateManager>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<ILightController>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<ITransmitterDeviceManager>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<ISerialCommunication>(sp => sp.GetRequiredService<DeviceStateManager>());

builder.Services.AddSingleton<IPagesController, PagesController>();

builder.Services.AddSingleton<MissionManager>();



builder.Services.AddHostedService<RandomTestService>();



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
    Width = 1920,
    Height = 1080,
    Fullscreenable = true,
    Fullscreen = true,
    AcceptFirstMouse = true,
    Focusable = true,
    Kiosk = true,
    Resizable = false,
    TitleBarStyle = TitleBarStyle.hidden,
});

app.WaitForShutdown();