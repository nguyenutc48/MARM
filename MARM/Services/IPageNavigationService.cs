﻿namespace MARM.Services
{
    public interface IPageNavigationService
    {
        event EventHandler<int> PageChanged;
        List<string> Pages { get; set; }
        void NavigateTo(int pageIndex);
        void NavigateBack();
        void NavigateForward();
    }
}
