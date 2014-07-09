
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DW.Profiles.AD.Models;

namespace DW.Profiles.AD.Context
{
	public class UserProfileImportFailsContext : DbContext
	{
		public DbSet<UserProfileImportFail> UserProfileImportFails { get; set; }
		public UserProfileImportFailsContext(String connectionString)			
		{
			//calling base constructor with UserProfileImportFails Connection string            
            var _dependency = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            base.Database.Connection.ConnectionString = connectionString;
		}

		#region Public methods

		public void ClearUserProfileImportFails()
		{
			this.UserProfileImportFails.RemoveRange(this.UserProfileImportFails);
			this.SaveChanges();
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
