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
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;


namespace ReportSingleGroupPlugin
{
    public class ReportSingleGroupPlugin : IReportMaker
    {
        public string GetInfo()
        {
            return "Создание отчета-статистики конкретного сообщества. Автор: tyo111a. Версия: 0.111a";
        }

        public string GetGUID()
        {
            return "{24B240FD-F597-465A-B7B8-DEC2041C443F}";
        }

        public string GetGUIinfo()
        {
            return "Посмотреть статистику конкретного сообщества";
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

            if (groupInfo != null)
            {
                // Достаём поля через Value<T>()
                long id = groupInfo.Value<long>("id");
                int membersCount = groupInfo.Value<int>("members_count");
                string name = groupInfo.Value<string>("name");

                // Статистика по постам
                string wallUrl = $"https://api.vk.com/method/wall.get?owner_id=-{id}&count=30&access_token={accessToken}&v=5.131";
                var wallResp = await client.GetStringAsync(wallUrl);
                dynamic wallData = JsonConvert.DeserializeObject(wallResp);

                int totalLikes = 0, totalComments = 0, totalReposts = 0;
                var postStats = new List<(DateTime Date, string Text, int Likes, int Comments, int Reposts)>();
                var postDates = new List<DateTime>();

                foreach (var post in wallData?.response?.items)
                {
                    if (post.is_pinned != null && post.is_pinned == 1) continue;

                    int likes = (int?)post.likes?.count ?? 0;
                    int comments = (int?)post.comments?.count ?? 0;
                    int reposts = (int?)post.reposts?.count ?? 0;
                    DateTime date = DateTimeOffset.FromUnixTimeSeconds((long?)post.date ?? 0).DateTime;

                    totalLikes += likes;
                    totalComments += comments;
                    totalReposts += reposts;
                    postDates.Add(date);

                    postStats.Add((date, Truncate((string)post.text, 100), likes, comments, reposts));
                }

                int postCount = postStats.Count;
                double daysSpan = (postDates.Max() - postDates.Min()).TotalDays;
                if (daysSpan < 1) daysSpan = 1;
                double postsPerWeek = postCount / daysSpan * 7;
                double er = (postCount > 0 && membersCount > 0)
                    ? ((totalLikes + totalComments + totalReposts) / (double)(membersCount * postCount)) * 100
                    : 0;

                // PDF отчёт
                var sb = new StringBuilder();
                sb.AppendLine($"ОТЧЕТ ПО СООБЩЕСТВУ ВК НА {DateTime.Now}");
                sb.AppendLine($"📌 Группа: {name} (ID: {id})");
                sb.AppendLine($"Участников: {membersCount:N0}");
                sb.AppendLine($"Описание: {groupInfo.Value<string>("description") ?? "–"}");
                sb.AppendLine($"Тематика: {groupInfo.Value<string>("activity") ?? "не указана"}");
                sb.AppendLine($"Возрастной рейтинг: {groupInfo.Value<int?>("age_limits") ?? 0}");
                sb.AppendLine($"Сайт: {groupInfo.Value<string>("site") ?? "–"}");
                sb.AppendLine($"Тип: {(groupInfo.Value<int>("is_closed") == 1 ? "Закрытая" : "Открытая")}");
                sb.AppendLine($"\n📊 Последние посты: {postCount}");
                sb.AppendLine($"Постов в неделю: {postsPerWeek:F2}");
                sb.AppendLine($"ER (вовлечённость): {er:F2}%");

                var topPosts = postStats.OrderByDescending(p => p.Likes + p.Comments + p.Reposts).Take(5).ToList();

                CreatePdf(path, sb.ToString(), topPosts.Select(p =>
                    (p.Date.ToShortDateString(), p.Text, p.Likes, p.Comments, p.Reposts)).ToList());

                MessageBox.Show("Отчёт со статистикой сохранен по адресу: " + path);
            }
        }

        // Добавим метод Truncate
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }



        private void CreatePdf(string filePath, string textContent, List<(string Date, string Text, int Likes, int Comments, int Reposts)> posts)
        {
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NotoSans.ttf");

            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new iTextSharp.text.Font(baseFont, 10, iTextSharp.text.Font.NORMAL);
            var boldFont = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.BOLD);

            var document = new Document(PageSize.A4, 40, 40, 40, 40);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var paragraph = new Paragraph(textContent, font);
                document.Add(paragraph);

                document.Add(new Paragraph("\nПодробная статистика по постам:\n", boldFont));

                PdfPTable table = new PdfPTable(5)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 2f, 5f, 1.5f, 1.5f, 1.5f });

                AddCell(table, "Дата", boldFont);
                AddCell(table, "Текст (обрезан)", boldFont);
                AddCell(table, "Лайки", boldFont);
                AddCell(table, "Комментарии", boldFont);
                AddCell(table, "Репосты", boldFont);

                foreach (var post in posts)
                {
                    AddCell(table, post.Date, font);
                    AddCell(table, Truncate(post.Text, 100), font);
                    AddCell(table, post.Likes.ToString(), font);
                    AddCell(table, post.Comments.ToString(), font);
                    AddCell(table, post.Reposts.ToString(), font);
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


