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
                    UlElementClasses = new[] {"pagination pagination-sm"}
                };
            }
        }
    }
}