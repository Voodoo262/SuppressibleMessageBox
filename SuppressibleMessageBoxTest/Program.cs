using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.RobFaust.Common.UserInterface
{
    static class Program
    {
        private static Configuration config;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string path = string.Join(Path.DirectorySeparatorChar.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AssemblyProduct, "settings.xml");
            ExeConfigurationFileMap customConfigFileMap = new ExeConfigurationFileMap();
            customConfigFileMap.ExeConfigFilename = path;
            config = ConfigurationManager.OpenMappedExeConfiguration(customConfigFileMap, ConfigurationUserLevel.None);

            // Use case #1, informational message with no response
            SuppressibleMessageBox.Show(config, "msg1", "This message does not return a response", "Test Message #1");

            // Use case #2, evaluate whether or not the message was shown
            DialogResult result = SuppressibleMessageBox.Show(config, "msg2", "This message DOES return a response", "Test Message #2");
            if (result == DialogResult.None)
            {
                MessageBox.Show("The original message was suppressed due to settings");
            }
            else
            {
                MessageBox.Show($"The message box was not suppressed and the result was \"{result.ToString()}\"");
            }
        }

        internal static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
    }
}
