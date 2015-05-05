using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;

namespace ServerCache
{
    public class CacheProviderSection : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            CacheProviderSettings settings = null;
            if (section == null)
                return settings;

            settings = new CacheProviderSettings();

            settings.ProjectBaseBusinessObject = Type.GetType(section.Attributes["ProjectBaseBusinessObject"].InnerText);

            string sEnumStr = section.Attributes["ProviderName"].InnerText;
            Array enumValues = Enum.GetValues(typeof(EnumCacheProviders));
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (Enum.GetName(typeof(EnumCacheProviders), enumValues.GetValue(i)) == sEnumStr)
                {
                    settings.ProviderName = (EnumCacheProviders)enumValues.GetValue(i);
                    break;
                }
            }

            settings.CacheItems = new List<CacheItem>();
            foreach (XmlElement xEl in section.SelectNodes(@"./CacheItems/CacheItem"))
            {

                CacheItem ci = new CacheItem();
                ci.CacheItemName = xEl.GetAttribute("CacheItemName");
                ci.MinutesToExpire = Convert.ToInt32(xEl.GetAttribute("MinutesToExpire"));
                ci.EFObjectName = Type.GetType(xEl.GetAttribute("EFObjectName"));
                ci.ConnectionName = xEl.GetAttribute("ConnectionName");
                settings.CacheItems.Add(ci);

            }

            return settings;
        }

        #endregion
    }

    public class CacheProviderSettings
    {
   
        public Type ProjectBaseBusinessObject { get; set; }

        public EnumCacheProviders ProviderName { get; set; }

        public List<CacheItem> CacheItems { get; set; }

        
    }


    public class CacheItem
    {
        private string mCacheItemName;
        private int mMinutesToExpire;
        private Type mEfObjectName;
        private string mConnectionName;




        [XmlAttribute()]
        public string ConnectionName
        {
            get { return mConnectionName; }
            set { mConnectionName = value; }
        }

        [XmlAttribute()]
        public Type EFObjectName
        {
            get { return mEfObjectName; }
            set { mEfObjectName = value; }
        }


        [XmlAttribute()]
        public int MinutesToExpire
        {
            get { return mMinutesToExpire; }
            set { mMinutesToExpire = value; }
        }

        [XmlAttribute()]
        public string CacheItemName
        {
            get { return mCacheItemName; }
            set { mCacheItemName = value; }
        }

    }

}
