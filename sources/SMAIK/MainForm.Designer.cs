namespace SMAIK
{
    partial class MainForm
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

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCalendar = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LBDATE = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnprevious = new System.Windows.Forms.Button();
            this.btnnext = new System.Windows.Forms.Button();
            this.daycontainer = new System.Windows.Forms.FlowLayoutPanel();
            this.planningPanel = new System.Windows.Forms.Panel();
            this.LBDAY = new System.Windows.Forms.Label();
            this.buttonAddPost = new System.Windows.Forms.Button();
            this.tabGroups = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBoxTgGr = new System.Windows.Forms.ListBox();
            this.contextMenuGroupEditing = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forgetGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonAddGroupTG = new System.Windows.Forms.Button();
            this.listBoxVkGr = new System.Windows.Forms.ListBox();
            this.buttonAddGroupVK = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.groupBoxPlugins = new System.Windows.Forms.GroupBox();
            this.listBoxPlugins = new System.Windows.Forms.ListBox();
            this.buttonAddPlugin = new System.Windows.Forms.Button();
            this.tabAnalytics = new System.Windows.Forms.TabPage();
            this.pluginsPanel = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabCalendar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabGroups.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuGroupEditing.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBoxPlugins.SuspendLayout();
            this.tabAnalytics.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabCalendar);
            this.tabControl1.Controls.Add(this.tabGroups);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabAnalytics);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1061, 586);
            this.tabControl1.TabIndex = 0;
            // 
            // tabCalendar
            // 
            this.tabCalendar.Controls.Add(this.splitContainer1);
            this.tabCalendar.Location = new System.Drawing.Point(4, 25);
            this.tabCalendar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabCalendar.Name = "tabCalendar";
            this.tabCalendar.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabCalendar.Size = new System.Drawing.Size(1053, 557);
            this.tabCalendar.TabIndex = 0;
            this.tabCalendar.Text = "Календарь";
            this.tabCalendar.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 2);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LBDATE);
            this.splitContainer1.Panel1.Controls.Add(this.label12);
            this.splitContainer1.Panel1.Controls.Add(this.label11);
            this.splitContainer1.Panel1.Controls.Add(this.label9);
            this.splitContainer1.Panel1.Controls.Add(this.label10);
            this.splitContainer1.Panel1.Controls.Add(this.label8);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.btnprevious);
            this.splitContainer1.Panel1.Controls.Add(this.btnnext);
            this.splitContainer1.Panel1.Controls.Add(this.daycontainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.planningPanel);
            this.splitContainer1.Panel2.Controls.Add(this.LBDAY);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAddPost);
            this.splitContainer1.Panel2MinSize = 250;
            this.splitContainer1.Size = new System.Drawing.Size(1047, 553);
            this.splitContainer1.SplitterDistance = 699;
            this.splitContainer1.TabIndex = 0;
            // 
            // LBDATE
            // 
            this.LBDATE.Location = new System.Drawing.Point(207, 15);
            this.LBDATE.Name = "LBDATE";
            this.LBDATE.Size = new System.Drawing.Size(268, 23);
            this.LBDATE.TabIndex = 6;
            this.LBDATE.Text = "MONTH YEAR";
            this.LBDATE.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(615, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(25, 16);
            this.label12.TabIndex = 4;
            this.label12.Text = "ВС";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(515, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 16);
            this.label11.TabIndex = 4;
            this.label11.Text = "СБ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(317, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 16);
            this.label9.TabIndex = 4;
            this.label9.Text = "ЧТ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(415, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 16);
            this.label10.TabIndex = 4;
            this.label10.Text = "ПТ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 16);
            this.label8.TabIndex = 4;
            this.label8.Text = "СР";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(127, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 16);
            this.label7.TabIndex = 4;
            this.label7.Text = "ВТ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "ПН";
            // 
            // btnprevious
            // 
            this.btnprevious.AllowDrop = true;
            this.btnprevious.Location = new System.Drawing.Point(7, 6);
            this.btnprevious.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnprevious.Name = "btnprevious";
            this.btnprevious.Size = new System.Drawing.Size(111, 41);
            this.btnprevious.TabIndex = 3;
            this.btnprevious.Text = "<- Прошлый ";
            this.btnprevious.UseVisualStyleBackColor = true;
            this.btnprevious.Click += new System.EventHandler(this.btnprevious_Click);
            // 
            // btnnext
            // 
            this.btnnext.Location = new System.Drawing.Point(545, 6);
            this.btnnext.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnnext.Name = "btnnext";
            this.btnnext.Size = new System.Drawing.Size(139, 41);
            this.btnnext.TabIndex = 2;
            this.btnnext.Text = "Грядущий ->";
            this.btnnext.UseVisualStyleBackColor = true;
            this.btnnext.Click += new System.EventHandler(this.btnnext_Click);
            // 
            // daycontainer
            // 
            this.daycontainer.Location = new System.Drawing.Point(4, 78);
            this.daycontainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.daycontainer.Name = "daycontainer";
            this.daycontainer.Size = new System.Drawing.Size(679, 466);
            this.daycontainer.TabIndex = 1;
            // 
            // planningPanel
            // 
            this.planningPanel.Location = new System.Drawing.Point(4, 45);
            this.planningPanel.Name = "planningPanel";
            this.planningPanel.Size = new System.Drawing.Size(338, 478);
            this.planningPanel.TabIndex = 2;
            // 
            // LBDAY
            // 
            this.LBDAY.AutoSize = true;
            this.LBDAY.Location = new System.Drawing.Point(125, 15);
            this.LBDAY.Name = "LBDAY";
            this.LBDAY.Size = new System.Drawing.Size(109, 16);
            this.LBDAY.TabIndex = 1;
            this.LBDAY.Text = "SELECTED DAY";
            // 
            // buttonAddPost
            // 
            this.buttonAddPost.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAddPost.Location = new System.Drawing.Point(0, 528);
            this.buttonAddPost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddPost.Name = "buttonAddPost";
            this.buttonAddPost.Size = new System.Drawing.Size(342, 23);
            this.buttonAddPost.TabIndex = 0;
            this.buttonAddPost.Text = "Добавить публикацию";
            this.buttonAddPost.UseVisualStyleBackColor = true;
            this.buttonAddPost.Click += new System.EventHandler(this.buttonAddPost_Click);
            // 
            // tabGroups
            // 
            this.tabGroups.Controls.Add(this.tableLayoutPanel1);
            this.tabGroups.Location = new System.Drawing.Point(4, 25);
            this.tabGroups.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabGroups.Name = "tabGroups";
            this.tabGroups.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabGroups.Size = new System.Drawing.Size(1053, 557);
            this.tabGroups.TabIndex = 1;
            this.tabGroups.Text = "Сообщества";
            this.tabGroups.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.47036F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.52964F));
            this.tableLayoutPanel1.Controls.Add(this.listBoxTgGr, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonAddGroupTG, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.listBoxVkGr, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonAddGroupVK, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.87719F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.12281F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1047, 553);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listBoxTgGr
            // 
            this.listBoxTgGr.ContextMenuStrip = this.contextMenuGroupEditing;
            this.listBoxTgGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTgGr.FormattingEnabled = true;
            this.listBoxTgGr.ItemHeight = 16;
            this.listBoxTgGr.Location = new System.Drawing.Point(512, 80);
            this.listBoxTgGr.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxTgGr.Name = "listBoxTgGr";
            this.listBoxTgGr.Size = new System.Drawing.Size(530, 468);
            this.listBoxTgGr.TabIndex = 3;
            this.listBoxTgGr.SelectedIndexChanged += new System.EventHandler(this.listBoxTgGr_SelectedIndexChanged);
            this.listBoxTgGr.DoubleClick += new System.EventHandler(this.editGroupToolStripMenuItem_Click);
            // 
            // contextMenuGroupEditing
            // 
            this.contextMenuGroupEditing.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuGroupEditing.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editGroupToolStripMenuItem,
            this.forgetGroupToolStripMenuItem});
            this.contextMenuGroupEditing.Name = "contextMenuGroupEditing";
            this.contextMenuGroupEditing.Size = new System.Drawing.Size(232, 52);
            // 
            // editGroupToolStripMenuItem
            // 
            this.editGroupToolStripMenuItem.Name = "editGroupToolStripMenuItem";
            this.editGroupToolStripMenuItem.Size = new System.Drawing.Size(231, 24);
            this.editGroupToolStripMenuItem.Text = "Редактировать группу";
            this.editGroupToolStripMenuItem.Click += new System.EventHandler(this.editGroupToolStripMenuItem_Click);
            // 
            // forgetGroupToolStripMenuItem
            // 
            this.forgetGroupToolStripMenuItem.Name = "forgetGroupToolStripMenuItem";
            this.forgetGroupToolStripMenuItem.Size = new System.Drawing.Size(231, 24);
            this.forgetGroupToolStripMenuItem.Text = "Забыть группу";
            this.forgetGroupToolStripMenuItem.Click += new System.EventHandler(this.forgetGroupToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "ВКонтакте";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(744, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Telegram";
            // 
            // buttonAddGroupTG
            // 
            this.buttonAddGroupTG.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAddGroupTG.Location = new System.Drawing.Point(724, 41);
            this.buttonAddGroupTG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddGroupTG.Name = "buttonAddGroupTG";
            this.buttonAddGroupTG.Size = new System.Drawing.Size(105, 28);
            this.buttonAddGroupTG.TabIndex = 1;
            this.buttonAddGroupTG.Text = "+";
            this.buttonAddGroupTG.UseVisualStyleBackColor = true;
            this.buttonAddGroupTG.Click += new System.EventHandler(this.buttonAddGroupTG_Click);
            // 
            // listBoxVkGr
            // 
            this.listBoxVkGr.ContextMenuStrip = this.contextMenuGroupEditing;
            this.listBoxVkGr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxVkGr.FormattingEnabled = true;
            this.listBoxVkGr.ItemHeight = 16;
            this.listBoxVkGr.Location = new System.Drawing.Point(5, 80);
            this.listBoxVkGr.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxVkGr.Name = "listBoxVkGr";
            this.listBoxVkGr.Size = new System.Drawing.Size(498, 468);
            this.listBoxVkGr.TabIndex = 2;
            this.listBoxVkGr.SelectedIndexChanged += new System.EventHandler(this.listBoxVkGr_SelectedIndexChanged);
            this.listBoxVkGr.DoubleClick += new System.EventHandler(this.editGroupToolStripMenuItem_Click);
            // 
            // buttonAddGroupVK
            // 
            this.buttonAddGroupVK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonAddGroupVK.Location = new System.Drawing.Point(206, 41);
            this.buttonAddGroupVK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddGroupVK.Name = "buttonAddGroupVK";
            this.buttonAddGroupVK.Size = new System.Drawing.Size(95, 28);
            this.buttonAddGroupVK.TabIndex = 1;
            this.buttonAddGroupVK.Text = "+";
            this.buttonAddGroupVK.UseVisualStyleBackColor = true;
            this.buttonAddGroupVK.Click += new System.EventHandler(this.buttonAddGroupVK_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.buttonAbout);
            this.tabSettings.Controls.Add(this.groupBoxPlugins);
            this.tabSettings.Location = new System.Drawing.Point(4, 25);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(1053, 557);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Настройки";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // buttonAbout
            // 
            this.buttonAbout.Location = new System.Drawing.Point(29, 463);
            this.buttonAbout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(161, 23);
            this.buttonAbout.TabIndex = 1;
            this.buttonAbout.Text = "О программе";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // groupBoxPlugins
            // 
            this.groupBoxPlugins.Controls.Add(this.listBoxPlugins);
            this.groupBoxPlugins.Controls.Add(this.buttonAddPlugin);
            this.groupBoxPlugins.Location = new System.Drawing.Point(27, 16);
            this.groupBoxPlugins.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxPlugins.Name = "groupBoxPlugins";
            this.groupBoxPlugins.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBoxPlugins.Size = new System.Drawing.Size(987, 425);
            this.groupBoxPlugins.TabIndex = 0;
            this.groupBoxPlugins.TabStop = false;
            this.groupBoxPlugins.Text = "Плагины";
            // 
            // listBoxPlugins
            // 
            this.listBoxPlugins.FormattingEnabled = true;
            this.listBoxPlugins.ItemHeight = 16;
            this.listBoxPlugins.Location = new System.Drawing.Point(7, 21);
            this.listBoxPlugins.Name = "listBoxPlugins";
            this.listBoxPlugins.Size = new System.Drawing.Size(974, 372);
            this.listBoxPlugins.TabIndex = 1;
            this.listBoxPlugins.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListBoxPlugins_MouseDown);
            // 
            // buttonAddPlugin
            // 
            this.buttonAddPlugin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonAddPlugin.Location = new System.Drawing.Point(3, 400);
            this.buttonAddPlugin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddPlugin.Name = "buttonAddPlugin";
            this.buttonAddPlugin.Size = new System.Drawing.Size(981, 23);
            this.buttonAddPlugin.TabIndex = 0;
            this.buttonAddPlugin.Text = "Добавить плагин";
            this.buttonAddPlugin.UseVisualStyleBackColor = true;
            this.buttonAddPlugin.Click += new System.EventHandler(this.buttonAddPlugin_Click);
            // 
            // tabAnalytics
            // 
            this.tabAnalytics.Controls.Add(this.pluginsPanel);
            this.tabAnalytics.Location = new System.Drawing.Point(4, 25);
            this.tabAnalytics.Name = "tabAnalytics";
            this.tabAnalytics.Size = new System.Drawing.Size(1053, 557);
            this.tabAnalytics.TabIndex = 3;
            this.tabAnalytics.Text = "Аналитика";
            this.tabAnalytics.UseVisualStyleBackColor = true;
            // 
            // pluginsPanel
            // 
            this.pluginsPanel.Location = new System.Drawing.Point(9, 16);
            this.pluginsPanel.Name = "pluginsPanel";
            this.pluginsPanel.Size = new System.Drawing.Size(1036, 533);
            this.pluginsPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 586);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SMAIK";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCalendar.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabGroups.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuGroupEditing.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.groupBoxPlugins.ResumeLayout(false);
            this.tabAnalytics.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCalendar;
        private System.Windows.Forms.TabPage tabGroups;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonAddGroupVK;
        private System.Windows.Forms.Button buttonAddGroupTG;
        private System.Windows.Forms.GroupBox groupBoxPlugins;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Button buttonAddPlugin;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonAddPost;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnprevious;
        private System.Windows.Forms.Button btnnext;
        private System.Windows.Forms.FlowLayoutPanel daycontainer;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label LBDATE;
        private System.Windows.Forms.Label LBDAY;
        private System.Windows.Forms.ListBox listBoxTgGr;
        private System.Windows.Forms.ListBox listBoxVkGr;
        private System.Windows.Forms.ContextMenuStrip contextMenuGroupEditing;
        private System.Windows.Forms.ToolStripMenuItem editGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forgetGroupToolStripMenuItem;
        private System.Windows.Forms.Panel planningPanel;
        private System.Windows.Forms.ListBox listBoxPlugins;
        private System.Windows.Forms.TabPage tabAnalytics;
        private System.Windows.Forms.Panel pluginsPanel;
    }
}

