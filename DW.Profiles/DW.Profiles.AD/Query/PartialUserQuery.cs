using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DW.Profiles.AD.Common;

namespace DW.Profiles.AD
{
    public class PartialUserQuery : UserQuery
    {

        #region Constructor
        public PartialUserQuery()
        {
            this.username = base.username;
            this.password = base.password;
            this.serviceUrl = base.serviceUrl;
        }
        public PartialUserQuery(String username, String password, String serviceUrl)
        {
        }
        public PartialUserQuery(String username, String password, String LDAPPath, String serviceUrl)
        {            
            base.password = password;
            base.username = username;
            base.LDAPPath = LDAPPath;
            base.serviceUrl = serviceUrl;
        }
        #endregion

        public override string QueryFilter()
        {
            //Only run for users objects amended in the last 4 hours            
            return "(whenChanged>=" + DateTime.UtcNow.Subtract(DateTime.Now.AddHours(-Constants.lastQueryOffset)) + ")";
        }
    }
}
