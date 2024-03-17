using MARM.Enums;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Reflection.Emit;

namespace MARM.Services;

public class DeviceStateManager : ITargetConnectStateManager, ILightController, ITransmitterDeviceManager, IComDataService, IPageNavigationService
{
    public event Action<TargetConnectState>? MainTargetConnectStateChanged;
    public TargetConnectState MainTargetConnectState { get; private set; }

    public event Action<TargetConnectState>? SubTargetConnectStateChanged;
    public TargetConnectState SubTargetConnectState { get; private set; }

    public event Action<int, int>? BatteryLevelChanged;
    public int BatteryLevel1 { get; private set; }
    public int BatteryLevel2 { get; private set; }

    public event Action<int, bool>? LightStateChanged;

    // Serial port
    private SerialPort serialPort;
    private bool listening;
    private int _pageIndex;
    public event Action<byte[]>? DataReceived;
    private byte[] buffer = new byte[255];
    private int _indexSend = 0;
    public bool _isDataSendReceived { get; set; } = false;
    private byte _dataSendReceived = 0x00;

    public event Action<int>? PageChanged;
    public int TotalPage { get; set; }
    
    enum ButtonMode : int
    {
        ErrorConfirm = 0x00,
        Shot = 0x01,
        AddList = 0x02,
        StopShot = 0x03,
        BackShot = 0x04,
        
        PrevPage = 0x05,
        BackPage = 0x06,
        Home = 0x07,
        Setting = 0x0C,
        Data = 0x0D,
        ShotResult = 0x0E,
        PrevShot = 0x0F,
    }

    private Dictionary<int, int> _pageNames = new Dictionary<int, int> { { (int)ButtonMode.Home, 0 }, { (int)ButtonMode.Setting, 1 }, { (int)ButtonMode.Data, 2 }, { (int)ButtonMode.ShotResult, 3 } };

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
        _pageIndex = 0;
        SendUpdateStatus();
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
                    await SendOutputTransmiter(0, state);
                    LightStateChanged?.Invoke(0, state);
                }
                break;
            case 1:
                if (Light2 != state)
                {
                    Light2 = state;
                    await SendOutputTransmiter(1, state);
                    LightStateChanged?.Invoke(1, state);
                }
                break;
            case 2:
                if (Light3 != state)
                {
                    Light3 = state;
                    await SendOutputTransmiter(2, state);
                    LightStateChanged?.Invoke(2, state);
                }
                break;
            case 3:
                if (Light4 != state)
                {
                    Light4 = state;
                    await SendOutputTransmiter(3, state);
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
        if(batteryNumber == 1)
        {
            if(level != BatteryLevel1)
            {
                BatteryLevel1 = level;
                BatteryLevelChanged?.Invoke(BatteryLevel1, BatteryLevel2);
            }
        }
        if (batteryNumber == 2)
        {
            if (level != BatteryLevel2)
            {
                BatteryLevel2 = level;
                BatteryLevelChanged?.Invoke(BatteryLevel1, BatteryLevel2);
            }
        }
    }

    #region Comport

    public void Open(string comPort, int baudrate)
    {
        try
        {
            serialPort = new SerialPort(comPort, baudrate);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
            listening = true;
        }
        catch (Exception ex)
        {
            var a = ex.Message;
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var data = GetFrame(serialPort);
        OnDataReceived(data);

        

        // Button check
        if (IsButtonInput(data))
        {
            switch (GetDataButtonInput(data))
            {
                case (byte)ButtonMode.Home:
                    NavigateTo(_pageNames[GetDataButtonInput(data)]);
                    break;
                case (byte)ButtonMode.Setting:
                    NavigateTo(_pageNames[GetDataButtonInput(data)]);
                    break;
                case (byte)ButtonMode.Data:
                    NavigateTo(_pageNames[GetDataButtonInput(data)]);
                    break;
                case (byte)ButtonMode.ShotResult:
                    NavigateTo(_pageNames[GetDataButtonInput(data)]);
                    break;
                case (byte)ButtonMode.BackPage:
                    NavigateBack();
                    break;
                case (byte)ButtonMode.PrevPage:
                    NavigateForward();
                    break;
                default:
                    break;
            }
        }

        // 
        if (IsStatusUpdate(data))
        {
            switch (data[4])
            {
                case 0x00: //mach chinh
                    SetBattery(1, (int)data[5]);
                    if (data[6] == 0xFF) SetMainTargetConnectState(TargetConnectState.Good);
                    else SetMainTargetConnectState(TargetConnectState.Lost);
                    break;
                case 0x01: // mach phu
                    SetBattery(2, (int)data[5]);
                    if (data[6] == 0xFF) SetSubTargetConnectState(TargetConnectState.Good);
                    else SetSubTargetConnectState(TargetConnectState.Lost);
                    break;
                default:
                    break;
            }

        }
        Console.WriteLine(_dataSendReceived.ToString("x2"));

        if (data[3] == _dataSendReceived)
        {
            _isDataSendReceived = false;
            Console.WriteLine("vao day");
            SendUpdateStatus();
        }    
            
    }

    private bool IsStatusUpdate(byte[] data)
    {
        if (data[3] == 0x01)
            return true;
        else return false;
    }

    private bool IsButtonInput(byte[] data)
    {
        if (data[3] == 0x04)
            return true;
        else return false;
    }

    private byte GetDataButtonInput(byte[] data)
    {
        return (byte)data[4];
    }

    protected virtual void OnDataReceived(byte[] data)
    {
        DataReceived?.Invoke(data);
    }

    public void Close()
    {
        try
        {
            serialPort.DataReceived -= SerialPort_DataReceived;
            serialPort.Close();
            listening = false;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task SendData(string data)
    {
        await Task.Delay(100);
        serialPort.WriteLine(data);
    }

    public async Task<string> ReadData()
    {
        await Task.Delay(100);
        return serialPort.ReadLine();
    }

    public bool IsConnected()
    {
        return listening;
    }

    private byte[] GetFrame(SerialPort serialPort)
    {
        int bytesRead = serialPort.Read(buffer, 0, buffer.Length);
        //Console.WriteLine("Received Bytes: " + BitConverter.ToString(buffer));
        if (bytesRead > 0)
        {
            int startFrameIndex = Array.IndexOf(buffer, (byte)0x02);
            int endFrameIndex = Array.IndexOf(buffer, (byte)0x03);
            if (startFrameIndex >= 0 && endFrameIndex >= 0)
            {
                // Khởi tạo mảng mới để lưu trữ dữ liệu được sao chép
                byte[] receivedFrame = new byte[endFrameIndex - startFrameIndex + 1];

                // Sao chép dữ liệu từ mảng gốc sang mảng mới
                Array.Copy(buffer, startFrameIndex, receivedFrame, 0, endFrameIndex - startFrameIndex + 1);

                // In ra mảng đã sao chép để kiểm tra
                Console.WriteLine("Received Bytes: " + BitConverter.ToString(receivedFrame));
                if (CheckCRC(receivedFrame))
                    return receivedFrame;
                else return buffer;
            }
            else
            {
                Console.WriteLine("Không tìm thấy byte 0x02 hoặc 0x03 trong mảng gốc.");
                return buffer;
            }
        }
        else 
        {
            Console.WriteLine("Không đủ data");
            return buffer; 
        }
    }

    private byte[] CheckFrameSend(byte[] buffer)
    {
        int startFrameIndex = Array.IndexOf(buffer, (byte)0x01);
        int endFrameIndex = Array.IndexOf(buffer, (byte)0x03);
        if (startFrameIndex >= 0 && endFrameIndex >= 0)
        {
            byte[] receivedFrame = new byte[endFrameIndex - startFrameIndex + 1];

            Array.Copy(buffer, startFrameIndex, receivedFrame, 0, endFrameIndex - startFrameIndex + 1);

            Console.WriteLine("Check Send Bytes: " + BitConverter.ToString(receivedFrame));
            if (CheckCRC(receivedFrame))
                return receivedFrame;
            else return new byte[0];
        }
        else
        {
            Console.WriteLine("Không tìm thấy byte 0x02 hoặc 0x03 trong mảng gốc.");
            return new byte[0];
        }
    }

    private bool CheckCRC(byte[] buffer)
    {
        byte CRC = 0;
        for (int i = 1; i < buffer.Length - 2; i++)
        {
            CRC ^= buffer[i];
        }
        if (CRC == buffer[buffer.Length - 2]) return true;
        else return false;
    }

    private byte GetCRC(byte[] data)
    {
        byte CRC = 0;
        for (int i = 1; i < data.Length - 2; i++)
        {
            CRC ^= data[i];
        }
        return CRC;
    }

    private async Task SendOutputTransmiter(int lightNumber, bool state)
    {
        _isDataSendReceived = true;
        byte[] frame = new byte[3];
        frame[0] = 0x06;
        frame[1] = 0x00;
        _dataSendReceived = frame[0];

        byte byteValue = 0x00; // Khởi tạo byte với giá trị ban đầu là 0x00
        byteValue |= (byte)(lightNumber << 4); // Dịch số 3 vào 4 bit cao
        if(state == true) byteValue |= (byte)(1 << 2);  // Dịch số 1 vào 4 bit thấp
        else byteValue |= (byte)(0 << 2);
        
        byteValue >>= 1;

        frame[2] = byteValue;
        Console.WriteLine("Frame"+ lightNumber+";"+state+" Light Control Bytes: " + BitConverter.ToString(frame)); 
        await SendFrame(frame);

    }

    private async Task SendOutputReceived(int lightNumber, bool state)
    {
        byte[] frame = new byte[2];
        frame[0] = 0x05;

        byte byteValue = 0x00; // Khởi tạo byte với giá trị ban đầu là 0x00
        byteValue |= (byte)(lightNumber << 4); // Dịch số 3 vào 4 bit cao
        if (state == true) byteValue |= (byte)(1 << 2);  // Dịch số 1 vào 4 bit thấp
        else byteValue |= (byte)(0 << 2);

        byteValue >>= 1;

        frame[1] = byteValue;
        Console.WriteLine("Frame" + lightNumber + ";" + state + " Light Control Bytes: " + BitConverter.ToString(frame));
        await SendFrame(frame);

    }

    public async void SendUpdateStatus()
    {
        byte[] frame = new byte[1];
        frame[0] = 0x01;
        await SendFrame(frame);
    }

    
    public async Task SendByte(byte[] data)
    {
        await Task.Delay(100);
        serialPort.Write(data, 0, data.Length);
    }

    public async Task SendFrame(byte[] data)
    {
        byte[] dataSend = new byte[data.Length + 5];
        if (_indexSend == 255) _indexSend = 0;
        _indexSend++;
        dataSend[0] = 0x01;
        dataSend[1] = (byte)dataSend.Length;
        dataSend[2] = (byte)_indexSend;
        Array.Copy(data, 0, dataSend, 3, data.Length);
        dataSend[dataSend.Length - 1] = 0x03;
        dataSend[dataSend.Length - 2] = GetCRC(dataSend);
        Console.WriteLine("Send Bytes: " + BitConverter.ToString(dataSend));

        await SendByte(dataSend);

    }

    //public async Task SendFrame(byte[] data)
    //{
    //    if (_indexSend == 255) _indexSend = 0;
    //    _indexSend++;

    //    byte[] dataGetStatus = new byte[] { 0x01, 0x00, (byte)_indexSend, 0x01, 0x00, 0x03};
    //    dataGetStatus[1] = (byte)dataGetStatus.Length;
    //    dataGetStatus[dataGetStatus.Length - 2] = (byte)GetCRC(dataGetStatus);
    //    Console.WriteLine("Send Command Get Status Bytes: " + BitConverter.ToString(dataGetStatus));

    //    byte[] dataSend = new byte[data.Length + 5];
    //    if (_indexSend == 255) _indexSend = 0;
    //    _indexSend++;
    //    dataSend[0] = 0x01;
    //    dataSend[1] = (byte)dataSend.Length;
    //    dataSend[2] = (byte)_indexSend;
    //    Array.Copy(data, 0, dataSend, 3, data.Length);
    //    dataSend[dataSend.Length - 1] = 0x03;
    //    dataSend[dataSend.Length - 2] = GetCRC(dataSend);
    //    Console.WriteLine("Send Bytes: " + BitConverter.ToString(dataSend));

    //    byte[] mergedArray = dataGetStatus.Concat(dataSend).ToArray();
    //    Console.WriteLine("Send Bytes: " + BitConverter.ToString(mergedArray));
    //    await SendByte(mergedArray);

    //}

    #endregion
    #region NextPage
    private List<int> pageLightCode = new List<int> { 4,3,2,1};
    public async void NavigateTo(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex > TotalPage) _pageIndex = 0;
        else _pageIndex = pageIndex;
        PageChanged?.Invoke(_pageIndex);
        for (int i = 0; i < pageLightCode.Count; i++)
        {
            if (i == pageIndex)
                await SendOutputReceived(pageLightCode[i], true);
            else
                await SendOutputReceived(pageLightCode[i], false);
        }
    }

    public async void NavigateBack()
    {
        if (_pageIndex == 0)
        {
            _pageIndex = TotalPage - 1; 
        }
        else
        {
            _pageIndex--;
        }
        PageChanged?.Invoke(_pageIndex);
        for (int i = 0; i < pageLightCode.Count; i++)
        {
            if (i == _pageIndex)
                await SendOutputReceived(pageLightCode[i], true);
            else
                await SendOutputReceived(pageLightCode[i], false);
        }

    }

    public async void NavigateForward()
    {
        _pageIndex = (_pageIndex + 1) % TotalPage;
        PageChanged?.Invoke(_pageIndex);
        for (int i = 0; i < pageLightCode.Count; i++)
        {
            if (i == _pageIndex)
                await SendOutputReceived(pageLightCode[i], true);
            else
                await SendOutputReceived(pageLightCode[i], false);
        }

    }
    #endregion
}
