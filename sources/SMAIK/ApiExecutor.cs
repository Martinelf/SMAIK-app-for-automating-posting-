using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

public class ApiExecutor
{
    /// <summary>
    /// Выполняет HTTP POST-запрос к указанному URL.
    /// </summary>
    /// <param name="url">Полный URL для выполнения запроса.</param>
    /// <returns>Ответ от сервера в виде строки, либо пустая строка при ошибке.</returns>
    public static async Task<string> ExecuteApiRequestAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty; // Проверка входных параметров

        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Пустое тело запроса, параметры уже включены в URL
                var content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.PostAsync(url, content);

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
