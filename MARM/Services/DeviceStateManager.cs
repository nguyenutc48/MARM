using ElectronNET.API.Entities;
using MARM.Components.DashboardComponents;
using MARM.Data;
using MARM.Enums;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Reflection.Emit;

namespace MARM.Services;

public class DeviceStateManager : ITargetConnectStateManager, ILightController, ITransmitterDeviceManager, IPageNavigationService, INavalTreeManagerService
{
    public event Action<TargetConnectState>? MainTargetConnectStateChanged;
    public TargetConnectState MainTargetConnectState { get; private set; }

    public event Action<TargetConnectState>? SubTargetConnectStateChanged;
    public TargetConnectState SubTargetConnectState { get; private set; }

    public event Action<int, int>? BatteryLevelChanged;
    public int BatteryLevel1 { get; private set; }
    public int BatteryLevel2 { get; private set; }

    public event Action<int, bool>? LightStateChanged;

    public event Action<int>? PageChanged;
    public event Action<string>? PageNameChanged;
    private List<string> _pageNames = new List<string>();

    public bool Light1 { get; private set; }
    public bool Light2 { get; private set; }
    public bool Light3 { get; private set; }
    public bool Light4 { get; private set; }
    public NavalUnitModel? SelectedNavalUnit { get; set; }
    public HashSet<NavalUnitModel>? NavalUnits { get; set; }
    public NavalMission? SelectedNavalMission { get; set; }
    public List<ButtonLightPageModel> ButtonLightPages { get; set; }

    private DataSendService _dataSendService;

    public DeviceStateManager(DataSendService dataSendService)
    {
        _dataSendService = dataSendService;
        _dataSendService.RemoteStateReceived += _dataSendService_RemoteStateReceived;
        _dataSendService.ButtonReceived += _dataSendService_ButtonReceived;
        _dataSendService.LeakStateReceived += _dataSendService_LeakStateReceived;
        MainTargetConnectState = TargetConnectState.Lost;
        SubTargetConnectState = TargetConnectState.Lost;
        BatteryLevel1 = -1;
        BatteryLevel2 = -1;
        ButtonLightPages = new List<ButtonLightPageModel>() { 
            new ButtonLightPageModel {Index = 1,  PageName = "dashboard",   ButtonAddr = (int)ButtonMode.Home,          LightAddr = (int)Light.Home         },
            new ButtonLightPageModel {Index = 2,  PageName = "setting",     ButtonAddr = (int)ButtonMode.Setting,       LightAddr = (int)Light.Setting      },
            new ButtonLightPageModel {Index = 3,  PageName = "navalunits",  ButtonAddr = (int)ButtonMode.Data,          LightAddr = (int)Light.Data         },
            new ButtonLightPageModel {Index = 4,  PageName = "data",        ButtonAddr = (int)ButtonMode.ShotResult,    LightAddr = (int)Light.ShotResult,  },
            new ButtonLightPageModel {Index = 5,  PageName = "",            ButtonAddr = (int)ButtonMode.PrevPage,      LightAddr = (int)Light.PrevPage,    },
            new ButtonLightPageModel {Index = 6,  PageName = "",            ButtonAddr = (int)ButtonMode.BackPage,      LightAddr = (int)Light.BackPage,    },
            new ButtonLightPageModel {Index = 7,  PageName = "",            ButtonAddr = (int)ButtonMode.PrevShot,      LightAddr = (int)Light.PrevShot,    },
            new ButtonLightPageModel {Index = 8,  PageName = "",            ButtonAddr = (int)ButtonMode.BackShot,      LightAddr = (int)Light.BackShot,    },
            new ButtonLightPageModel {Index = 9,  PageName = "",            ButtonAddr = (int)ButtonMode.Shot,          LightAddr = (int)Light.Shot,        },
            new ButtonLightPageModel {Index = 10, PageName = "",            ButtonAddr = (int)ButtonMode.AddList,       LightAddr = (int)Light.AddList,     },
            new ButtonLightPageModel {Index = 11, PageName = "",            ButtonAddr = (int)ButtonMode.StopShot,      LightAddr = (int)Light.StopShot,    },
            new ButtonLightPageModel {Index = 12, PageName = "",            ButtonAddr = (int)ButtonMode.ErrorConfirm,  LightAddr = (int)Light.ErrorConfirm,},
        };
        _pageNames = ButtonLightPages.Where(p => !string.IsNullOrEmpty(p.PageName)).Select(x=>x.PageName).ToList();
        Console.WriteLine(_pageNames.Count.ToString());
    }

    private async void _dataSendService_LeakStateReceived(byte[] obj)
    {
        await _dataSendService.LightControl((int)Light.ErrorConfirm, true);
    }

    private async void _dataSendService_ButtonReceived(byte buttonAddress)
    {
        if (buttonAddress == (int)ButtonMode.ErrorConfirm)
        { 
            await _dataSendService.LightControl((int)Light.ErrorConfirm, false); 
        }
        var pageExist = ButtonLightPages.FirstOrDefault(p=>p.ButtonAddr == buttonAddress);
        if (pageExist != null)
        {
            if(!string.IsNullOrEmpty(pageExist.PageName))
            {
                NavigateTo(pageExist.PageName.Trim());
            }
            else
            {
                if(buttonAddress == (int)ButtonMode.PrevPage)
                {
                    await _dataSendService.LightControl((int)Light.PrevPage, true);
                    await Task.Delay(200);
                    NavigateForward();
                }
                if (buttonAddress == (int)ButtonMode.BackPage)
                {
                    await _dataSendService.LightControl((int)Light.BackPage, true);
                    await Task.Delay(200);
                    NavigateBack();
                }
            }
        }
    }

    private async void ButtonLightControl(int buttonAddress)
    {

        foreach (var button in ButtonLightPages)
        {
            if (button.LightAddr != (int)Light.ErrorConfirm) 
            {
                if (button.ButtonAddr == buttonAddress)
                {
                    await _dataSendService.LightControl(button.LightAddr, true);
                }
                else
                {
                    await _dataSendService.LightControl(button.LightAddr, false);
                }
                await Task.Delay(100);
            }
        }
    }

    private void _dataSendService_RemoteStateReceived(byte[] data)
    {
        if (data[0] == (byte)RemoteAddress.Main)
        {
            SetBattery((int)RemoteAddress.Main, (int)data[1]);
            SetMainTargetConnectState(TargetConnectState.Good);
        }
        if (data[0] == (byte)RemoteAddress.Sub)
        {
            SetBattery((int)RemoteAddress.Sub, (int)data[1]);
            SetSubTargetConnectState(TargetConnectState.Good);
        }
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
                    await _dataSendService.RemoteLightControl(0, state);
                    LightStateChanged?.Invoke(0, state);
                }
                break;
            case 1:
                if (Light2 != state)
                {
                    Light2 = state;
                    await _dataSendService.RemoteLightControl(1, state);
                    LightStateChanged?.Invoke(1, state);
                }
                break;
            case 2:
                if (Light3 != state)
                {
                    Light3 = state;
                    await _dataSendService.RemoteLightControl(2, state);
                    LightStateChanged?.Invoke(2, state);
                }
                break;
            case 3:
                if (Light4 != state)
                {
                    Light4 = state;
                    await _dataSendService.RemoteLightControl(3, state);
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

    public void SetBattery(int batteryNumber, int level)
    {
        if(batteryNumber == 0)
        {
            if(level != BatteryLevel1)
            {
                BatteryLevel1 = level;
                BatteryLevelChanged?.Invoke(BatteryLevel1, BatteryLevel2);
            }
        }
        if (batteryNumber == 1)
        {
            if (level != BatteryLevel2)
            {
                BatteryLevel2 = level;
                BatteryLevelChanged?.Invoke(BatteryLevel1, BatteryLevel2);
            }
        }
    }


    #region NextPage

    string oldPageSelected = "";
    public void NavigateTo(string pageName)
    {
        if (pageName == "")
        {
            ButtonLightControl(99);
        }
        else
        {
            var page = ButtonLightPages.FirstOrDefault(p => p.PageName == pageName);
            if (page != null)
            {
                PageNameChanged?.Invoke(pageName);
                ButtonLightControl(page.ButtonAddr);
                oldPageSelected = page.PageName;
            }
        }
    }


    public async void NavigateBack()
    {
        await Task.Yield();
        int pageIndex = 0;
        
        if(_pageNames.IndexOf(oldPageSelected) == 0)
        {
            pageIndex = _pageNames.Count - 1;
        }
        else
        {
            pageIndex = _pageNames.IndexOf(oldPageSelected) - 1;
        }
        Console.WriteLine(_pageNames[pageIndex]);
        NavigateTo(_pageNames[pageIndex]);
    }

    public async void NavigateForward()
    {
        await Task.Yield();

        var pageIndex = (_pageNames.IndexOf(oldPageSelected) + 1) % _pageNames.Count;
        Console.WriteLine(_pageNames[pageIndex]);
        NavigateTo(_pageNames[pageIndex]);
    }

    #endregion
}

