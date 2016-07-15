using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Colin.Gimbal;

namespace Ultra
{
    public class ModLoader
    {
        static Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
        public static Dictionary<string, string> ModMap = new Dictionary<string, string>();
        public static HashSet<Type> ModSet = new HashSet<Type>();

        public static bool IsModPart(Component c)
        {
            return ModSet.Contains(c.GetType());
        }

        static ModLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(ModMap.ContainsKey(args.Name))
            {
                return Assembly.Load(File.ReadAllBytes(ModMap[args.Name]));
            }
            return null;
        }

        public static void LoadMod(string name)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var mod = Assembly.Load(File.ReadAllBytes(Path.Combine(path, name)));
            ModMap.Add(mod.FullName, Path.Combine(path, name));

            foreach (var t in mod.GetTypes())
            {
                if(t.GetCustomAttributes(typeof(CustomPartAttribute), false).Any())
                {
                    ModSet.Add(t);
                }
                if (!TypeMap.ContainsKey(t.FullName))
                {
                    TypeMap.Add(t.FullName, t);
                }
            }
        }

        public static object CreateInstance(string name)
        {
            if(TypeMap.ContainsKey(name))
            {
                return Activator.CreateInstance(TypeMap[name]);
            }
            else
            {
                return Assembly.GetAssembly(typeof(Colin.Gimbal.Component)).CreateInstance(name);
            }
        }
    }
}
