using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VpNet.Abstract;
using VpNet.Extensions;
using VpNet.NativeApi;
using VpNet.VpConsoleServices.PluginFramework;
using VpNet.VpConsoleServices.PluginFramework.Interfaces;
using VpNet.VpConsole.Commands;
using VpNet.VpConsole.Gui;
using VpNet.VpConsoleServices.Abstract;

namespace VpNet.ManagedApi.System.ConsoleEx
{
    public class ProgramInstance
    {
        private VpPluginContext _context = new VpPluginContext();
        private Instance Vp;
        private ConsoleHelpers Cli = new ConsoleHelpers();
        public string _userName;
        private string _password;
        private string _world;

        public ProgramInstance()
        {
            Vp = new Instance();
            _context.Cli = Cli;
            _context.Plugins = new HotSwapPlugins<BaseInstancePlugin>();
            _context.Plugins.OnPluginUnloaded += _plugins_OnPluginUnloaded;
            Console.BufferHeight = 9999;
            Console.Title = "Virtual Paradise Console";
            Console.CursorSize = 100;
            Console.SetWindowSize(120, 40);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
____   ____.__         __               .__    __________                          .___.__               
\   \ /   /|__|_______/  |_ __ _______  |  |   \______   \_____ ____________     __| _/|__| ______ ____  
 \   Y   / |  \_  __ \   __\  |  \__  \ |  |    |     ___/\__  \\_  __ \__  \   / __ | |  |/  ___// __ \ 
  \     /  |  ||  | \/|  | |  |  // __ \|  |__  |    |     / __ \|  | \// __ \_/ /_/ | |  |\___ \\  ___/ 
   \___/   |__||__|   |__| |____/(____  /____/  |____|    (____  /__|  (____  /\____ | |__/____  >\___  >
                                      \/                       \/           \/      \/         \/ SDK \/ 
");
            Console.ForegroundColor = ConsoleColor.Gray;
           // Console.WriteLine("VP SDK Version: {0}", System.Reflection.Assembly.GetAssembly(typeof(Instance)).GetName().Version.ToString());
           // Console.WriteLine("VP Console Version: {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
           // Console.WriteLine("Copyright (c) 2012-2016 CUBE3 (Cit:36) under LGPL license\n");
        }

        public async Task Run()
        {
            await Connect();
        }

        private async Task Reset()
        {
            // ignore any other exceptions (system wide)
            RcDefault.IgnoreExceptions = true;
            // turn of system wide exception handling.
            RcDefault.OnVpException -= RcDefault_OnVpException;
            // unload all active plugins
            foreach (var plugin in _context.Plugins.ActivePlugins())
            {
                _context.Plugins.Deactivate(plugin);
            }
            RcDefault.IgnoreExceptions = false;
            // attempt to reconnect
            _isLoadedFromConfiguration = false;
            Vp.Configuration.IsChildInstance = false;
            Vp.Dispose();
            Vp = new Instance();
            await Connect();
        }

        /// <summary>
        /// System wide exception handling.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void RcDefault_OnVpException(Interfaces.IRc sender, EventArgs args)
        {
            Cli.WriteLine(ConsoleMessageType.Error, sender.Exception.Message);
            switch (sender.Exception.Reason)
            {
                case ReasonCode.NotInUniverse:
                case ReasonCode.ConnectionError:
                case ReasonCode.NotInWorld:
                    Reset();
                    break;
                default:
                    // no further action taken.
                    break;
            }
        }

        private void _plugins_OnPluginUnloaded(HotSwapPlugins<BaseInstancePlugin> sender, PluginUnloadedArguments<BaseInstancePlugin> args)
        {
            args.NewInstance.InitializePlugin(Vp);
            Cli.WriteLine(ConsoleMessageType.Information, string.Format("Plugin {0} reinitialized by dll replacement.", args.NewInstance.Description.Name));
            _context.Plugins.Activate(args.NewInstance);
        }

        private void AutoReconnect()
        {
            if (IsAutoReconnect)
            {
                Cli.WriteLine(ConsoleMessageType.Error, "Can't connect to universe. Reconnecting in 10 seconds...");
                // start a reconnect timer.
                new Timer(Reconnect, null, 10000, 0);
            }
            else
            {
                Cli.WriteLine(ConsoleMessageType.Error, "Can't connect to universe.");
                Cli.GetPromptTarget = RetryPrompt;
                Cli.ParseCommandLine = RetryuniverseConnect;
                Cli.ReadLine();
            }
        }

        private async Task Connect()
        {
            Cli.WriteLine(ConsoleMessageType.Information, "Connecting...");
            try
            {
                await Vp.ConnectAsync();
                Cli.WriteLine(ConsoleMessageType.Information, "Connected to universe.\n");
                if (File.Exists(AutoLogin.LoginconfigurationXmlPath))
                {
                    Cli.WriteLine(ConsoleMessageType.Information, "Autologin configuration enabled, attempting auto logon.");
                    var config = SerializableExtensions.Deserialize<InstanceConfiguration<World>>(AutoLogin.LoginconfigurationXmlPath);
                    try
                    {
                        await Vp.LoginAsync(config.UserName, config.Password, config.BotName);
                        await Vp.EnterAsync(config.World.Name);
                        _world = config.World.Name;
                        Vp.UpdateAvatar();
                        ProceedAfterLogin(true);

                    }
                    catch (VpException ex)
                    {
                        if (ex.Reason == ReasonCode.NotInUniverse)
                        {
                            // strange native vpsdk bug, after reating a new native instance, upon first time 
                            // the sdk does not seem to connect properly.
                            await Connect();
                        }
                        Cli.WriteLine(ConsoleMessageType.Information,
                            "Autologin failed. Reason" + ex.Reason.ToString() + " " + ex.Message);
                        AutoReconnect();
                    }
                }
                else
                {
                    Cli.GetPromptTarget = LoginPrompt;
                    Cli.ParseCommandLine = ProcessUserName;
                    Cli.ReadLine();
                }
            }
            catch (VpException ex)
            {
                AutoReconnect();
            }
        }

        private async void Reconnect(object state)
        {
            await Connect();
        }

        protected bool IsAutoReconnect = true;

        private void ProcessUserName(string userName)
        {
            _userName = userName;
            Cli.GetPromptTarget = PasswordPrompt;
            Cli.ParseCommandLine = ProcessPassword;
            Cli.IsMaskedInput = true;
            Cli.ReadLine();
        }

        private void ProcessPassword(string password)
        {
            _password = password;
            Cli.IsMaskedInput = false;
            try
            {
                Vp.LoginAsync(_userName, password, "vpnetconsole").GetAwaiter().GetResult();
            }
            catch (VpException ex)
            {
                Cli.WriteLine(ConsoleMessageType.Error, ex.Message);
                Cli.GetPromptTarget = LoginPrompt;
                Cli.ParseCommandLine = ProcessUserName;
                Cli.ReadLine();
                return;
            }
            ProceedAfterLogin(false);
        }

        private void ProceedAfterLogin(bool enteredWorld)
        {
            Cli.WriteLine(ConsoleMessageType.Information, "Logged into universe server");
            Cli.WriteLine(ConsoleMessageType.Information, "Retrieving world list.\r\n");
            Vp.OnUniverseDisconnect += Vp_OnUniverseDisconnect;
            Vp.OnWorldList += Vp_OnWorldList;
            Vp.OnAvatarEnter += Vp_OnAvatarEnter;
            Vp.OnAvatarLeave += Vp_OnAvatarLeave;
            Vp.ListWorlds();
            if (enteredWorld)
            {
                Cli.GetPromptTarget = WorldPrompt;
                Cli.ParseCommandLine = ProcessCommand;
                // once logged in enable system wide exception handling.
                RcDefault.OnVpException += RcDefault_OnVpException;
                LoadPlugins();
            }
            else
            {
                Cli.GetPromptTarget = EnterWorldPrompt;
                Cli.ParseCommandLine = ProcessEnterWorld;
            }
            Cli.ReadLine();
        }

        private async void Vp_OnUniverseDisconnect(Instance sender, UniverseDisconnectEventArgs args)
        {
            Cli.WriteLine(ConsoleMessageType.Error, "Disconnected from universe, reconnecting.");
            await Reset();
        }

        private void Vp_OnAvatarLeave(Instance sender, AvatarLeaveEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            Cli.WriteLine(ConsoleMessageType.Event, "   *** " + args.Avatar.Name + " left.");
        }

        private void Vp_OnAvatarEnter(Instance sender, AvatarEnterEventArgsT<Avatar<Vector3>, Vector3> args)
        {
            Cli.WriteLine(ConsoleMessageType.Event, "   *** " + args.Avatar.Name + "(" + args.Avatar.Session + ") enters.");
        }

        private string EnterWorldPrompt()
        {
            return "[" + DateTime.Now.ToShortTimeString() + " Enter World>: ";
        }

        private void ProcessEnterWorld(string world)
        {
            _world = world;
            try
            {
                Vp.EnterAsync(world).GetAwaiter().GetResult();
            }
            catch (VpException ex)
            {
                Cli.WriteLine(ConsoleMessageType.Error, ex.Message);
                Cli.ReadLine();
                return;
            }
            // once logged in enable system wide exception handling.
            RcDefault.OnVpException += RcDefault_OnVpException;

            Vp.UpdateAvatar();
            Cli.GetPromptTarget = WorldPrompt;
            Cli.ParseCommandLine = ProcessCommand;
            LoadPlugins();
            Cli.ReadLine();
        }

        private static bool _isLoadedFromConfiguration = false;

        private void LoadPlugins()
        {
            if (_isLoadedFromConfiguration || !File.Exists(@"pluginConfiguration.xml"))
                return;

            foreach (var item in _context.Plugins.LoadConfiguration(@"pluginConfiguration.xml"))
            {
                var plugin = _context.Plugins.Instances.Find(p => p.Description.Name.ToLower() == item.Name.ToLower());
                if (plugin == null)
                {
                    Cli.WriteLine(ConsoleMessageType.Error, string.Format("Plugin named {0} not found. Can't load.", item.Name));

                }
                else
                {
                    plugin.Console = Cli;
                    plugin.InitializePlugin(Vp);
                    _context.Plugins.Activate(plugin);
                    Cli.WriteLine(ConsoleMessageType.Information, string.Format("Plugin named {0} initialized from configuration.", item.Name));
                }
            }

            _isLoadedFromConfiguration = true;
        }

        private void ProcessCommand(string command)
        {
            // vp instance context is switchable (multiple instances in different worlds), therefore we need to provide it.
            bool isHandled = false;
            _context.Vp = Vp;
            var result = _context.Cmd.Parse(command);
            if (result == null)
            {
                switch (command.ToLower())
                {
                    case "enter":
                        Vp.Leave();
                        Cli.GetPromptTarget = EnterWorldPrompt;
                        Cli.ParseCommandLine = ProcessEnterWorld;
                        isHandled = true;
                        break;
                    case "list plugins":
                        foreach (var plugin in _context.Plugins.Instances)
                        {
                            Cli.WriteLine(ConsoleMessageType.Information,
                                            plugin.Description.Name.PadRight(20) + " : " + plugin.Description.Description);
                        }
                        isHandled = true;
                        break;
                    default:
                        // check if a plugin can handle the command.
                        foreach (var plugin in _context.Plugins.ActivePlugins())
                        {
                            if (plugin.HandleConsoleInput(command))
                            {
                                isHandled = true;
                                break;
                            }
                        }
                        break;

                }
            }
            else
            {
                isHandled = result.Execute(_context);
            }
            if (!isHandled)
                Cli.WriteLine(ConsoleMessageType.Error, "?Unkonwn Syntax Error.");
            Cli.ReadLine();
        }

        private void Vp_OnWorldList(Instance sender, WorldListEventArgs args)
        {
            Cli.WriteLine(ConsoleMessageType.Event, "   -> " + args.World.Name + " (" + args.World.UserCount + " users)");
        }

        private void RetryuniverseConnect(string yesno)
        {
            if (yesno.ToLower() == "y")
            {
                Connect();
            }
        }

        private string WorldPrompt()
        {
            return "[" + DateTime.Now.ToShortTimeString() + " " + _world + ">: ";
        }

        private string Prompt()
        {
            return "[" + DateTime.Now.ToShortTimeString() + ">: ";
        }

        private string LoginPrompt()
        {
            return "Login: ";
        }

        private string PasswordPrompt()
        {
            return "Password: ";
        }

        private string RetryPrompt()
        {
            return "Retry (Y/N): ";
        }

    }
}
