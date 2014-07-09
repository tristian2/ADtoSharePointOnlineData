using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.Common
{
    public static class Helpers
    {
        public static string GetAccountClaims(string email)
        {
            return String.Format("i:0#.f|membership|{0}", email);
        }
    }
}
