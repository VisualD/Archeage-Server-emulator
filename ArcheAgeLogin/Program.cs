﻿using ArcheAgeLogin.ArcheAge.Structuring;
using LocalCommons.Logging;
using LocalCommons.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ArcheAgeLogin.ArcheAge;
using ArcheAgeLogin.ArcheAge.Holders;
using ArcheAgeLogin.ArcheAge.Network;
using ArcheAgeLogin.Properties;

namespace ArcheAgeLogin
{
    /// <summary>
    /// Main Class For Program Entering.
    /// </summary>
    class Program
    {
        static string ServerClientVersion = "3";
        // .method private hidebysig static void Main(string[] args) cil managed
        static void Main(string[] args)
        {
            Console.Title = "ARCHEAGE LOGIN SERVER";
            //Console.Write(System.Text.UTF8Encoding.UTF8.GetByteCount("jangmi"));
            Console.CancelKeyPress += Console_CancelKeyPress;
            Stopwatch watch = Stopwatch.StartNew();
            watch.Start();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            selectVersion();
            LoadExecutingAssembly(args);
            watch.Stop();
            Logger.Trace("ArcheAge Login Server started in {0} seconds", (watch.ElapsedMilliseconds / 1000.0).ToString("0.00"));
            watch = null;
            Key_Pressed();
           
        }

        static void selectVersion()
        {
            Console.WriteLine("Select Client Version: Default 3");
            Console.WriteLine("1:   1.0");
            Console.WriteLine("3:   3.0");
            Console.WriteLine("4:   4.0");
            //0 is manually selected
            if (Settings.Default.ServerClientVersion == "0")
            {
                Program.ServerClientVersion = Console.ReadLine();
                if (Program.ServerClientVersion == "")
                {
                    //The default is 3
                    Program.ServerClientVersion = "3";
                }
            }
            else {
                Console.WriteLine("AutoSelectServerClientVersion:" + Settings.Default.ServerClientVersion);
                Program.ServerClientVersion = Settings.Default.ServerClientVersion;
            }

        }
        static void Key_Pressed()
        {
           ConsoleKeyInfo info = Console.ReadKey();
           if (info != null)
           {
               Key_Pressed();
           }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                Shutdown();
            }
            else
            {
                return;
            }
        }

        static void Shutdown()
        {
            //TODO : Here Shutdowning.
        }

        /// <summary>
        /// Calls When Program Catches a Exception That Wasn't Catched By Try-Catch Block. (Unhandled)
        /// </summary>
        /// <param name="sender">Exception Sender - AppDomain</param>
        /// <param name="e">Event Arguments</param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Trace("Unhandled Exception - Sender: {0} , Exception - \n{1}", sender.GetType().Name, ((Exception)e.ExceptionObject).ToString());
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Shutdown();
        }

        //.method hidebysig static void LoadExecutngAssembly(string[] args) cil managed
        static void LoadExecutingAssembly(string[] args)
        {
            Logger.Init();
			
			//Logger.Trace("TODO: REMAKE ALL CONTAINSKEY [] - TO TRYGETVALUE");

            Settings m_Current = Settings.Default;

            //--------------- Init Commons ----------------------
            LocalCommons.Main.InitializeStruct(args);

            //------------- Controllers -------------------------
            Logger.Section("Controllers");
            GameServerController.LoadAvailableGameServers();

            //--------------- MySQL ---------------------------
            Logger.Section("MySQL");
            AccountHolder.LoadAccountData();

            //----------------Network ---------------------------
            Logger.Section("Network");
            PacketList.Initialize(Program.ServerClientVersion);
            new AsyncListener(m_Current.Main_IP, m_Current.Game_Port, defined: typeof(GameConnection)); //Waiting For Game Server Connections
            new AsyncListener(m_Current.Main_IP, m_Current.ArcheAge_Port, defined: typeof(ArcheAgeConnection)); //Waiting For ArcheAge Connections
        }
    }
}
