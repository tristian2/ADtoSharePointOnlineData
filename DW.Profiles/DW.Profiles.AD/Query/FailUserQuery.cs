using DW.Profiles.Common;
using DW.Profiles.AD.Context;
using DW.Profiles.AD.Models;
using DW.Profiles.MMD.Taxonomy;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DW.Profiles.AD
{
	public class FailUserQuery : UserQuery
	{
		private String currentUserFailEmail;

        public FailUserQuery()
		{
            this.username = base.username;
            this.password = base.password;
            this.serviceUrl = base.serviceUrl;            
        }

		public FailUserQuery(UserQuery userQuery)
		{
			this.Search = userQuery.Search;
			this.Entry = userQuery.Entry;
            this.password = userQuery.password;
            this.username = userQuery.username;
            this.serviceUrl = userQuery.serviceUrl;

		}

		public void InitializeQuery(string userProfileImportFail)
		{
			this.currentUserFailEmail = userProfileImportFail;
			ActiveDirectoryConnection();
		}

		public override string QueryFilter()
		{
			return String.Format("(mail={0})", this.currentUserFailEmail);
		}

		public void RetryImportUserProfile(List<TaxonomyTermSet> ts)
		{			

			Search.Filter = "(&(objectClass=user)" + QueryFilter() + ")";

			List<UserExtensionMapping.UserPropertyMapping> properties = UserExtensionMapping.Mapping();

			 SearchResult sr = Search.FindOne();

			if (sr != null)
			{
				string accountName = Helpers.GetAccountClaims(this.currentUserFailEmail);

				//process each AD User property
				foreach (UserExtensionMapping.UserPropertyMapping prop in properties)
				{
					UpdateProfileProperty(accountName,
											GetProperty(sr, prop.ActiveDirectoryProperty),
											prop.SharePointOnlineProperty,
											MMD.Common.Helpers.GetTermSet(ts, prop.AssociatedTermSetName),
											prop.IsManagedMetaData);
				}

                PropertyImport.ServiceUrl = this.serviceUrl;
				PropertyImport.CommitAllProperties(accountName); 

			}
			else
			{
				throw new ArgumentNullException(String.Format("User with Email: {0} not found in AD", this.currentUserFailEmail));
			}
		}

	}
}
