using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

class Program
{
    private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scheduler.log");

    public class Community
    {
        public string GroupId { get; set; }       // groupID из таблицы groups
        public string GroupName { get; set; }     // groupName из таблицы groups
        public string Token { get; set; }         // token из таблицы tokens
        public string SocialNetwork { get; set; } // snName из таблицы socialnetworks
        public bool needTaskScheduler { get; set; } // нужен ли свой планировщик задач или API соцсети его включает
        public string idInGroup { get; set; } // нужен ли свой планировщик задач или API соцсети его включает

        //форма записи для представления в списках всех групп
        public override string ToString()
        {
            return $"{GroupName} ({GroupId})";
        }
    }

    static async Task Main(string[] args)
    {
        Log($"[{DateTime.Now}] Запуск планировщика...");
        try
        {
            DBconnector db = new DBconnector();

            string selectQuery = @"
            SELECT 
                p.publID,
                p.textPubl,
                p.timePubl,
                p.imagePath,
                g.groupID,
                g.groupName,
                t.token,
                sn.snName
            FROM publications p
            JOIN groups g ON p.groupID = g.groupID
            JOIN tokens t ON g.tokenID = t.tokenID
            JOIN socialnetworks sn ON t.snID = sn.snID
            WHERE p.ifPosted = 0
        ";

            DataTable table = db.ExecuteQuery(selectQuery);

            foreach (DataRow row in table.Rows)
            {
                if (!DateTime.TryParse(row["timePubl"].ToString(), out DateTime timePubl))
                    continue;

                if (timePubl > DateTime.Now)
                    continue;

                var group = new Community
                {
                    GroupId = row["groupID"].ToString(),
                    GroupName = row["groupName"].ToString(),
                    Token = row["token"].ToString(),
                    SocialNetwork = row["snName"].ToString()
                };

                string postText = row["textPubl"].ToString();
                string imagePath = row["imagePath"]?.ToString();
                int publID = Convert.ToInt32(row["publID"]);

                var apiClient = new SocialApiClient(group.SocialNetwork);
                long unixTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

                string response;

                // 🔽 Выбор метода публикации: с изображением или без
                if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
                {
                    response = await apiClient.PostWithImage(group.GroupId, group.Token, postText, imagePath, unixTimestamp);
                }
                else
                {
                    response = await apiClient.Post(group.GroupId, group.Token, postText, unixTimestamp);
                }

                // 🔽 Обработка ответа
                if (!response.Contains("error"))
                {
                    try
                    {
                        using JsonDocument doc = JsonDocument.Parse(response);
                        JsonElement root = doc.RootElement;

                        if (root.GetProperty("ok").GetBoolean())
                        {
                            if (root.TryGetProperty("result", out JsonElement resultElement) &&
                                resultElement.TryGetProperty("message_id", out JsonElement messageIdElement))
                            {
                                int messageId = messageIdElement.GetInt32();

                                string updateQuery = "UPDATE publications SET ifPosted = 1, idInGroup = @msgId WHERE publID = @id";
                                var parameters = new SQLiteParameter[]
                                {
                                new SQLiteParameter("@msgId", messageId),
                                new SQLiteParameter("@id", publID)
                                };
                                db.ExecuteNonQuery(updateQuery, parameters);

                                Log($"[OK] Пост ID={publID} успешно опубликован. message_id={messageId}");
                            }
                            else
                            {
                                Log($"[WARNING] Пост опубликован, но message_id не найден.");
                            }
                        }
                        else
                        {
                            Log("[ERROR] Ответ Telegram содержит ok: false.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"[ERROR] Ошибка при обработке ответа Telegram: {ex.Message}");
                    }
                }
                else
                {
                    Log($"[ERROR] Ошибка при публикации ID={publID}: {response}");
                }
            }

            Log("Завершено без фатальных ошибок.\n");
        }
        catch (Exception ex)
        {
            Log($"[FATAL ERROR] {ex.Message}\n{ex.StackTrace}\n");
        }
    }


    static void Log(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}\n");
        }
        catch
        {
            // Игнорируем ошибки логирования, чтобы не повредить основной поток
        }
    }
}
