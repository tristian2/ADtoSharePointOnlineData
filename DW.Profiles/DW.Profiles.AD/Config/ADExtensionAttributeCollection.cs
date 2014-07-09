using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DW.Profiles.AD.Config
{


    [ConfigurationCollection(typeof(ADExtensionAttributeElement), AddItemName = "ADExtensionAttribute", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ADExtensionAttributeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ADExtensionAttributeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ADExtensionAttributeElement)(element)).Name;
        }

        public ADExtensionAttributeElement this[int idx]
        {
            get
            {
                return (ADExtensionAttributeElement)BaseGet(idx);
            }
        }
    }

}
