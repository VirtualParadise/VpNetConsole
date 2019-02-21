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
    /// <summary>
    /// This examples shows how to teleport an avatar and how to receive the Teleport event.
    /// The bot will teleport a user to a location in the world upon entry.
    /// </summary>
    public class TeleportExample
    {
        private readonly string _world;

        public TeleportExample(string user, string password, string botname, string world)
        {
            _world = world;
            var vp = new Instance();
            vp.ConnectAsync();
            vp.LoginAsync(user, password, botname);
            vp.EnterAsync(world);
            vp.UpdateAvatar();
            vp.OnChatMessage += vp_OnChatMessage;
            vp.OnTeleport += vp_OnTeleport;
            vp.OnAvatarEnter += vp_OnAvatarEnter;

            Console.WriteLine("Press any key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    vp.Wait();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);


            // leave the world.
            vp.Leave();

        }

        void vp_OnChatMessage(Instance sender, ChatMessageEventArgsT<Avatar, ChatMessage> args)
        {
            if (args.ChatMessage.Message == "!teleport")
            {
                args.Avatar.Position = new Vector3(100, 0, 100);
                args.Avatar.Rotation = new Vector3(90, 0, 0);
                sender.TeleportAvatar(args.Avatar);
            }
        }

        void vp_OnTeleport(Instance sender, TeleportEventArgsT<Teleport<World, Avatar>, World, Avatar> args)
        {
            Console.WriteLine("{0} teleported to location {1},{2},{3} yaw {4} pitch {5}.",
                args.Teleport.Avatar.Name,
                args.Teleport.Position.X,
                args.Teleport.Position.Y,
                args.Teleport.Position.Z,
                args.Teleport.Rotation.X,
                args.Teleport.Rotation.Y
                );
        }

        void vp_OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar> args)
        {
            Console.WriteLine("{0} entered. Beginning teleport");
            args.Avatar.Position = new Vector3(100, 0, 100);
            args.Avatar.Rotation = new Vector3(0, 90, 0);
            sender.TeleportAvatar(args.Avatar);
        }
    }
}
