using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler
{
    abstract class Part
    {
        public string Name;
        public StaticData data;

        public Config config;

        public Part(Config config)
        {
            this.config = config;
            this.Name = config.Get("name", "blank");
            this.data = new StaticData(config.Get("static", new Config()));
        }

        public static Part FromConfig(Config config)
        {
            switch(config.Get("type", "armor"))
            {
                case "armor":
                    return new Armor(config);
                case "airframe":
                    return new Airframe(config);
                case "structure":
                    return new Structure(config);
                case "thruster":
                    return new Thruster(config);
            }
            return null;
        }

        public virtual TypeDefinition AddType(Mod mod, Type BaseType, Colin.Gimbal.PartsCategories Category)
        {
            var t = new TypeDefinition(mod.Namespace, Name, TypeAttributes.Public, mod.mod.Import(BaseType));
            mod.mod.Types.Add(t);
            var cpattr = new CustomAttribute(mod.mod.Import(typeof(Ultra.CustomPartAttribute).GetConstructors()[0]));
            cpattr.ConstructorArguments.Add(new CustomAttributeArgument(mod.mod.Import(typeof(Colin.Gimbal.PartsCategories)), Category));
            if (config.Get("MastermindOnly", false))
            {
                cpattr.Properties.Add(new CustomAttributeNamedArgument("MastermindOnly", new CustomAttributeArgument(mod.mod.TypeSystem.Boolean, true)));
            }
            t.CustomAttributes.Add(cpattr);
            t.IsSerializable = true;
            AddData(mod, t);
            var Constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, mod.mod.TypeSystem.Void);
            t.Methods.Add(Constructor);
            var il = Constructor.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            il.Append(il.Create(OpCodes.Call, mod.mod.Import(t.BaseType.Resolve().Methods.First(m => m.IsConstructor && m.Parameters.Count == 0))));
            il.Append(il.Create(OpCodes.Ret));

            AddNewInstance(mod, t);
            data.AddLoadStaticData(mod, t);
            return t;
        }

        public virtual void AddData(Mod mod, TypeDefinition t)
        {
            var Data = new FieldDefinition("Data", FieldAttributes.Static, mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData)));
            t.Fields.Add(Data);

            var ComponentLevelStaticData = new PropertyDefinition("ComponentLevelStaticData", PropertyAttributes.None, mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData)));
            ComponentLevelStaticData.GetMethod = new MethodDefinition("get_ComponentLevelStaticData", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual, mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData)));
            var il = ComponentLevelStaticData.GetMethod.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldsfld, Data));
            il.Append(il.Create(OpCodes.Ret));
            il.Body.OptimizeMacros();
            t.Properties.Add(ComponentLevelStaticData);
            t.Methods.Add(ComponentLevelStaticData.GetMethod);
        }

        public virtual void AddNewInstance(Mod mod, TypeDefinition t)
        {
            var GetNewInstance = new MethodDefinition("GetNewInstance", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.Import(typeof(Colin.Gimbal.Component)));
            var il = GetNewInstance.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Newobj, t.GetConstructors().First(c => c.Parameters.Count == 0)));
            il.Append(il.Create(OpCodes.Ret));
            t.Methods.Add(GetNewInstance);
        }

        public abstract TypeDefinition AddPart(Mod mod);


    }
}
