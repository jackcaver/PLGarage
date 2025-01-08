using System.Linq;
using System;

namespace GameServer.Utils
{
    public class PageCalculator
    {
        public static int GetPageStart(int page, int perPage) => (page - 1) * perPage;
        public static int GetPageEnd(int page, int perPage) => GetPageStart(page, perPage) + perPage;
        public static int GetTotalPages(int total, int perPage) => (int)Math.Ceiling((double)total / perPage);

        //public static int GetTotalPages(int PerPage, int AmountOfContent)
        //{
        //    if (AmountOfContent <= 0 || PerPage <= 0)
        //        return 0;
        //    int result = 0;
        //    for (int i = 0; i < AmountOfContent;)
        //    {
        //        result++;
        //        i += PerPage;
        //    }
        //    if (result <= 0)
        //    {
        //        return 1;
        //    }
        //    return result;
        //}
    }
}
