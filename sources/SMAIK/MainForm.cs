using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using IniParser.Model;
using IniParser;

using PluginBase;
using System.Diagnostics.Eventing.Reader;


namespace SMAIK
{
    public partial class MainForm : Form
    {

        public static MainForm Instance;
        private readonly DBconnector db = new DBconnector();

        //если данная задача ещё не назначена - запустить taskSсheduler Windows поручить ему каждую минуту запускать postSheduler.exe (лежит в одной папке с приложением)

        private void EnsureTaskSchedulerJob()
                {
                    string taskName = "PostSchedulerTask";
                    string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "postSheduler.exe");

                    if (!File.Exists(exePath))
                    {
                        MessageBox.Show($"Файл не найден:\n{exePath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (TaskService ts = new TaskService())
                    {
                        var existingTask = ts.FindTask(taskName, true);
                        if (existingTask != null)
                            return; // Задача уже существует — ничего не делаем

                        TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Runs postSheduler.exe every minute to post scheduled Telegram messages.";
                
                        // Настройка задачи
                        td.Settings.DisallowStartIfOnBatteries = false;
                        td.Settings.StopIfGoingOnBatteries = false;
                        td.Settings.RunOnlyIfIdle = false;
                        td.Settings.RunOnlyIfNetworkAvailable = false;
                        td.Settings.AllowHardTerminate = false;
                        td.Settings.WakeToRun = true; // по желанию
                        //td.Principal.RunLevel = TaskRunLevel.Highest;
                        td.Settings.Hidden = true;

                        // Триггер: каждую минуту
                        td.Triggers.Add(new TimeTrigger
                        {
                            StartBoundary = DateTime.Now,
                            Repetition = new RepetitionPattern(TimeSpan.FromMinutes(1), TimeSpan.FromDays(365)),
                            Enabled = true
                        });

                        // Скрытый запуск через cmd
                        //td.Actions.Add(new ExecAction("cmd.exe", $"/C start /B \"\" \"{exePath}\"", Path.GetDirectoryName(exePath)));



                        // Действие: запуск postSheduler.exe
                        td.Actions.Add(new ExecAction(exePath, null, Path.GetDirectoryName(exePath)));

                        // Задание будет работать от имени текущего пользователя
                        ts.RootFolder.RegisterTaskDefinition(taskName, td);
                    }
                }


        public MainForm()
        {
            InitializeComponent();
            CommunityCache.LoadCommunities();
            PublicationCache.month = DateTime.Now.Month;
            PublicationCache.LoadPublications();
            MonthCache.Load(DateTime.Now);
            Instance = this;

            EnsureTaskSchedulerJob();
            RefreshListBoxes();

            //CheckLicenseAndUpdateUI();

           

            PluginManager pm = new PluginManager();
            pm.LoadPluginsFromIni();
        }


        #region работа с файлом конфигурации (.ini)
        public class PluginManager
        {
            private readonly string configPath = "config.ini";
            private readonly FileIniDataParser parser = new FileIniDataParser();

            public void LoadPluginsFromIni()
            {
                if (!File.Exists(configPath))
                    return;

                IniData data = parser.ReadFile(configPath);

                if (data.Sections.ContainsSection("Plugins"))
                {
                    MainForm.plugins.Clear();
                    MainForm.pluginPaths.Clear();
                    MainForm.guids.Clear();

                    foreach (var key in data["Plugins"])
                    {
                        string dllPath = key.Value;

                        if (!File.Exists(dllPath))
                            continue;

                        try
                        {
                            Assembly pluginAssembly = Assembly.LoadFrom(dllPath);
                            var pluginTypes = pluginAssembly.GetTypes()
                                .Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                         && !t.IsInterface && !t.IsAbstract);

                            foreach (var type in pluginTypes)
                            {
                                if (Activator.CreateInstance(type) is IPlugin plugin)
                                {
                                    string guid = plugin.GetGUID();

                                    if (!MainForm.guids.Contains(guid))
                                    {
                                        MainForm.plugins.Add(plugin);
                                        MainForm.pluginPaths.Add(dllPath);
                                        MainForm.guids.Add(guid);


                                        if (Activator.CreateInstance(type) is IReportMaker reportPlugin)
                                        {
                                            MainForm.Instance.AddPluginReportMakerButton(reportPlugin);
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки плагина {dllPath}:\n{ex.Message}", "Ошибка");
                        }
                    }
                }
                MainForm.Instance.updatePluginListGUI();
            }


            public void WritePluginsToIni()
            {
                IniData data = File.Exists(configPath)
                    ? parser.ReadFile(configPath)
                    : new IniData();

                data.Sections.RemoveSection("Plugins");
                data.Sections.AddSection("Plugins");

                for (int i = 0; i < MainForm.pluginPaths.Count; i++)
                {
                    string key = $"plugin{i + 1}";
                    string path = MainForm.pluginPaths[i];
                    data["Plugins"][key] = path;
                }

                parser.WriteFile(configPath, data);
            }

        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PluginManager pm = new PluginManager();
            pm.WritePluginsToIni();
        }



        #endregion


        private void MainForm_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;

            UpdateMonth(month);
            UpdateDayPlan(DateTime.Now.Day);
        }

        #region CacheStructuries - communities, publications

        public class Community
        {
            public string GroupId { get; set; }       // groupID из таблицы groups
            public string GroupName { get; set; }     // groupName из таблицы groups
            public string Token { get; set; }         // token из таблицы tokens
            public string SocialNetwork { get; set; } // snName из таблицы socialnetworks
            public bool needTaskScheduler { get; set; } // нужен ли свой планировщик задач или API соцсети его включает

            //форма записи для представления в списках всех групп
            public override string ToString()
            {
                return $"{GroupName} ({GroupId})";
            }
        }

        public static class CommunityCache
        {
            private static Dictionary<string, Community> _cache;

            public static void LoadCommunities()
            {
                _cache = new Dictionary<string, Community>();

                using (var db = new DBconnector())
                {
                    var sql = @"
                        SELECT 
                            g.groupID, 
                            g.groupName, 
                            t.token, 
                            s.snName
                        FROM groups g
                        JOIN tokens t ON g.tokenID = t.tokenID
                        JOIN socialnetworks s ON t.snID = s.snID";

                    var table = db.ExecuteQuery(sql);

                    foreach (DataRow row in table.Rows)
                    {
                        var community = new Community
                        {
                            GroupId = row["groupID"].ToString(),
                            GroupName = row["groupName"].ToString(),
                            Token = row["token"].ToString(),
                            SocialNetwork = row["snName"].ToString(),
                            needTaskScheduler = false
                        };

                        //соцсети которым нужен отдельный планировщик публикаций вне апи
                        if (community.SocialNetwork == "tg" || community.SocialNetwork == "ok")
                        {
                            community.needTaskScheduler = true;
                        }

                        _cache[community.GroupId] = community;
                    }
                }
            }

            public static Community GetCommunity(string groupId)
            {
                if (_cache == null)
                    LoadCommunities();

                return _cache.TryGetValue(groupId, out var community) ? community : null;
            }

            public static IEnumerable<Community> GetAll()
            {
                if (_cache == null)
                    LoadCommunities();

                return _cache.Values;
            }

            public static void Refresh()
            {
                LoadCommunities();
            }

            public static IEnumerable<Community> GetBySocialNetwork(string socialNetworkName)
            {
                if (_cache == null)
                    LoadCommunities();

                return _cache.Values
                    .Where(c => c.SocialNetwork.Equals(socialNetworkName, StringComparison.OrdinalIgnoreCase));
            }

        }

        public void LoadTgGroups()
        {
            listBoxTgGr.Items.Clear();

            var tgCommunities = CommunityCache.GetBySocialNetwork("tg");

            foreach (var community in tgCommunities)
            {
                listBoxTgGr.Items.Add(community); // добавляем объект, не строку!
            }
        }

        public void LoadVkGroups()
        {
            listBoxVkGr.Items.Clear();

            var vkCommunities = CommunityCache.GetBySocialNetwork("vk");

            foreach (var community in vkCommunities)
            {
                listBoxVkGr.Items.Add(community); // добавляем объект, не строку!
            }
        }


        public void RefreshListBoxes()
        {
            LoadTgGroups();
            LoadVkGroups();
        }

        // структура для публикаций
        public class Publication
        {
            public int PublID { get; set; }
            public DateTime TimePubl { get; set; }
            public string TextPubl { get; set; }
            public string GroupID { get; set; }
            public bool IfPosted { get; set; }
            public string SocialNetwork { get; set; }
            public string idInGroup { get; set; }
            public string ImagePath { get; set; }  // 🔽 Новое свойство

            public override string ToString()
            {
                return $"[{TimePubl}: {TextPubl[..Math.Min(30, TextPubl.Length)]}...";
            }
        }


        public static class PublicationCache
        {
            private static Dictionary<int, Publication> _cache;

            public static int month { get; set; }

            public static Publication RefreshPublication(int publID)
            {
                using (var db = new DBconnector())
                {
                    var sql = @"
                SELECT 
                    p.publID, 
                    p.timePubl, 
                    p.textPubl, 
                    p.groupID, 
                    p.ifPosted,
                    p.idInGroup,
                    p.imagePath,
                    s.snName
                FROM publications p
                JOIN groups g ON p.groupID = g.groupID
                JOIN tokens t ON g.tokenID = t.tokenID
                JOIN socialnetworks s ON t.snID = s.snID
                WHERE p.publID = @publID";

                    var parameters = new[] {
                new SQLiteParameter("@publID", publID)
            };

                    var table = db.ExecuteQuery(sql, parameters);
                    if (table.Rows.Count > 0)
                    {
                        var pub = CreatePublicationFromRow(table.Rows[0]);
                        _cache[pub.PublID] = pub;
                        return pub;
                    }

                    return null;
                }
            }
            private static Publication CreatePublicationFromRow(DataRow row)
            {
                return new Publication
                {
                    PublID = Convert.ToInt32(row["publID"]),
                    TimePubl = DateTime.Parse(row["timePubl"].ToString()),
                    TextPubl = row["textPubl"].ToString(),
                    GroupID = row["groupID"].ToString(),
                    IfPosted = Convert.ToInt32(row["ifPosted"]) == 1,
                    SocialNetwork = row["snName"].ToString(),
                    idInGroup = row["idInGroup"].ToString(),
                    ImagePath = row["imagePath"]?.ToString()
                };
            }

            public static void LoadPublications()
            {
                _cache = new Dictionary<int, Publication>();

                using (var db = new DBconnector())
                {
                    var sql = @"
                        SELECT 
                            p.publID, 
                            p.timePubl, 
                            p.textPubl, 
                            p.groupID, 
                            p.ifPosted,
                            p.idInGroup,
                            p.imagePath,            -- 🔽 Добавлено
                            s.snName
                        FROM publications p
                        JOIN groups g ON p.groupID = g.groupID
                        JOIN tokens t ON g.tokenID = t.tokenID
                        JOIN socialnetworks s ON t.snID = s.snID
                        WHERE strftime('%m', p.timePubl) = @month;";

                    var parameters = new[] {
                        new SQLiteParameter("@month", month.ToString("D2"))
                    };

                    var table = db.ExecuteQuery(sql, parameters);

                    foreach (DataRow row in table.Rows)
                    {
                        var pub = new Publication
                        {
                            PublID = Convert.ToInt32(row["publID"]),
                            TimePubl = DateTime.Parse(row["timePubl"].ToString()),
                            TextPubl = row["textPubl"].ToString(),
                            GroupID = row["groupID"].ToString(),
                            IfPosted = Convert.ToInt32(row["ifPosted"]) == 1,
                            SocialNetwork = row["snName"].ToString(),
                            idInGroup = row["idInGroup"].ToString(),
                            ImagePath = row["imagePath"]?.ToString() // 🔽 Поддержка NULL
                        };

                        _cache[pub.PublID] = pub;
                    }
                }
            }


            public static IEnumerable<Publication> GetByDate(DateTime date)
            {
                if (_cache == null)
                    return Enumerable.Empty<Publication>();

                return _cache.Values.Where(pub =>
                    pub.TimePubl.Date == date.Date);
            }

            public static Publication GetPublication(int publId)
            {
                if (_cache == null)
                    LoadPublications();

                return _cache.TryGetValue(publId, out var pub) ? pub : null;
            }

            public static IEnumerable<Publication> GetAll()
            {
                if (_cache == null)
                    LoadPublications();

                return _cache.Values;
            }

            public static void Refresh()
            {
                LoadPublications();
            }

            public static IEnumerable<Publication> GetByGroupId(string groupId)
            {
                if (_cache == null)
                    LoadPublications();

                return _cache.Values
                    .Where(p => p.GroupID == groupId);
            }

            public static IEnumerable<Publication> GetUnposted()
            {
                if (_cache == null)
                    LoadPublications();

                return _cache.Values
                    .Where(p => !p.IfPosted);
            }

            public static IEnumerable<Publication> GetScheduledBefore(DateTime time)
            {
                if (_cache == null)
                    LoadPublications();

                return _cache.Values
                    .Where(p => p.TimePubl <= time && !p.IfPosted);
            }
        }

        public static class MonthCache
        {
            //ТУДУ переделать на на обращение к бд а на нужную комплектацию PublicationCache
            
            // Ключ — дата (только день), значение — строка вида "N постов ТГ, M постов ВК"
            private static Dictionary<DateTime, string> _cache;

            public static void Load(DateTime month)
            {
                _cache = new Dictionary<DateTime, string>();

                using (var db = new DBconnector())
                {
                    // Начало и конец месяца
                    var firstDay = new DateTime(month.Year, month.Month, 1);
                    var lastDay = firstDay.AddMonths(1).AddDays(-1);

                    var sql = @"
                        SELECT 
                            DATE(p.timePubl) as pubDate,
                            s.snName,
                            COUNT(*) as postCount
                        FROM publications p
                        JOIN groups g ON p.groupID = g.groupID
                        JOIN tokens t ON g.tokenID = t.tokenID
                        JOIN socialnetworks s ON t.snID = s.snID
                        WHERE DATE(p.timePubl) BETWEEN @start AND @end
                        GROUP BY pubDate, s.snName";

                    var parameters = new[]
                    {
                        new SQLiteParameter("@start", firstDay.ToString("yyyy-MM-dd")),
                        new SQLiteParameter("@end", lastDay.ToString("yyyy-MM-dd"))
                    };

                    var table = db.ExecuteQuery(sql, parameters);

                    // группируем и формируем строки
                    var temp = new Dictionary<DateTime, List<string>>();

                    foreach (DataRow row in table.Rows)
                    {
                        var date = DateTime.Parse(row["pubDate"].ToString());
                        var sn = row["snName"].ToString();
                        var count = Convert.ToInt32(row["postCount"]);

                        if (!temp.ContainsKey(date))
                            temp[date] = new List<string>();

                        // Пример: "2 поста ВК" или "1 пост ТГ"
                        string label = $"{count} {FormatSN(sn)}\n";
                        temp[date].Add(label);
                    }

                    // формируем финальные строки
                    foreach (var kvp in temp)
                    {
                        _cache[kvp.Key] = string.Join("", kvp.Value);
                    }
                }
            }

            public static string GetInfo(DateTime date)
            {
                if (_cache == null)
                    Load(date);

                return _cache.TryGetValue(date.Date, out var result) ? result : "";
            }

            public static void Refresh(DateTime month)
            {
                Load(month);
            }

            private static string FormatSN(string snCode)
            {
                return snCode switch
                {
                    "vk" => "ВК",
                    "tg" => "ТГ",
                    "ok" => "ОК",
                    "inst" => "Instagram",
                    _ => snCode
                };
            }
        }


        #endregion

        #region Calendar
        public int month, year;
        public int selectedDay;
        string monthname;
        public void UpdateMonth(int month)
        {
            PublicationCache.month = month;
            PublicationCache.Refresh();
            DateTime dtMonth = new DateTime(year, month, 1);
            MonthCache.Refresh(dtMonth);

            daycontainer.Controls.Clear();

            monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            LBDATE.Text = monthname + " " + year;

            DateTime startofthemont = new DateTime(year, month, 1);

            int days = DateTime.DaysInMonth(year, month);

            int dayoftheweek = Convert.ToInt32(startofthemont.DayOfWeek.ToString("d"));
            if (dayoftheweek == 0) dayoftheweek = 7;

            for (int i = 1; i < dayoftheweek; i++)
            {
                UserControlBlank ucblanck = new UserControlBlank();
                daycontainer.Controls.Add(ucblanck);
            }

            DateTime dateToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            for (int i = 1; i <= days; i++)
            {
                UserControlDays ucdays = new UserControlDays();

                DateTime date = new DateTime(year, month, i);

                string info = MonthCache.GetInfo(date); // например: "2 поста ВК, 1 пост ТГ"
                if (date == dateToday) ucdays.BorderStyle = BorderStyle.FixedSingle;

                ucdays.days(i, info);

                daycontainer.Controls.Add(ucdays);



                // подписывается на событие обновления дня при клике на день
                ucdays.OnDayPlanUpdateNeeded = UpdateDayPlan;
                // подписка на даблклик чтобы тогда открывать окно добавления поста
                ucdays.OnAddPublicationNeeded = AddPostViaDoubleClick;
            }
        }

        private void btnprevious_Click(object sender, EventArgs e)
        {
            if (month == 1)
            {
                year--;
                month = 12;
            }
            else month--;

            UpdateMonth(month);
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            if (month == 12)
            {
                year++;
                month = 1;
            }
            else month++;

            UpdateMonth(month);
        }

        public void UpdateDayPlan(int day)
        {
            selectedDay = day;
            //gui: выделяю нужный элемент в календарике
            foreach (Control control in daycontainer.Controls)
            {
                if (control is UserControlDays ucdays)
                {
                    if (ucdays.nday == day)
                    {
                        ucdays.BackColor = ColorTranslator.FromHtml("#aec6cf");
                    }
                    else
                    {
                        ucdays.BackColor = Color.White;
                    }
                }
            }

            // бубликации какого дня смотрим
            LBDAY.Text = day.ToString() + " " + monthname + " " + year.ToString();

            //в прошлое нельзя добавлять публикации
            DateTime selectedDate = new DateTime(year, month, day);
            if (selectedDate < DateTime.Today) buttonAddPost.Enabled = false;
            else buttonAddPost.Enabled = true;

            //из PublicationCache публикации которые соответствуют выбранному дню
            //отображаем публикации в виде лейблов в SplitContainer1.Panel2 сверху вниз в хронологическом порядке
            // 1. Очищаем старые лейблы
            planningPanel.Controls.Clear();

            // 3. Получаем публикации на этот день
            var pubsToday = PublicationCache.GetByDate(selectedDate);

            // 4. Сортируем по времени публикации
            var sorted = pubsToday.OrderBy(p => p.TimePubl).ToList();

            // 5. Добавляем публикации в интерфейс
            int top = 10;
            foreach (var pub in sorted)
            {
                Label lbl = new Label();
                lbl.AutoSize = true;

                string cleanText = pub.TextPubl
                    .Replace("\r", "")
                    .Replace("\n", " "); // Заменяем на пробел, чтобы слова не слипались

                string fullText = $"{pub.TimePubl:HH:mm} [{pub.SocialNetwork}] - {pub.GroupID} - {cleanText}";

                lbl.Text = TruncateWithEllipsis(fullText, 40);

                lbl.Top = top;
                lbl.Left = 10;

                lbl.Tag = pub; // Сохраняем публикацию в Tag
                
                lbl.DoubleClick += LabelPubl_DoubleClick;

                planningPanel.Controls.Add(lbl);
                top += lbl.Height + 5;
            }
        }

        private string TruncateWithEllipsis(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - 3) + "...";
        }

        private void LabelPubl_DoubleClick(object sender, EventArgs e)
        {
            /*
            if (LicenseManager.disablEditPublication)
            {
                MessageBox.Show("У вас недостаточно прав на редактирование публикации");
                return;
            }
            */

            if (sender is Label lbl && lbl.Tag is Publication pub)
            {
                // 🔄 Обновляем публикацию из БД и кэш
                var refreshedPub = PublicationCache.RefreshPublication(pub.PublID);
                if (refreshedPub == null)
                {
                    MessageBox.Show("Публикация не найдена в базе данных");
                    return;
                }

                lbl.Font = new Font(lbl.Font, FontStyle.Bold);

                using (var dlg = new EditPostDialogForm(refreshedPub))
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        lbl.Text = $"{refreshedPub.TimePubl:HH:mm} [{refreshedPub.SocialNetwork}] {refreshedPub.TextPubl}";
                        UpdateMonth(PublicationCache.month);
                        UpdateDayPlan(selectedDay);
                    }

                    lbl.Font = new Font(lbl.Font, FontStyle.Regular);
                }
            }
        }

        #endregion

        #region GroupEditing

        private void addGroupForm(string socialNetwork)
        {
            AddGroupDialogForm dialog = new AddGroupDialogForm(socialNetwork);
            dialog.ShowDialog(); // Окно блокирует родительскую форму
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(); // Модальное окно
        }

        private void buttonAddGroupVK_Click(object sender, EventArgs e)
        {
            addGroupForm("vk");
        }

        private void buttonAddGroupTG_Click(object sender, EventArgs e)
        {
            addGroupForm("tg");
        }

        private void buttonAddGroupOK_Click(object sender, EventArgs e)
        {
            addGroupForm("ok");
        }

        private void editGroupForm(string socialNetwork, string token, string id)
        {
            AddGroupDialogForm dialog = new AddGroupDialogForm(socialNetwork, token, id);
            dialog.ShowDialog();
        }

        private void editGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (LicenseManager.disableEditGr)
            {
                MessageBox.Show("У вас недостаточно прав для редактирования группы");
                return;
            }
            */

            Community selectedItem = null;
            if (listBoxTgGr.SelectedItem != null)
            {
                selectedItem = (Community)listBoxTgGr.SelectedItem;
            }
            else if (listBoxVkGr.SelectedItem != null)
            {
                selectedItem = (Community)listBoxVkGr.SelectedItem;
            }
            
            
            if (selectedItem != null)
            {
                editGroupForm(selectedItem.SocialNetwork, selectedItem.Token, selectedItem.GroupId);
            }
            
        }

        private void forgetGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (LicenseManager.disableEditGr)
            {
                MessageBox.Show("У вас недостаточно прав для удаления группы");
                return;
            }
            */

            // добавить месседж бокс с текстом типо "Вы уверены что хотите забыть эту группу в программе? Отменить это действие будет невозможно"

            Community selectedItem = new Community();
            if (listBoxTgGr.SelectedItem != null)
            {
                selectedItem = (Community)listBoxTgGr.SelectedItem;
            }
            else if (listBoxVkGr.SelectedItem != null)
            {
                selectedItem = (Community)listBoxVkGr.SelectedItem;
            }
            

            // Запрос подтверждения
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить группу '{selectedItem.GroupName}'?\nЭто действие нельзя отменить.",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;


            deleteGroupFromDB(selectedItem);

            CommunityCache.Refresh();
            RefreshListBoxes();

        }

        public void deleteGroupFromDB(Community group)
        {
            //clear all publications 
            string queryDelPublications = "DELETE FROM publications WHERE groupID = @groupID;";
            var parameter = new SQLiteParameter[] {new SQLiteParameter("@groupID", group.GroupId)};
            var res1 = db.ExecuteNonQuery(queryDelPublications, parameter);

            //clear from table groups 
            string queryDelGroup = "DELETE FROM groups WHERE groupID = @groupID AND groupName = @groupName AND tokenID = (SELECT tokenID FROM tokens WHERE token = @token LIMIT 1);";
            var parameters = new SQLiteParameter[] {
                new SQLiteParameter("@groupID", group.GroupId),
                new SQLiteParameter("@groupName", group.GroupName),
                new SQLiteParameter("@token", group.Token)
            };
            var res2 = db.ExecuteNonQuery(queryDelGroup, parameters);

            //clear token from "tokens" if there is no his usage in table "groups"
            string queryCleanTokens = @"DELETE FROM tokens 
                                        WHERE tokenID = (SELECT tokenID FROM tokens WHERE token = @token LIMIT 1)
                                        AND NOT EXISTS (
                                        SELECT 1 FROM groups WHERE tokenID = tokens.tokenID);";
            var tokenParameters = new SQLiteParameter[] {new SQLiteParameter("@token", group.Token) };
            db.ExecuteNonQuery(queryCleanTokens, tokenParameters);
        }

        private void listBoxVkGr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxVkGr.SelectedIndex != -1)
            {
                listBoxTgGr.ClearSelected();
            }
        }

        private void listBoxTgGr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTgGr.SelectedIndex != -1)
            {
                listBoxVkGr.ClearSelected();
            }
        }

        #endregion

        #region licence 


        private async void CheckLicenseAndUpdateUI()
        {

            await System.Threading.Tasks.Task.Run(() =>
            {
                LicenseManager.CheckForLicense();
            });



            // Возврат в UI-поток происходит после await — здесь Handle уже готов
            SetupUIForVersion();

            string versionInfo = LicenseManager.IsFullVersion
                ? $"Администратор (Пользователь: {LicenseManager.CurrentUserName})"
                : $"Контент-мейкер (Пользователь: {LicenseManager.CurrentUserName})";
            this.Text = $"SMAIK - {versionInfo}";

            if (LicenseManager.IsFullVersion)
            {
                if (DateTime.Now > LicenseManager.ExpirationDate)
                {
                    MessageBox.Show("Срок действия лицензии истек. Программа переключена в демо-режим. Теперь вы не сможете планировать больше 1 публикации в день");
                    SetupUIForVersion();
                }
            }
        }

        //лицензия
        /*
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                switch ((int)m.WParam)
                {
                    case DBT_DEVICEARRIVAL:
                        CheckLicenseAndUpdateUI();
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        CheckLicenseAndUpdateUI();
                        break;
                }
            }
            base.WndProc(ref m);
        }
        */

        private void SetupUIForVersion()
        {
            try
            {
                bool isFullVersion = LicenseManager.IsFullVersion;
                
                buttonAddGroupVK.Enabled = !LicenseManager.disableEditGr;
                buttonAddGroupTG.Enabled = !LicenseManager.disableEditGr;
                buttonAddPlugin.Enabled = !LicenseManager.disablePlugin;
                buttonAddPlugin.Enabled = !LicenseManager.disablePlugin;
                

                string versionInfo = LicenseManager.IsFullVersion
                ? $"Администратор"
                : "Контент-мейкер";
                this.Text = $"SMAIK - {versionInfo}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region plugins
        public static List<IPlugin> plugins = new List<IPlugin>();

        public static List<string> pluginPaths = new List<string>(); // путь к DLL

        public static List<string> guids = new List<string>();

        public void updatePluginListGUI()
        {
            foreach (IPlugin plugin in plugins)
            {
                if (plugin != null)
                {
                    AddPluginToListBox(plugin.GetInfo());
                }
            }
        }
        public void AddPluginReportMakerButton(IReportMaker plugin)
        {
            Button myButton = new Button();

            myButton.Text = plugin.GetGUIinfo();
            myButton.Dock = DockStyle.Top;
            myButton.AutoSize = false;
            //myButton.Height = (pluginsPanel.Size.Height);

            myButton.Click += (s, e) => {
                // TODO поменять
                try
                {
                    string settingsString = plugin.SetSettings();
                    plugin.MakeReport(settingsString);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при работе плагина: " + ex.Message);
                }


            };

            pluginsPanel.Controls.Add(myButton);
        }

        private void buttonAddPlugin_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Plugin DLL (*.dll)|*.dll",
                Title = "Выберите DLL с плагином"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string dllPath = ofd.FileName;

                try
                {
                    Assembly pluginAssembly = Assembly.LoadFrom(dllPath);

                    var pluginTypes = pluginAssembly.GetTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                 && !t.IsInterface
                                 && !t.IsAbstract);

                    foreach (var type in pluginTypes)
                    {
                        if (Activator.CreateInstance(type) is IReportMaker plugin)
                        {
                            string guid = plugin.GetGUID();

                            if (guids.Contains(guid))
                            {
                                MessageBox.Show("Этот плагин уже подключен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            else
                            {
                                plugins.Add(plugin);
                                guids.Add(plugin.GetGUID());
                                pluginPaths.Add(dllPath); // <--- сохраняем путь к DLL

                                AddPluginToListBox(plugin.GetInfo());
                                AddPluginReportMakerButton(plugin);

                            }
                        }
                        else if (Activator.CreateInstance(type) is ITextEditor pluginContext)
                        {
                            string guid = pluginContext.GetGUID();

                            if (guids.Contains(guid))
                            {
                                MessageBox.Show("Этот плагин уже подключен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            else
                            {
                                plugins.Add(pluginContext);
                                guids.Add(pluginContext.GetGUID());
                                pluginPaths.Add(dllPath); // <--- сохраняем путь к DLL

                                AddPluginToListBox(pluginContext.GetInfo());
                                

                            }
                        }

                        //if type == другому то обработка другая
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    string errors = string.Join("\n", ex.LoaderExceptions.Select(error => error.Message));
                    MessageBox.Show($"Ошибка при загрузке типов из сборки:\n{errors}", "Ошибка");
                }
            }
        }

        private void ListBoxPlugins_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = listBoxPlugins.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBoxPlugins.SelectedIndex = index;

                    string selectedItemText = listBoxPlugins.Items[index].ToString();

                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem removeItem = new ToolStripMenuItem("Забыть плагин");

                    removeItem.Click += (s, ev) =>
                    {
                        int currentIndex = listBoxPlugins.Items.IndexOf(selectedItemText);
                        if (currentIndex != -1)
                        {
                            pluginRemove(selectedItemText);
                            listBoxPlugins.Items.RemoveAt(currentIndex);
                        }
                    };

                    contextMenu.Items.Add(removeItem);
                    contextMenu.Show(Cursor.Position);
                }
            }
        }


        public void AddPluginToListBox(string text)
        {
            listBoxPlugins.Items.Add(text);
        }




        private void pluginRemove(string pluginInfo)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                if (plugins[i].GetInfo() == pluginInfo)
                {
                    string guid = plugins[i].GetGUID();

                    guids.Remove(guid);

                    plugins.RemoveAt(i);

                    pluginPaths.RemoveAt(i);

                    updatePluginsLabels();
                    //updatePluginsButtons();
                    return;
                }
            }
        }

        private void updatePluginsLabels()
        {
            // Очищаем ListBox от всех элементов
           listBoxPlugins.Items.Clear();

            foreach (IPlugin plugin in plugins)
            {
                AddPluginToListBox(plugin.GetInfo());
            }
        }

       




        #endregion

        #region PostEditing
        private void buttonAddPost_Click(object sender, EventArgs e) 
        {
            DateTime dayDt = new DateTime(year, month, selectedDay);
            EditPostDialogForm dialog = new EditPostDialogForm(dayDt);
            DialogResult result = dialog.ShowDialog(); // Окно блокирует родительскую форму

        }

        private void AddPostViaDoubleClick(int day)
        {            
            DateTime dayDt = new DateTime(year, month, selectedDay,23, 59, 59);
            //если публикация не прошлом
            if (dayDt >= DateTime.Now)
            {
                EditPostDialogForm dialog = new EditPostDialogForm(dayDt);
                DialogResult result = dialog.ShowDialog(); 
            }
        }


        #endregion



    }
}
