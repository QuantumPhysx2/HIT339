using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tennis339.Models;

namespace Tennis339.ViewModels
{
    public class ScheduleViewModel
    {
        public Schedule Schedule { get; set; }
        public IEnumerable<Coach> Coach { get; set; }
    }
}
