using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using DW.Profiles.Extensions;
using DW.Profiles.UserProfileServiceReference;

namespace DW.Profiles
{
    public class PropertyImport
    {

        #region Properties

        public static UserProfileService Service
        {
            get;
            set;
        }

        public static String ServiceUrl
        {
            get;
            set;
        }

		private static List<PropertyData> newDataList = new List<PropertyData>();

        #endregion

        #region Public Methods

        public static void CreateSPOConnection(string username, string password, string serviceUrl) {
	        SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username, password.ToSecureString());
            Uri tenantAdminSiteUri = new Uri(serviceUrl);

            string authCookie = credentials.GetAuthenticationCookie(tenantAdminSiteUri);

			CookieContainer authCookieContainer = new CookieContainer();
			authCookieContainer.SetCookies(tenantAdminSiteUri, authCookie);

            Service = new UserProfileService();
            Service.CookieContainer = authCookieContainer;       
        }

        public static void SetPropertyValue(string propertyName, string propertyValue)
        {
            PropertyData data = new PropertyData();
            data.Name = propertyName;

            List<ValueData> valueDataList = new List<ValueData>();

            foreach (string prop 
                in propertyValue.Split(';'))
                valueDataList.Add(CreateValueData(prop));

            data.Values = valueDataList.ToArray<ValueData>();
            data.IsValueChanged = true;
            
            newDataList.Add(data);
        }

		public static void CommitAllProperties(string accountName)
		{
			PropertyData[] newDataArray = newDataList.ToArray();

			try
			{
                Service.Url=ServiceUrl;
				Service.ModifyUserPropertyByAccountName(accountName, newDataArray);
			}
			catch (Exception ex)
			{
				//dont throw new UserProfileImportException(newDataList, accountName, ex); as it will stop the process for all users
                //just log instead - TODO 
                Console.WriteLine("accountname failed to committ contact AD team!" + accountName);
			}
			finally
			{
				newDataList.Clear();
			}
		}

        public List<string> GetPropertyValue(string accountName, string propertyName)
        {
            PropertyData property = Service.GetUserPropertyByAccountName(accountName, propertyName);
            List<string> propertyValues = new List<string>();

            foreach (var data in property.Values)
                     propertyValues.Add(data.Value.ToString());
            
            return propertyValues;
        }

        #endregion

        #region Private Methods


        private static ValueData CreateValueData(string value)
        {
            ValueData vd = new ValueData();
            vd.Value = value;

            return vd;
        }

        private static void ReadUserProfileProperty(string propertyName, string accountName, UserProfileService service)
        {
            PropertyData property = service.GetUserPropertyByAccountName(accountName, propertyName);

            foreach (var data in property.Values)
            {
                Console.WriteLine(data.Value);
            }
        }


        #endregion

    }
}
