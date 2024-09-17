using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Size
{
    public class Config : IConfig
    {
        [Description("Should the plugin be active?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Specifies whether debugging messages will be displayed.")]
        public bool Debug { get; set; } = false;
    }
}
