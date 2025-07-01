using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PluginBase;

namespace ListCreatorPlugin
{
    public class LinkMakerPlugin : ITextEditor
    {
        public string GetInfo()
        {
            return "Плагин для создания ссылок формата [url|text]. Автор: Коллективное творчество ссыльных декабристов. Версия: 18.27a";
        }

        public string GetGUID()
        {
            return "{DBE3895C-CF96-48E8-9025-CC5D2FB8EF50}";
        }

        public string GetGUIinfo()
        {
            return "Вставить URL";
        }

        public string GetPluginType()
        {
            return "TextEditor";
        }

        public string SetSettings()
        {
            return null;
        }

        public string EditText(string input, string settings = null)
        {
            
            string defaultUserOrUrl = "введитеСсылку";
            string defaultDisplayText = "введитеТекст";

            if (input == null)
            {
                return $"[{defaultUserOrUrl}|{defaultDisplayText}]";
            }

            if (IsUrl(input))
            {
                // Выделен текст похожий на ссылку
                return $"[{input}|{defaultDisplayText}]";
            }
            else
            {
                // Обычный выделенный текст
                return $"[{defaultUserOrUrl}|{input}]";
            }
        }

        // Простой метод для определения, является ли текст ссылкой (упрощённо)
        bool IsUrl(string text)
        {
            return text.Contains("/") || text.Contains("@");
        }

    }
}
