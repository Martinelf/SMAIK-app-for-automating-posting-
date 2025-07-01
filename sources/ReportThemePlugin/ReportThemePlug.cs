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
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace ReportThemePlugin
{
    public class ReportByThemePlugin : IReportMaker
    {
        public string GetInfo()
        {
            return "Создание отчета-статистики сообществ по заданному ключевому слову. Автор: Анализатор. Версия: 9.111a";
        }

        public string GetGUID()
        {
            return "{F69A0361-76F2-4751-ACAC-003B2D179E15}";
        }

        public string GetGUIinfo()
        {
            return "Создать статистику по ключевому слову в названии сообщества";
        }

        public string GetPluginType()
        {
            return "ReportMaker";
        }


        public string SetSettings()
        {
            using (Form form = new Form())
            {
                form.Text = "Настройки отчета";
                form.FormBorderStyle = FormBorderStyle.FixedDialog; // Фиксированное окно
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ClientSize = new Size(420, 130); // Достаточный размер
                form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                Label topicLabel = new Label { Text = "Тема:", Left = 10, Top = 13, Width = 40 };
                TextBox topicBox = new TextBox { Left = 60, Top = 10, Width = 340 };

                Label pathLabel = new Label { Text = "Путь:", Left = 10, Top = 43, Width = 40 };
                TextBox pathBox = new TextBox { Left = 60, Top = 40, Width = 260 };
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
                        MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                form.Controls.Add(topicLabel);
                form.Controls.Add(topicBox);
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
            string theme = parts[0];
            string path = parts[1];
            string accessToken = "vk1.a.sgdK2fQvXQMv2jU8PdT7hCqEgpUonC-BLjAoKnZE79lLoWCD61SETE6PPVZ9F1sVjMV9qya5fFPnSELI8NalcZ4b5XXtSW09NY1raHHBXxblcNwuXr7t2Ky0ENcS57WMg9yH6uj475_MUfDAnSofMr6C4xyCRFvROS53-PQoq064AKdhJyUQ5saLCP7wPQ4wzTlndbR_ug9GrpA1HMeHgw";

            using HttpClient client = new HttpClient();

            var allGroups = new List<dynamic>();
            for (int offset = 0; offset < 200; offset += 100)
            {
                string searchUrl = $"https://api.vk.com/method/groups.search?q={Uri.EscapeDataString(theme)}&count=100&offset={offset}&access_token={accessToken}&v=5.131";
                var response = await client.GetStringAsync(searchUrl);
                dynamic result = JsonConvert.DeserializeObject(response);
                if (result?.response?.items != null)
                    allGroups.AddRange(result.response.items);
                await Task.Delay(350);
            }

            var groupIds = allGroups.Select(g => g.id.ToString()).Take(100).ToList(); // Ограничим
            var idChunks = ChunkList(groupIds, 500);
            var detailedGroups = new List<dynamic>();

            foreach (var chunk in idChunks)
            {
                string ids = string.Join(",", chunk);
                string infoUrl = $"https://api.vk.com/method/groups.getById?group_ids={ids}&fields=members_count,activity,age_limits,is_closed,site,description&access_token={accessToken}&v=5.131";
                var response = await client.GetStringAsync(infoUrl);
                dynamic result = JsonConvert.DeserializeObject(response);
                if (result?.response != null)
                    detailedGroups.AddRange(result.response);
                await Task.Delay(350);
            }

            // Метрики
            var averageMembers = detailedGroups.Average(g => (int?)g.members_count ?? 0);
            var activityStats = detailedGroups.GroupBy(g => (string)g.activity ?? "Не указано")
                .ToDictionary(g => g.Key, g => g.Count());
            var ageLimits = detailedGroups.GroupBy(g => (int?)g.age_limits ?? 0)
                .ToDictionary(g => g.Key, g => g.Count());
            var closedStats = detailedGroups.GroupBy(g => (int?)g.is_closed ?? 0)
                .ToDictionary(g => g.Key == 1 ? "Закрытые" : "Открытые", g => g.Count());
            var withLinks = detailedGroups.Count(g => !string.IsNullOrEmpty((string?)g.site));
            var hasYoutube = detailedGroups.Count(g => ((string?)g.site)?.Contains("youtube") == true ||
                                                        ((string?)g.description)?.ToLower().Contains("youtube") == true);

            // 📊 Анализ активности постов:
            var postStats = new List<(string Name, int Members, double ER, double PostsPerWeek)>();

            foreach (var group in detailedGroups.Take(20))
            {
                string screenName = (string)group.screen_name ?? $"club{group.id}";
                int members = (int?)group.members_count ?? 1;

                string wallUrl = $"https://api.vk.com/method/wall.get?owner_id=-{group.id}&count=10&access_token={accessToken}&v=5.131";
                var response = await client.GetStringAsync(wallUrl);
                dynamic wallData = JsonConvert.DeserializeObject(response);

                int totalLikes = 0, totalComments = 0, totalReposts = 0;
                var postDates = new List<DateTime>();

                if (wallData?.response?.items != null)
                {
                    var items = ((IEnumerable<dynamic>)wallData.response.items).ToList().Where(p => p.is_pinned == null || p.is_pinned == 0) // ❗ фильтруем закрепы
                    .ToList(); ;

                    foreach (var post in items)
                    {
                        totalLikes += (int?)post.likes?.count ?? 0;
                        totalComments += (int?)post.comments?.count ?? 0;
                        totalReposts += (int?)post.reposts?.count ?? 0;
                        var postDate = DateTimeOffset.FromUnixTimeSeconds((long?)post.date ?? 0).DateTime;
                        postDates.Add(postDate);
                    }

                    int postCount = items.Count;
                    double daysSpan = postCount > 1
                    ? (postDates.Max() - postDates.Min()).TotalDays
                    : 1;

                    if (daysSpan < 1) daysSpan = 1;
                    double postsPerWeek = (postCount / daysSpan) * 7;

                    double er = (postCount > 0)
                        ? ((totalLikes + totalComments + totalReposts) / (double)(members * postCount)) * 100
                        : 0;

                    postStats.Add((screenName, members, Math.Round(er, 3), Math.Round(postsPerWeek, 2)));
                }

                await Task.Delay(400);
            }


            // Формируем отчёт
            var sb = new StringBuilder();
            sb.AppendLine($"📌 Ключевое слов (фраза): {theme}");
            sb.AppendLine($"Выборка, кол-во сообществ: {detailedGroups.Count}");
            sb.AppendLine($"Среднее число участников: {averageMembers:N0}\n");

            sb.AppendLine("📊 Распределение сообществ по тематикам:");
            foreach (var kvp in activityStats.OrderByDescending(x => x.Value))
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");

            // Словарь для расшифровки возрастных ограничений
            var ageLimitLabels = new Dictionary<int, string>
            {
                [0] = "Без ограничений",
                [1] = "До 16 лет",
                [2] = "16+",
                [3] = "18+"
            };

            sb.AppendLine("\n🔞 Возрастные ограничения:");
            foreach (var kvp in ageLimits.OrderBy(x => x.Key))
            {
                string label = ageLimitLabels.ContainsKey(kvp.Key) ? ageLimitLabels[kvp.Key] : $"Неизвестно ({kvp.Key})";
                sb.AppendLine($"{label}: {kvp.Value}");
            }

            sb.AppendLine("\n🔐 Типы сообществ:");
            foreach (var kvp in closedStats)
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");

            sb.AppendLine($"\n🌐 С внешними сайтами: {withLinks}");
            sb.AppendLine($"▶️ Упоминание YouTube: {hasYoutube}");

            CreatePdf(path, sb.ToString(), postStats);
            MessageBox.Show("Отчёт со статистикой сохранен по адресу: " + path);
        }

        // Метод создания PDF из строки
        private void CreatePdf(string filePath, string textContent, List<(string Name, int Members, double ER, double PostsPerWeek)> postStats)
        {
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NotoSans.ttf");

            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
            var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);

            var document = new Document(PageSize.A4, 40, 40, 40, 40);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Основной текст
                var paragraph = new Paragraph(textContent, font);
                document.Add(paragraph);

                document.Add(new Paragraph("\nТОП-20 сообществ по вовлечённости:\n", boldFont));

                // Таблица: 4 колонки
                PdfPTable table = new PdfPTable(4)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 3f, 2f, 2f, 2f }); // Пропорции ширины столбцов

                // Заголовки
                AddCell(table, "Название", boldFont);
                AddCell(table, "Участников", boldFont);
                AddCell(table, "Постов/нед", boldFont);
                AddCell(table, "ER (%)", boldFont);

                foreach (var stat in postStats.OrderByDescending(p => p.ER))
                {
                    AddCell(table, stat.Name, font);
                    AddCell(table, stat.Members.ToString("N0"), font);
                    AddCell(table, stat.PostsPerWeek.ToString("F2"), font);
                    AddCell(table, stat.ER.ToString("F2"), font);
                }

                document.Add(table);
                document.Close();
            }
        }

        // Утилита для добавления ячеек в таблицу
        private void AddCell(PdfPTable table, string text, iTextSharp.text.Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
        }


    }
}


