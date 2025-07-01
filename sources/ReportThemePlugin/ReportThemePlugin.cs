using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using PluginBase;
using System.IO;
using System.Xml.Linq;

namespace ReportThemePlugin
{
    public class ReportByThemePlugin : IReportMaker
    {
        public string GetInfo()
        {
            return "Создание отчета-аналитики сообществ по заданной теме. Автор: Анализатор. Версия: 9.111a";
        }
        f
        public string GetGUID()
        {
            return "{F69A0361-76F2-4751-ACAC-003B2D179E15}";
        }

        public string GetGUIinfo()
        {
            return "Создать отчет по теме";
        }

        public string GetPluginType()
        {
            return "ReportMaker";
        }

        public string SetSettings()
        {
            // TODO вызов окошка с полем для ввода темы, пути куда сохранить отчет. 
            // настройки сохранить по строке с фиксированным разделителем (например $) и эту строку вернуть.

            using (Form form = new Form())
            {
                form.Text = "Настройки отчета";
                TextBox topicBox = new TextBox { Left = 10, Top = 10, Width = 300 };
                TextBox pathBox = new TextBox { Left = 10, Top = 40, Width = 300 };
                Button browseBtn = new Button { Left = 320, Top = 40, Text = "Обзор" };
                Button okBtn = new Button { Left = 10, Top = 70, Text = "OK" };

                browseBtn.Click += (s, e) =>
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                            pathBox.Text = saveDialog.FileName;
                    }
                };

                string result = null;
                okBtn.Click += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(topicBox.Text) && !string.IsNullOrWhiteSpace(pathBox.Text))
                    {
                        result = $"{topicBox.Text}${pathBox.Text}";
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!");
                    }
                };

                form.Controls.Add(topicBox);
                form.Controls.Add(pathBox);
                form.Controls.Add(browseBtn);
                form.Controls.Add(okBtn);
                form.AcceptButton = okBtn;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ClientSize = new System.Drawing.Size(400, 110);

                return form.ShowDialog() == DialogResult.OK ? result : null;
            }
        }



        public void MakeReport(string settings)
        {
            // Шаг 2: Поиск групп по теме
            string searchUrl = $"https://api.vk.com/method/groups.search?q={Uri.EscapeDataString(theme)}&count=50&access_token={access_token}&v=5.131";

            using var client = new HttpClient();
            string searchResponse = await client.GetStringAsync(searchUrl);
            var searchData = JsonSerializer.Deserialize<VkGroupSearchResponse>(searchResponse);

        }
    }
}


