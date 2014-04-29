namespace NAppTracking.Server
{
    using PagedList.Mvc;

    public class PagedListHelper
    {
        public static PagedListRenderOptions DefaultOptions
        {
            get
            {
                return new PagedListRenderOptions
                {
                    UlElementClasses = new[] { "pagination pagination-sm" },
                    Display = PagedListDisplayMode.Always,
                    DisplayEllipsesWhenNotShowingAllPageNumbers = true,
                    DisplayLinkToPreviousPage = PagedListDisplayMode.Always,
                    DisplayLinkToNextPage = PagedListDisplayMode.Always
                };
            }
        }
    }
}