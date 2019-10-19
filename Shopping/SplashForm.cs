using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace Shopping
{
    public partial class SplashForm : SplashScreen
    {
        public SplashForm()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © Uğur Ayaz - " + DateTime.Now.Year.ToString();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
        }
    }
}