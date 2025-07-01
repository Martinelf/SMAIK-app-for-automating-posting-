using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Principal;

namespace SMAIK
{
    public partial class AddGroupDialogForm : Form
    {
        public string token = string.Empty;
        public string id = string.Empty;
        public string socialNet = string.Empty;

        private readonly DBconnector db = new DBconnector();

        //конструктор с передачей только соцсети
        public AddGroupDialogForm(string socialNetwork)
        {
            InitializeComponent();
            socialNet = socialNetwork;
            token = string.Empty;
            id = string.Empty;
        }

        //конструктор с передачей id группы для редактирования
        public AddGroupDialogForm(string socialNetwork, string tokenGr, string idGroup)
        {
            socialNet = socialNetwork;
            token = tokenGr;
            id = idGroup;

            InitializeComponent();

            textBoxId.Text = id;
            textBoxToken.Text = token;
            buttonAddGroup.Text = "Изменить";

            //замена метода сохранения на метод редактирования НЕ РАБОТАЕТ
            buttonAddGroup.Click -= buttonAddGroup_Click;
            buttonAddGroup.Click += (sender, e) => editGroupInfo(socialNetwork, tokenGr, textBoxToken.Text, idGroup, textBoxId.Text);

            MainForm.CommunityCache.Refresh();
            MainForm.Instance.RefreshListBoxes();
        }

        public async void editGroupInfo(string socialNet, string oldToken, string newToken, string oldGroupId, string newGroupId)
        {
            try
            {
                //находим имя + проверка на существование группы
                SocialApiClient apiClient = new SocialApiClient(socialNet);
                string newGroupName = await apiClient.GetGroupName(newGroupId, newToken);
                if (newGroupName == null) throw new ArgumentNullException();

                string sql = @"
                UPDATE groups
                SET 
                    tokenID = (SELECT tokenID FROM tokens WHERE token = @OldToken),
                    groupID = @NewGroupId,
                    groupName = @NewGroupName
                WHERE groupID = @OldGroupId;

                UPDATE tokens
                SET 
                    token = @NewToken
                WHERE tokenID = (SELECT tokenID FROM tokens WHERE token = @OldToken);
                ";

                var parameters = new SQLiteParameter[]
                {
                new SQLiteParameter("@NewToken", newToken),
                new SQLiteParameter("@NewGroupId", newGroupId),
                new SQLiteParameter("@NewGroupName", newGroupName), // новый параметр
                new SQLiteParameter("@OldToken", oldToken),
                new SQLiteParameter("@OldGroupId", oldGroupId)
                };

                db.ExecuteNonQuery(sql, parameters);

                MainForm.CommunityCache.Refresh();
                MainForm.Instance.RefreshListBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        public AddGroupDialogForm()
        {
            InitializeComponent();
        }



        private async void buttonAddGroup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxToken.Text))
            {
                MessageBox.Show("Токен не может быть пустым!");
                return;
            }

            token = textBoxToken.Text;
            id = textBoxId.Text;

            try
            {
                // Получаем ID социальной сети из базы
                int snId = GetSocialNetworkId(socialNet);
                if (snId == -1)
                {
                    MessageBox.Show("Указанная социальная сеть не найдена в базе!");
                    return;
                }

                //find name (if error - will be exeption, will not insert anything)
                SocialApiClient apiClient = new SocialApiClient(socialNet); 
                string groupName = await apiClient.GetGroupName(id, token);
                if (groupName == null) throw new ArgumentNullException(); 

                // Добавляем токен в таблицу tokens
                int tokenId = AddToken(token, snId);

                // Добавляем группу в таблицу groups
                AddGroup(id, tokenId.ToString(), groupName);

                //MessageBox.Show("Группа успешно добавлена!");
                this.DialogResult = DialogResult.OK;
                this.Close();

                MainForm.CommunityCache.Refresh();
                MainForm.Instance.RefreshListBoxes();

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //error means that group was not found in socialNet
                if (ex is ArgumentNullException)
                {
                    error = "Похоже данная группа не существует. Убедитесь в правильности токена и id группы, проверьте подключение к интернету. Затем повторите ввод";
                }
                MessageBox.Show($"Ошибка при добавлении группы:\n{error}");
            }
        }

        private int GetSocialNetworkId(string socialNetworkName)
        {
            string query = "SELECT snID FROM socialnetworks WHERE snName = @name";
            var parameters = new SQLiteParameter[] {
                new SQLiteParameter("@name", socialNetworkName)
            };

            var result = db.ExecuteQuery(query, parameters);
            if (result.Rows.Count > 0)
            {
                return Convert.ToInt32(result.Rows[0]["snID"]);
            }
            return -1;
        }

        private int AddToken(string token, int snId)
        {
            // Сначала пытаемся вставить, если токен уже есть — игнорируем
            string insertQuery = "INSERT OR IGNORE INTO tokens (token, snID) VALUES (@token, @snId);";
            var parameters = new SQLiteParameter[] {
                new SQLiteParameter("@token", token),
                new SQLiteParameter("@snId", snId)
            };
            db.ExecuteNonQuery(insertQuery, parameters);

            // Затем возвращаем ID токена (в любом случае он уже в базе)
            string selectQuery = "SELECT tokenID FROM tokens WHERE token = @token;";
            var selectParam = new SQLiteParameter[] {
                new SQLiteParameter("@token", token)
             };
            var result = db.ExecuteQuery(selectQuery, selectParam);
            return Convert.ToInt32(result.Rows[0][0]);
        }

        private void AddGroup(string groupId, string tokenId, string groupName)
        {
            string query = "INSERT INTO groups (groupID, tokenID, groupName) VALUES (@groupId, @tokenId, @groupName)";
            var parameters = new SQLiteParameter[] {
            new SQLiteParameter("@groupId", groupId),
            new SQLiteParameter("@tokenId", tokenId),
            new SQLiteParameter("@groupName", groupName ?? (object)DBNull.Value)
        };

            db.ExecuteNonQuery(query, parameters);
        }

        // мб переделать на ссылки на более понятные статьи
        private void buttonInfo_Click(object sender, EventArgs e)
        {
            if (socialNet == "tg")
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://webosnova.com/ru/blog/sozdanie-telegram-bota-api-token",
                    UseShellExecute = true
                });
            }
            else if (socialNet == "vk")
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://id.vk.com/about/business/go/docs/ru/vkid/latest/vk-id/connection/tokens/access-token",
                    UseShellExecute = true
                });
            }
            else if (socialNet == "ok")
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://apiok.ru/dev/app/create",
                    UseShellExecute = true
                });
            }
        }
    }
}
