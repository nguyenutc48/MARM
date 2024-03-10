
namespace MARM.Services
{
    public class PagesController : IPagesController
    {
        public event Action<int, bool> LightStateChanged;

        public int GetPageIndex()
        {
            throw new NotImplementedException();
        }

        public string GetPageName()
        {
            throw new NotImplementedException();
        }

        public void NextPage()
        {
            throw new NotImplementedException();
        }

        public void PreviousPage()
        {
            throw new NotImplementedException();
        }
    }
}
