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
using System.Net.Http;
using Newtonsoft.Json;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;




namespace ReportActivityGrowthPlugin
{
    public class ReportActivityGrowthPlugin : IReportMaker
    {
        public string GetInfo()
        {
            return "Создание отчета о росте активности конкретного сообщества. Автор: tyo111a. Версия: 0.111a";
        }

        public string GetGUID()
        {
            return "{22518501-1046-41E2-88AA-0A4B3FD26564}";
        }

        public string GetGUIinfo()
        {
            return "Посмотреть статистику роста активности";
        }

        public string GetPluginType()
        {
            return "ReportMaker";
        }

        public string SetSettings()
        {
            using (Form form = new Form())
            {
                form.Text = "Анализ сообщества";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ClientSize = new Size(420, 130);

                Label groupLabel = new Label { Text = "Группа:", Left = 10, Top = 13, Width = 60 };
                TextBox groupBox = new TextBox { Left = 70, Top = 10, Width = 330 };

                Label pathLabel = new Label { Text = "Путь:", Left = 10, Top = 43, Width = 60 };
                TextBox pathBox = new TextBox { Left = 70, Top = 40, Width = 260 };
                Button browseBtn = new Button { Left = 330, Top = 38, Width = 70, Text = "Обзор" };

                Button okBtn = new Button { Text = "OK", Left = 280, Width = 60, Top = 80, DialogResult = DialogResult.OK };
                Button cancelBtn = new Button { Text = "Отмена", Left = 350, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

                browseBtn.Click += (s, e) =>
                {
                    using (SaveFileDialog saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
                        if (saveDialog.ShowDialog() == DialogResult.OK)
                            pathBox.Text = saveDialog.FileName;
                    }
                };

                // Создаём папку reports в корне приложения, если её нет
                string reportsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports");
                if (!Directory.Exists(reportsDir))
                {
                    Directory.CreateDirectory(reportsDir);
                }

                // Формируем путь вида reports/report_20250616_143012.pdf
                string defaultFileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string defaultPath = Path.Combine(reportsDir, defaultFileName);

                // Подставляем путь в поле
                pathBox.Text = defaultPath;

                string result = null;
                okBtn.Click += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(groupBox.Text) && !string.IsNullOrWhiteSpace(pathBox.Text))
                    {
                        result = $"{groupBox.Text}${pathBox.Text}";
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    }
                    else
                    {
                        MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                form.Controls.Add(groupLabel);
                form.Controls.Add(groupBox);
                form.Controls.Add(pathLabel);
                form.Controls.Add(pathBox);
                form.Controls.Add(browseBtn);
                form.Controls.Add(okBtn);
                form.Controls.Add(cancelBtn);
                form.AcceptButton = okBtn;
                form.CancelButton = cancelBtn;

                return form.ShowDialog() == DialogResult.OK ? result : null;
            }
        }



        public class VkGroupSearchResponse
        {
            public SearchResponse response { get; set; }
        }

        public class SearchResponse
        {
            public List<VkGroupItem> items { get; set; }
        }

        public class VkGroupItem
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class VkGroupStatsResponse
        {
            public List<VkGroupFull> response { get; set; }
        }

        public class VkGroupFull
        {
            public int id { get; set; }
            public string name { get; set; }
            public int? members_count { get; set; }
            public string activity { get; set; }
            public int? age_limits { get; set; }
        }

        public static IEnumerable<List<T>> ChunkList<T>(List<T> source, int chunkSize)
        {
            for (int i = 0; i < source.Count; i += chunkSize)
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
        }


        public async void MakeReport(string settings)
        {
            var parts = settings.Split('$');
            string groupName = parts[0];
            string path = parts[1];
            string accessToken = "vk1.a.sgdK2fQvXQMv2jU8PdT7hCqEgpUonC-BLjAoKnZE79lLoWCD61SETE6PPVZ9F1sVjMV9qya5fFPnSELI8NalcZ4b5XXtSW09NY1raHHBXxblcNwuXr7t2Ky0ENcS57WMg9yH6uj475_MUfDAnSofMr6C4xyCRFvROS53-PQoq064AKdhJyUQ5saLCP7wPQ4wzTlndbR_ug9GrpA1HMeHgw";

            using HttpClient client = new HttpClient();

            // Получаем данные о группе
            string infoUrl = $"https://api.vk.com/method/groups.getById?group_ids={groupName}&fields=members_count,activity,age_limits,is_closed,site,description,screen_name&access_token={accessToken}&v=5.131";
            var response = await client.GetStringAsync(infoUrl);

            JObject jsonObj = JObject.Parse(response);
            JToken groupInfo = jsonObj["response"]?.FirstOrDefault();

            if (groupInfo == null)
            {
                MessageBox.Show("Группа не найдена или ошибка API");
                return;
            }

            long groupId = groupInfo.Value<long>("id");
            int membersCount = groupInfo.Value<int>("members_count");

            // Получаем посты за 30 дней
            string wallUrl = $"https://api.vk.com/method/wall.get?owner_id=-{groupId}&count=100&access_token={accessToken}&v=5.131";
            var wallResp = await client.GetStringAsync(wallUrl);
            dynamic wallData = JsonConvert.DeserializeObject(wallResp);

            var postsByDay = new Dictionary<DayOfWeek, int>();
            var postsByHour = new int[24];
            var contentTypesCount = new Dictionary<string, int>();

            DateTime now = DateTime.UtcNow;
            DateTime cutoffDate = now.AddDays(-30);

            if (wallData?.response?.items == null)
            {
                MessageBox.Show("Нет данных по постам");
                return;
            }

            foreach (var post in wallData.response.items)
            {
                if (post.is_pinned != null && post.is_pinned == 1) continue;

                DateTime postDate = DateTimeOffset.FromUnixTimeSeconds((long)post.date).UtcDateTime;
                if (postDate < cutoffDate) continue;

                var dow = postDate.DayOfWeek;
                if (!postsByDay.ContainsKey(dow)) postsByDay[dow] = 0;
                postsByDay[dow]++;

                postsByHour[postDate.Hour]++;

                if (post.attachments != null)
                {
                    foreach (var att in post.attachments)
                    {
                        string type = att.type?.ToString() ?? "unknown";
                        if (!contentTypesCount.ContainsKey(type)) contentTypesCount[type] = 0;
                        contentTypesCount[type]++;
                    }
                }
            }

            // --- Создаём PDF ---
            CreatePdf(path, groupInfo, membersCount, postsByDay, postsByHour, contentTypesCount);

            MessageBox.Show("PDF-отчёт сохранён по адресу: " + path);
        }


        // Добавим метод Truncate
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }



        private void CreatePdf(string path, JToken groupInfo, int membersCount,
                Dictionary<DayOfWeek, int> postsByDay, int[] postsByHour, Dictionary<string, int> contentTypes)
        {
            using var fs = new FileStream(path, FileMode.Create);
            var doc = new Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // Загружаем шрифт Noto Sans (предположим, файл лежит в ./Fonts/NotoSans-Regular.ttf)
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NotoSans.ttf");
            BaseFont notoBaseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var notoFont = new Font(notoBaseFont, 12, Font.NORMAL);
            var notoBold = new Font(notoBaseFont, 14, Font.BOLD);

            // Заголовок
            doc.Add(new Paragraph("Отчёт по активности сообщества ВКонтакте", notoBold) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph($"Название группы: {groupInfo.Value<string>("name")}", notoFont));
            doc.Add(new Paragraph($"ID группы: {groupInfo.Value<long>("id")}", notoFont));
            doc.Add(new Paragraph($"Число участников: {membersCount:N0}", notoFont));
            doc.Add(new Paragraph("Анализ постов за последние 30 дней\n", notoFont));

            // Таблица по дням недели
            doc.Add(new Paragraph("Активность по дням недели:", notoBold));
            var tableDays = new PdfPTable(2);
            tableDays.WidthPercentage = 100;
            tableDays.AddCell(new PdfPCell(new Phrase("День", notoFont)));
            tableDays.AddCell(new PdfPCell(new Phrase("Постов", notoFont)));

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                tableDays.AddCell(new Phrase(day.ToString(), notoFont));
                tableDays.AddCell(new Phrase(postsByDay.ContainsKey(day) ? postsByDay[day].ToString() : "0", notoFont));
            }
            doc.Add(tableDays);
            doc.Add(new Paragraph("\n"));

            // Таблица по часам
            doc.Add(new Paragraph("Активность по часам суток:", notoBold));
            var tableHours = new PdfPTable(2);
            tableHours.WidthPercentage = 100;
            tableHours.AddCell(new PdfPCell(new Phrase("Час", notoFont)));
            tableHours.AddCell(new PdfPCell(new Phrase("Постов", notoFont)));

            for (int i = 0; i < 24; i++)
            {
                tableHours.AddCell(new Phrase($"{i}:00", notoFont));
                tableHours.AddCell(new Phrase(postsByHour[i].ToString(), notoFont));
            }
            doc.Add(tableHours);
            doc.Add(new Paragraph("\n"));

            // Таблица типов контента
            doc.Add(new Paragraph("Типы контента в постах:", notoBold));
            var tableTypes = new PdfPTable(2);
            tableTypes.WidthPercentage = 100;
            tableTypes.AddCell(new PdfPCell(new Phrase("Тип", notoFont)));
            tableTypes.AddCell(new PdfPCell(new Phrase("Количество", notoFont)));

            foreach (var kvp in contentTypes.OrderByDescending(k => k.Value))
            {
                tableTypes.AddCell(new Phrase(kvp.Key, notoFont));
                tableTypes.AddCell(new Phrase(kvp.Value.ToString(), notoFont));
            }
            doc.Add(tableTypes);

            doc.Close();
        }


        // Утилита для добавления ячеек в таблицу
        private void AddCell(PdfPTable table, string text, iTextSharp.text.Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
        }


    }
}


