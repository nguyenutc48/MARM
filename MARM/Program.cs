using ElectronNET.API;
using ElectronNET.API.Entities;
using MARM;
using MARM.Data;
using MARM.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.Configure<IDataSettingService>(
    builder.Configuration.GetSection("AppOptions"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite($"Data Source=C:\\Users\\Admin\\MARM\\MARM\\database.db");
});
builder.Services.AddSingleton<ComDataService>();
builder.Services.AddSingleton<DataSendService>();
builder.Services.AddSingleton<DataSettingService>();
builder.Services.AddSingleton<IComDataService>(sp => sp.GetRequiredService<ComDataService>());
builder.Services.AddSingleton<IDataSendService>(sp => sp.GetRequiredService<DataSendService>());
builder.Services.AddSingleton<IDataSettingService>(sp => sp.GetRequiredService<DataSettingService>());
builder.Services.AddSingleton<DeviceStateManager>();
builder.Services.AddSingleton<ITargetConnectStateManager>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<ILightController>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<ITransmitterDeviceManager>(sp => sp.GetRequiredService<DeviceStateManager>());
builder.Services.AddSingleton<IPageNavigationService>(sp => sp.GetRequiredService<DeviceStateManager>());


builder.Services.AddSingleton<MissionManager>();



builder.Services.AddHostedService<DoBackGroud>();



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
