using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models.Json
{
    public class AtmOverviewMenu
    {
        public string AccountNumber { get; set; }
        public string AccountValue { get; set; }
        public List<AtmOverviewMenuHistory> History { get; set; }

        public AtmOverviewMenu()
        {
            History = new List<AtmOverviewMenuHistory>();
        }
    }

    public class AtmOverviewMenuHistory
    {
        public string Type { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
    }
}
