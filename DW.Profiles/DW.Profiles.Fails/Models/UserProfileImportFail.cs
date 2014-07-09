using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arup.DW.Profiles.Fails.Models
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

		public override string ToString()
		{
			return String.Format("{0} - {1} - {2} - {3}", this.AccountName, this.Timestamp, this.ErrorMessage, this.PropertyDataSerialized);
		}
	}
}
