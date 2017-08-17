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
using VpNet.VpConsoleServices.PluginFramework.Interfaces;
using VpNet.VpConsoleServices.PluginFramework.Interfaces.IConsoleDelegate;

namespace VpNet.Plugins
{
    /// <summary>
    /// Example of plugin 
    /// </summary>
    public class ConsoleChatPlugin : BaseInstancePlugin
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
            Vp.OnAvatarLeave += OnAvatarLeave;
            Vp.OnChatMessage += OnChatMessage;
        }

        void OnChatMessage(Instance sender, ChatMessageEventArgsT<Avatar<Vector3>, ChatMessage, Vector3, Color> args)
        {
            Console.WriteLine(ConsoleMessageType.Information, string.Format(">{0}{1}", args.ChatMessage.Name.PadRight(17), args.ChatMessage.Message));
        }

        void OnAvatarLeave(Instance sender, AvatarLeaveEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            Console.WriteLine(ConsoleMessageType.Event, string.Format("   *** {0} left {1}.", args.Avatar.Name, sender.Configuration.World.Name));
        }

        void OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            Console.WriteLine(ConsoleMessageType.Event,string.Format("   *** {0} entered {1}.",args.Avatar.Name,sender.Configuration.World.Name));
        }


        public override bool HandleConsoleInput(string input)
        {
            if (input=="/c")
            {
                // go into chat mode. redirect command processing.
                Console.ParseCommandLine = ProcessCommandLine;
                Console.GetPromptTarget = ChatPrompt;
                return true;
            }
            return false;
        }

        private void ProcessCommandLine(string command)
        {
            if (command == "/x")
            {
                // exit chat mode, releaset command processing and return to previous processor.
               Console.RevertPrompt();
            }
            else
            {
                Vp.Say(command);
            }
            Console.ReadLine();
        }

        private string ChatPrompt()
        {
            return "> ";
        }

        override public PluginDescription Description
        {
            get
            {
                return new PluginDescription()
                {
                    Name = "chatconsole",
                    Description = "This plugin allows you to chat from the console."
                };
            }
        }

        public override void Unload()
        {
            Vp.OnAvatarEnter -= OnAvatarEnter;
            Vp.OnAvatarLeave -= OnAvatarLeave;
            Vp.OnChatMessage -= OnChatMessage;
        }
    }
}
