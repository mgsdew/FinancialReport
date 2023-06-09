using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorePlus.ViewModels
{
    public class FilterViewModel
    {
        public int practitionerId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}