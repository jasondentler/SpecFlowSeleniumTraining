using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Specs.Infrastructure
{
    public class Website
    {
        private readonly string _websiteLocation;

        public Website()
            : this(Settings.WebsitePath)
        {
        }

        public Website(string websiteLocation)
        {
            _websiteLocation = websiteLocation;
        }
        
        public string Location { get { return _websiteLocation; }}

        public void RecycleAppPool()
        {
            // To reset the app, let's poke at the web.config
            var content = File.ReadAllText(WebConfigPath);
            File.WriteAllText(WebConfigPath, content);
        }

        public void SetConnectionString(string name, string connectionString)
        {
            SetConnectionString(name, connectionString, null);
        }

        public void SetConnectionString(string name, string connectionString, string providerName)
        {
            var doc = XDocument.Load(WebConfigPath);
            
            var container = doc.Root
                .Element("connectionStrings");

            if (container == null)
            {
                container = new XElement("connectionStrings");
                doc.Root.Add(container);
            }

            var element = container
                .Elements("add")
                .LastOrDefault(e =>
                                   {
                                       var xAttribute = e.Attribute("name");
                                       return xAttribute != null && xAttribute.Value == name;
                                   });

            if (element == null)
            {
                element = new XElement("add");
                container.Add(element);
            }
            
            element.RemoveAttributes();
            element.SetAttributeValue("name", name);
            element.SetAttributeValue("connectionString", connectionString);

            if (providerName != null)
                element.SetAttributeValue("providerName", providerName);

            doc.Save(WebConfigPath);

        }

        private string WebConfigPath { get { return Path.Combine(_websiteLocation, "web.config"); } }

    }
}
