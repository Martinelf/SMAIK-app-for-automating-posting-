namespace SMAIK
{
    partial class EditPostDialogForm
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
            this.components = new System.ComponentModel.Container();
            this.richTextBoxPublication = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripTextEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonPlan = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.comboBoxGroups = new System.Windows.Forms.ComboBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.listViewEmo = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBoxPublication = new System.Windows.Forms.PictureBox();
            this.contextMenuStripPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPhotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemovePhotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPublication)).BeginInit();
            this.contextMenuStripPictureBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxPublication
            // 
            this.richTextBoxPublication.ContextMenuStrip = this.contextMenuStripTextEdit;
            this.richTextBoxPublication.Font = new System.Drawing.Font("Segoe UI Symbol", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxPublication.Location = new System.Drawing.Point(12, 46);
            this.richTextBoxPublication.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBoxPublication.Name = "richTextBoxPublication";
            this.richTextBoxPublication.Size = new System.Drawing.Size(564, 449);
            this.richTextBoxPublication.TabIndex = 0;
            this.richTextBoxPublication.Text = "";
            this.richTextBoxPublication.Enter += new System.EventHandler(this.richTextBoxPublication_Enter);
            this.richTextBoxPublication.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richTextBoxPublication_KeyPress);
            this.richTextBoxPublication.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richTextBoxPublication_KeyUp);
            this.richTextBoxPublication.MouseUp += new System.Windows.Forms.MouseEventHandler(this.richTextBoxPublication_MouseUp);
            // 
            // contextMenuStripTextEdit
            // 
            this.contextMenuStripTextEdit.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripTextEdit.Name = "contextMenuStripTextEdit";
            this.contextMenuStripTextEdit.Size = new System.Drawing.Size(61, 4);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Текст публикации";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 517);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Место";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(333, 517);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Время";
            // 
            // buttonPlan
            // 
            this.buttonPlan.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonPlan.Location = new System.Drawing.Point(15, 562);
            this.buttonPlan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonPlan.Name = "buttonPlan";
            this.buttonPlan.Size = new System.Drawing.Size(193, 44);
            this.buttonPlan.TabIndex = 2;
            this.buttonPlan.Text = "Запланировать";
            this.buttonPlan.UseVisualStyleBackColor = true;
            this.buttonPlan.Click += new System.EventHandler(this.buttonPlan_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonDelete.Location = new System.Drawing.Point(732, 562);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(193, 44);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Удалить публикацию";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new System.Drawing.Point(75, 514);
            this.comboBoxGroups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new System.Drawing.Size(221, 24);
            this.comboBoxGroups.TabIndex = 3;
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker.Location = new System.Drawing.Point(387, 514);
            this.dateTimePicker.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(221, 22);
            this.dateTimePicker.TabIndex = 4;
            // 
            // listViewEmo
            // 
            this.listViewEmo.HideSelection = false;
            this.listViewEmo.Location = new System.Drawing.Point(604, 282);
            this.listViewEmo.MultiSelect = false;
            this.listViewEmo.Name = "listViewEmo";
            this.listViewEmo.Size = new System.Drawing.Size(321, 213);
            this.listViewEmo.TabIndex = 5;
            this.listViewEmo.UseCompatibleStateImageBehavior = false;
            this.listViewEmo.SelectedIndexChanged += new System.EventHandler(this.listViewEmo_SelectedIndexChanged);
            this.listViewEmo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewEmo_MouseClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(601, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Добавить эмодзи";
            // 
            // pictureBoxPublication
            // 
            this.pictureBoxPublication.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPublication.ContextMenuStrip = this.contextMenuStripPictureBox;
            this.pictureBoxPublication.Location = new System.Drawing.Point(604, 46);
            this.pictureBoxPublication.Name = "pictureBoxPublication";
            this.pictureBoxPublication.Size = new System.Drawing.Size(321, 214);
            this.pictureBoxPublication.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPublication.TabIndex = 6;
            this.pictureBoxPublication.TabStop = false;
            this.pictureBoxPublication.Click += new System.EventHandler(this.pictureBoxPublication_Click);
            this.pictureBoxPublication.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxPublication_DragDrop);
            this.pictureBoxPublication.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxPublication_DragEnter);
            // 
            // contextMenuStripPictureBox
            // 
            this.contextMenuStripPictureBox.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStripPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPhotoToolStripMenuItem,
            this.RemovePhotoToolStripMenuItem});
            this.contextMenuStripPictureBox.Name = "contextMenuStripPictureBox";
            this.contextMenuStripPictureBox.Size = new System.Drawing.Size(184, 52);
            // 
            // addPhotoToolStripMenuItem
            // 
            this.addPhotoToolStripMenuItem.Name = "addPhotoToolStripMenuItem";
            this.addPhotoToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.addPhotoToolStripMenuItem.Text = "Добавить фото";
            this.addPhotoToolStripMenuItem.Click += new System.EventHandler(this.pictureBoxPublication_Click);
            // 
            // RemovePhotoToolStripMenuItem
            // 
            this.RemovePhotoToolStripMenuItem.Name = "RemovePhotoToolStripMenuItem";
            this.RemovePhotoToolStripMenuItem.Size = new System.Drawing.Size(183, 24);
            this.RemovePhotoToolStripMenuItem.Text = "Удалить фото";
            this.RemovePhotoToolStripMenuItem.Click += new System.EventHandler(this.RemovePhotoToolStripMenuItem_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(601, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "Фото публикации";
            // 
            // EditPostDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 617);
            this.Controls.Add(this.pictureBoxPublication);
            this.Controls.Add(this.listViewEmo);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.comboBoxGroups);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonPlan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxPublication);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "EditPostDialogForm";
            this.Text = "Добавление публикации";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPublication)).EndInit();
            this.contextMenuStripPictureBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.RichTextBox richTextBoxPublication;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonPlan;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.ComboBox comboBoxGroups;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.ListView listViewEmo;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ContextMenuStrip contextMenuStripTextEdit;
        private System.Windows.Forms.PictureBox pictureBoxPublication;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPictureBox;
        private System.Windows.Forms.ToolStripMenuItem addPhotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemovePhotoToolStripMenuItem;
    }
}