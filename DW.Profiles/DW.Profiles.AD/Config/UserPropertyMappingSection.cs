using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DW.Profiles.AD.Config
{
    /// <summary>
    /// Stores Server Connection config sections.
    /// </summary>
    public class UserPropertyMappingSection : ConfigurationSection
    {
        [ConfigurationProperty("ADExtensionAttributes")]
        public ADExtensionAttributeCollection ADExtensionAttributes
        {
            get { return ((ADExtensionAttributeCollection)(base["ADExtensionAttributes"])); }
        }

    }

}
