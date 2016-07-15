using System;
using System.IO;
using System.Reflection;

namespace GimbalServerUltra
{
	internal class Base
	{
		static Base()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Base.Resolver);
		}

		private static Assembly Resolver(object sender, ResolveEventArgs args)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("GimbalServerUltra." + args.Name.Split(new char[]
			{
				','
			})[0] + ".dll");
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, array.Length);
			return Assembly.Load(array);
		}

		private static void Main(string[] args)
		{
			Program.Main(args);
		}
	}
}
