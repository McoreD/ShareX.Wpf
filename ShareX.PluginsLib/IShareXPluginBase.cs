using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.PluginsLib
{
    public interface IShareXPluginBase
    {
        string Name { get; }
        string Publisher { get; }
    }
}