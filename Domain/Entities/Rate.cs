using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Rate
    {
            public int RateId { get; set; }
            public double high { get; set; }
            public double last { get; set; }
            public int timestamp { get; set; }
            public double bid { get; set; }
            public double vwap { get; set; }
            public double volume { get; set; }
            public double low { get; set; }
            public double ask { get; set; }
            public double open { get; set; }
    }
}
