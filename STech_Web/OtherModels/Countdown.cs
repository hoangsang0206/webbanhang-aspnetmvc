using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class Countdown
    {
        public DateTime startDate {  get; set; }
        public DateTime endDate { get; set; }

        public Countdown() { }

        public Countdown(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

    }
}