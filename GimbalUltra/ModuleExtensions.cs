using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;

namespace GimbalServerUltra
{
	public static class ModuleExtensions
	{
		public static void HookMethodAction(this ModuleDefinition mod, string Class, string Method, TypeDefinition Ultra, string Hook, bool skip, params TypeReference[] Types)
		{
			FieldDefinition fieldDefinition = new FieldDefinition(Hook, FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static, mod.Import(ModuleExtensions.MakeAction(mod, Types)));
			Ultra.Fields.Add(fieldDefinition);
			MethodDefinition methodDefinition = (from td in mod.Types
			where td.Name == Class
			select td).First<TypeDefinition>().Methods.First((MethodDefinition md) => md.Name == Method);
			mod.AssemblyReferences.Add(fieldDefinition.FieldType.Module.Assembly.Name);
			MethodBodyRocks.SimplifyMacros(methodDefinition.Body);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction target = methodDefinition.Body.Instructions.First<Instruction>();
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Brfalse, target));
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			for (int i = 0; i < Types.Length; i++)
			{
				iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldarg, i));
			}
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Callvirt, ModuleExtensions.MakeActionInvoke(mod, Types)));
			if (skip)
			{
				iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ret));
			}
			MethodBodyRocks.OptimizeMacros(methodDefinition.Body);
		}

		public static void HookMethodFunc(this ModuleDefinition mod, string Class, string Method, TypeDefinition Ultra, string Hook, params TypeReference[] Types)
		{
			FieldDefinition fieldDefinition = new FieldDefinition(Hook, FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static, mod.Import(ModuleExtensions.MakeFunc(mod, Types)));
			Ultra.Fields.Add(fieldDefinition);
			MethodDefinition methodDefinition = (from td in mod.Types
			where td.Name == Class
			select td).First<TypeDefinition>().Methods.First((MethodDefinition md) => md.Name == Method);
			mod.AssemblyReferences.Add(fieldDefinition.FieldType.Module.Assembly.Name);
			MethodBodyRocks.SimplifyMacros(methodDefinition.Body);
			ILProcessor iLProcessor = methodDefinition.Body.GetILProcessor();
			Instruction target = methodDefinition.Body.Instructions.First<Instruction>();
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Brfalse, target));
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldsfld, fieldDefinition));
			for (int i = 0; i < Types.Length - 1; i++)
			{
				iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldarg, i));
			}
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Callvirt, ModuleExtensions.MakeFuncInvoke(mod, Types)));
			iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ret));
			MethodBodyRocks.OptimizeMacros(methodDefinition.Body);
		}

		public static GenericInstanceType MakeAction(ModuleDefinition mod, params TypeReference[] Types)
		{
			GenericInstanceType genericInstanceType = null;
			switch (Types.Count<TypeReference>())
			{
			case 1:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int>));
				break;
			case 2:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int>));
				break;
			case 3:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int, int>));
				break;
			case 4:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int, int, int>));
				break;
			}
			genericInstanceType.GenericArguments.Clear();
			for (int i = 0; i < Types.Length; i++)
			{
				TypeReference item = Types[i];
				genericInstanceType.GenericArguments.Add(item);
			}
			return genericInstanceType;
		}

		public static MethodReference MakeActionInvoke(ModuleDefinition mod, params TypeReference[] Types)
		{
			GenericInstanceType genericInstanceType = null;
			switch (Types.Count<TypeReference>())
			{
			case 1:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int>));
				break;
			case 2:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int>));
				break;
			case 3:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int, int>));
				break;
			case 4:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Action<int, int, int, int>));
				break;
			}
			genericInstanceType.GenericArguments.Clear();
			for (int i = 0; i < Types.Length; i++)
			{
				TypeReference item = Types[i];
				genericInstanceType.GenericArguments.Add(item);
			}
			MethodReference methodReference = mod.Import(genericInstanceType.Resolve().Methods.First((MethodDefinition m) => m.Name == "Invoke"));
			methodReference.DeclaringType = genericInstanceType;
			return methodReference;
		}

		public static GenericInstanceType MakeFunc(ModuleDefinition mod, params TypeReference[] Types)
		{
			GenericInstanceType genericInstanceType = null;
			switch (Types.Count<TypeReference>())
			{
			case 1:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int>));
				break;
			case 2:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int>));
				break;
			case 3:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int, int>));
				break;
			case 4:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int, int, int>));
				break;
			}
			genericInstanceType.GenericArguments.Clear();
			for (int i = 0; i < Types.Length; i++)
			{
				TypeReference item = Types[i];
				genericInstanceType.GenericArguments.Add(item);
			}
			return genericInstanceType;
		}

		public static MethodReference MakeFuncInvoke(ModuleDefinition mod, params TypeReference[] Types)
		{
			GenericInstanceType genericInstanceType = null;
			switch (Types.Count<TypeReference>())
			{
			case 1:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int>));
				break;
			case 2:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int>));
				break;
			case 3:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int, int>));
				break;
			case 4:
				genericInstanceType = (GenericInstanceType)mod.Import(typeof(Func<int, int, int, int>));
				break;
			}
			genericInstanceType.GenericArguments.Clear();
			for (int i = 0; i < Types.Length; i++)
			{
				TypeReference item = Types[i];
				genericInstanceType.GenericArguments.Add(item);
			}
			MethodReference methodReference = mod.Import(genericInstanceType.Resolve().Methods.First((MethodDefinition m) => m.Name == "Invoke"));
			methodReference.DeclaringType = genericInstanceType;
			return methodReference;
		}
	}
}
