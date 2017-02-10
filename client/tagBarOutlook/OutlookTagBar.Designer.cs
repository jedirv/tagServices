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
            this.comboBoxTags = new System.Windows.Forms.ComboBox();
            this.buttonAddTag = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxTags
            // 
            this.comboBoxTags.FormattingEnabled = true;
            this.comboBoxTags.Location = new System.Drawing.Point(832, 3);
            this.comboBoxTags.Name = "comboBoxTags";
            this.comboBoxTags.Size = new System.Drawing.Size(281, 21);
            this.comboBoxTags.TabIndex = 0;
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1213, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(158, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NewTagKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1135, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "New Tag :";
            // 
            // OutlookTagBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonAddTag);
            this.Controls.Add(this.comboBoxTags);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "OutlookTagBar";
            this.Size = new System.Drawing.Size(1400, 38);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxTags;
        private System.Windows.Forms.Button buttonAddTag;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}
