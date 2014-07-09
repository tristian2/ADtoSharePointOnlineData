using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DW.Profiles.AD.Context;
using DW.Profiles.AD.Models;
using DW.Profiles.MMD.Taxonomy;
using DW.Exception;

namespace DW.Profiles.AD.Fails
{
	public static class FailsManager
	{
		public static void RetryUserProfileImportFails(List<TaxonomyTermSet> ts, UserQuery userQuery, String connectionString)
		{
			using (var db = new UserProfileImportFailsContext(connectionString))
			{
				var userProfileImportWithFails = from fails in db.UserProfileImportFails
												 select fails;

				Console.WriteLine("");
				Console.WriteLine("   Retrying User profile import fails from local DB:");

				FailUserQuery failUserQuery = new FailUserQuery(userQuery);

				if (userProfileImportWithFails.Count() > 0)
				{
					foreach (var userProfileImportFail in userProfileImportWithFails)
					{
                        try
                        {
                            failUserQuery.InitializeQuery(userProfileImportFail.Email);
                            Console.WriteLine("     {0}", userProfileImportFail.Email);
                            failUserQuery.RetryImportUserProfile(ts);
                            db.UserProfileImportFails.Remove(userProfileImportFail);
                            Console.WriteLine("     Retry import user profile successfully!");
                        }
                        catch (UserProfileImportException importException)
                        {
                            userProfileImportFail.ErrorMessage = importException.OrginalException.ToString();
                            userProfileImportFail.NumberOfRetries = userProfileImportFail.NumberOfRetries + 1;
                            userProfileImportFail.PropertyDataSerialized = importException.ToString();
                            userProfileImportFail.Timestamp = DateTime.Now;
                            db.Entry(userProfileImportFail).State = EntityState.Modified;
                            Console.WriteLine("     Retry import user profile failed");                            
                            //throw new Exception("     Retry import user profile failed" + importException.Message, importException);    
                        }
                            /*
                        catch (System.Exception ex)
                        {
                            throw new Exception.Exception("     Retry import user profile failed - general exception" + ex.Message, ex);
                            //Console.WriteLine("error occurred putting term in for " + spPropertyName + " and " + adAttributeValue + " probably as there is an  data mismiatch.. ignoring and moving on!");
                        }*/
					}

					db.SaveChanges();
				}
				else
				{
					Console.WriteLine("     No user profile import fails to retry");
				}				
			}
		}

		public static void AddNewUserProfileImportFail(string accountName, string email, UserProfileImportException importException, String connectionString)
		{
			UserProfileImportFail userProfileImportFail = new UserProfileImportFail();
			userProfileImportFail.FillUserProfileImportFail(accountName, email, importException, userProfileImportFail);

            using (var db = new UserProfileImportFailsContext(connectionString))
			{
				db.UserProfileImportFails.Add(userProfileImportFail);
				db.SaveChanges();
			}
		}

        public static void AddOrUpdateNewUserProfileImportFail(string accountName, string email, UserProfileImportException importException, String connectionString)
		{
            using (var db = new UserProfileImportFailsContext(connectionString))
			{
				var query = from fails in db.UserProfileImportFails
							where fails.Email.ToLower().Equals(email.ToLower())
							select fails;

				if (query != null && query.Count() > 0)
				{
					var userProfileImportFail = query.FirstOrDefault();
					userProfileImportFail.FillUserProfileImportFail(accountName, email, importException, userProfileImportFail);
					db.Entry(userProfileImportFail).State = EntityState.Modified;
				}
				else
				{
					UserProfileImportFail userProfileImportFail = new UserProfileImportFail();
					userProfileImportFail.FillUserProfileImportFail(accountName, email, importException, userProfileImportFail);
					db.UserProfileImportFails.Add(userProfileImportFail);
				}

				db.SaveChanges();
			}
		}

		public static void CleanDatabase(String connectionString)
		{
            using (var db = new UserProfileImportFailsContext(connectionString))
			{
				db.ClearUserProfileImportFails();
			}
		}
	}
}
