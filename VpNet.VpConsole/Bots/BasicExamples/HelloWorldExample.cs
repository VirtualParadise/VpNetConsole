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

namespace VpNet.VpConsole
{
    public class HelloWorldExample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelloWorldExample"/> class.
        /// The hello world example is one of the most basic examples and uses vp instance directly.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="botname">The name of the bot.</param>
        /// <param name="world">The world to world to enter.</param>
        public HelloWorldExample(string user, string password, string botname, string world)
        {
            var vp = new Instance();
            vp.ConnectAsync();
            vp.LoginAsync(user,password,botname);
            vp.EnterAsync(world);
            // send a standard black hellow world console message.
            vp.ConsoleMessage("Hello World standard black console message");
            // send a red hello world console message.
            vp.ConsoleMessage("Hello World red console message",new Color(255,0,0));
            // send a blue bold hello world message.
            vp.ConsoleMessage("Hello World blue bold console message", new Color(0, 0, 255),TextEffectTypes.Bold);
            // send the commands to the world server by calling wait.
            vp.Wait();
            // call this to announce the avatar in the world. This is needed to actually 
            // Say something in the chat room rather than sending a consolemessage/
            vp.UpdateAvatar();
            // send a standard user chat message.
            vp.Say("Hello world! standard chat message");
            // send the commands to the world server by calling wait.
            vp.Wait();
            // leave the world.
            vp.Leave();
        }
    }
}
