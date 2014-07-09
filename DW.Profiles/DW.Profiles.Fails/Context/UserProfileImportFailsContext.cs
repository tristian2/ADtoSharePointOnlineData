using Arup.DW.Profiles.Fails.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arup.DW.Profiles.Fails.Context
{
	public class UserProfileImportFailsContext : DbContext
	{
		public DbSet<UserProfileImportFail> UserProfileImportFails { get; set; }
		public UserProfileImportFailsContext()
			: base("ArupUserProfileImportFails")
		{
			//calling base constructor with ArupUserProfileImportFails Connection string
		}

		#region Public methods

		public void ClearUserProfileImportFails()
		{
			this.UserProfileImportFails.RemoveRange(this.UserProfileImportFails);
		}
		public void SeedDemoData()
		{
			Random rnd = new Random();
			int demoItemsCount = rnd.Next(20);
			for (int i = 0; i < demoItemsCount; i++)
			{
				InsertRandomFail();
			}
		}

		#endregion

		#region Private methods
		private void InsertRandomFail()
		{
			Random rnd = new Random();

			UserProfileImportFail fail = new UserProfileImportFail()
			{
				AccountName = String.Format("{0}@server.com", Guid.NewGuid().ToString("N")),
				Timestamp = DateTime.Now.AddDays(-1 * rnd.Next(2)),
				ErrorCode = rnd.Next(1),
				ErrorMessage = "TermSet not available",
				NumberOfRetries = rnd.Next(5),
				PropertyDataSerialized = "JSON object serialized",
				State = rnd.Next(1)
			};

			this.UserProfileImportFails.Add(fail);
			this.SaveChanges();
		}

		#endregion
	}
}
