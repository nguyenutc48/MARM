namespace MARM.Services
{
    public interface INavalTreeManagerService
    {
        NavalUnitModel? SelectedNavalUnit { get; set; }
        NavalMission? SelectedNavalMission { get; set; }
    }
}
