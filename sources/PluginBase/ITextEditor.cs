using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PluginBase
{
    public interface ITextEditor : IPlugin
    {
        string EditText(string input, string settings = null);
    }
}
