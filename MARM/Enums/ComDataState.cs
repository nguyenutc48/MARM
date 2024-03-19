namespace MARM.Enums
{
    public enum CallbackType : int
    {
        ButtonCallback = 0x04,
        RemoteStateCallback = 0x01
    }
    public enum ButtonMode : int
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
}
