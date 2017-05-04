using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public abstract class PaginationResult
    {
        public IEnumerable<object> Results { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalResults { get; set; }
        public int PageCount
        {
            get
            {
                int pcount = TotalResults / Limit;
                if ((TotalResults % Limit) > 0)
                {
                    pcount++;
                }
                return pcount;
            }
        }
    }
}