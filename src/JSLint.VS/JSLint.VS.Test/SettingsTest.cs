using System.IO;
using JSLint.Framework.OptionClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSLint.VS.Test
{
    [TestClass]
    public class SettingsTest
    {
        public Options LoadOptionsFromEmbeddedResource(string name)
        {
            using (Stream s = typeof(SettingsTest).Assembly.GetManifestResourceStream("JSLint.VS.Test." + name))
            {
                OptionsSerializer serializer = new OptionsSerializer();
                return serializer.Deserialize(s);
            }
        }

        [TestMethod]
        public void TestLoadingAllErrors1_2_4()
        {
            // no longer upgraded - just testing it doesn''t err

            Options options = LoadOptionsFromEmbeddedResource("SettingFiles._1_2_4.ALLErrors.xml");
        }

        [TestMethod]
        public void TestLoadingNoErrors1_2_4()
        {
            // no longer upgraded - just testing it doesn''t err

            Options options = LoadOptionsFromEmbeddedResource("SettingFiles._1_2_4.NOErrors.xml");
        }

		[TestMethod]
		public void TestLoadingJSHint()
		{
			// no longer upgraded - just testing it doesn''t err

			Options options = LoadOptionsFromEmbeddedResource("SettingFiles.JsHint.xml");
		}
    }
}
