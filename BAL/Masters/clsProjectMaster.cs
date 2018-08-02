using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Masters
{
    public class clsProjectMaster : clsOtherProperties
    {

        // THIS IS THE LIST OF THE COLUMN OF THE TABLE DEFINE BELOW

        public int Link { get; set; }
        public string ProjectEmail { get; set; }
        public string ProjectNo { get; set; }
        public string CustomerNo { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string AssignUser { get; set; }
        public DateTime? KickOffDate { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public string CreatedHostName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? DateOfModification { get; set; }
        public string ModifiedHostName { get; set; }
        public bool IsDeactivate { get; set; }
        public string CompanyLink { get; set; }
        public string Flag { get; set; }
        public string InitialRequirement { get; set; }
        public string SalesRepresentive { get; set; }
        public string ContactName1 { get; set; }
        public string ContactPhone1 { get; set; }
        public string PhoneCode1 { get; set; }
        public string ContactName2 { get; set; }
        public string ContactPhone2 { get; set; }
        public string PhoneCode2 { get; set; }
        public string UnitNo { get; set; }
        public string City { get; set; }
        public string AreaLink { get; set; }


        // TABLE NAME AND ABOVE VARIABLE IS THE COLUMN OF THIS TABLE
        public clsProjectMaster()
        {
            this.TableName = "ProjectMaster";
        }


    }
}
