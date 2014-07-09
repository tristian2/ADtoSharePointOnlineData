using System;
using System.Collections.Generic;
using DW.Profiles.Common;
using DW.Profiles.AD;
using DW.Profiles.MMD.Taxonomy;
using DW.Profiles.AD.Fails;
using DW.Exception;

namespace DW.Profiles.Import
{
    /// <summary>
    /// THIS IS a test program to run the Active directory to SharePoint online import process - this is not for production!!!
    /// the solution will porduce a set of dlls, these should be used by say SQL server integration services to invoke the process
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
			//FailsManager.CleanDatabase();
            // DEV settings - need to comment out lines in extensionAttributen lines in UserQuery.cs            
            /*
            string user = "tristian@dev.onmicrosoft.com";
            string password = "Surf1ng_3";                  
            string adusername = "Azure-spol-agent@.com";// Console.ReadLine();
            string adpassword = "C0mpl1cated0ne";
            string TermSetGroupName = " Digital Workspace";
            string TermStoreGuid = "5899732101e5463fa02f703befb5e401";  
            string url = "https://dev.sharepoint.com/";
            string ldapTESTPath = "LDAP://GLOBALAD1.global..com/OU=O365-Dev,OU=Users,OU=Global,DC=global,DC=,DC=com"; //dont work with my username
            string serviceUrl = "https://dev-admin.sharepoint.com/_vti_bin/userprofileservice.asmx";
            string TermSetsCSV = "Regions";            
            */

            // TEST settings - need to comment out lines in extensionAttributen lines in UserQuery.cs            
            
            string user = "kevin.beckett@test.onmicrosoft.com";
            string password = "Surf1ng_";
            string adusername = "Azure-spol-agent@.com";// Console.ReadLine();
            string adpassword = "C0mpl1cated0ne";// Console.ReadLine();
            string TermSetGroupName = " Digital Workspace";
            string TermStoreGuid = "5cdaaa3f5d7e46b0971a6b1a42e0d22c";  
            string url = "https://test.sharepoint.com/";
            string ldapTESTPath = "LDAP://GLOBALAD1.global..com/OU=O365-Test,OU=Users,OU=Global,DC=global,DC=,DC=com"; //dont work with my username
            string serviceUrl = "https://test-admin.sharepoint.com/_vti_bin/userprofileservice.asmx";
            //string TermSetsCSV = "Formal Appointments,Grades,Regions,Countries,Groups,Office Locations,Floor,Cost Centres";
            //string TermSetsCSV = "Grades,Regions,Office Locations,Floor,Cost Centres";
            string TermSetsCSV = "Regions, Cost Centres";
            

            // UAT settings
            /*
            string user = "Azure-SPSync-Agent@UAT.onmicrosoft.com";
            string password = "C0ntent&C0de";
            string adusername = "Azure-spol-agent@.com";
            string adpassword = "C0mpl1cated0ne";
            string TermSetGroupName = " Digital Workspace";
            string TermStoreGuid = "20184c3d5d924a05bc9921e085c774c3";   
            string url = "https://uat.sharepoint.com/";
            string ldapTESTPath = "LDAP://GLOBALAD1.global..com/OU=O365-UAT,OU=Users,OU=Global,DC=global,DC=,DC=com";  //all 1000ish users                      
            string serviceUrl = "https://uat-admin.sharepoint.com/_vti_bin/userprofileservice.asmx";
            string TermSetsCSV = "Regions";
            */

            String connectionString = "Data Source=server;Initial Catalog=DigitalWorkspace;Integrated Security=True";

            /*
            //prod
            // whole of  - for production string ldapTESTPath = "LDAP://GLOBALAD1.global..com/DC=global,DC=,DC=com";
             * */
            TaxonomyManager taxonomyManager = null;
            UserQuery query = null;
            List<TaxonomyTermSet> termSets = null; 
            try
            {
                taxonomyManager = new TaxonomyManager(url, user, password, TermStoreGuid, TermSetGroupName);
            }
            catch (DW.Exception.Exception aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // cant do the full list at the moment.  as terms do not match AD values for some termsets
            /*
			List<TaxonomyTermSet> termSets = .DW.Profiles.MMD.Common.Helpers.GetTermSetCollection(taxonomyManager,
                                                                                                "Formal Appointments,Grades,Regions,Countries,Groups,Office Locations,Floor,Cost Centres",
																								true,
																								false);
             * */
            /*
            List<TaxonomyTermSet> termSets = .DW.Profiles.MMD.Common.Helpers.GetTermSetCollection(taxonomyManager,
                                                                                                "Grades,Regions,Office Locations,Floor",
                                                                                                true,
                                                                                                false);
             * */

           
            
            try
            {
                termSets = DW.Profiles.MMD.Common.Helpers.GetTermSetCollection(taxonomyManager,
                                                                                        TermSetsCSV,
                                                                                        true,
                                                                                        false);
            }
            catch (DW.Exception.Exception aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //PartialUserQuery query = new PartialUserQuery(user, password, serviceUrl);

            try
            {
                query = new UserQuery(adusername, adpassword, user, password, ldapTESTPath, serviceUrl, connectionString);
                query.ActiveDirectoryConnection();
            }
            catch (DW.Exception.Exception aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                FailsManager.RetryUserProfileImportFails(termSets, query, connectionString);
                Console.WriteLine();
                Console.WriteLine("Processing AD to SPO User Profile Import");
            }
            catch (DW.Exception.Exception aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                query.ProcessUsers(termSets);
            }
            catch (DW.Exception.Exception  aex)
            {
                Console.WriteLine(aex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Write("press any key to go out!");
            Console.ReadLine();
            
        }


    }
}
