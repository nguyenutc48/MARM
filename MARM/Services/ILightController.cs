namespace MARM.Services;

public interface ILightController
{
    event Action<int, bool> LightStateChanged;
    bool Light1 { get; }
    bool Light2 { get; }
    bool Light3 { get; }
    bool Light4 { get; }

    Task TurnOnLight(int index);
    Task TurnOffLight(int index);
}
