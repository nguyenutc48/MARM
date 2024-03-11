namespace MARM.Services
{
    public interface IComDataService
    {
        event EventHandler<string>? DataReceived;
        void Open(string comPort, int baudrate);
        void Close();
        bool IsConnected();
        Task SendData(string data);
        Task<string> ReadData();
    }
}
