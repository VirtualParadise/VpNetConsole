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
using Attribute = VpNet.VpConsole.Attribute;

namespace VpNet.VpConsole
{
    /// <summary>
    /// This example shows how to pass your own Avatar object type to a VP Instance using templating.
    /// </summary>
    public class InstanceTemplating
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceTemplating"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="botname">The botname.</param>
        /// <param name="world">The world.</param>
        public InstanceTemplating(string user, string password, string botname, string world)
        {
            var vp = new Instance<RcDefault, RpgAvatar>();

            vp.OnAvatarEnter += OnAvatarEnter;
            vp.OnObjectClick += OnObjectClick;
            vp.ConnectAsync();
            vp.LoginAsync(user, password, botname);
            vp.EnterAsync(world);
            // announce your avatar so it can receive avatar events.
            vp.UpdateAvatar();

            // create a child instance with a different avatar template.
            var vpCasino = new Instance<RcDefault, CasinoAvatar>(vp);
            vpCasino.OnAvatarEnter += vpCasino_OnAvatarEnter;
            
   

            Console.WriteLine("Press any key to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    vp.Wait();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        void vpCasino_OnAvatarEnter(Instance<RcDefault, CasinoAvatar> sender, AvatarEnterEventArgsT<CasinoAvatar> args)
        {
            args.Avatar.Credits = 1000;
            sender.Commit(args.Avatar);
            sender.ConsoleMessage(args.Avatar, string.Empty,
                                 string.Format("*** Welcome to casino you now have {0} credits.",args.Avatar.Credits), TextEffectTypes.Bold, 0, 0, 128);
        }

        void OnObjectClick(Instance<RcDefault, RpgAvatar> sender, ObjectClickArgsT<RpgAvatar, VpObject> args)
        {
            var strength = args.Avatar.Attributes.Find(p => p.Type == "strength");
            if (strength.Level<0)
            {
                sender.ConsoleMessage(args.Avatar, string.Empty,
                                 string.Format("*** You have no strength left to hit the object {0}",
                                               strength.Level), TextEffectTypes.Bold, 128, 0, 0);
                return;
            }

            sender.ConsoleMessage(args.Avatar, string.Empty,
                                  string.Format("*** You hit the object with a strength of {0}",
                                                strength.Level),TextEffectTypes.Bold,0,0,128);
            strength.Level -= 0.01f;
            sender.Commit(args.Avatar);
        }

        void OnAvatarEnter(Instance<RcDefault, RpgAvatar> sender, AvatarEnterEventArgsT<RpgAvatar> args)
        {
            sender.ConsoleMessage(args.Avatar, string.Empty,
                                 string.Format("*** Welcome to rpg game, we are going to assign you attributes", TextEffectTypes.Bold, 0, 0, 128));
            // create traits with a random value.
            var r = new Random();
            
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="strength"});
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="dexterity"});
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="constitution"});
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="intelligence"});
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="wisdom"});
            args.Avatar.Attributes.Add(new Attribute(){Level=r.NextDouble(),Type="charisma"});
            // commit the changes to the instance, note that only extended fields declared specifically within
            // rpg avatar will be committed. Other properties that are managed by the VPNET sdk will not be
            // overwitten.
            sender.Commit(args.Avatar);
        }
    }
}
