using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using System.IO;

namespace RaceDay.Utilities
{
    public class RecaptchaConfiguration : ConfigurationSection
    {
        private static RecaptchaConfiguration instance;
        private static object lockInstance = new object();

        // Methods
        public static RecaptchaConfiguration Instance
        {
            get
            {
                // Avoid claiming lock if already loaded
                if (instance == null)
                {
                    lock (lockInstance)
                    {
                        // Do this again to make sure _provider is still null
                        if (instance == null)
                        {
                            instance = (ConfigurationManager.GetSection("RecaptchaConfiguration") as RecaptchaConfiguration);

                            if (instance == null)
                                throw new ConfigurationErrorsException("RecaptchaConfiguration section is missing");
                        }
                    }
                }

                return instance;
            }
        }

        [ConfigurationProperty("SiteKey", IsRequired = true)]
        public String SiteKey
        {
            get
            {
                return Convert.ToString(base["SiteKey"]);
            }
        }

        [ConfigurationProperty("SecretKey", IsRequired = true)]
        public String SecretKey
        {
            get
            {
                return Convert.ToString(base["SecretKey"]);
            }
        }

    }
}
