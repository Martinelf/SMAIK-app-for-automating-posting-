using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

public class ApiExecutor
{
    /// <summary>
    /// Выполняет HTTP GET-запрос к указанному URL.
    /// </summary>
    /// <param /name="url">Полный URL для выполнения запроса.</param>
    /// <returns>Возвращает true, если запрос выполнен успешно, иначе false.</returns>
    public static async Task<string> ExecuteApiRequestAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty; // Проверка входных параметров

        try
        {
            // Создаем HTTP-клиент и отправляем GET-запрос
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                // Читаем ответ от сервера (если нужно, можно его вернуть)
                string responseBody = await response.Content.ReadAsStringAsync();

                // Проверяем успешность выполнения запроса
                return responseBody;
            }
        }
        catch
        {
            return string.Empty; // В случае ошибки возвращаем false
        }
    }
}