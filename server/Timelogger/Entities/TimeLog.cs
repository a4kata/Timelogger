using System;
using System.Collections.Generic;
using System.Text;

namespace Timelogger.Entities
{
    public class TimeLog
    {
        public int Id { get; set; }
        public int Time { get; set; }
        public string Note { get; set; }
    }
}
