using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorePlus.Models
{
    public class Appointments
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string client_name { get; set; }
        public string appointment_type { get; set; }
        public int duration { get; set; }
        public double revenue { get; set; }
        public double cost { get; set; }
        public int practitioner_id { get; set; }
    }
}