/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using Autofac;
using GloomyTale.Communication.RPC;
using GloomyTale.Master;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Exceptions;
using Grpc.Core;
using log4net;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.DAL.EF.Helpers;
using OpenNos.Data;
using OpenNos.GameObject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpenNos.Master
{
    internal static class Program
    {
        #region Members

        private static readonly ManualResetEvent _run = new ManualResetEvent(true);

        private static bool _isDebug;

        #endregion

        #region Methods
        private static IContainer InitializePlugins()
        {
            var pluginBuilder = new ContainerBuilder();IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            coreBuilder.RegisterAssemblyTypes(typeof(Program).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
            coreBuilder.RegisterAssemblyTypes(typeof(MasterCommunicator).Assembly).AsSelf().AsImplementedInterfaces().SingleInstance();
            foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                try
                {
                    plugin.OnLoad(coreBuilder);
                }
                catch (PluginException e)
                {
                }
            }

            //coreBuilder.Register(_ => new ToolkitMapper()).As<IMapper>().SingleInstance();
            return coreBuilder.Build();
        }
        public static void Main(string[] args)
        {
            try
            {
#if DEBUG
                _isDebug = true;
#endif
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                Console.Title = $"OpenNos Master Server{(_isDebug ? " Development Environment" : "")}";

                bool ignoreStartupMessages = false;
                bool ignoreTelemetry = false;
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "--nomsg":
                            ignoreStartupMessages = true;
                            break;

                        case "--notelemetry":
                            ignoreTelemetry = true;
                            break;
                    }
                }

                // initialize Logger
                Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

                int port = 4545;
                if (!ignoreStartupMessages)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string text = $"MASTER SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port} by OpenNos Team";
                    int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                    string separator = new string('=', Console.WindowWidth);
                    Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                }

                // initialize DB
                if (!DataAccessHelper.Initialize())
                {
                    Console.ReadLine();
                    return;
                }

                Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                try
                {
                    using (IContainer coreContainer = InitializePlugins())
                    {
                        // configure Services and Service Host
                        string ip = "82.165.19.227";
                        var server = new Server
                        {
                            Services = { global::Master.BindService(coreContainer.Resolve<MasterImpl>()) },
                            Ports = { new ServerPort(ip, port, ServerCredentials.Insecure) }
                        };
                        Logger.Log.Info($"[RPC-SERVER] Listening on {ip}:{port}");
                        server.Start();
                        Logger.Info(Language.Instance.GetMessageFromKey("STARTED"));
                        if (!ignoreTelemetry)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("General Error Server", ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("General Error", ex);
                Console.ReadKey();
            }
        }

        #endregion
    }
}