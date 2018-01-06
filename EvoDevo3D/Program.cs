#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using EvoDevo4;

#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
#endregion

namespace EvoDevo3D
{
    static class Program
    {
        private static EvoArea game;

        internal static void RunGame()
        {
            game = new EvoArea();
            game.Simulation = new Simulation(Cell.Recompile(
                    str => Console.Error.WriteLine(str)));
            game.Run();
            game.Dispose();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        #if !MONOMAC
        [STAThread]
        #endif
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify program as argument");
                return;
            }

            string programFile = null;
            string programPath = null;
            bool expectSeed = false;
            int seed = 0;
            foreach (string arg in args)
            {
                if (expectSeed)
                {
                    seed = int.Parse(arg);
                }
                else if (arg == "-S")
                {
                    expectSeed = true;
                }
                else if (programFile == null)
                {
                    programFile = arg;
                }
                else if (programPath == null)
                {
                    programPath = arg;
                }
                else
                {
                    Console.WriteLine("Unrecognized: " + arg);
                }
            }

            Cell.Program = new FileInfo(programPath == null ? programFile : programPath);
            Cell.GeneticCode = File.ReadAllText(programFile);
            Cell.Random = new Random(seed);

            #if MONOMAC
            NSApplication.Init ();

            using (var p = new NSAutoreleasePool ()) {
                NSApplication.SharedApplication.Delegate = new AppDelegate();
                NSApplication.Main(args);
            }
            #else
            RunGame();
            #endif
        }
    }

    #if MONOMAC
    class AppDelegate : NSApplicationDelegate
    {
        public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs a) =>  {
                if (a.Name.StartsWith("MonoMac")) {
                    return typeof(MonoMac.AppKit.AppKitFramework).Assembly;
                }
                return null;
            };
            Program.RunGame();
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
        {
            return true;
        }
    }  
    #endif
}

