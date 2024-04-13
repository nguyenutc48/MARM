namespace MARM.Enums
{
    public enum RemoteAddress : byte
    {
        Main = 0x00,
        Sub = 0x01,
        Main1 = 0x02,
        Sub1 = 0x03,
    }
    public enum CommandType : byte
    {
        
        RemoteStateCallback = 0x01,
        RemoteShotCallback = 0x02,
        RemoteLeakCallback = 0x03,
        ButtonCallback = 0x04,
        Output = 0x05,
        RemoteOutput = 0x06,

    }
    public enum ButtonMode : byte
    {
        ErrorConfirm = 0x00,
        Shot = 0x01,
        AddList = 0x02,
        StopShot = 0x03,
        BackShot = 0x13,
        PrevShot = 0x04,
        PrevPage = 0x05,
        BackPage = 0x06,
        Home = 0x07,
        Setting = 0x10,
        Data = 0x11,
        ShotResult = 0x12,
    }

    public enum Light : int
    {
        ErrorConfirm = 11,
        Shot = 10,
        AddList = 9,
        StopShot = 8,
        BackShot = 0,
        PrevShot = 7,
        PrevPage = 6,
        BackPage = 5,
        Home = 4,
        Setting = 3,
        Data = 12,
        ShotResult = 1,
    }
    public enum Page : int
    {
        ErrorConfirm = 11,
        Shot = 10,
        AddList = 9,
        StopShot = 8,
        BackShot = 0,
        PrevShot = 7,
        PrevPage = 6,
        BackPage = 5,
        Home = 0,
        Setting = 1,
        Data = 2,
        ShotResult = 3,
    }
}
