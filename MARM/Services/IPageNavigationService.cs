namespace MARM.Services
{
    public interface IPageNavigationService
    {
        event EventHandler<int> PageChanged;
        void NavigateTo(int pageIndex);
        void NavigateTo(string url);
        void NavigateBack();
        void NavigateForward();
    }
}
