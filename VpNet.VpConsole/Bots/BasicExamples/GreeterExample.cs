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

namespace VpNet.VpConsole
{
    public class GreeterExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldExample"/> class.
        /// The events example shows how to receive events from the world and universe server.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="botname">The name of the bot.</param>
        /// <param name="world">The world to world to enter.</param>
        public GreeterExample(string user, string password, string botname, string world)
        {
            var vp = new Instance();
            vp.ConnectAsync();
            vp.LoginAsync(user, password, botname);
            vp.EnterAsync(world);
            // announce your avatar so it can receive avatar events.
            vp.UpdateAvatar();

            // Register to VP events
            vp.OnAvatarLeave += vp_OnAvatarLeave;
            vp.OnAvatarEnter += vp_OnAvatarEnter;

            Console.WriteLine("Press any key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    vp.Wait();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);


        }

        void vp_OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar> args)
        {
            sender.ConsoleMessage(string.Format("*** {0} enters.", args.Avatar.Name), new Color(0, 0, 128));
        }

        void vp_OnAvatarLeave(Instance sender, AvatarLeaveEventArgsT<Avatar> args)
        {
            sender.ConsoleMessage(string.Format("*** {0} left.", args.Avatar.Name), new Color(0, 0, 128));
        }

    }
}
