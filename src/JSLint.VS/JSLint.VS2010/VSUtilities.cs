using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EnvDTE;

namespace JSLint.VS2010
{
    public static class VSUtilities
    {
        private static readonly string WebsiteCacheDir = string.Concat(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "\\Microsoft\\WebsiteCache\\");

        private static readonly string WebsitesFileName = string.Concat(
            WebsiteCacheDir, "Websites.xml");

        public static string GetWebsiteCacheFolder(Project proj)
        {
            if (File.Exists(WebsitesFileName))
            {
                XDocument doc = XDocument.Load(WebsitesFileName);
                XAttribute attr = (from e in doc.Root.Elements("Website")
                                   where e.Attribute("RootUrl").Value.Equals(
                                    proj.FullName, StringComparison.OrdinalIgnoreCase)
                                   select e.Attribute("CacheFolder")).FirstOrDefault();
                if (attr != null)
                {
                    return string.Concat(WebsiteCacheDir, attr.Value, "\\");
                }
            }

            return null;
        }
    }
}
