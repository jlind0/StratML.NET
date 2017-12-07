using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.Two;

namespace StratML.Core.IRS990
{
    public class IRS990DollarPoints
    {
        public string OrgId { get; set; }
        public DollarPoint[] Assets { get; set; }
        public DollarPoint[] Income { get; set; }
        public DollarPoint[] Revenue { get; set; }
    }

    public class DollarPoint
    {
        public DateTime? AsOfDate { get; set; }
        public double? Amount { get; set; }
    }
}
