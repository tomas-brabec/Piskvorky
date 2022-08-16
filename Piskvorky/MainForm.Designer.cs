namespace Piskvorky
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.labelLeft = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.popupWindow = new Piskvorky.PopupWindow();
            this.panelRight = new System.Windows.Forms.Panel();
            this.labelRight = new System.Windows.Forms.Label();
            this.panelLeft.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(794, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 429);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(794, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.labelLeft);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 25);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(120, 404);
            this.panelLeft.TabIndex = 2;
            // 
            // labelLeft
            // 
            this.labelLeft.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelLeft.Location = new System.Drawing.Point(0, 10);
            this.labelLeft.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelLeft.Name = "labelLeft";
            this.labelLeft.Size = new System.Drawing.Size(120, 23);
            this.labelLeft.TabIndex = 0;
            this.labelLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.popupWindow);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(120, 25);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(554, 404);
            this.panelCenter.TabIndex = 3;
            this.panelCenter.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCenter_Paint);
            this.panelCenter.Resize += new System.EventHandler(this.panelCenter_Resize);
            // 
            // popupWindow
            // 
            this.popupWindow.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.popupWindow.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.popupWindow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.popupWindow.Location = new System.Drawing.Point(127, 112);
            this.popupWindow.Name = "popupWindow";
            this.popupWindow.Padding = new System.Windows.Forms.Padding(3);
            this.popupWindow.Size = new System.Drawing.Size(300, 180);
            this.popupWindow.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.labelRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(674, 25);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(120, 404);
            this.panelRight.TabIndex = 4;
            // 
            // labelRight
            // 
            this.labelRight.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelRight.Location = new System.Drawing.Point(0, 10);
            this.labelRight.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.labelRight.Name = "labelRight";
            this.labelRight.Size = new System.Drawing.Size(120, 23);
            this.labelRight.TabIndex = 1;
            this.labelRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 451);
            this.Controls.Add(this.panelCenter);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.MinimumSize = new System.Drawing.Size(656, 486);
            this.Name = "MainForm";
            this.Text = "Form";
            this.panelLeft.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip;
        private StatusStrip statusStrip;
        private Panel panelLeft;
        private Panel panelCenter;
        private Panel panelRight;
        private PopupWindow popupWindow;
        private Label labelLeft;
        private Label labelRight;
    }
}