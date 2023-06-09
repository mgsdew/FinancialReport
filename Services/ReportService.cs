using CorePlus.Models;
using CorePlus.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CorePlus.Services
{
    public class ReportService : IReportService
    {
        IEnumerable<Practitioners> practitionersList;
        IEnumerable<Appointments> appointmentsList;

        public ReportService()
        {
            using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/Files/practitioners.json")))
            {
                practitionersList = JsonConvert.DeserializeObject<List<Practitioners>>(sr.ReadToEnd());
            }

            using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/Files/appointments.json")))
            {
                appointmentsList = JsonConvert.DeserializeObject<List<Appointments>>(sr.ReadToEnd());
            }
        }

        public async Task<IEnumerable<Practitioners>> GetPractitionersList()
        {
            try
            {
                return practitionersList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetFinancialSummary()
        {
            var summaryDate = new List<ReportViewModel>();

            var reportSerializers = practitionersList.Join(appointmentsList,
                                                            prac => prac.id,
                                                            app => app.practitioner_id,
                                                            (prac, app) => new ReportViewModel
                                                            {
                                                                practitioners = new Practitioners
                                                                {
                                                                    id = prac.id,
                                                                    name = prac.name
                                                                },
                                                                appointments = new Appointments
                                                                {
                                                                    date = app.date,
                                                                    cost = app.cost,
                                                                    revenue = app.revenue,
                                                                },
                                                                monthName = app.date.ToString("MMMM"),
                                                                monthValue = app.date.Month,
                                                                year = app.date.Year
                                                            }).ToList();

            var years = reportSerializers.Select(y => y.year).Distinct();
            var months = reportSerializers.Select(y => new { month_name = y.monthName, month_id = y.monthValue }).Distinct().OrderBy(x => x.month_id);

            foreach (var item in practitionersList)
            {
                foreach (var year in years)
                {
                    foreach (var month in months)
                    {
                        var reportSerializer = reportSerializers
                                                .Where(n => n.year == year && n.monthName == month.month_name && n.practitioners.id == item.id)
                                                .GroupBy(x => new { x.practitioners.id, x.practitioners.name, x.year, x.monthName })
                                                .Select(g => new ReportViewModel
                                                {
                                                    practitioners = new Practitioners
                                                    {
                                                        id = g.Key.id,
                                                        name = g.Key.name
                                                    },
                                                    year = g.Key.year,
                                                    monthName = g.Key.monthName,
                                                    totalCost = g.Where(c => c.appointments.date.Month == month.month_id).Sum(c => c.appointments.cost),
                                                    totalRevenue = g.Where(c => c.appointments.date.Month == month.month_id).Sum(c => c.appointments.revenue),
                                                }).FirstOrDefault();
                        if (reportSerializer != null)
                        {
                            summaryDate.Add(reportSerializer);
                        }

                    }
                }
            }
            var Data = (from report in summaryDate select new { report.practitioners.id, report.practitioners.name, report.year, report.monthName, report.totalCost, report.totalRevenue }).ToList();

            return Data;
        }

        public async Task<IEnumerable<dynamic>> FilterFinancialReport(int practitionerId, DateTime fromDate, DateTime toDate)
        {
            List<ReportViewModel> reportData = new List<ReportViewModel>();

            var reportSerializers = practitionersList.Join(appointmentsList.Where(a => a.practitioner_id == practitionerId && a.date >= fromDate && a.date <= toDate),
                                                    prac => prac.id,
                                                    app => app.practitioner_id,
                                                    (prac, app) => new ReportViewModel
                                                    {
                                                        practitioners = new Practitioners
                                                        {
                                                            id = prac.id,
                                                            name = prac.name
                                                        },
                                                        appointments = new Appointments
                                                        {
                                                            date = app.date,
                                                            cost = app.cost,
                                                            revenue = app.revenue,
                                                        },
                                                        monthName = app.date.ToString("MMMM"),
                                                        monthValue = app.date.Month,
                                                        year = app.date.Year
                                                    }).ToList();

            var years = reportSerializers.Select(y => y.year).Distinct();
            var months = reportSerializers.Select(y => new { month_name = y.monthName, month_id = y.monthValue }).Distinct().OrderBy(x => x.month_id);

            foreach (var year in years)
            {
                foreach (var month in months)
                {
                    var reportSerializer = reportSerializers
                                            .Where(n => n.year == year && n.monthName == month.month_name)
                                            .GroupBy(x => new { x.practitioners.id, x.practitioners.name, x.year, x.monthName })
                                            .Select(g => new ReportViewModel
                                            {
                                                practitioners = new Practitioners
                                                {
                                                    id = g.Key.id,
                                                    name = g.Key.name
                                                },
                                                year = g.Key.year,
                                                monthName = g.Key.monthName,
                                                totalCost = g.Where(c => c.appointments.date.Month == month.month_id).Sum(c => c.appointments.cost),
                                                totalRevenue = g.Where(c => c.appointments.date.Month == month.month_id).Sum(c => c.appointments.revenue),
                                            }).FirstOrDefault();
                    if (reportSerializer != null)
                    {
                        reportData.Add(reportSerializer);
                    }

                }
            }
            var Data = (from report in reportData select new { report.practitioners.id, report.practitioners.name, report.year, report.monthName, report.totalCost, report.totalRevenue }).ToList();

            return Data;
        }

        public async Task<List<ReportViewModel>> AppointmentHistoryByPractitioner(int practitionerId, DateTime fromDate, DateTime toDate)
        {
            List<ReportViewModel> dataList = practitionersList
                .Join(appointmentsList.Where(a => a.practitioner_id == practitionerId && a.date >= fromDate && a.date <= toDate),
                                                    p => p.id,
                                                    a => a.practitioner_id,
                                                    (p, a) => new ReportViewModel
                                                    {
                                                        practitioners = new Practitioners
                                                        {
                                                            id = p.id,
                                                            name = p.name
                                                        },
                                                        appointments = new Appointments
                                                        {
                                                            date = a.date,
                                                            client_name = a.client_name,
                                                            appointment_type = a.appointment_type,
                                                            duration = a.duration,
                                                            cost = a.cost,
                                                            revenue = a.revenue,
                                                        },

                                                    }).OrderBy(x=>x.appointments.date).ToList();

            return dataList;

        }

    }
}