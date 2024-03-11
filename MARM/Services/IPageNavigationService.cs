namespace MARM.Services
{
    public interface IPageNavigationService
    {
        void NavigateTo(string page);
        void NavigateBack();
        void NavigateForward();
    }
}
