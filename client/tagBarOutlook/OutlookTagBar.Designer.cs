namespace OutlookTagBar
{
    partial class OutlookTagBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutlookTagBar));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonAddTag = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(832, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(281, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // buttonAddTag
            // 
            this.buttonAddTag.AutoSize = true;
            this.buttonAddTag.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonAddTag.FlatAppearance.BorderSize = 3;
            this.buttonAddTag.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAddTag.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddTag.Image")));
            this.buttonAddTag.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddTag.Location = new System.Drawing.Point(746, 3);
            this.buttonAddTag.Margin = new System.Windows.Forms.Padding(0);
            this.buttonAddTag.Name = "buttonAddTag";
            this.buttonAddTag.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonAddTag.Size = new System.Drawing.Size(84, 29);
            this.buttonAddTag.TabIndex = 1;
            this.buttonAddTag.Text = "Add Tag";
            this.buttonAddTag.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonAddTag.UseVisualStyleBackColor = true;
            this.buttonAddTag.Click += new System.EventHandler(this.ButtonAddTag_Click);
            // 
            // OutlookTagBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.buttonAddTag);
            this.Controls.Add(this.comboBox1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutlookTagBar";
            this.Size = new System.Drawing.Size(1116, 38);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttonAddTag;
    }
}
