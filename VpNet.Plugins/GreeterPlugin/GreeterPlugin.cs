#region Copyright notice
/*
____   ___.__         __               .__    __________                        .__.__                
\   \ /   |__________/  |_ __ _______  |  |   \______   _____ ____________    __| _|__| ______ ____   
 \   Y   /|  \_  __ \   __|  |  \__  \ |  |    |     ___\__  \\_  __ \__  \  / __ ||  |/  ____/ __ \  
  \     / |  ||  | \/|  | |  |  // __ \|  |__  |    |    / __ \|  | \// __ \/ /_/ ||  |\___ \\  ___/  
   \___/  |__||__|   |__| |____/(____  |____/  |____|   (____  |__|  (____  \____ ||__/____  >\___  > 
                                     \/                      \/           \/     \/        \/     \/  
    This file is part of VPNET Version 1.0

    Copyright (c) 2012-2016 CUBE3 (Cit:36)

    VPNET is free software: you can redistribute it and/or modify it under the terms of the 
    GNU Lesser General Public License (LGPL) as published by the Free Software Foundation, either
    version 2.1 of the License, or (at your option) any later version.

    VPNET is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; without even
    the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the LGPL License
    for more details.

    You should have received a copy of the GNU Lesser General Public License (LGPL) along with VPNET.
    If not, see <http://www.gnu.org/licenses/>. 
*/
#endregion

using VpNet.Abstract;
using VpNet.PluginFramework;

namespace VpNet.Plugins
{
    /// <summary>
    /// Example of plugin 
    /// </summary>
    public class GreeterPlugin : BaseInstancePlugin
    {
        /// <summary>
        /// Initializes the plugin as a child plugin of the main instance.
        /// The plugin assumes the masterbot has already entered a world.
        /// </summary>
        /// <param name="baseInstance">The base instance.</param>
        public override void InitializePlugin(BaseInstanceEvents<World> baseInstance)
        {
            Vp = new Instance(baseInstance);
            Vp.OnAvatarEnter += OnAvatarEnter;
        }

        void OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            Vp.ConsoleMessage(
                args.Avatar, "Immigration",
                string.Format("{0} welcome to {1}.", args.Avatar.Name, sender.Configuration.World.Name)
                , new Color(0, 0, 192), TextEffectTypes.Bold);
        }

        override public PluginDescription Description
        {
            get
            {
                return new PluginDescription()
                           {
                               Name = "gbot",
                               Description = "This plugin greets people as they enter a world."
                           };
            }
        }

        public override void Unload()
        {
            Vp.OnAvatarEnter -= OnAvatarEnter;
        }
    }
}
