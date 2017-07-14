using System.ComponentModel.Composition;
using VpNet.Abstract;

namespace VpNet.Examples
{
    /// <summary>
    /// Example of plugin architecture using Microsoft .NET MEF Framework.
    /// </summary>
    [Export(typeof(BaseInstancePlugin))]
    public class InstancePlugin : BaseInstancePlugin
    {
        private Instance _vp;

        /// <summary>
        /// Initializes the plugin as a child plugin of the main instance.
        /// The plugin assumes the masterbot has already entered a world.
        /// </summary>
        /// <param name="baseInstance">The base instance.</param>
        public override void InitializePlugin(BaseInstanceEvents<World> baseInstance)
        {
            _vp = new Instance(baseInstance);
            _vp.ConsoleMessage(string.Format(
                "[Export(typeof(IInstancePlugin))] MEF Intialized Plugin in world {0}", _vp.Configuration.World.Name));
            _vp.OnAvatarEnter += OnAvatarEnter;
        }

        void OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            _vp.ConsoleMessage(string.Format("[Export(typeof(IInstancePlugin))] MEF Intialized Plugin says hello to {0}", args.Avatar.Name));
        }
    }
}
