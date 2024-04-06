

using MARM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace MARM.Services
{
    public class DataSettingService : IDataSettingService
    {
        private readonly IConfiguration _configuration;
        public DataSettingService(IConfiguration configuration)
        {
            _configuration = configuration;
            Transmit1 = configuration.GetValue<bool>("DataSetting:Transmit1");
            Transmit2 = configuration.GetValue<bool>("DataSetting:Transmit2");
            Transmit3 = configuration.GetValue<bool>("DataSetting:Transmit3");
            Transmit4 = configuration.GetValue<bool>("DataSetting:Transmit4");
            TimerInterval = configuration.GetValue<int>("DataSetting:TimerInterval");
            Light1Mode = configuration.GetValue<int>("DataSetting:Light1Mode");
            Light2Mode = configuration.GetValue<int>("DataSetting:Light2Mode");
            Light3Mode = configuration.GetValue<int>("DataSetting:Light3Mode");
            Light4Mode = configuration.GetValue<int>("DataSetting:Light4Mode");
            Baudrate = configuration.GetValue<int>("DataSetting:Baudrate");
            Port = configuration.GetValue<string>("DataSetting:Port");
        }

        public string ReadDataSetting(string key)
        {
            return _configuration.GetValue<string>(key);
        }
        public void WriteSetting(string key, string value)
        {
            var json = File.ReadAllText("appsettings.json");
            var jsonObj = JObject.Parse(json);

            // Kiểm tra nếu mục MySettings không tồn tại
            if (jsonObj["DataSetting"] == null)
            {
                // Tạo một mục MySettings mới
                jsonObj["DataSetting"] = new JObject();
            }

            // Gán giá trị cho key
            jsonObj["DataSetting"][key] = value;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("appsettings.json", output);
        }
        private bool _Transmit1 { get; set; } = false;
        public event Action<bool>? Transmit1Changed;
        private bool _Transmit2 { get; set; }
        public event Action<bool>? Transmit2Changed;
        private bool _Transmit3 { get; set; }
        public event Action<bool>? Transmit3Changed;
        private bool _Transmit4 { get; set; }
        public event Action<bool>? Transmit4Changed;

        private int _TimerInterval { get; set; } = 500;
        public event Action<int>? TimerIntervalChanged;
        private int _Light1Mode { get; set; }
        public event Action<int>? Light1ModeChanged;
        private int _Light2Mode { get; set; }
        public event Action<int>? Light2ModeChanged;
        private int _Light3Mode { get; set; }
        public event Action<int>? Light3ModeChanged;
        private int _Light4Mode { get; set; }
        public event Action<int>? Light4ModeChanged;
        private int _Baudrate { get; set; } = 57600;
        public event Action<int>? BaudrateChanged;

        private string _Port { get; set; } = "COM16";
        public event Action<string>? PortChanged;


        public bool Transmit1 { get { return _Transmit1; } set { _Transmit1 = value; Transmit1Changed?.Invoke(_Transmit1); WriteSetting("Transmit1", Transmit1.ToString()); } }
        public bool Transmit2 { get { return _Transmit2; } set { _Transmit2 = value; Transmit2Changed?.Invoke(_Transmit1); WriteSetting("Transmit2", Transmit2.ToString()); } }
        public bool Transmit3 { get { return _Transmit3; } set { _Transmit3 = value; Transmit3Changed?.Invoke(_Transmit1); WriteSetting("Transmit3", Transmit3.ToString()); } }
        public bool Transmit4 { get { return _Transmit4; } set { _Transmit4 = value; Transmit4Changed?.Invoke(_Transmit1); WriteSetting("Transmit4", Transmit4.ToString()); } }

        public int TimerInterval { get { return _TimerInterval; } set { _TimerInterval = value; TimerIntervalChanged?.Invoke(_TimerInterval); WriteSetting("TimerInterval", TimerInterval.ToString());  } }
        public int Light1Mode { get {return _Light1Mode; } set { _Light1Mode = value; Light1ModeChanged?.Invoke(_Light1Mode); WriteSetting("Light1Mode", Light1Mode.ToString());} }
        public int Light2Mode { get {return _Light2Mode; } set { _Light2Mode = value; Light2ModeChanged?.Invoke(_Light2Mode); WriteSetting("Light2Mode", Light2Mode.ToString());} }
        public int Light3Mode { get {return _Light3Mode; } set { _Light3Mode = value; Light3ModeChanged?.Invoke(_Light3Mode); WriteSetting("Light3Mode", Light3Mode.ToString());} }
        public int Light4Mode { get {return _Light4Mode; } set { _Light4Mode = value; Light4ModeChanged?.Invoke(_Light4Mode); WriteSetting("Light4Mode", Light4Mode.ToString());} }
        public int Baudrate { get {return _Baudrate; } set { _Baudrate = value; BaudrateChanged?.Invoke(_Baudrate); WriteSetting("Baudrate", Baudrate.ToString()); } }

        public string Port { get { return _Port; } set { _Port = value; PortChanged?.Invoke(_Port); WriteSetting("Port",Port); } }

    }
}
