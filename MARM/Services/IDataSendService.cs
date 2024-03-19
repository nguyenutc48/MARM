namespace MARM.Services
{
    public interface IDataSendService
    {
        event Action<byte>? ButtonReceived;
        event Action<byte[]>? RemoteStateReceived;
        event Action<byte[]>? ShotStateReceived;

        Task RemoteLightControl(int lightNumber, bool state);
        Task RemoteUpdateStatus();
        Task LightControl(int lightNumber, bool state);
    }
}
