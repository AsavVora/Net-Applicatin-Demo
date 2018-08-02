using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Masters
{
    public class clsCustomerMaster : clsOtherProperties
    {
        // THIS IS THE LIST OF THE COLUMN OF THE TABLE DEFINE BELOW

        public int Link { get; set; }
        public string CustomerNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string OfficeNumber { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneCode { get; set; }
        public bool Sex { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Image { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime? DateOfCreation { get; set; }
        public string CreatedHostName { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedHost { get; set; }
        public bool IsDeactivate { get; set; }
        public string CompanyLink { get; set; }
        public bool IsApplyTax { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        // TABLE NAME AND ABOVE VARIABLE IS THE COLUMN OF THIS TABLE
        public clsCustomerMaster()
        {
            this.TableName = "customermaster";
        } 

    }
}
