namespace MARM.Services
{
    public class DataSettingService : IDataSettingService
    {
        private bool _Transmit1;
        private bool _Transmit2;
        private bool _Transmit3;
        private bool _Transmit4;
        private bool _EditComPort;
        private bool _EditLightMode;
        private bool _IsExpanded0;
        private bool _IsExpanded1;
        private bool _IsExpanded2;
        private bool _IsExpanded3;
        private int _Light1Mode;
        private int _Light2Mode;
        private int _Light3Mode;
        private int _Light4Mode;
        private int _Baudrate;
        private string _Port = "";

        public bool Transmit1 { get => _Transmit1; set { _Transmit1 = value; DataSettingChanged?.Invoke("");}}
        public bool Transmit2 { get => _Transmit2; set { _Transmit2 = value; DataSettingChanged?.Invoke("");}}
        public bool Transmit3 { get => _Transmit3; set { _Transmit3 = value; DataSettingChanged?.Invoke("");}}
        public bool Transmit4 { get => _Transmit4; set { _Transmit4 = value; DataSettingChanged?.Invoke("");}}
        public bool EditComPort { get => _EditComPort; set { _EditComPort = value; DataSettingChanged?.Invoke("");}}
        public bool EditLightMode { get => _EditLightMode; set { _EditLightMode = value; DataSettingChanged?.Invoke("");}}
        public bool IsExpanded0 { get => _IsExpanded0; set { _IsExpanded0 = value; DataSettingChanged?.Invoke("");}}
        public bool IsExpanded1 { get => _IsExpanded1; set { _IsExpanded1 = value; DataSettingChanged?.Invoke("");}}
        public bool IsExpanded2 { get => _IsExpanded2; set { _IsExpanded2 = value; DataSettingChanged?.Invoke("");}}
        public bool IsExpanded3 { get => _IsExpanded3; set { _IsExpanded3 = value; DataSettingChanged?.Invoke("");}}
        public int Light1Mode { get => _Light1Mode; set { _Light1Mode = value; DataSettingChanged?.Invoke("");}}
        public int Light2Mode { get => _Light2Mode; set { _Light2Mode = value; DataSettingChanged?.Invoke("");}}
        public int Light3Mode { get => _Light3Mode; set { _Light3Mode = value; DataSettingChanged?.Invoke("");}}
        public int Light4Mode { get => _Light4Mode; set { _Light4Mode = value; DataSettingChanged?.Invoke("");}}
        public int Baudrate { get => _Baudrate; set { _Baudrate = value; DataSettingChanged?.Invoke("");}}
        public string Port { get => _Port; set { _Port = value; DataSettingChanged?.Invoke(""); }}

        public event Action<string>? DataSettingChanged;
    }
}
