using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    public async Task<string> Post(string groupId, string apiToken, string text, long dateTime) 
    {
        string apiUrl, response;

        // Преобразуем UNIX timestamp обратно в DateTime, если он у вас уже в секундах
        DateTimeOffset postTime = DateTimeOffset.FromUnixTimeSeconds(dateTime);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;

        switch (_socialNetwork.ToLower())
        {
            case "tg":
                // TODO: если в text есть конструкция типо [ссылка|текст] - то отправить отформатировав ссылку в такой вид  <a href="https://example.com">this site</a>
                apiUrl = $"https://api.telegram.org/bot{apiToken}/sendMessage?chat_id={groupId}&text={text}";
                response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);
                if (!response.Contains("error")) ; //внести в поле ifPosted таблицы publications в строку публикации значение 1
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
            /*
            case "ok":
                string applicationKey = "CHIHENLGDIHBABABA";
                string appSecretKey = "BCF88E46A69DE9F701461F4E";
                string gid = groupId; // ID группы
                string method = "mediatopic.post";

                var attachmentJson = $"{{\"media\":[{{\"type\":\"text\",\"text\":\"{text}\"}}]}}";

                var parameters = new Dictionary<string, string>
                    {
                        { "application_key", applicationKey },
                        { "method", method },
                        { "gid", gid },
                        { "type", "GROUP_THEME" },
                        { "attachment", attachmentJson },
                        { "format", "json" },
                        { "access_token", apiToken }
                    };

                string sig = GenerateSig(parameters, apiToken, appSecretKey);
                parameters.Add("sig", sig);

                var client = new HttpClient();
                var content = new FormUrlEncodedContent(parameters);
                var result = await client.PostAsync("https://api.ok.ru/fb.do", content);
                var responseText = await result.Content.ReadAsStringAsync();

                MessageBox.Show(responseText);
                return responseText;
            */
            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }

    public async Task<string> PostWithImage(string groupId, string apiToken, string text, string imagePath, long dateTime = 0)
    {
        DateTimeOffset postTime = DateTimeOffset.FromUnixTimeSeconds(dateTime);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;

        switch (_socialNetwork.ToLower())
        {
            case "tg":
                {
                    using var httpClient = new HttpClient();

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        // Отправляем фото с подписью
                        using var form = new MultipartFormDataContent();

                        var imageContent = new ByteArrayContent(File.ReadAllBytes(imagePath));
                        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                        form.Add(imageContent, "photo", Path.GetFileName(imagePath));

                        form.Add(new StringContent(groupId), "chat_id");

                        // Можно добавить минимальную поддержку [ссылка|текст] -> <a href="ссылка">текст</a>
                        string caption = ConvertLinksToHtml(text);
                        form.Add(new StringContent(caption), "caption");
                        form.Add(new StringContent("HTML"), "parse_mode");

                        string apiUrl = $"https://api.telegram.org/bot{apiToken}/sendPhoto";

                        try
                        {
                            var response = await httpClient.PostAsync(apiUrl, form);
                            string responseString = await response.Content.ReadAsStringAsync();

                            if (!responseString.Contains("error"))
                            {
                                // Можно здесь ставить ifPosted = 1
                            }
                            return responseString;
                        }
                        catch (Exception ex)
                        {
                            return $"error: {ex.Message}";
                        }
                    }
                    else
                    {
                        // Отправляем просто текстовое сообщение
                        string textProcessed = ConvertLinksToHtml(text);
                        string apiUrl = $"https://api.telegram.org/bot{apiToken}/sendMessage?chat_id={groupId}&text={Uri.EscapeDataString(textProcessed)}&parse_mode=HTML";
                        string response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

                        if (!response.Contains("error"))
                        {
                            // ifPosted = 1
                        }

                        return response;
                    }
                }

            case "vk":
                {
                    using var httpClient = new HttpClient();

                    string attachment = null;

                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        // 1. Получаем upload URL для стены 
                        string uploadServerUrl = $"https://api.vk.com/method/photos.getWallUploadServer?group_id={groupId}&access_token={apiToken}&v=5.131";
                        
                        var uploadServerResponseStr = await ApiExecutor.ExecuteApiRequestAsync(uploadServerUrl);
                        using var uploadServerDoc = JsonDocument.Parse(uploadServerResponseStr);
                        var uploadServerRoot = uploadServerDoc.RootElement;

                        if (!uploadServerRoot.TryGetProperty("response", out var uploadResponse))
                            return "Ошибка получения URL для загрузки фотографии";

                        string uploadUrl = uploadResponse.GetProperty("upload_url").GetString();

                        // 2. Загружаем фото
                        using var fileStream = File.OpenRead(imagePath);
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                        using var form = new MultipartFormDataContent();
                        form.Add(fileContent, "photo", Path.GetFileName(imagePath));

                        var uploadResult = await httpClient.PostAsync(uploadUrl, form);
                        var uploadResultStr = await uploadResult.Content.ReadAsStringAsync();

                        MessageBox.Show("Upload response: " + uploadResultStr);

                        if (!uploadResultStr.TrimStart().StartsWith("{"))
                        {
                            MessageBox.Show("Ошибка: неверный JSON от VK.");
                            return null;
                        }

                        using var uploadResultDoc = JsonDocument.Parse(uploadResultStr);
                        var uploadResultRoot = uploadResultDoc.RootElement;

                        // потом достаешь photo, hash, server и отправляешь их в photos.saveWallPhoto

                        if (!uploadResultRoot.TryGetProperty("server", out var serverElement) ||
                            !uploadResultRoot.TryGetProperty("photo", out var photoElement) ||
                            !uploadResultRoot.TryGetProperty("hash", out var hashElement))
                        {
                            return "Ошибка загрузки фотографии";
                        }

                        string server = serverElement.GetInt32().ToString();
                        
                        string hash = hashElement.GetString();

                        // Достаём только нужное поле photo
                        string photoRaw = photoElement.GetString();
                        using var nestedDoc = JsonDocument.Parse(photoRaw);
                        //var innerPhoto = nestedDoc.RootElement[0].GetProperty("photo").GetString();

                        // Формируем правильную строку
                        //string cleanedPhoto = $"[{{\"photo\":\"{innerPhoto}\"}}]";
                        string encodedPhoto = Uri.EscapeDataString(photoRaw);

                        // 3. Сохраняем фото на стене
                        string savePhotoUrl = $"https://api.vk.com/method/photos.saveWallPhoto?" +
                                              $"group_id={groupId}&server={server}&photo={encodedPhoto}&hash={hash}" +
                                              $"&access_token={apiToken}&v=5.131";

                        var savePhotoResponseStr = await ApiExecutor.ExecuteApiRequestAsync(savePhotoUrl);
                        using var savePhotoDoc = JsonDocument.Parse(savePhotoResponseStr);
                        var savePhotoRoot = savePhotoDoc.RootElement;

                        if (!savePhotoRoot.TryGetProperty("response", out var savePhotoResponseArr) || savePhotoResponseArr.GetArrayLength() == 0)
                            return "Ошибка сохранения фотографии";

                        var firstPhoto = savePhotoResponseArr[0];
                        string ownerId = firstPhoto.GetProperty("owner_id").GetInt32().ToString();
                        string mediaId = firstPhoto.GetProperty("id").GetInt32().ToString();

                        attachment = $"photo{ownerId}_{mediaId}";
                    }

                    // 4. Формируем URL публикации
                    string apiUrl;
                    if (postTime <= currentTime)
                    {
                        apiUrl = $"https://api.vk.com/method/wall.post?owner_id=-{groupId}&message={Uri.EscapeDataString(text)}" +
                                 $"{(attachment != null ? $"&attachments={attachment}" : "")}&access_token={apiToken}&v=5.131";
                    }
                    else
                    {
                        apiUrl = $"https://api.vk.com/method/wall.post?owner_id=-{groupId}&message={Uri.EscapeDataString(text)}" +
                                 $"{(attachment != null ? $"&attachments={attachment}" : "")}&publish_date={dateTime}&access_token={apiToken}&v=5.131";
                    }

                    string responseVK = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);
                    return responseVK;
                }

            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }

    // Мини-функция для конвертации ссылок [ссылка|текст] в <a href=...>
    private string ConvertLinksToHtml(string input)
    {
        // Простая замена по шаблону [url|text] -> <a href="url">text</a>
        return System.Text.RegularExpressions.Regex.Replace(input, @"\[(.*?)\|(.*?)\]", m =>
        {
            var url = m.Groups[1].Value;
            var text = m.Groups[2].Value;
            return $"<a href=\"{url}\">{text}</a>";
        });
    }

    /*
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
    */

    public async Task<string> EditPublication(string groupId, string apiToken, string idInGroup, string newText)
    {
        string apiUrl, response;

        switch (_socialNetwork.ToLower())
        {
            case "tg":
                // Попробовать сначала как текстовое сообщение
                string editTextUrl = $"https://api.telegram.org/bot{apiToken}/editMessageText?chat_id={groupId}&message_id={idInGroup}&text={Uri.EscapeDataString(newText)}";
                var editTextResponse = await ApiExecutor.ExecuteApiRequestAsync(editTextUrl);

                if (!editTextResponse.Contains("\"error_code\""))
                {
                    MessageBox.Show("Сообщение отредактировано (текст)!");
                    return editTextResponse;
                }

                // Если не получилось — попробовать как подпись к изображению
                string editCaptionUrl = $"https://api.telegram.org/bot{apiToken}/editMessageCaption?chat_id={groupId}&message_id={idInGroup}&caption={Uri.EscapeDataString(newText)}";
                var editCaptionResponse = await ApiExecutor.ExecuteApiRequestAsync(editCaptionUrl);

                if (!editCaptionResponse.Contains("\"error_code\""))
                {
                    MessageBox.Show("Сообщение отредактировано (подпись)!");
                    return editCaptionResponse;
                }

                MessageBox.Show("Ошибка при редактировании сообщения в Telegram");
                return "Ошибка при редактировании сообщения в Telegram";

            case "vk":
                // idInGroup — это post_id, groupId — числовой ID группы без минуса
                apiUrl = $"https://api.vk.com/method/wall.edit?owner_id=-{groupId}&post_id={idInGroup}&message={Uri.EscapeDataString(newText)}&access_token={apiToken}&v=5.131";
                response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

                return response;

            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }

    public async Task<string> EditPublicationWithImage(string groupId, string apiToken, string idInGroup, string newText, string imagePath)
    {
        switch (_socialNetwork.ToLower())
        {
            case "tg":
                {
                    // Telegram не позволяет менять картинку в уже отправленном сообщении через editMessageMedia
                    // Можно только редактировать текст. Для смены картинки нужно удалить и отправить заново.
                    MessageBox.Show("Редактирование картинки в Telegram не поддерживается. Пожалуйста, удалите и создайте публикацию заново.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Редактируем только текст
                    string editUrl = $"https://api.telegram.org/bot{apiToken}/editMessageText?chat_id={groupId}&message_id={idInGroup}&text={Uri.EscapeDataString(newText)}";
                    var editResponse = await ApiExecutor.ExecuteApiRequestAsync(editUrl);
                    return editResponse;
                }

            case "vk":
                {
                    // Шаг 1: Получаем URL для загрузки фото на стену
                    string uploadServerUrl = $"https://api.vk.com/method/photos.getWallUploadServer?group_id={groupId}&access_token={apiToken}&v=5.131";
                    var uploadServerResponseStr = await ApiExecutor.ExecuteApiRequestAsync(uploadServerUrl);
                    using var uploadServerDoc = JsonDocument.Parse(uploadServerResponseStr);
                    var uploadServerRoot = uploadServerDoc.RootElement;

                    if (!uploadServerRoot.TryGetProperty("response", out var uploadResponse))
                        return "Ошибка получения URL для загрузки фотографии";

                    string uploadUrl = uploadResponse.GetProperty("upload_url").GetString();

                    // Шаг 2: Загружаем фото (multipart/form-data)
                    using var httpClient = new HttpClient();
                    using var content = new MultipartFormDataContent();
                    using var fileStream = File.OpenRead(imagePath);
                    content.Add(new StreamContent(fileStream), "photo", Path.GetFileName(imagePath));

                    var uploadResult = await httpClient.PostAsync(uploadUrl, content);
                    var uploadResultStr = await uploadResult.Content.ReadAsStringAsync();

                    using var uploadResultDoc = JsonDocument.Parse(uploadResultStr);
                    var uploadResultRoot = uploadResultDoc.RootElement;

                    if (!uploadResultRoot.TryGetProperty("server", out var serverElement) ||
                        !uploadResultRoot.TryGetProperty("photo", out var photoElement) ||
                        !uploadResultRoot.TryGetProperty("hash", out var hashElement))
                    {
                        return "Ошибка загрузки фотографии";
                    }

                    string server = serverElement.GetRawText();
                    string photo = photoElement.GetString();
                    string hash = hashElement.GetString();

                    // Шаг 3: Сохраняем фото на стене
                    string savePhotoUrl = $"https://api.vk.com/method/photos.saveWallPhoto?" +
                                          $"group_id={groupId}&server={server}&photo={Uri.EscapeDataString(photo)}&hash={hash}" +
                                          $"&access_token={apiToken}&v=5.131";

                    var savePhotoResponseStr = await ApiExecutor.ExecuteApiRequestAsync(savePhotoUrl);
                    using var savePhotoDoc = JsonDocument.Parse(savePhotoResponseStr);
                    var savePhotoRoot = savePhotoDoc.RootElement;

                    if (!savePhotoRoot.TryGetProperty("response", out var savePhotoResponseArr) || savePhotoResponseArr.GetArrayLength() == 0)
                        return "Ошибка сохранения фотографии";

                    var firstPhoto = savePhotoResponseArr[0];
                    string ownerId = firstPhoto.GetProperty("owner_id").GetInt32().ToString();
                    string mediaId = firstPhoto.GetProperty("id").GetInt32().ToString();

                    // Формируем attachment для редактирования поста
                    string attachment = $"photo{ownerId}_{mediaId}";

                    // Шаг 4: Редактируем пост с новым текстом и новым вложением (картинкой)
                    string apiUrl = $"https://api.vk.com/method/wall.edit?" +
                                    $"owner_id=-{groupId}&post_id={idInGroup}&message={Uri.EscapeDataString(newText)}&attachments={attachment}" +
                                    $"&access_token={apiToken}&v=5.131";

                    string response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

                    return response;
                }

            default:
                throw new NotSupportedException($"API для {_socialNetwork} не поддерживается.");
        }
    }


    public async Task<string> DeletePublication(string groupId, string apiToken, string idInGroup)
    {
        string apiUrl, response;

        switch (_socialNetwork.ToLower())
        {
            case "tg":
                string deleteUrl = $"https://api.telegram.org/bot{apiToken}/deleteMessage?chat_id={groupId}&message_id={idInGroup}";
                var deleteResponse = await ApiExecutor.ExecuteApiRequestAsync(deleteUrl);

                if (!deleteResponse.Contains("error"))
                {
                    MessageBox.Show("Сообщение удалено!");
                    
                }
                else
                {
                    return "Сообщение не найдено для удаления";
                }
                return deleteResponse;


            case "vk":
                // idInGroup — это post_id (например, 123), groupId — числовой ID группы без минуса
                apiUrl = $"https://api.vk.com/method/wall.delete?owner_id=-{groupId}&post_id={idInGroup}&access_token={apiToken}&v=5.131";
                response = await ApiExecutor.ExecuteApiRequestAsync(apiUrl);

                return response;
            
            default:
                throw new NotSupportedException($"API for {_socialNetwork} is not supported.");
        }
    }
}
