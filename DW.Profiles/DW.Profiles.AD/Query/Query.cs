using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using DW.Profiles;
using DW.Profiles.Common;
using DW.Profiles.MMD.Taxonomy;

namespace DW.Profiles.AD
{
    public class Query
    {
		
        #region Properties

        public DirectorySearcher Search
        {
            get;
            set;
        }

        public DirectoryEntry Entry
        {
            get;
            set;
        }

        #endregion

		#region Public Methods

		/// <summary>
        /// LDAP Filter for User Object Search
        /// </summary>
        /// <returns>The Filter String</returns>
        public virtual string QueryFilter()
        {
            return String.Empty;
        }

        #endregion
    }
}