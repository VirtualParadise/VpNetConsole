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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace VpNet.VpConsole
{
    public class InterWorldChat{
        private readonly string _user;
        private readonly string _password;
        private readonly string _botname;

        private List<Instance> _instances;
        private Timer _t;
        private Timer _t2;
        private List<World> _worlds; 

        public InterWorldChat(string user, string password, string botname)
        {
            _user = user;
            _password = password;
            _botname = botname;
            _instances = new List<Instance>();
            _instances.Insert(0, new Instance());
            _instances[0].ConnectAsync();
            _instances[0].LoginAsync(user, password, botname);
            _instances[0].OnWorldList += InterWorldChat_OnWorldList;


            _worlds = new List<World>();
            _instances[0].ListWorlds();
            _t = new Timer(Wait,this,0,30);
        }

        void InterWorldChat_OnWorldList(Instance sender, WorldListEventArgsT<World> args)
        {
            if (_t2 != null)
                _t2.Dispose();
            _t2 = new Timer(WorldListTimer,null,1000,0);
            //if (args.World.Name == "Blizzard")
            //    return;
            _worlds.Add(args.World);
        }

        private void WorldListTimer(object state)
        {
            _t.Dispose();
            Debug.WriteLine("end of list");
            _t2.Dispose();
            if (_worlds.Count == 0)
                return;
            _instances[0].OnWorldList -= InterWorldChat_OnWorldList;
            _instances[0].OnChatMessage += vp_OnChatMessage;
            _instances[0].OnAvatarEnter += InterWorldChat_OnAvatarEnter;
            _instances[0].OnAvatarLeave += InterWorldChat_OnAvatarLeave;
            _instances[0].EnterAsync(_worlds[0]);
            _instances[0].ConsoleMessage("interchat", "inter world chat is now online", new Color(192, 0, 0));
            _instances[0].UpdateAvatar();
            foreach (var world in _worlds.Skip(1))
            {
                lock (this)
                {
                    _instances.Insert(0, new Instance());
                    _instances[0].OnChatMessage += vp_OnChatMessage;
                    _instances[0].ConnectAsync();
                    _instances[0].LoginAsync(_user, _password, _botname);
                    _instances[0].EnterAsync(world.Name);
                    _instances[0].ConsoleMessage("interchat", "inter world chat is now online", new Color(192, 0, 0));
                    _instances[0].UpdateAvatar();
                    _instances[0].OnAvatarEnter += InterWorldChat_OnAvatarEnter;
                    _instances[0].OnAvatarLeave += InterWorldChat_OnAvatarLeave;
                  
                }
            }
            _t = new Timer(Wait,this,30,30);
        }

        void InterWorldChat_OnAvatarLeave(Instance sender, AvatarLeaveEventArgsT<Avatar> args)
        {
            foreach (var vp in _instances)
            {
                if (vp.Configuration.World.Name != sender.Configuration.World.Name)
                {
                    vp.ConsoleMessage(_botname,
                        string.Format("[{0}] has left {1}",args.Avatar.Name, sender.Configuration.World.Name), new Color(128, 32, 128));
                  
                }
            }
        }

        void InterWorldChat_OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar> args)
        {
            foreach (var vp in _instances)
            {
                if (vp.Configuration.World.Name != sender.Configuration.World.Name)
                {
                    vp.ConsoleMessage(_botname,
                        string.Format("[{0}] has entered {1}", args.Avatar.Name, sender.Configuration.World.Name), new Color(128,32,128));
                  
                }
            }
        }

        private void Wait(object state)
        {
            foreach (var vp in _instances)
            {
                vp.Wait(0);
            }
        }

        void vp_OnChatMessage(Instance sender, ChatMessageEventArgsT<Avatar, ChatMessage> args)
        {
            if ((args.ChatMessage.Type == ChatMessageTypes.Console && args.Avatar.Name=="interchat"))
                return;
            foreach (var vp in _instances)
            {
                if (vp.Configuration.World.Name != sender.Configuration.World.Name)
                {
                    vp.ConsoleMessage(args.ChatMessage.Name, 
                        string.Format("[{0}] {1}",sender.Configuration.World.Name,args.ChatMessage.Message), new Color(64, 64, 64));
                    vp.Wait();
                }    
            }
           
        }
    }
}
