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

using System;
using VpNet.Extensions;

namespace VpNet.VpConsole
{
    /// <summary>
    /// The events example shows how to receive events from the world and universe server.
    /// </summary>
    public class EventsExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldExample"/> class.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="botname">The name of the bot.</param>
        /// <param name="world">The world to world to enter.</param>
        public EventsExample(string user, string password, string botname, string world)
        {
            var vp = new Instance();
            vp.ConnectAsync();
            vp.LoginAsync(user,password,botname);
            vp.EnterAsync(world);
            // announce your avatar so it can receive avatar events.
            vp.UpdateAvatar();
            // Register to VP events
            vp.OnObjectChange += vp_OnObjectChange;
            vp.OnChatMessage += vp_OnChatMessage;
            vp.OnObjectClick += vp_OnObjectClick;
            vp.OnObjectBump += vp_OnObjectBump;

            Console.WriteLine("Press any key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    vp.Wait();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                    

        }

        void vp_OnObjectBump(Instance sender, ObjectBumpArgsT<Avatar, VpObject> args)
        {
            Console.WriteLine("Object bump received.\r\n{0}", args.Serialize());            
        }

        void vp_OnObjectClick(Instance sender, ObjectClickArgsT<Avatar, VpObject> args)
        {
            Console.WriteLine("Object click received.\r\n{0}", args.Serialize());
        }

        /// <summary>
        /// Called when a chat message arrives.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        void vp_OnChatMessage(Instance sender, ChatMessageEventArgsT<Avatar, ChatMessage> args)
        {
            // Write a serialized version showing all the data of the chat message event using VpNet.Extension methods.
            Console.WriteLine("Chat message received.\r\n{0}",args.Serialize());
            if (args.ChatMessage.Message.ToLower().StartsWith("!consoletest"))
            {
                // test a console message in pretty pink :)
                sender.ConsoleMessage("Hello", "World",new Color(255,64,128));
            }
        }

        /// <summary>
        /// Called when an object is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        void vp_OnObjectChange(Instance sender, ObjectChangeArgsT<Avatar, VpObject> args)
        {
            // Write a serialized version showing all the data of object changed event using VpNet.Extension methods.
            Console.WriteLine("Object changed received.\r\n{0}", args.Serialize());
        }
    }
}
