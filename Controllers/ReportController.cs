using CorePlus.Services;
using CorePlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CorePlus.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController() : this(new ReportService())
        {
        }
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<ActionResult> FinancialReport()
        {
            var practitionersList = await _reportService.GetPractitionersList();
            ViewData["Practitioners"] = new SelectList(practitionersList.ToList(), "id", "name");
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> AppointmentHistory(string id, string dateFrom, string dateTo)
        {
            List<ReportViewModel> reports = null;
            ViewBag.PractitionerName = "";
            int practitionerId = Convert.ToInt32(id);
            DateTime fromDate = Convert.ToDateTime(dateFrom).Date;
            DateTime toDate = Convert.ToDateTime(dateTo).Date;

            var Practitoners = await _reportService.GetPractitionersList();
            ViewBag.PractitionerName = Practitoners.Where(a => a.id == practitionerId).Select(b => b.name).FirstOrDefault();

            reports = await _reportService.AppointmentHistoryByPractitioner(practitionerId, fromDate, toDate);

            return View(reports);

        }

        [HttpGet]
        public async Task<JsonResult> GetFinancialSummary()
        {
            try
            {
                var reportData = await _reportService.GetFinancialSummary();
                JsonResult result = Json(reportData, JsonRequestBehavior.AllowGet);
                return result;
            }
           catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> FilterFinancialReport(FilterViewModel filterModel)
        {
            try
            {
                int practitionerId = Convert.ToInt32(filterModel.practitionerId);
                DateTime fromDate = Convert.ToDateTime(filterModel.dateFrom).Date;
                DateTime toDate = Convert.ToDateTime(filterModel.dateTo).Date;

                if (practitionerId != 0)
                {
                    return Json(await _reportService.FilterFinancialReport(practitionerId, fromDate, toDate), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(await _reportService.GetFinancialSummary(), JsonRequestBehavior.AllowGet);
                }
            }
           catch(Exception ex)
           {
                throw ex;
           }
        }
    }
}