#region Copyright notice
/*
____   ___.__         __               .__    __________                        .__.__                
\   \ /   |__________/  |_ __ _______  |  |   \______   _____ ____________    __| _|__| ______ ____   
 \   Y   /|  \_  __ \   __|  |  \__  \ |  |    |     ___\__  \\_  __ \__  \  / __ ||  |/  ____/ __ \  
  \     / |  ||  | \/|  | |  |  // __ \|  |__  |    |    / __ \|  | \// __ \/ /_/ ||  |\___ \\  ___/  
   \___/  |__||__|   |__| |____/(____  |____/  |____|   (____  |__|  (____  \____ ||__/____  >\___  > 
                                     \/                      \/           \/     \/        \/     \/  
    This file is part of VPNET Version 1.0

    Copyright (c) 2012-2014 CUBE3 (Cit:36)

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

using System.IO;
using VpNet.CommandLine;
using VpNet.CommandLine.Attributes;
using VpNet.Extensions;
using VpNet.PluginFramework;
using VpNet.PluginFramework.Interfaces;

namespace VpNet.VpConsole.Commands
{
    [Command(Literal="autologin")]
    public class AutoLogin : IParsableCommand<VpPluginContext>
    {
        [BoolFlag(False="disable", True="enable")]
        public bool Enabled { get; set; }
        public static string LoginconfigurationXmlPath = @"loginConfiguration.xml";

        public bool Execute(VpPluginContext context)
        {
            if (Enabled)
            {
                context.Vp.Configuration.Serialize(LoginconfigurationXmlPath);
                context.Cli.WriteLine(ConsoleMessageType.Information,"autologin configuration saved and enabled.");
            }
            else
            {
                if (File.Exists(LoginconfigurationXmlPath))
                    File.Delete(LoginconfigurationXmlPath);
                context.Cli.WriteLine(ConsoleMessageType.Information, "autologin configuration deleted and disabled.");
            }
            return true;
        }
    }
}
