using System;
using System.IO;
using System.Xml.Serialization;

namespace GimbalServerPro
{
	[Serializable]
	public class ServerProOptions
	{
		public int MaxBudget = 20000;

		public SerializableDictionary<string, int> PartLimits = new SerializableDictionary<string, int>
		{
			{
				"Series 100 Naval Gun",
				12
			}
		};

		public bool CustomBots;

		public string CustomBotsFolder = "Bots";

		public static ServerProOptions Load(string filename)
		{
			ServerProOptions result;
			try
			{
				using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerProOptions));
					result = (ServerProOptions)xmlSerializer.Deserialize(fileStream);
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Exception loading server pro options");
				ServerProOptions serverProOptions = new ServerProOptions();
				serverProOptions.Save(filename);
				result = serverProOptions;
			}
			return result;
		}

		public void Save(string filename)
		{
			using (FileStream fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				try
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerProOptions));
					xmlSerializer.Serialize(fileStream, this);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception saving server pro options: " + ex.ToString());
				}
			}
		}
	}
}
