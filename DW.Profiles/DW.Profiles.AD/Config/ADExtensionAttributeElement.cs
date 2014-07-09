using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DW.Profiles.AD.Config
{
    
    public class ADExtensionAttributeElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "extensionAttribute3",IsKey=true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        [ConfigurationProperty("sharePointUserProfileProperty", DefaultValue = "Grade", IsRequired = true)]
        public string SharePointUserProfileProperty
        {
            get
            {
                return (string)this["sharePointUserProfileProperty"];
            }
        }

        [ConfigurationProperty("sharePointTermset", DefaultValue = "Grades", IsKey = false, IsRequired = true)]
        public string SharePointTermset
        {
            get
            {
                return (string)this["sharePointTermset"];
            }
        }

        [ConfigurationProperty("useTermSet", DefaultValue = true, IsKey = false, IsRequired = false)]
        public Boolean UseTermSet
        {
            get
            {
                return (Boolean)this["useTermSet"];
            }
        }




    }

}
