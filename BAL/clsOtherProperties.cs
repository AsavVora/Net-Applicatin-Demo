using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public enum USERTYPES
    {
        SUPER_ADMIN = 101,
        SUBSCRIBER = 102,
        SUBSCRIBER_ADMIN = 103,
        TERMINAL_ADMIN = 104,
        SHIPPING_COMPANY_ADMIN = 105,
        CLEARING_AGENT = 106,
        IMPORTER = 108,
        RELISING_OFFICER = 110,
        Terminal_Officer = 111,
        Finance_Officer = 112,
        Scheduler_Officer = 113,
        Supervisor = 114,
        Gate_Officer = 115,
        Depot_Admin = 116,
        Importer_Admin = 117,
        Yard_Admin = 118
    }

    public class clsOtherProperties
    {
        public string TableName { get; set; }
        public string RefrenceNumber { get; set; }
        public string QuestionTypeName { get; set; }
        public string ExtSurveyName { get; set; }
        public string UserFullName { get; set; }
        public string ExtProjectName { get; set; }
        public string ExtTotal { get; set; } 
    }
}
