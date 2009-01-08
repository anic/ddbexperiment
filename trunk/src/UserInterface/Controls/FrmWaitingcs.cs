using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DistDBMS.Common.Execution;

namespace DistDBMS.UserInterface.Controls
{
    public partial class FrmWaitingcs : Form
    {
        public FrmWaitingcs()
        {
            InitializeComponent();
        }

        ExecutionResult result;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }



    }
}
