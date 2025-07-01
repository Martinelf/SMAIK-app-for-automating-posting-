using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


// using Newtonsoft.Json;

public class SocialApiClient
{
    private readonly string _socialNetwork;

    // Конструктор, принимающий название соцсети (например, "Telegram", "VK")
    public SocialApiClient(string socialNetwork)
    {
        _socialNetwork = socialNetwork;
    }

    // Метод для получения имени группы по ID и токену
    public async Task<string> GetGroupName(string groupId, string apiToken)
    {
        switch (_socialNetwork.ToLower())
        {
            case "tg":
                return await GetTgGroupName(groupId, apiToken);
            case "vk":
                return await GetVkGroupName(groupId, apiToken);
            case "ok":
                //return await GetOkGroupName(groupId, apiToken);
            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }

    public static async Task<string> GetTgGroupName(string chatId, string botToken)
    {
        string apiUrl = $"https://api.telegram.org/bot{botToken}/getChat?chat_id={chatId}";
        string response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

        if (string.IsNullOrEmpty(response))
            return "Ошибка: не удалось получить данные";

        try
        {
            // Парсим JSON-ответ (можно использовать Newtonsoft.Json или System.Text.Json)
            var json = JsonDocument.Parse(response);
            string groupName = json.RootElement.GetProperty("result").GetProperty("title").GetString();
            return groupName;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GetVkGroupName(string groupId, string accessToken)
    {

        // Реализация через VK API (groups.getById)
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api.vk.com/method/groups.getById?group_id={groupId}&access_token={accessToken}&v=5.131");
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            return data.GetProperty("response")[0].GetProperty("name").GetString();
        }
        catch {  return null; }
        
    }

    public async Task<string> PostWithImage(string chatId, string apiToken, string caption, string imagePath, long unixTimestamp = 0)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Файл изображения не найден", imagePath);

        using var httpClient = new HttpClient();
        using var form = new MultipartFormDataContent();

        // Добавляем картинку
        var imageContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // или image/png, если нужно
        form.Add(imageContent, "photo", Path.GetFileName(imagePath));

        // Добавляем остальные параметры
        form.Add(new StringContent(chatId), "chat_id");
        form.Add(new StringContent(caption), "caption");
        form.Add(new StringContent("HTML"), "parse_mode");

        string apiUrl = $"https://api.telegram.org/bot{apiToken}/sendPhoto";

        try
        {
            var response = await httpClient.PostAsync(apiUrl, form);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }
    }


    public async Task<string> Post(string groupId, string apiToken, string text, long dateTime) 
    {
        string apiUrl, response;

        // Преобразуем UNIX timestamp обратно в DateTime, если он у вас уже в секундах
        DateTimeOffset postTime = DateTimeOffset.FromUnixTimeSeconds(dateTime);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;

        switch (_socialNetwork.ToLower())
        {
            case "tg":
                // Заменить [ссылка|текст] на <a href="ссылка">текст</a>
                string processedText = Regex.Replace(
                    text,
                    @"\[(https?://[^\]|]+)\|([^\]]+)\]",
                    "<a href=\"$1\">$2</a>"
                );

                // Обязательно экранируем URL
                string escapedText = Uri.EscapeDataString(processedText);

                apiUrl = $"https://api.telegram.org/bot{apiToken}/sendMessage?chat_id={groupId}&text={escapedText}&parse_mode=HTML";

                response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

                if (!response.Contains("error"))
                {
                    // TODO: Внести в таблицу publications значение 1 в поле ifPosted
                }

                return response;



            case "vk":
                if (postTime <= currentTime)
                {
                    // Публикация сразу, без параметра publish_date
                    apiUrl = $"https://api.vk.com/method/wall.post?owner_id=-{groupId}&message={Uri.EscapeDataString(text)}&access_token={apiToken}&v=5.131";
                }
                else
                {
                    // Запланированная публикация
                    apiUrl = $"https://api.vk.com/method/wall.post?owner_id=-{groupId}&message={Uri.EscapeDataString(text)}&publish_date={dateTime}&access_token={apiToken}&v=5.131";
                }
                
                response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);
                //MessageBox.Show(response);
                return response;

            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }
}
