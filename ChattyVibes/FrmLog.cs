using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
namespace ChattyVibes
{
    public partial class FrmLog : ChildForm
    {
        public FrmLog()
        {
            InitializeComponent();
        }

        private void FrmLog_Load(object sender, EventArgs e)
        {
            tbLog.Lines = MainFrm.LogMessages.ToArray();
            tbLog.Update();
        }

        internal void AddLogMsg(string msg) =>
            tbLog.AppendText($"\r\n{msg}");
    }
}
