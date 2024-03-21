namespace MARM.Services
{
    public interface IDataSettingService
    {
        event Action<string> DataSettingChanged;
        bool Transmit1 { get; set; }
        bool Transmit2 { get; set; }
        bool Transmit3 { get; set; }
        bool Transmit4 { get; set; }
        bool EditComPort { get; set; }
        bool EditLightMode { get; set; }
        bool IsExpanded0 { get; set; }
        bool IsExpanded1 { get; set; }
        bool IsExpanded2 { get; set; }
        bool IsExpanded3 { get; set; }
        int Light1Mode { get; set; }
        int Light2Mode { get; set; }
        int Light3Mode { get; set; }
        int Light4Mode { get; set; }
        int Baudrate { get; set; }
        string Port { get; set; }
    }
}
