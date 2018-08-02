using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Masters
{
    public class clsCustomerContactMaster : clsOtherProperties
    {
        // THIS IS THE LIST OF THE COLUMN OF THE TABLE DEFINE BELOW
        public int Link { get; set; }
        public string CustomerNo { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPersonName { get; set; } 
        public string ContactEmail { get; set; }
        public DateTime? ContactDateOfBirth { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCellNumber { get; set; }
        public bool ContactSex { get; set; }
        public string ContactAddress { get; set; }
        public string ContactZipCode { get; set; }
        public string ContactCity { get; set; }
        public string ContactCountry { get; set; }
        public string ContactDepartment { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public string CreatedHostName { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedHost { get; set; }
        public bool IsDeactivate { get; set; }
        public string CompanyLink { get; set; }

        // TABLE NAME AND ABOVE VARIABLE IS THE COLUMN OF THIS TABLE
        public clsCustomerContactMaster()
        {
            this.TableName = "customercontact";
        } 

    }
}
