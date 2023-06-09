using CorePlus.Models;
using CorePlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePlus.Services
{
    public interface IReportService
    {
        Task<IEnumerable<Practitioners>> GetPractitionersList();
        Task<IEnumerable<dynamic>> GetFinancialSummary();
        Task<List<ReportViewModel>> AppointmentHistoryByPractitioner(int practitionerId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<dynamic>> FilterFinancialReport(int practitionerId, DateTime fromDate, DateTime toDate);
    }
}
