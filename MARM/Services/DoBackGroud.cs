using MARM.Data;
using MARM.Enums;
using Microsoft.EntityFrameworkCore;

namespace MARM.Services;

public class DoBackGroud : IHostedService
{
    private readonly DeviceStateManager _deviceStateManager;
    private readonly DataSendService _dataSendService;
    private readonly ComDataService _comDataService;
    private readonly IDataSettingService _dataSettingService;
    private readonly System.Timers.Timer _timer = new()
    {
        Interval = 5000,
    };


    private readonly IConfiguration _configuration;


    private readonly Random random = new();
    private readonly Array TargetConnectStateValues = Enum.GetValues(typeof(TargetConnectState));

    public DoBackGroud(DeviceStateManager deviceStateManager, DataSendService dataSendService, ComDataService comDataService, IDataSettingService dataSettingService, IConfiguration configuration )
    {
        _deviceStateManager = deviceStateManager;
        _dataSendService = dataSendService;
        _comDataService = comDataService;
        _configuration = configuration;

        _timer.Elapsed += _timer_Elapsed;
        _dataSettingService = dataSettingService;
        _dataSettingService.PortChanged += _dataSettingService_PortChanged;
    }

    private void _dataSettingService_PortChanged(string obj)
    {
        //_comDataService.Open("COM16", 57600);
    }

    private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {

        
    }



    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Start();
        var port = _configuration["DataSetting:Port"];
        var baudratestr = _configuration["DataSetting:Baudrate"];
        Console.WriteLine(port + baudratestr);
        if(!string.IsNullOrEmpty(port) && !string.IsNullOrEmpty( baudratestr))
        {
            int baudrate = int.Parse(baudratestr);
            _comDataService.Open(port, baudrate);
        }
        
        return Task.CompletedTask;

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        return Task.CompletedTask;
    }
}
