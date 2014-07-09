using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services;
using DW.Profiles;
using DW.Profiles.Common;
using DW.Profiles.MMD.Taxonomy;
using DW.Profiles.AD.Fails;
using DW.Exception;
using DW.Profiles.AD.Config;

namespace DW.Profiles.AD
{
    public class UserQuery : Query
    {
        public String username { get; set; }
        public String password { get; set; }
        protected String adusername { get; set; }
        protected String adpassword { get; set; }
        protected String LDAPPath { get; set; }
        public String serviceUrl { get; set; }
        public String connectionString { get; set; }

        #region Constructor
        public UserQuery()
        {
        }
        public UserQuery(String adusername, String adpassword, String username, String password, String LDAPPath, String serviceUrl, String connectionString)
        {
            this.password = password;
            this.username = username;
            this.adusername = adusername;
            this.adpassword = adpassword;
            this.LDAPPath = LDAPPath;
            this.serviceUrl = serviceUrl;
            this.connectionString = connectionString;
        }
        #endregion
        #region Public Methods

        /// <summary>
        /// Initiates the LDAP Active Directory Connection (Ready for the Query)
        /// </summary>
        public void ActiveDirectoryConnection()
        {
            if (Entry == null && Search == null)
            {
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {                    
                    Entry = new DirectoryEntry(this.LDAPPath);
                    Entry.Username = adusername;
                    Entry.Password = adpassword;

                    Search = new DirectorySearcher(Entry);
                }
            }
        }

        /// <summary>
        /// Iterates through each user object based on the filter specified
        /// </summary>
        /// <param name="ts"></param>
        public void ProcessUsers(List<TaxonomyTermSet> ts)
        {
            ActiveDirectoryConnection();

            Search.Filter = "(&(objectClass=user)" + QueryFilter() + ")";

            List<UserExtensionMapping.UserPropertyMapping> properties = UserExtensionMapping.Mapping();
            Console.WriteLine("   ");
            Console.WriteLine("   Query AD for Users:");

            //for each user object
            foreach (SearchResult sr in Search.FindAll())
            {
                string email = GetProperty(sr, "mail");

				if (!String.IsNullOrEmpty(email))
				{
					Console.WriteLine("     " + email);

					string accountName = Helpers.GetAccountClaims(email);

					//process each AD User property
					foreach (UserExtensionMapping.UserPropertyMapping prop in properties)
					{
						UpdateProfileProperty(accountName,
												GetProperty(sr, prop.ActiveDirectoryProperty),
												prop.SharePointOnlineProperty,
												MMD.Common.Helpers.GetTermSet(ts, prop.AssociatedTermSetName),
												prop.IsManagedMetaData);
					}

					Console.Write("     Import result.... ");
                    try
                    {
                        PropertyImport.ServiceUrl = this.serviceUrl;
                        PropertyImport.CommitAllProperties(accountName);
                        Console.WriteLine("OK");
                    }
                    catch (UserProfileImportException importException)
                    {
                        FailsManager.AddOrUpdateNewUserProfileImportFail(accountName, email, importException, connectionString);
                        //Console.WriteLine("Fail");
                        throw new Exception.Exception("Fail" + importException.Message, importException);   
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception.Exception("Fail - for uncaught reason" + ex.Message, ex);   
                        //Console.WriteLine("Fail - for uncaught reason");
                    }
				}		                  
            }

            Console.WriteLine("** Finished Processing User Objects **");
            Console.ReadLine();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Using the .DW.Profiles.Import library this method updates a user profile with the active directory property value
        /// </summary>
        /// <param name="accountName">The SPO Account Name (Claims)</param>
        /// <param name="adAttributeValue">The Active Directory Attribute Value</param>
        /// <param name="spPropertyName">The SPO Property Name</param>
        /// <param name="termSet">The TermSet name associated with the User Profile Taxonomy Property</param>
        /// <param name="isManagedMetaData">If the SPO Property is a Taxonomy Field</param>
        public void UpdateProfileProperty(string accountName, string adAttributeValue, string spPropertyName, TaxonomyTermSet termSet, bool isManagedMetaData)
        {
            if (!string.IsNullOrEmpty(adAttributeValue))
            {
                if (PropertyImport.Service == null)
                    PropertyImport.CreateSPOConnection(this.username, this.password, this.serviceUrl);

                if (!isManagedMetaData)
                    PropertyImport.SetPropertyValue(spPropertyName, adAttributeValue);
                else
                {
                    try
                    {
                        //check to make sure the MMD property value is valid
                        if (termSet.Terms.Select(t => t.Name.ToLower() == adAttributeValue.ToLower()) != null)
                            PropertyImport.SetPropertyValue(spPropertyName, adAttributeValue);
                    }
                    catch (System.Exception ex)
                    {
                        //throw new Exception.Exception("     Retry import user profile failed" + ex.Message, ex);    
                        //Console.WriteLine("error occurred putting term in for " + spPropertyName + " and " + adAttributeValue + " probably as there is an  data mismiatch.. ignoring and moving on!");
                    }
                }

                Console.WriteLine("         " + spPropertyName + ": " + adAttributeValue);
            }
            else
            {
                Console.WriteLine("         " + spPropertyName + " is empty, moving on...");
            }           
        } 

        /// <summary>
        /// Gets a specific Active Directory User Object Property Value
        /// </summary>
        /// <param name="searchResult">The User Query Result</param>
        /// <param name="PropertyName">Active Directory User Object Property </param>
        /// <returns></returns>
        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))            
                return searchResult.Properties[PropertyName][0].ToString();            
            else             
                return string.Empty;            
        }

        #endregion        
    }


    public class UserExtensionMapping
    {
        /// <summary>
        /// IEnumerable List object of User Propery Mappings
        /// </summary>
        private static List<UserPropertyMapping> Properties
        {
            get;
            set;
        }
        
        /// <summary>
        /// Map the properties we need to process for each Active Directory User Object
        /// </summary>
        /// <returns>List</UserPropertyMapping> of Mappings for processing per user object</returns>
        public static List<UserPropertyMapping> Mapping()
        {            
            Config.
            UserPropertyMappingSection section = (UserPropertyMappingSection) ConfigurationManager.GetSection("UserPropertyMappingSection") as UserPropertyMappingSection;
            var appSettings = (ADExtensionAttributeCollection)section.ADExtensionAttributes;

            Properties = new List<UserPropertyMapping>();



            foreach (ADExtensionAttributeElement ADExtensionAttributeElement in appSettings)
            {
                Console.WriteLine("Name:" + ADExtensionAttributeElement.Name);
                Console.WriteLine("SharePointTermset:" + ADExtensionAttributeElement.SharePointTermset);
                Console.WriteLine("SharePointUserProfileProperty:" + ADExtensionAttributeElement.SharePointUserProfileProperty);
                Console.WriteLine("UseTermSet:" + ADExtensionAttributeElement.UseTermSet);

                Properties.Add(new UserPropertyMapping(ADExtensionAttributeElement.Name, ADExtensionAttributeElement.SharePointUserProfileProperty, ADExtensionAttributeElement.SharePointTermset, ADExtensionAttributeElement.UseTermSet));

            }

            return Properties;
        }

        /// <summary>
        /// The class object used to store the AD to SPO User Profile Property Mapping
        /// </summary>
        public class UserPropertyMapping
        {
            public string ActiveDirectoryProperty { get; set; }
            public string SharePointOnlineProperty { get; set; }
            public string AssociatedTermSetName { get; set; }
            public bool IsManagedMetaData { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="activeDirectoryProperty">the active directory extension attribute e.g. extensionAttribute3 or msExchExtensionCustomAttribute1</param>
            /// <param name="sharePointOnlineProperty">The user profile property to e.g. -OfficeLocation</param>
            /// <param name="associatedTermSetName">SharePoint termset e.g. Countries</param>
            /// <param name="isManagedMetaData">is it a managhed metadata column  or a string?</param>
            public UserPropertyMapping(string activeDirectoryProperty, string sharePointOnlineProperty, string associatedTermSetName, bool isManagedMetaData)
            {
                ActiveDirectoryProperty = activeDirectoryProperty;
                SharePointOnlineProperty = sharePointOnlineProperty;
                AssociatedTermSetName = associatedTermSetName;
                IsManagedMetaData = isManagedMetaData;
            }
        }    
    }
}