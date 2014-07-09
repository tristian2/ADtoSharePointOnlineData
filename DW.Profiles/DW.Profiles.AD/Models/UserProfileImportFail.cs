using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.AD.Models
{
	public class UserProfileImportFail
	{
		[Key]
		public string AccountName { get; set; }
		public string Email { get; set; }
		public DateTime Timestamp { get; set; }
		public int State { get; set; }
		public int ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
		public int NumberOfRetries { get; set; }
		public string PropertyDataSerialized { get; set; }

		public void FillUserProfileImportFail(string accountName, string email, UserProfileImportException importException, UserProfileImportFail userProfileImportFail)
		{
			this.AccountName = accountName;
			this.Email = email;
			this.ErrorCode = 1;
			this.ErrorMessage = importException.OrginalException.ToString();
			this.NumberOfRetries = 0;
			this.PropertyDataSerialized = importException.ToString();
			this.State = 1;
			this.Timestamp = DateTime.Now;
		}

		public override string ToString()
		{
			return String.Format("{0} - {1} - {2} - {3}", this.AccountName, this.Timestamp, this.ErrorMessage, this.PropertyDataSerialized);
		}
	}
}
