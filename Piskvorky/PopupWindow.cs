using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class PopupWindow : UserControl
    {
        public PopupWindow()
        {
            InitializeComponent();
            SetPromptMode();
        }

        public void Center()
        {
            var parent = Parent;
            Location = new Point(parent.Width / 2 - Width / 2, parent.Height / 2 - Height / 2);
        }

        private void PopupWindow_VisibleChanged(object sender, EventArgs e)
        {
            if(Visible == true)
                Center();
        }

        public void SetPromptMode()
        {
            btnCancel.Visible = false;
            btnConfirm.Visible = true;
            panelInfo.Visible = false;
            panelPrompt.Visible = true;
        }

        public void SetInfoMode(string infoText)
        {
            btnCancel.Visible = true;
            btnConfirm.Visible = false;
            panelInfo.Visible = true;
            panelPrompt.Visible = false;
            labelInfo.Text = infoText;
        }
    }
}
