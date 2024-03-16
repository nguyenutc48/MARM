namespace MARM.Services
{
    public interface IComDataService
    {
        event Action<byte[]>? DataReceived;
        void Open(string comPort, int baudrate);
        void Close();
        bool IsConnected();
        Task SendData(string data);
        Task<string> ReadData();
        Task SendByte(byte[] data);
    }
}
