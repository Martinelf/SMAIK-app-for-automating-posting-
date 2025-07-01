using PluginBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Eventing.Reader;

namespace SMAIK
{
    public partial class EditPostDialogForm : Form
    {
        public MainForm.Publication editingPublication; //поле на случай если пост редактируется
        public MainForm.Community oldGroupInPublication; //переменная на случай замены группы при редактировании публикации

        const string TabString = "    "; // 4 пробела или "\t"

        



        // добавление новой публикации
        public EditPostDialogForm(DateTime day)
        {
            InitializeComponent();
            LoadEmojiesToListView();
            pictureBoxPublication.AllowDrop = true;
            InitializeContextMenuPlugins();

            
            this.Text = "Добавление новой публикации";

            dateTimePicker.Value = day;

            // Предположим, MainForm.CommunityCache — это List<Group>
            foreach (var group in MainForm.CommunityCache.GetAll())
            {
                // Например, можно добавить строку: "GroupName (ID)"
                comboBoxGroups.Items.Add(group);
            }

        }

        // редактирование существующей публикации 
        public EditPostDialogForm(MainForm.Publication pub)
        {
            InitializeComponent();
            LoadEmojiesToListView();

            InitializeContextMenuPlugins();

            pictureBoxPublication.Enabled = false;

            this.Text = "Редактирование публикации";
            editingPublication = pub;

            // Заполнение групп
            foreach (var group in MainForm.CommunityCache.GetAll())
            {
                comboBoxGroups.Items.Add(group);
            }

            // Установка текста и времени
            richTextBoxPublication.Text = pub.TextPubl;
            dateTimePicker.Value = pub.TimePubl;

            // Установка выбранной группы
            foreach (var item in comboBoxGroups.Items)
            {
                if (item is MainForm.Community group && group.GroupId == pub.GroupID)
                {
                    comboBoxGroups.SelectedItem = item;
                    oldGroupInPublication = (MainForm.Community)item;
                    break;
                }
            }

            // Блокировка редактирования времени и группы, если пост уже в прошлом
            if (editingPublication.TimePubl < DateTime.Now)
            {
                dateTimePicker.Enabled = false;
                comboBoxGroups.Enabled = false;
            }

            // Установка текста на кнопке
            buttonPlan.Text = "Редактировать";
            buttonPlan.Click -= buttonPlan_Click;
            buttonPlan.Click += buttonEdit_Click;

            // === 🔽 ЗАГРУЗКА КАРТИНКИ В PICTUREBOX ===
            if (!string.IsNullOrWhiteSpace(pub.ImagePath) && File.Exists(pub.ImagePath))
            {
                try
                {
                    pictureBoxPublication.Image = Image.FromFile(pub.ImagePath);
                    pictureBoxPublication.Tag = pub.ImagePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось загрузить изображение: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void InitializeContextMenuPlugins()
        {
            foreach (var pluginObj in MainForm.plugins)
            {
                if (pluginObj is ITextEditor plugin)
                {
                    var item = new ToolStripMenuItem(plugin.GetGUIinfo());
                    item.Click += (s, e) =>
                    {
                        string selectedText = richTextBoxPublication.SelectedText;
                        string settings = plugin.SetSettings();
                        string result = plugin.EditText(selectedText, settings);

                        if (!string.IsNullOrWhiteSpace(selectedText))
                        {
                            richTextBoxPublication.SelectedText = result;
                        }
                        else
                        {
                            int pos = richTextBoxPublication.SelectionStart;
                            richTextBoxPublication.Text = richTextBoxPublication.Text.Insert(pos, result);
                            richTextBoxPublication.SelectionStart = pos + result.Length;
                        }
                    };
                    richTextBoxPublication.ContextMenuStrip.Items.Add(item);
                }
            }
        }

        #region Emoji
        private int emojiInsertPosition = -1;

        private void LoadEmojiesToListView()
        {
            string[] emojis = {
                "🔥", "🎉", "✨", "📢", "✅", "🚀", "🆕", "📌", "📎", "📍",
                "💬", "📝", "🔔", "📅", "⏰", "💡", "📊", "📈", "🔝", "🎯",
                "💰", "💸", "🏆", "🥇", "🌟", "🎁", "🔒", "🔓", "📦", "🚨",
                "❤️", "💙", "💚", "💛", "🧡", "💜", "🤍", "🖤", "🤝", "👏",
                "🙌", "👍", "👎", "👀", "👉", "👈", "⬆️", "⬇️", "➡️", "⬅️",
                "🔗", "🖱️", "📲", "💻", "📱", "🖥️", "🧠", "🛠️", "🗂️", "📂",
                "🌐", "🔍", "❗", "❕", "❓", "❔", "⚠️", "🚫", "✅", "☑️",
                "🔄", "♻️", "🔃", "💬", "📣", "🤔", "😎", "😉", "😊", "😇",
                "😄", "😍", "🥰", "🥳", "😅", "😂", "🤣", "😭", "😢", "🤯",
                "😱", "😤", "😡", "🤬", "🤗", "🙃", "🤑", "🤩", "🎓", "🎬",
                "🏁", "📚", "🏫", "🧾", "🪪", "🛒", "📥", "📤", "📧", "✉️"
            };


            listViewEmo.Items.Clear();
            listViewEmo.View = View.Tile;
            listViewEmo.TileSize = new Size(50, 50); // можно подправить размер
            listViewEmo.Font = new Font("Segoe UI Emoji", 20);

            foreach (string emoji in emojis)
            {
                listViewEmo.Items.Add(emoji);
            }

            // Настроить отображение в 5 колонок можно через ширину контрола
            listViewEmo.MultiSelect = false;
            listViewEmo.Scrollable = true;


        }

        private void richTextBoxPublication_Enter(object sender, EventArgs e)
        {
            emojiInsertPosition = richTextBoxPublication.SelectionStart;
        }

        private void richTextBoxPublication_MouseUp(object sender, MouseEventArgs e)
        {
            emojiInsertPosition = richTextBoxPublication.SelectionStart;
        }

        private void richTextBoxPublication_KeyUp(object sender, KeyEventArgs e)
        {
            emojiInsertPosition = richTextBoxPublication.SelectionStart;
        }


        private void listViewEmo_MouseClick(object sender, MouseEventArgs e)
        {
            var item = listViewEmo.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                // Фокус на текстовое поле
                richTextBoxPublication.Focus();

                // Если позиция известна и в пределах текста — вставляем туда
                if (emojiInsertPosition >= 0 && emojiInsertPosition <= richTextBoxPublication.TextLength)
                {
                    richTextBoxPublication.SelectionStart = emojiInsertPosition;
                }
                else
                {
                    // иначе — в конец
                    richTextBoxPublication.SelectionStart = richTextBoxPublication.TextLength;
                }

                // Вставка
                richTextBoxPublication.SelectedText = item.Text;

                // Обновляем позицию курсора после вставки
                emojiInsertPosition = richTextBoxPublication.SelectionStart;

                listViewEmo.SelectedItems.Clear();
            }
        }


        private void listViewEmo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion


        private void richTextBoxPublication_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                int pos = richTextBoxPublication.SelectionStart;
                richTextBoxPublication.Text = richTextBoxPublication.Text.Insert(pos, "\n\n");
                richTextBoxPublication.SelectionStart = pos + 1;
                e.Handled = true;
            }
        }

        //TODO добавить фото
        private async void buttonEdit_Click(object sender, EventArgs e)
        {
            // Собираем данные из формы
            string newText = richTextBoxPublication.Text.Trim();
            DateTime newTime = dateTimePicker.Value;

            // Переводим в UNIX timestamp (секунды с 1970-01-01 UTC)
            DateTimeOffset dto = new DateTimeOffset(newTime);
            long unixTimestamp = dto.ToUnixTimeSeconds();


            if (comboBoxGroups.SelectedItem is MainForm.Community selectedGroup)
            {
                MainForm.Community newGroup = (MainForm.Community)comboBoxGroups.SelectedItem;
                int ifPosted = 0;
                string idInGroup = editingPublication.idInGroup;

                string newImagePath = pictureBoxPublication.ImageLocation; // или свой способ получить путь

                bool imageChanged = editingPublication.ImagePath != newImagePath; // Проверка изменения картинки
                bool imageRemoved = editingPublication.ImagePath != null && string.IsNullOrEmpty(newImagePath); // Удаление картинки


                // ----пост в прошлом----
                if (editingPublication.TimePubl < DateTime.Now)
                {
                    SocialApiClient apiClient = new SocialApiClient(selectedGroup.SocialNetwork);
                    string response;

                    if (string.IsNullOrEmpty(newImagePath))
                    {
                        // Без картинки - редактирование текста
                        response = await apiClient.EditPublication(selectedGroup.GroupId, selectedGroup.Token, editingPublication.idInGroup, newText);
                    }
                    else
                    {
                        // С картинкой - редактируем с изображением
                        response = await apiClient.EditPublicationWithImage(selectedGroup.GroupId, selectedGroup.Token, editingPublication.idInGroup, newText, newImagePath);
                    }

                    if (!response.Contains("error")) ifPosted = 1;
                    else MessageBox.Show("Произошла ошибка при редактировании: " + response, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                // ----пост в будущем----
                else 
                {
                    //--удаление из бывшего места--

                    // пост уже на сервере
                    if (editingPublication.IfPosted)
                    {
                        //апи клиент для удаления старой запланированной
                        SocialApiClient apiClientForDel = new SocialApiClient(editingPublication.SocialNetwork);
                        string responseDel = await apiClientForDel.DeletePublication(oldGroupInPublication.GroupId, oldGroupInPublication.Token, editingPublication.idInGroup);
                        if (!responseDel.Contains("error")) ifPosted = 1;
                        else MessageBox.Show("Произошла ошибка при удалении прошлой публикации: " + responseDel, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        
                    }
                    // не опубликован (случай тг) - скип, т к все равно в бд обновится

                    //--поместить в новое место--

                    SocialApiClient apiClient = new SocialApiClient(selectedGroup.SocialNetwork);
                    string response;
                    if (string.IsNullOrEmpty(newImagePath))
                    {
                        response = await apiClient.Post(selectedGroup.GroupId, selectedGroup.Token, newText, unixTimestamp);
                    }
                    else
                    {
                        response = await apiClient.PostWithImage(selectedGroup.GroupId, selectedGroup.Token, newText, newImagePath, unixTimestamp);
                    }


                    if (!response.Contains("error"))
                    {
                        ifPosted = 1;

                        try
                        {
                            using JsonDocument doc = JsonDocument.Parse(response);
                            JsonElement root = doc.RootElement;

                            if (root.TryGetProperty("response", out JsonElement responseElement) &&
                                responseElement.TryGetProperty("post_id", out JsonElement postIdElement))
                            {
                                idInGroup = postIdElement.GetInt32().ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при обработке ответа VK: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при редактировании публикации: " + response, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }





                //обновляем бд
                using (var db = new DBconnector())
                {
                    string sql;
                    List<SQLiteParameter> parameters = new List<SQLiteParameter>
                    {
                        new SQLiteParameter("@text", newText),
                        new SQLiteParameter("@time", newTime.ToString("yyyy-MM-dd HH:mm:ss")),
                        new SQLiteParameter("@groupId", newGroup.GroupId),
                        new SQLiteParameter("@ifPosted", ifPosted),
                        new SQLiteParameter("@idInGroup", idInGroup),
                        new SQLiteParameter("@id", editingPublication.PublID)
                    };

                    if (!string.IsNullOrEmpty(newImagePath))
                    {
                        // Обновляем imagePath, если новое значение задано
                        sql = @"
                            UPDATE publications 
                            SET textPubl = @text, 
                                timePubl = @time, 
                                groupID = @groupId,
                                ifPosted = @ifPosted,
                                idInGroup = @idInGroup,
                                imagePath = @imagePath
                            WHERE publID = @id";

                        parameters.Add(new SQLiteParameter("@imagePath", newImagePath));
                    }
                    else
                    {
                        // Не трогаем поле imagePath
                        sql = @"
                        UPDATE publications 
                        SET textPubl = @text, 
                            timePubl = @time, 
                            groupID = @groupId,
                            ifPosted = @ifPosted,
                            idInGroup = @idInGroup
                        WHERE publID = @id";
                    }

                    db.ExecuteNonQuery(sql, parameters.ToArray());
                }


                // Обновляем кэш
                editingPublication.TextPubl = newText;
                editingPublication.TimePubl = newTime;
                editingPublication.GroupID = selectedGroup.GroupId;
                editingPublication.SocialNetwork = selectedGroup.SocialNetwork;
                editingPublication.ImagePath = string.IsNullOrEmpty(newImagePath) ? null : newImagePath;

                DialogResult = DialogResult.OK;
                Close();

                //обновляем ui
                MainForm.Instance.UpdateMonth(MainForm.Instance.month);
                MainForm.Instance.UpdateDayPlan(MainForm.Instance.selectedDay);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите группу.");
            }
        }

        private async void buttonPlan_Click(object sender, EventArgs e)
        {
            string imagePath = (string)pictureBoxPublication.Tag;

            // Получаем текст из richTextBox
            string postText = richTextBoxPublication.Text;

            DateTime selectedDateTime = dateTimePicker.Value;
            DateTimeOffset dto = new DateTimeOffset(selectedDateTime);
            long unixTimestamp = dto.ToUnixTimeSeconds();

            MainForm.Community group = (MainForm.Community)comboBoxGroups.SelectedItem;

            planPost(group, postText, unixTimestamp, selectedDateTime, imagePath);

            

        }

        public async void planPost(MainForm.Community group, string postText, long unixTimestamp, DateTime selectedDateTime, string imagePath = null)
        {
            int ifPosted = 0;
            string idInGroup = null;

            // Публикуем, если не нужен планировщик
            if (!group.needTaskScheduler)
            {
                SocialApiClient apiClient = new SocialApiClient(group.SocialNetwork);

                string response;
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // Если указана картинка и она существует — отправляем пост с картинкой
                    response = await apiClient.PostWithImage(group.GroupId, group.Token, postText, imagePath, unixTimestamp);
                }
                else
                {
                    // Иначе — обычная публикация
                    response = await apiClient.Post(group.GroupId, group.Token, postText, unixTimestamp);
                }

                if (!response.Contains("error"))
                {
                    ifPosted = 1;

                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(response);
                        JsonElement root = doc.RootElement;

                        if (root.TryGetProperty("response", out JsonElement responseElement) &&
                            responseElement.TryGetProperty("post_id", out JsonElement postIdElement))
                        {
                            idInGroup = postIdElement.GetInt32().ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обработке ответа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }    
                else
                {
                    MessageBox.Show("Произошла ошибка при публикации: " + response, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // добавление в БД
            DBconnector db = new DBconnector();

            if (selectedDateTime <= DateTime.Now) selectedDateTime = DateTime.Now;

            string query = @"INSERT INTO publications (timePubl, textPubl, groupID, ifPosted, idInGroup, imagePath)
                     VALUES (@timePubl, @textPubl, @groupID, @ifPosted, @idInGroup, @imagePath)";
            var parameters = new SQLiteParameter[] {
                new SQLiteParameter("@groupID", group.GroupId),
                new SQLiteParameter("@textPubl", postText),
                new SQLiteParameter("@timePubl", selectedDateTime.ToString("yyyy-MM-dd HH:mm:ss")),
                new SQLiteParameter("@ifPosted", ifPosted),
                new SQLiteParameter("@idInGroup", idInGroup),
                new SQLiteParameter("@imagePath", imagePath ?? string.Empty)
            };

            var res = db.ExecuteNonQuery(query, parameters);

            MainForm.Instance.UpdateMonth(MainForm.Instance.month);
            MainForm.Instance.UpdateDayPlan(MainForm.Instance.selectedDay);

        }

        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxGroups.SelectedItem is MainForm.Community selectedGroup)
            {
                var confirmResult = MessageBox.Show(
                    "Вы уверены, что хотите удалить публикацию?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult != DialogResult.Yes)
                    return;
                string response = null;
                if (editingPublication.IfPosted || !selectedGroup.needTaskScheduler)
                {
                    SocialApiClient apiClient = new SocialApiClient(selectedGroup.SocialNetwork);
                    response = await apiClient.DeletePublication(selectedGroup.GroupId, selectedGroup.Token, editingPublication.idInGroup);
                   
                }
                if (response == null || !response.Contains("error"))
                {
                    try
                    {
                        DBconnector db = new DBconnector();

                        object tempIdInGroup = editingPublication.idInGroup;
                        bool idIsNull = string.IsNullOrWhiteSpace(editingPublication.idInGroup);

                        string query = idIsNull
                            ? "DELETE FROM publications WHERE idInGroup IS NULL AND groupID = @groupID"
                            : "DELETE FROM publications WHERE idInGroup = @idInGroup AND groupID = @groupID";

                        var parameters = idIsNull
                            ? new SQLiteParameter[]
                            {
                                new SQLiteParameter("@groupID", selectedGroup.GroupId)
                            }
                            : new SQLiteParameter[]
                            {
                                new SQLiteParameter("@idInGroup", editingPublication.idInGroup),
                                new SQLiteParameter("@groupID", selectedGroup.GroupId)
                            };

                        int rowsAffected = db.ExecuteNonQuery(query, parameters);


                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Публикация успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Можно обновить UI, например, удалить из списка
                        }
                        else
                        {
                            MessageBox.Show("Публикация не найдена в базе данных.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                else MessageBox.Show("Произошла ошибка при удалении: " + response, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MainForm.Instance.UpdateMonth(MainForm.Instance.month);
            MainForm.Instance.UpdateDayPlan(MainForm.Instance.selectedDay);
        }

        private void toolStripMenuItemMakeURL_Click(object sender, EventArgs e)
        {
            /*
            string selectedText =richTextBoxPublication.SelectedText;
            int selectionStart =richTextBoxPublication.SelectionStart;
            int selectionLength =richTextBoxPublication.SelectionLength;

            string defaultUserOrUrl = "введитеСсылку";
            string defaultDisplayText = "введитеТекст";

            // Простой метод для определения, является ли текст ссылкой (упрощённо)
            bool IsUrl(string text)
            {
                return text.Contains("/");
            }


            string formattedText;

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                // Ничего не выделено или только пробелы
                formattedText = $"[{defaultUserOrUrl}|{defaultDisplayText}]";
               richTextBoxPublication.Text =richTextBoxPublication.Text.Insert(selectionStart, formattedText);
               richTextBoxPublication.SelectionStart = selectionStart + formattedText.Length;
            }
            else if (IsUrl(selectedText))
            {
                // Выделен текст похожий на ссылку
                formattedText = $"[{selectedText}|{defaultDisplayText}]";
                ReplaceSelectedText(formattedText);
            }
            else
            {
                // Обычный выделенный текст
                formattedText = $"[{defaultUserOrUrl}|{selectedText}]";
                ReplaceSelectedText(formattedText);
            }

            void ReplaceSelectedText(string text)
            {
               richTextBoxPublication.Text =richTextBoxPublication.Text.Remove(selectionStart, selectionLength)
                                     .Insert(selectionStart, text);
               richTextBoxPublication.SelectionStart = selectionStart + text.Length;
            }
            */
        }

        #region attachPhoto
        private void pictureBoxPublication_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Выберите изображение";
                openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = openFileDialog.FileName;
                    Image image = Image.FromFile(sourcePath);
                    pictureBoxPublication.Image = image;
                    pictureBoxPublication.SizeMode = PictureBoxSizeMode.Zoom;

                    // Папка для сохранения
                    string picturesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pictures");

                    if (!Directory.Exists(picturesFolder))
                        Directory.CreateDirectory(picturesFolder);

                    // Имя файла (например, как в оригинале, либо уникальное)
                    string fileName = Path.GetFileName(sourcePath);
                    string destPath = Path.Combine(picturesFolder, fileName);

                    // Если файл с таким именем уже есть — делаем уникальное
                    int count = 1;
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);

                    while (File.Exists(destPath))
                    {
                        fileName = $"{nameWithoutExt}_{count}{ext}";
                        destPath = Path.Combine(picturesFolder, fileName);
                        count++;
                    }

                    image.Save(destPath); // сохраняем копию в папку

                    pictureBoxPublication.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBoxPublication.Tag = destPath; // Сохраняем путь
                }
            }
        }

        private void pictureBoxPublication_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBoxPublication_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && IsImageFile(files[0]))
            {
                string sourcePath = files[0];
                Image image = Image.FromFile(sourcePath);
                pictureBoxPublication.Image = image;
                pictureBoxPublication.SizeMode = PictureBoxSizeMode.Zoom;

                string picturesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pictures");
                if (!Directory.Exists(picturesFolder))
                    Directory.CreateDirectory(picturesFolder);

                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(picturesFolder, fileName);

                int count = 1;
                string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);

                while (File.Exists(destPath))
                {
                    fileName = $"{nameWithoutExt}_{count}{ext}";
                    destPath = Path.Combine(picturesFolder, fileName);
                    count++;
                }

                image.Save(destPath);

                pictureBoxPublication.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBoxPublication.Tag = destPath; // Сохраняем путь

            }
        }
        private bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
        }

        private void RemovePhotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBoxPublication.Image == null)
            {
                MessageBox.Show("Нет изображения для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Удалить изображение?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Удаляем изображение из PictureBox
                pictureBoxPublication.Image.Dispose(); // важно: освобождаем ресурсы
                pictureBoxPublication.Image = null;

                // Удаляем файл, если путь указан в Tag
                if (pictureBoxPublication.Tag is string imagePath && File.Exists(imagePath))
                {
                    try
                    {
                        File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Не удалось удалить файл: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Очищаем Tag
                pictureBoxPublication.Tag = null;
            }
        }



        #endregion


    }

}

