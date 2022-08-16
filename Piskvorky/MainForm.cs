namespace Piskvorky
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void panelCenter_Resize(object sender, EventArgs e)
        {
            if(popupWindow.Visible == true)
                popupWindow.Center();
        }

        //private void btnCancel_Click(object sender, EventArgs e)
        //{

        //}

        //private void btnConfirm_Click(object sender, EventArgs e)
        //{

        //}
    }
}