namespace SMAIK
{
    partial class AddGroupDialogForm
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
            this.textBoxToken = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxId = new System.Windows.Forms.TextBox();
            this.buttonAddGroup = new System.Windows.Forms.Button();
            this.buttonInfo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxToken
            // 
            this.textBoxToken.Location = new System.Drawing.Point(24, 92);
            this.textBoxToken.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxToken.Name = "textBoxToken";
            this.textBoxToken.Size = new System.Drawing.Size(665, 22);
            this.textBoxToken.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "AccessToken:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "GroupID:";
            // 
            // textBoxId
            // 
            this.textBoxId.Location = new System.Drawing.Point(24, 171);
            this.textBoxId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxId.Name = "textBoxId";
            this.textBoxId.Size = new System.Drawing.Size(665, 22);
            this.textBoxId.TabIndex = 0;
            // 
            // buttonAddGroup
            // 
            this.buttonAddGroup.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonAddGroup.Location = new System.Drawing.Point(24, 249);
            this.buttonAddGroup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddGroup.Name = "buttonAddGroup";
            this.buttonAddGroup.Size = new System.Drawing.Size(667, 39);
            this.buttonAddGroup.TabIndex = 2;
            this.buttonAddGroup.Text = "Добавить";
            this.buttonAddGroup.UseVisualStyleBackColor = true;
            this.buttonAddGroup.Click += new System.EventHandler(this.buttonAddGroup_Click);
            // 
            // buttonInfo
            // 
            this.buttonInfo.Location = new System.Drawing.Point(660, 14);
            this.buttonInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonInfo.Name = "buttonInfo";
            this.buttonInfo.Size = new System.Drawing.Size(31, 26);
            this.buttonInfo.TabIndex = 2;
            this.buttonInfo.Text = "?";
            this.buttonInfo.UseVisualStyleBackColor = true;
            this.buttonInfo.Click += new System.EventHandler(this.buttonInfo_Click);
            // 
            // AddGroupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 305);
            this.Controls.Add(this.buttonInfo);
            this.Controls.Add(this.buttonAddGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxId);
            this.Controls.Add(this.textBoxToken);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "AddGroupDialogForm";
            this.Text = "Добавление нового сообщества";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxId;
        private System.Windows.Forms.Button buttonAddGroup;
        private System.Windows.Forms.Button buttonInfo;
    }
}