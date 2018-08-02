using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    [AttributeUsage(AttributeTargets.All)]
    public class IsToUpdate : System.Attribute
    {
        public bool flag;

        public IsToUpdate(bool val)
        {
            this.flag = val;
        }

        public bool GetVal()
        {
            return this.flag;
        }
    }
}
