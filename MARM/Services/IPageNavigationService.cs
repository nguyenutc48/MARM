namespace MARM.Services
{
    public interface IPageNavigationService
    {
        event Action<int> PageChanged;
        event Action<string> PageNameChanged;
        //List<string> PageNames { get; set; }
        //int TotalPage { get; set; }
        //void NavigateTo(int pageIndex);
        void NavigateTo(string pageName);
        void NavigateBack();
        void NavigateForward();
    }
}
