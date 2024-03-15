using MARM.Enums;

namespace MARM.Services;

public class RandomTestService : IHostedService
{
    private readonly DeviceStateManager deviceStateManager;
    private readonly System.Timers.Timer _timer = new()
    {
        Interval = 1000,
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
        //deviceStateManager.SetMainTargetConnectState(TargetConnectState.Good);
        //deviceStateManager.SetSubTargetConnectState(TargetConnectState.Lost);
        //if (deviceStateManager.IsConnected())
        //{
        //    deviceStateManager.SetMainTargetConnectState(TargetConnectState.Good);
        //}
        //else
        //{
        //    deviceStateManager.SetMainTargetConnectState(TargetConnectState.Lost);
        //}

        int level1 = deviceStateManager.BatteryLevel1 + random.Next(-3, 3);
        int level2 = deviceStateManager.BatteryLevel2 + random.Next(-3, 3);

        if (level1 > 100) level1 = 100;
        if (level1 < 0) level1 = 0;
        if (level2 > 100) level2 = 100;
        if (level2 < 0) level2 = 0;

        deviceStateManager.SetBatteryLevel(level1, level2);
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
