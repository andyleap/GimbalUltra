using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GimbalUltra.ModCompiler;
using GimbalUltra.ModCompiler.Parser;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace GimbalUltra
{
	internal class Program
	{

		public static void Main(string[] args)
		{
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Console.WriteLine("Gimbal Ultra v{0} Running", Assembly.GetExecutingAssembly().GetName().Version);
            Log.Debug("Gimbal Ultra v{0} Running", Assembly.GetExecutingAssembly().GetName().Version);

            if (File.Exists("Gimbal.bak"))
			{
                ModuleDefinition gimbalInfo = ModuleDefinition.ReadModule("Gimbal.exe");
                if(gimbalInfo.CustomAttributes.Any(ca => ca.AttributeType.Name == "GimbalUltra"))
                {
                    Log.Debug("Gimbal.exe is ultraed, replacing with backup gimbal");
                    File.Delete("Gimbal.exe");
                    File.Move("Gimbal.bak", "Gimbal.exe");
                } else
                {
                    Log.Debug("Gimbal.exe is not ultraed");
                    File.Delete("Gimbal.bak");
                }
			}
            Log.Debug("Reading Gimbal");
            var moduleDefinition = ModuleDefinition.ReadModule("Gimbal.exe");

            Log.Debug("Adding Ultra Attribute");
            moduleDefinition.CustomAttributes.Add(new CustomAttribute(moduleDefinition.Import(typeof(GimbalUltra).GetConstructor(new Type[] { }))));

            Log.Debug("Patching Gimbal");
            AutoHook ah = new AutoHook(moduleDefinition);

            Log.Debug("Adding Ultra.dll hooks");
            var mod = ModuleDefinition.ReadModule("Ultra.dll");
            ah.HookMod(mod, "Ultra.dll");

            foreach (var modDir in Directory.GetDirectories("mods"))
            {
                Log.Debug("Working on {0}", modDir);
                Mod modDll = new Mod(new DirectoryInfo(modDir).Name, moduleDefinition);
                bool WriteDll = false;
                foreach (var confFile in Directory.GetFiles(modDir, "*.conf"))
                {
                    Log.Debug("Loading {0}", confFile);
                    string file = File.ReadAllText(confFile);
                    var configs = CommonGrammar.Object.Mult().Parse(file);

                    Log.Debug("Adding {0} Parts", configs.Count);
                    foreach (var config in configs)
                    {
                        var part = Part.FromConfig(config);
                        if (part != null)
                        {
                            part.AddPart(modDll);
                            WriteDll = true;
                        }
                    }
                }
                if (WriteDll)
                {
                    Log.Debug("Writing {0}", new DirectoryInfo(modDir).Name + ".dll");
                    modDll.mod.Write(Path.Combine(modDir, new DirectoryInfo(modDir).Name + ".dll"));
                }
                foreach (var modFile in Directory.GetFiles(modDir, "*.dll"))
                {
                    Log.Debug("Adding {0}", modFile);
                    mod = ModuleDefinition.ReadModule(modFile);
                    ah.HookMod(mod, modFile);                    
                }
            }
            Log.Debug("Backing up existing Gimbal.exe");
            File.Move("Gimbal.exe", "Gimbal.bak");
            Log.Debug("Writing ultraed Gimbal.exe");
            moduleDefinition.Write("Gimbal.exe");
            Log.Debug("GimbalUltra finished");
            Console.WriteLine("Patching complete, press enter to close this window");
            Console.ReadLine();
		}

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Exception((Exception)e.ExceptionObject);
        }
    }
}
