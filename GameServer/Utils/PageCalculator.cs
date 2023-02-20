using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameServer.Utils
{
    public class PageCalculator
    {
        public static int GetPageEnd(int Page, int PerPage)
        {
            return Page * PerPage;
        }

        public static int GetPageStart(int Page, int PerPage)
        {
            return GetPageEnd(Page, PerPage) - PerPage;
        }

        public static int GetTotalPages(int PerPage, int AmountOfContent)
        {
            return AmountOfContent/PerPage;
        }
    }
}
