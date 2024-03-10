using MARM.Enums;
using System.Reflection.Emit;

namespace MARM.Services;

public class DeviceStateManager : ITargetConnectStateManager, ILightController, ITransmitterDeviceManager
{
    public event Action<TargetConnectState>? MainTargetConnectStateChanged;
    public TargetConnectState MainTargetConnectState { get; private set; }

    public event Action<TargetConnectState>? SubTargetConnectStateChanged;
    public TargetConnectState SubTargetConnectState { get; private set; }

    public event Action<int, int>? BatteryLevelChanged;
    public int BatteryLevel1 { get; private set; }
    public int BatteryLevel2 { get; private set; }

    public event Action<int, bool>? LightStateChanged;
    public bool Light1 { get; private set; }
    public bool Light2 { get; private set; }
    public bool Light3 { get; private set; }
    public bool Light4 { get; private set; }


    public DeviceStateManager()
    {
        MainTargetConnectState = TargetConnectState.Lost;
        SubTargetConnectState = TargetConnectState.Lost;
        BatteryLevel1 = 50;
        BatteryLevel2 = 50;
    }

    public void SetMainTargetConnectState(TargetConnectState targetConnectState)
    {
        if (MainTargetConnectState == targetConnectState) return;

        MainTargetConnectState = targetConnectState;
        MainTargetConnectStateChanged?.Invoke(targetConnectState);
    }

    public void SetSubTargetConnectState(TargetConnectState targetConnectState)
    {
        if (SubTargetConnectState == targetConnectState) return;

        SubTargetConnectState = targetConnectState;
        SubTargetConnectStateChanged?.Invoke(targetConnectState);
    }

    private async Task TurnLight(int index, bool state)
    {
        await Task.Delay(100);
        switch (index)
        {
            case 0:
                if (Light1 != state)
                {
                    Light1 = state;
                    LightStateChanged?.Invoke(0, state);
                }
                break;
            case 1:
                if (Light2 != state)
                {
                    Light2 = state;
                    LightStateChanged?.Invoke(1, state);
                }
                break;
            case 2:
                if (Light3 != state)
                {
                    Light3 = state;
                    LightStateChanged?.Invoke(2, state);
                }
                break;
            case 3:
                if (Light4 != state)
                {
                    Light4 = state;
                    LightStateChanged?.Invoke(3, state);
                }
                break;
        }
    }

    public Task TurnOnLight(int index) => TurnLight(index, true);

    public Task TurnOffLight(int index) => TurnLight(index, false);

    public void SetBatteryLevel(int level1, int level2)
    {
        if (level1 != BatteryLevel1 || level2 != BatteryLevel2)
        {
            BatteryLevel1 = level1;
            BatteryLevel2 = level2;
            BatteryLevelChanged?.Invoke(BatteryLevel1, BatteryLevel2);
        }
    }
}
