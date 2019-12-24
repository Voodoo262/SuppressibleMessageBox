using System;
using System.Configuration;
using System.Media;
using System.Windows.Forms;

namespace Com.RobFaust.Common.UserInterface
{
    public partial class SuppressibleMessageBox : Form
    {
        #region Constructors
        protected SuppressibleMessageBox(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
        }

        protected SuppressibleMessageBox(string message, string caption) : this(message)
        {
            Text = caption;
        }
        #endregion

        #region Public Properties
        public bool Suppress => chkDontShow.Checked;
        #endregion

        #region Static Methods
        public static DialogResult Show(Configuration config, string messageId, string message) => Show(config, messageId, message, "Error");

        /// <summary>
        /// For Program.AddUpdateAppSettings and Program.ReadSetting implementations, please visit
        /// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager.appsettings?view=netframework-4.8
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static DialogResult Show(Configuration config, string messageId, string message, string caption)
        {
            DialogResult result = DialogResult.None;
            string settingId = GetSettingId(messageId);
            bool suppress;
            bool.TryParse(ReadSetting(config, settingId), out suppress);
            if (!suppress)
            {
                SystemSounds.Beep.Play();
                using (SuppressibleMessageBox frm = new SuppressibleMessageBox(message, caption))
                {
                    result = frm.ShowDialog();
                    if (frm.Suppress)
                    {
                        AddUpdateAppSettings(config, settingId, bool.TrueString);
                    }
                }
            }
            return result;
        }

        public static string GetSettingId(string messageId)
        {
            return $"messages.{messageId}.hide";
        }
        #endregion

        #region Configuration Methods
        private static string ReadSetting(Configuration config, string key)
        {
            try
            {
                AppSettingsSection section = config.GetSection("appSettings") as AppSettingsSection;
                if (section != null)
                {
                    KeyValueConfigurationCollection appSettings = section.Settings;
                    if (appSettings[key] != null)
                    {
                        return appSettings[key].Value;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
            }
            return string.Empty;
        }

        private static bool AddUpdateAppSettings(Configuration config, string key, string value)
        {
            try
            {
                AppSettingsSection section = config.GetSection("appSettings") as AppSettingsSection;
                if (section != null)
                {
                    KeyValueConfigurationCollection settings = section.Settings;
                    if (settings[key] == null)
                    {
                        settings.Add(key, value);
                    }
                    else
                    {
                        settings[key].Value = value;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    return true;
                }
            }
            catch (Exception e)
            {
                HandleConfigError(e);
            }
            return false;
        }

        private static void HandleConfigError(Exception e)
        {
            MessageBox.Show(e.Message, "Error accessing configuration file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }
}
