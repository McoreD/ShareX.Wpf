using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLib
{
    public interface IPluginBase
    {
        string Name { get; set; }
        string Publisher { get; }
    }
}