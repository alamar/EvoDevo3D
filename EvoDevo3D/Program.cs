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
            Cell.Program = new FileInfo(args.Length > 1 ? args[1] : args[0]);
            Cell.GeneticCode = File.ReadAllText(args[0]);
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

