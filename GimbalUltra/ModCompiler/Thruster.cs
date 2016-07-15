using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace GimbalUltra.ModCompiler
{
    class Thruster : Part
    {
        FieldDefinition Forward;
        FieldDefinition Backward;


        public Thruster(Config config) : base(config)
        {
        }

        public override TypeDefinition AddPart(Mod mod)
        {
            var thruster = AddType(mod, typeof(Colin.Gimbal.BaseThruster), Colin.Gimbal.PartsCategories.Propulsion);

            AddInit(mod, thruster);

            AddDecals(mod, thruster);

            AddUpdate(mod, thruster);

            return thruster;
        }

        public void AddUpdate(Mod mod, TypeDefinition t)
        {
            var update = new MethodDefinition("Update", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
            t.Methods.Add(update);
            var GameTime = new ParameterDefinition(mod.mod.Import(typeof(Microsoft.Xna.Framework.GameTime)));
            update.Parameters.Add(GameTime);
            var TeamColor = new VariableDefinition(mod.mod.Import(typeof(Microsoft.Xna.Framework.Graphics.Color)));
            update.Body.Variables.Add(TeamColor);
            var absSpool = new VariableDefinition(mod.mod.Import(typeof(float)));
            update.Body.Variables.Add(absSpool);
            var il = update.Body.GetILProcessor();

            var mainStart = il.Create(OpCodes.Nop);

            var Decals = config.Get("decals", new Config());

            var forThrust = Decals.Get("forwardThrust", new Config());
            var backThrust = Decals.Get("backwardThrust", new Config());

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetProperty("Alive").GetGetMethod()));
            il.Emit(OpCodes.Brtrue, mainStart);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetProperty("FunctionA").GetSetMethod()));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetProperty("FunctionB").GetSetMethod()));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg, GameTime);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetMethod("Update")));

            il.Emit(OpCodes.Ret);
            il.Append(mainStart);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Forward);

            var ldfalse = il.Create(OpCodes.Ldc_I4, 0);
            var stval = il.Create(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("On").GetSetMethod()));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetProperty("Active").GetGetMethod()));
            il.Emit(OpCodes.Brfalse, ldfalse);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.BaseThruster).GetProperty("ShowThrust").GetGetMethod()));
            il.Emit(OpCodes.Brfalse, ldfalse);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.BaseThruster)).Resolve().Fields.First(f => f.Name == "mSpooler")));
            il.Emit(OpCodes.Callvirt, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.Spooler)).Resolve().Properties.First(p => p.Name == "SpoolOutput").GetMethod));
            il.Emit(OpCodes.Ldc_R4, 0f);
            il.Emit(OpCodes.Cgt);
            il.Emit(OpCodes.Br, stval);

            il.Append(ldfalse);
            il.Append(stval);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Backward);

            ldfalse = il.Create(OpCodes.Ldc_I4, 0);
            stval = il.Create(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("On").GetSetMethod()));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetProperty("Active").GetGetMethod()));
            il.Emit(OpCodes.Brfalse, ldfalse);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.BaseThruster).GetProperty("ShowThrust").GetGetMethod()));
            il.Emit(OpCodes.Brfalse, ldfalse);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.BaseThruster)).Resolve().Fields.First(f => f.Name == "mSpooler")));
            il.Emit(OpCodes.Callvirt, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.Spooler)).Resolve().Properties.First(p => p.Name == "SpoolOutput").GetMethod));
            il.Emit(OpCodes.Ldc_R4, 0f);
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Br, stval);

            il.Append(ldfalse);
            il.Append(stval);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.GetGetMethod("Component", "OwnerPlayer"));
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Call, mod.GetMethod("Player", "GetColor", 1));
            il.Emit(OpCodes.Ldc_R4, 0.7f);
            il.Emit(OpCodes.Call, mod.GetMethod("Utility", "MultiplyColorBrightness", 2));
            il.Emit(OpCodes.Stloc, TeamColor);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Forward);
            il.Emit(OpCodes.Ldloc, TeamColor);
            il.Emit(OpCodes.Call, mod.GetSetMethod("BaseDecal", "Color"));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Backward);
            il.Emit(OpCodes.Ldloc, TeamColor);
            il.Emit(OpCodes.Call, mod.GetSetMethod("BaseDecal", "Color"));
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, mod.GetField("BaseThruster", "mSpooler"));
            il.Emit(OpCodes.Callvirt, mod.GetGetMethod("Spooler", "SpoolOutput"));
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Math).GetMethod("Abs", new Type[] { typeof(float) })));
            il.Emit(OpCodes.Stloc, absSpool);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Forward);
            il.Emit(OpCodes.Ldloc, absSpool);
            if(forThrust.Get("scaleX", 1.0f) != 1.0f)
            {
                il.Emit(OpCodes.Ldc_R4, forThrust.Get("scaleX", 1.0f));
                il.Emit(OpCodes.Mul);
            }
            il.Emit(OpCodes.Ldloc, absSpool);
            if (forThrust.Get("scaleY", 1.0f) != 1.0f)
            {
                il.Emit(OpCodes.Ldc_R4, forThrust.Get("scaleY", 1.0f));
                il.Emit(OpCodes.Mul);
            }
            il.Emit(OpCodes.Newobj, mod.mod.Import(typeof(Microsoft.Xna.Framework.Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) })));
            il.Emit(OpCodes.Callvirt, mod.GetSetMethod("BaseDecal", "ScaleV"));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, Backward);
            il.Emit(OpCodes.Ldloc, absSpool);
            if (backThrust.Get("scaleX", 1.0f) != 1.0f)
            {
                il.Emit(OpCodes.Ldc_R4, backThrust.Get("scaleX", 1.0f));
                il.Emit(OpCodes.Mul);
            }
            il.Emit(OpCodes.Ldloc, absSpool);
            if (backThrust.Get("scaleY", 1.0f) != 1.0f)
            {
                il.Emit(OpCodes.Ldc_R4, backThrust.Get("scaleY", 1.0f));
                il.Emit(OpCodes.Mul);
            }
            il.Emit(OpCodes.Newobj, mod.mod.Import(typeof(Microsoft.Xna.Framework.Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) })));
            il.Emit(OpCodes.Callvirt, mod.GetSetMethod("BaseDecal", "ScaleV"));

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg, GameTime);

            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.BaseThruster).GetMethod("Update")));

            il.Emit(OpCodes.Ret);
        }

        public void AddDecals(Mod mod, TypeDefinition t)
        {
            Forward = new FieldDefinition("mForThrust", FieldAttributes.NotSerialized, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal)));
            t.Fields.Add(Forward);
            Backward = new FieldDefinition("mBackThrust", FieldAttributes.NotSerialized, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal)));
            t.Fields.Add(Backward);


            var vbd = new MethodDefinition("VirtualBuildDecals", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
            t.Methods.Add(vbd);
            var tempDecal = new VariableDefinition(mod.mod.Import(typeof(Colin.Gimbal.BaseDecal)));
            vbd.Body.Variables.Add(tempDecal);
            var il = vbd.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);

            MethodReference baseMethod = null;
            TypeDefinition baseType = t;
            while (baseMethod == null)
            {
                baseType = baseType.BaseType.Resolve();
                baseMethod = baseType.Resolve().Methods.FirstOrDefault(m => m.Name == "VirtualBuildDecals");
            }
            var baseM = mod.mod.Import(baseMethod);
            il.Append(il.Create(OpCodes.Call, baseM));

            var Decals = config.Get("decals", new Config());

            var forThrust = Decals.Get("forwardThrust", new Config());

            il.Emit(OpCodes.Ldc_R4, forThrust.Get("x", 0f));
            il.Emit(OpCodes.Ldc_R4, forThrust.Get("y", 0f));
            il.Emit(OpCodes.Ldc_R4, forThrust.Get("angle", 0f));
            il.Emit(OpCodes.Ldstr, forThrust.Get("texture", "thrust1"));
            il.Emit(OpCodes.Newobj, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(string) })));
            il.Emit(OpCodes.Stloc, tempDecal);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("On").GetSetMethod()));
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Ldc_R4, 0.1f);
            il.Emit(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("FudgeScaleWithZoomFactor").GetSetMethod()));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Stfld, Forward);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetMethod("AddGlowyDecalToThis")));

            var backThrust = Decals.Get("backwardThrust", new Config());

            il.Emit(OpCodes.Ldc_R4, backThrust.Get("x", 0f));
            il.Emit(OpCodes.Ldc_R4, backThrust.Get("y", 0f));
            il.Emit(OpCodes.Ldc_R4, backThrust.Get("angle", (float)Math.PI));
            il.Emit(OpCodes.Ldstr, backThrust.Get("texture", "thrust1"));
            il.Emit(OpCodes.Newobj, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(string) })));
            il.Emit(OpCodes.Stloc, tempDecal);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Ldc_I4, 0);
            il.Emit(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("On").GetSetMethod()));
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Ldc_R4, 0.1f);
            il.Emit(OpCodes.Callvirt, mod.mod.Import(typeof(Colin.Gimbal.BaseDecal).GetProperty("FudgeScaleWithZoomFactor").GetSetMethod()));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Stfld, Backward);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, tempDecal);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetMethod("AddGlowyDecalToThis")));

            il.Emit(OpCodes.Ret);
        }

        public void AddInit(Mod mod, TypeDefinition t)
        {
            var Init = new MethodDefinition("Initialize", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
            t.Methods.Add(Init);
            var il = Init.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Component).GetMethod("Initialize")));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldc_R4, config.Get("thrust", 0f));
            il.Emit(OpCodes.Stfld, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.BaseThruster)).Resolve().Fields.First(f => f.Name == "mThrust")));
            il.Emit(OpCodes.Ret);

            var Inst = new MethodDefinition("InstantiateMembers", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
            t.Methods.Add(Inst);
            il = Inst.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.Component)).Resolve().Methods.First(m => m.Name == "InstantiateMembers")));
            il.Emit(OpCodes.Ldarg_0);
            var spooler = config.Get("spooler", new Config());
            switch (spooler.Get("type", "CubicSpooler"))
            {
                case "InstantSpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.InstantSpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 3)));
                    break;
                case "DelaySpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.DelaySpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 3)));
                    break;
                case "EagerSpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.EagerSpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 3)));
                    break;
                case "EasilyWindedSpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("floor", 0.2f));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.EasilyWindedSpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 4)));
                    break;
                case "CubicSpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0 ));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.CubicSpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 3)));
                    break;
                case "ParabolicSpooler":
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampUpRate", 1f));
                    il.Emit(OpCodes.Ldc_R4, spooler.Get("rampDownRate", 1f));
                    il.Emit(OpCodes.Ldc_I4, (spooler.Get("allowNegative", true) ? 1 : 0));
                    il.Emit(OpCodes.Newobj, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.ParabolicSpooler)).Resolve().GetConstructors().First(c => c.Parameters.Count == 3)));
                    break;

            }
            il.Emit(OpCodes.Stfld, mod.mod.Import(mod.mod.Import(typeof(Colin.Gimbal.BaseThruster)).Resolve().Fields.First(f => f.Name == "mSpooler")));
            il.Emit(OpCodes.Ret);

            if (config.Get("manuevering", false))
            {
                var Controls = new MethodDefinition("AssignDefaultControls", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
                t.Methods.Add(Controls);
                il = Controls.Body.GetILProcessor();
                il.Emit(OpCodes.Ret);
            }
        }
    }
}


