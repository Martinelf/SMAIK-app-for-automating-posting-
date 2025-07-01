using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using PluginBase;

namespace ListCreatorPlugin
{
    public class ListCreatorPlugin : ITextEditor
    {
        public string GetInfo()
        {
            return "Плагин для создания списков. Автор: Листовой листовик. Версия: 12.3abc";
        }

        public string GetGUID()
        {
            return "{DFE0C3D7-4553-408A-A949-3B310C5E4736}";
        }

        public string GetGUIinfo()
        {
            return "Сделать список";
        }

        public string GetPluginType()
        {
            return "TextEditor";
        }

        public string SetSettings()
        {
            Form form = new Form
            {
                Text = "Выбор типа списка",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(250, 150),
                MaximizeBox = false,
                MinimizeBox = false
            };

            Button btnNumbered = new Button { Text = "Нумерованный", Location = new Point(10, 10), Size = new Size(120, 30) };
            Button btnLettered = new Button { Text = "Буквенный", Location = new Point(10, 50), Size = new Size(120, 30) };
            Button btnUnordered = new Button { Text = "Символьный", Location = new Point(10, 90), Size = new Size(120, 30) };

            Label lblSymbol = new Label { Text = "Символ:", Location = new Point(140, 95), AutoSize = true };
            TextBox txtSymbol = new TextBox { Text = "•", Location = new Point(190, 92), Width = 40 };

            string result = null;

            btnNumbered.Click += (s, e) =>
            {
                result = "1";
                form.DialogResult = DialogResult.OK;
                form.Close();
            };

            btnLettered.Click += (s, e) =>
            {
                result = "2";
                form.DialogResult = DialogResult.OK;
                form.Close();
            };

            btnUnordered.Click += (s, e) =>
            {
                string symbol = txtSymbol.Text.Trim();
                if (!string.IsNullOrEmpty(symbol))
                {
                    result = $"3:{symbol}";
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
                else
                {
                    MessageBox.Show("Введите символ маркера.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            form.Controls.AddRange(new Control[] { btnNumbered, btnLettered, btnUnordered, lblSymbol, txtSymbol });

            DialogResult dialogResult = form.ShowDialog();

            return dialogResult == DialogResult.OK ? result : null;
        }

        public string EditText(string input, string settings = null)
        {
            // TODO если input == null 
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(settings))
                return input;


            // 1. Удалить двойные пустые строки (\n\n → \n)
            input = input.Replace("\r\n", "\n"); // нормализуем окончания строк
            while (input.Contains("\n\n"))
                input = input.Replace("\n\n", "\n");

            string[] lines = input.Split('\n');
            var sb = new StringBuilder();

            int index = 1;
            char letter = 'a';

            string mode = "1"; // по умолчанию — нумерованный
            string symbol = "-"; // для маркера

            if (!string.IsNullOrEmpty(settings))
            {
                if (settings.StartsWith("3:"))
                {
                    mode = "3";
                    symbol = settings.Substring(2);
                }
                else
                {
                    mode = settings;
                }
            }

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string prefix = mode switch
                {
                    "1" => $"{index}. ",
                    "2" => $"{(char)(letter + (index - 1))}. ",
                    "3" => $"{symbol} ",
                    _ => "- "
                };

                sb.AppendLine(prefix + line);
                index++;
            }

            return sb.ToString().TrimEnd(); // удалить последний \n
        }

    }
}
