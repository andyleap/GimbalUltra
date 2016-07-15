using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace GimbalServerUltra
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			if (File.Exists("Gimbal.bak"))
			{
				File.Delete("Gimbal.exe");
				File.Move("Gimbal.bak", "Gimbal.exe");
			}
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule("Gimbal.exe");
			TypeDefinition typeDefinition = new TypeDefinition("Colin.Gimbal", "Ultra", TypeAttributes.Public);
			typeDefinition.BaseType = moduleDefinition.Import(typeof(object));
			moduleDefinition.Types.Add(typeDefinition);
			TypeReference typeReference = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "GameLogicHandler");
			TypeReference typeReference2 = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "Player");
			TypeReference typeReference3 = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "Team");
			TypeReference typeReference4 = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "GimbalGameInstance");
			TypeReference typeReference5 = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "AuthMessageRequestData");
			TypeDefinition typeDefinition2 = moduleDefinition.Types.First((TypeDefinition td) => td.Name == "GameMode");
			FieldDefinition fieldDefinition = new FieldDefinition("TeamDeathmatch", FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static | FieldAttributes.Literal, typeDefinition2);
			fieldDefinition.Constant = 7;
			typeDefinition2.Fields.Add(fieldDefinition);
			moduleDefinition.HookMethodAction("GameLogicHandler", "AwardMoneyKill", typeDefinition, "AwardMoneyKill", true, new TypeReference[]
			{
				typeReference,
				typeReference2,
				typeReference2
			});
			moduleDefinition.HookMethodAction("GimbalGameInstance", "HandleAuthRequest", typeDefinition, "HandleAuthRequest", false, new TypeReference[]
			{
				typeReference4,
				typeReference5
			});
			moduleDefinition.HookMethodFunc("BotConfigs", "MakeRandomFighter", typeDefinition, "MakeRandomFighter", new TypeReference[]
			{
				typeReference3,
				typeReference2
			});
			moduleDefinition.HookMethodFunc("GameLogicHandler", "GetHandlerInstance", typeDefinition, "GetHandlerInstance", new TypeReference[]
			{
				typeReference4,
				typeDefinition2,
				typeReference
			});
			new MemoryStream();
			File.Move("Gimbal.exe", "Gimbal.bak");
			moduleDefinition.Write("Gimbal.exe");
			AppDomain appDomain = AppDomain.CreateDomain("GimbalServer");
			appDomain.ExecuteAssembly("GimbalServerPro.exe", appDomain.Evidence, args);
		}
	}
}
