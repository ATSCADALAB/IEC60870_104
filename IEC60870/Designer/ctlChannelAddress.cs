using System.Windows.Forms;

namespace IEC60870Driver
{
    public partial class ctlChannelAddress : UserControl
    {
        private readonly ATDriver driver;

        #region PROPERTIES

        public string LifeTime
        {
            get => this.txtLifetime.Text.Trim();
            set => this.txtLifetime.Text = value;
        }

        #endregion

        public ctlChannelAddress(ATDriver driver)
        {
            InitializeComponent();
            this.driver = driver;

            KeyPress += (sender, e) => CheckKey(e.KeyChar);
            btnOK.Click += (sender, e) => UpdateChannel();
        }

        private void CheckKey(char keyChar)
        {
            if (keyChar == (char)13)
            {
                UpdateChannel();
            }
            else if (keyChar == (char)27)
            {
                Parent.Dispose();
            }
        }

        private void UpdateChannel()
        {
            if (!CheckChannelPropertis()) return;
            this.driver.ChannelAddress = LifeTime;
            Parent.Dispose();
        }

        private bool CheckChannelPropertis()
        {
            if (!uint.TryParse(LifeTime, out _))
            {
                MessageBox.Show($"Connection lifetime value must be an integer greater than or equal to 0.", "ATSCADA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}