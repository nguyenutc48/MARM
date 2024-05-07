namespace MARM.Services
{
    public interface INavalTreeManagerService
    {
        event Action<string> ShotButtonPushed;
        NavalUnitModel? SelectedNavalUnit { get; set; }
        NavalMission? SelectedNavalMission { get; set; }
        HashSet<NavalUnitModel>? NavalUnits { get; set; }
        string MissionUrl { get; set; }
        bool ShotStatus { get; set; }
    }
}
