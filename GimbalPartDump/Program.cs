using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Colin.Gimbal;

namespace GimbalPartDump
{
    class Program
    {
        static void Main(string[] args)
        {
            GimbalGameManager.ClientOptions = ClientOptions.Load();
            GimbalGameManager.ServerOptions = ServerOptions.Load();
            GimbalGameManager.ClientOptions.SoundOn = false;
            GimbalGameManager.ClientOptions.FlamesOn = false;
            GimbalGameManager.ClientOptions.MotionBlur = false;
            GimbalGameManager.ClientOptions.Multithreading = false;
            GimbalGameManager.ClientOptions.BloomOn = false;
            GimbalGameManager.ClientOptions.BackgroundDustOn = false;
            GimbalGameManager.ClientOptions.ShowParticles = false;
            GimbalGameManager.ClientOptions.ShowTrails = false;
            Colin.Gimbal.GimbalGameManager.Headless = false;
            Colin.Gimbal.GimbalGameManager gameManager = new Colin.Gimbal.GimbalGameManager();
            Assembly partAssem = Assembly.GetAssembly(typeof(Colin.Gimbal.Parts.AdvancedManueveringThruster));
            HashSet<String> cs = new HashSet<string>();
            foreach(var part in partAssem.GetTypes())
            {
                if(part.IsSubclassOf(typeof(Colin.Gimbal.Component)) && !part.IsAbstract)
                {
                    Colin.Gimbal.Component c = (Colin.Gimbal.Component)Activator.CreateInstance(part);
                    MethodInfo meth = part.GetMethod("LoadStaticData", BindingFlags.NonPublic | BindingFlags.Instance);
                    meth.Invoke(c, null);
                    cs.Add(c.DisplayName);
                    c.StaticData.BaseTexture.Save("parts\\"+part.Name, Microsoft.Xna.Framework.Graphics.ImageFileFormat.Png);
                }
            }

            foreach(var c in cs)
            {
                Console.WriteLine(c);
            }

            Console.ReadLine();
        }
    }
}
