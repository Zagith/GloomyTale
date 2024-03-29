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

using log4net;
using OpenNos.Core;
using OpenNos.DAL.EF.Helpers;
using OpenNos.GameObject;
using OpenNos.Handler;
using OpenNos.Master.Library.Client;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace OpenNos.Login
{
    public static class Program
    {
        #region Members

        private static bool _isDebug;

        private static int _port;

        #endregion

        #region Methods

        public static void Main(string[] args)
        {
            checked
            {
                try
                {
#if DEBUG
                    _isDebug = true;
#endif
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                    Console.Title = $"OpenNos Login Server{(_isDebug ? " Development Environment" : "")}";

                    bool ignoreStartupMessages = false;
                    foreach (string arg in args)
                    {
                        ignoreStartupMessages |= arg == "--nomsg";
                    }

                    // initialize Logger
                    Logger.InitializeLogger(LogManager.GetLogger(typeof(Program)));

                    int port = Convert.ToInt32(ConfigurationManager.AppSettings["LoginPort"]);
                    int portArgIndex = Array.FindIndex(args, s => s == "--port");
                    if (portArgIndex != -1
                        && args.Length >= portArgIndex + 1
                        && int.TryParse(args[portArgIndex + 1], out port))
                    {
                        Console.WriteLine("Port override: " + port);
                    }
                    _port = port;
                    if (!ignoreStartupMessages)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                        string text = $"LOGIN SERVER v{fileVersionInfo.ProductVersion}dev - PORT : {port} by OpenNos Team";
                        int offset = (Console.WindowWidth / 2) + (text.Length / 2);
                        string separator = new string('=', Console.WindowWidth);
                        Console.WriteLine(separator + string.Format("{0," + offset + "}\n", text) + separator);
                    }

                    // initialize api
                    if (CommunicationServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]))
                    {
                        Logger.Info(Language.Instance.GetMessageFromKey("API_INITIALIZED"));
                    }

                    // initialize DB
                    if (!DataAccessHelper.Initialize())
                    {
                        Console.ReadKey();
                        return;
                    }

                    Logger.Info(Language.Instance.GetMessageFromKey("CONFIG_LOADED"));

                    try
                    {
                        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("General Error", ex);
                    }

                    try
                    {
                        // initialize PacketSerialization
                        PacketFactory.Initialize<WalkPacket>();

                        NetworkManager<LoginCryptography> networkManager = new NetworkManager<LoginCryptography>(ConfigurationManager.AppSettings["IPAddress"], port, typeof(LoginPacketHandler), typeof(LoginCryptography), false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error Server", ex);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogEventError("INITIALIZATION_EXCEPTION", "General Error", ex);
                    Console.ReadKey();
                }
            }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject);
            try
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Debug("Login Server crashed! Rebooting gracefully...");
            Process.Start("OpenNos.Login.exe", $"--nomsg --port {_port}");
            Environment.Exit(1);
        }

        #endregion
    }
}