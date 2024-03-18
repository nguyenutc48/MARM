using MARM.Enums;

namespace MARM.Services;

public class RandomTestService : IHostedService
{
    private readonly DeviceStateManager deviceStateManager;
    private readonly System.Timers.Timer _timer = new()
    {
        Interval = 5000,
    };
    

    private readonly Random random = new();
    private readonly Array TargetConnectStateValues = Enum.GetValues(typeof(TargetConnectState));

    public RandomTestService(DeviceStateManager _deviceStateManager)
    {
        deviceStateManager = _deviceStateManager;
        

        _timer.Elapsed += _timer_Elapsed;
    }

    private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {

        //deviceStateManager.SendUpdateStatus();
    }



    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Start();
        deviceStateManager.Open("COM16", 57600);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        return Task.CompletedTask;
    }
}
