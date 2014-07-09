using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DW.Profiles.UserProfileServiceReference;

namespace DW.Profiles
{
	public class UserProfileImportException : Exception
	{
		private List<PropertyData> userProfilePropertiesData = new List<PropertyData>();
		private Exception originalException;

		public Exception OrginalException 
		{
			get 
			{
				return this.originalException;
			}
		}

		public UserProfileImportException(List<PropertyData> data, string accountName, Exception original)
		{
			this.userProfilePropertiesData.AddRange(data);
			this.originalException = original;
		}

		public override string ToString()
		{
			var exceptionMessage = new StringBuilder();

			foreach (var property in this.userProfilePropertiesData)
			{
				exceptionMessage.AppendFormat("{0} = ", property.Name);
				foreach (var value in property.Values)
				{
					exceptionMessage.AppendFormat("{0};", value.Value);
				}
				exceptionMessage.Append("    ");
			}			

			return exceptionMessage.ToString();
		}
	}
}
