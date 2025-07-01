namespace SMAIK
{
    partial class UserControlDays
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbdays = new System.Windows.Forms.Label();
            this.lbpubls = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbdays
            // 
            this.lbdays.AutoSize = true;
            this.lbdays.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbdays.Location = new System.Drawing.Point(3, 3);
            this.lbdays.Name = "lbdays";
            this.lbdays.Size = new System.Drawing.Size(23, 16);
            this.lbdays.TabIndex = 0;
            this.lbdays.Text = "00";
            this.lbdays.Click += new System.EventHandler(this.UserControlDays_Click);
            // 
            // lbpubls
            // 
            this.lbpubls.AutoSize = true;
            this.lbpubls.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbpubls.Location = new System.Drawing.Point(90, 0);
            this.lbpubls.Name = "lbpubls";
            this.lbpubls.Size = new System.Drawing.Size(0, 16);
            this.lbpubls.TabIndex = 1;
            this.lbpubls.Click += new System.EventHandler(this.UserControlDays_Click);
            // 
            // UserControlDays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lbpubls);
            this.Controls.Add(this.lbdays);
            this.Name = "UserControlDays";
            this.Size = new System.Drawing.Size(90, 70);
            this.Load += new System.EventHandler(this.UserControlDays_Load);
            this.Click += new System.EventHandler(this.UserControlDays_Click);
            this.DoubleClick += new System.EventHandler(this.UserControlDays_DoubleClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbdays;
        private System.Windows.Forms.Label lbpubls;
    }
}
