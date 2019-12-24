using SuppressibleMessageBox;
using System.Media;
using System.Windows.Forms;

namespace Com.RobFaust.Common.UserInterface
{
    public partial class SuppressibleMessageBox : Form
    {
        #region Constructors
        protected SuppressibleMessageBox()
        {
            InitializeComponent();
        }

        protected SuppressibleMessageBox(string message) : this()
        {
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
        public static DialogResult Show(string messageId, string message) => Show(messageId, message, "Error");

        /// <summary>
        /// For Program.AddUpdateAppSettings and Program.ReadSetting implementations, please visit
        /// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager.appsettings?view=netframework-4.8
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static DialogResult Show(string messageId, string message, string caption)
        {
            DialogResult result = DialogResult.None;
            string settingId = GetSettingId(messageId);
            bool suppress;
            bool.TryParse(Program.ReadSetting(settingId), out suppress);
            if (!suppress)
            {
                SystemSounds.Beep.Play();
                using (SuppressibleMessageBox frm = new SuppressibleMessageBox(message, caption))
                {
                    result = frm.ShowDialog();
                    if (frm.Suppress)
                    {
                        Program.AddUpdateAppSettings(settingId, bool.TrueString);
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
    }
}
