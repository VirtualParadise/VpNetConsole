using VpNet.Abstract;
using VpNet.VpConsoleServices.Abstract;
using VpNet.VpConsoleServices.PluginFramework;

namespace VpNet.Plugins
{
    public class Plugin1 : BaseInstancePlugin
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
                    Name = "plugin1",
                    Description = "This is an example plugin on which plugin2 is dependent."
                };
            }
        }

        public override void Unload()
        {
            
        }
    }
}
