using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace h_admin.Models
{
    public class GetsmsData

    {
        public string MsgID { get; set; }
        public string Body { get; set; }
        public string SendDate { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int FirstLocation { get; set; }
        public int CurrentLocation { get; set; }
        public int Parts { get; set; }
        public int RecCount { get; set; }
        public int RecFailed { get; set; }
        public int RecSuccess { get; set; }
        public bool IsUnicode { get; set; }

    }
}