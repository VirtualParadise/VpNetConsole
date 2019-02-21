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
using VpNet.VpConsoleServices.Abstract;
using VpNet.VpConsoleServices.PluginFramework;

namespace VpNet.Plugins
{
    /// <summary>
    /// Example of plugin 
    /// </summary>
    public class UrlPlugin : BaseInstancePlugin
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
            Vp.OnChatMessage += OnChatMessage;
        }

        void OnChatMessage(Instance sender, ChatMessageEventArgsT<Avatar, ChatMessage> args)
        {
            if (args.ChatMessage.Message.ToLower().StartsWith("//sendurl"))
            {
                var items = args.ChatMessage.Message.Split(' ');
                if (items.Length<2 || items.Length>2)
                {
                    sender.ConsoleMessage(args.Avatar, "sendurl", "Please use in the form of: //sendurl <yoururl>",new Color(255,0,0),TextEffectTypes.Bold);
                }
                else
                {
                    sender.UrlSendOverlay(args.Avatar, items[1]);
                }
            }
        }

        void OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar> args)
        {
            //sender.UrlSendOverlay(args.Avatar, "http://homepage.ntlworld.com/ray.hammond/compass/");
        }

        override public PluginDescription Description
        {
            get
            {
                return new PluginDescription()
                {
                    Name = "url",
                    Description = "This plugin pushes a url overlay on top of the 3d world view."
                };
            }
        }

        public override void Unload()
        {
            Vp.OnAvatarEnter -= OnAvatarEnter;
            Vp.OnChatMessage -= OnChatMessage;
        }
    }
}
