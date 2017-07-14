using System.Collections.Generic;
using VpNet.Abstract;
using VpNet.PluginFramework;

namespace VpNet.Plugins
{
    public class Plugin2 : BaseInstancePlugin
    {

        public override void InitializePlugin(BaseInstanceEvents<World> baseInstance)
        {

        }

        public override PluginDescription Description
        {
            get
            {
                return new PluginDescription()
                {
                    Name = "plugin2",
                    Description = "This is an example plugin which is dependend on plugin1."
                };
            }
        }

        public override List<string> DependentOn
        {
            get { return new List<string> {"plugin1"}; }
        }

        public override void Unload()
        {
        }
    }
}
