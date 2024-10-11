using DAPMDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAPMDuLich.Areas.Admin.Data.ViewModel
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public IEnumerable<TaiKhoan> Customers { get; set; }
        public IEnumerable<Contributor> Contributors { get; set; }
    }
}