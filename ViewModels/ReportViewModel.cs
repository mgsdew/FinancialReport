using CorePlus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorePlus.ViewModels
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            practitioners = new Practitioners();
            appointments = new Appointments();
        }
        public int monthValue { get; set; }
        public string monthName { get; set; }
        public int year { get; set; }
        public double totalCost { get; set; }
        public double totalRevenue { get; set; }

        public Practitioners practitioners { get; set; }
        public Appointments appointments { get; set; }
    }
}