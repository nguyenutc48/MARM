namespace MARM.Services
{
    public interface IPagesController
    {
        event Action<int, bool> LightStateChanged;
        string GetPageName();
        int GetPageIndex();
        void NextPage();
        void PreviousPage();
    }
}
