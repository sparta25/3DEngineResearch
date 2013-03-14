namespace TestOivWinForms
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._panelControls = new System.Windows.Forms.Panel();
            this._buttonRotate = new System.Windows.Forms.Button();
            this._panelView = new System.Windows.Forms.Panel();
            this._panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // _panelControls
            // 
            this._panelControls.Controls.Add(this._buttonRotate);
            this._panelControls.Dock = System.Windows.Forms.DockStyle.Right;
            this._panelControls.Location = new System.Drawing.Point(197, 0);
            this._panelControls.Name = "_panelControls";
            this._panelControls.Size = new System.Drawing.Size(87, 262);
            this._panelControls.TabIndex = 0;
            // 
            // _buttonRotate
            // 
            this._buttonRotate.Location = new System.Drawing.Point(12, 0);
            this._buttonRotate.Name = "_buttonRotate";
            this._buttonRotate.Size = new System.Drawing.Size(75, 23);
            this._buttonRotate.TabIndex = 2;
            this._buttonRotate.Text = "Rotate";
            this._buttonRotate.UseVisualStyleBackColor = true;
            this._buttonRotate.Click += new System.EventHandler(this._buttonRotate_Click);
            // 
            // _panelView
            // 
            this._panelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panelView.Location = new System.Drawing.Point(0, 0);
            this._panelView.Name = "_panelView";
            this._panelView.Size = new System.Drawing.Size(197, 262);
            this._panelView.TabIndex = 1;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this._panelView);
            this.Controls.Add(this._panelControls);
            this.Name = "TestForm";
            this.Text = "Test OIV WinForms";
            this._panelControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _panelControls;
        private System.Windows.Forms.Panel _panelView;
        private System.Windows.Forms.Button _buttonRotate;
    }
}

