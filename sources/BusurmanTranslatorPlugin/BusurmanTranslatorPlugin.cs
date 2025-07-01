using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PluginBase;

namespace BusurmanTranslatorPlugin
{
    public class BusurmanTranslatorPlugin : ITextEditor
    {
        public string GetInfo()
        {
            return "Плагин-переводчик с бусурманского языка. Автор: Чинсгисхан. Версия: 1.0";
        }

        public string GetGUID()
        {
            return "{9DD996BC-A8BD-4885-9BEC-7CF85A19F329}";
        }

        public string GetGUIinfo()
        {
            return "Исправить раскладку (RU <-> EN)";
        }

        public string GetPluginType()
        {
            return "TextEditor";
        }

        public string SetSettings()
        {
            return null;   
        }



        public string EditText(string input, string settings=null)
        {
            // Карты соответствия символов
            Dictionary<char, char> ruToEn = new Dictionary<char, char>()
            {
                {'й','q'},{'ц','w'},{'у','e'},{'к','r'},{'е','t'},{'н','y'},{'г','u'},{'ш','i'},{'щ','o'},{'з','p'},
                {'х','['},{'ъ',']'},{'ф','a'},{'ы','s'},{'в','d'},{'а','f'},{'п','g'},{'р','h'},{'о','j'},{'л','k'},
                {'д','l'},{'ж',';'},{'э','\''},{'я','z'},{'ч','x'},{'с','c'},{'м','v'},{'и','b'},{'т','n'},{'ь','m'},
                {'б',','},{'ю','.'},{'.','/'}
            };

            Dictionary<char, char> enToRu = ruToEn.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            int ruCount = input.Count(c => ruToEn.ContainsKey(char.ToLower(c)));
            int enCount = input.Count(c => enToRu.ContainsKey(char.ToLower(c)));

            bool isRussian = ruCount >= enCount;

            var map = isRussian ? ruToEn : enToRu;

            char ConvertChar(char c)
            {
                bool isUpper = char.IsUpper(c);
                char lower = char.ToLower(c);
                if (map.TryGetValue(lower, out char converted))
                    return isUpper ? char.ToUpper(converted) : converted;
                return c;
            }

            return new string(input.Select(ConvertChar).ToArray());
        }

    }
}
