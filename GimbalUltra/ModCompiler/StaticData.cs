using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GimbalUltra.ModCompiler
{
    class StaticData
    {
        Dictionary<System.Reflection.FieldInfo, object> PlainData = new Dictionary<System.Reflection.FieldInfo, object>();

        string Texture = "";

        float Mass = 0;
        float Moment = 0;
        Config CoM;

        public StaticData(Config data)
        {
            foreach(var kvp in data)
            {
                AddValue(kvp.Key, kvp.Value);
            }
        }

        public void AddValue(string name, object value)
        {
            if(name == "Texture")
            {
                Texture = (string)value;
                return;
            }
            if (name == "Mass")
            {
                Mass = (float)Convert.ChangeType(value, typeof(float));
                return;
            }
            if (name == "Moment")
            {
                Moment = (float)Convert.ChangeType(value, typeof(float));
                return;
            }
            if(name == "CoM")
            {
                CoM = (Config)value;
                return;
            }


            var field = typeof(Colin.Gimbal.ComponentStaticData).GetField(name);
            if(field != null)
            {
                PlainData.Add(field, value);
            }
        }

        public void AddLoadStaticData(Mod mod, TypeDefinition t)
        {
            var lsd = new MethodDefinition("LoadStaticData", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, mod.mod.TypeSystem.Void);
            t.Methods.Add(lsd);
            var Data = new VariableDefinition(mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData)));
            var Physics = new VariableDefinition(mod.mod.Import(typeof(Colin.Gimbal.Physics)));
            lsd.Body.Variables.Add(Data);
            lsd.Body.Variables.Add(Physics);

            var il = lsd.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Ldarg_0));
            il.Append(il.Create(OpCodes.Newobj, mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData).GetConstructor(new Type[] { typeof(Colin.Gimbal.Component) }))));
            il.Append(il.Create(OpCodes.Stloc, Data));
            il.Append(il.Create(OpCodes.Ldloc, Data));
            il.Append(il.Create(OpCodes.Stsfld, t.Fields.First(f => f.Name == "Data")));

            foreach(var fd in PlainData)
            {
                il.Append(il.Create(OpCodes.Ldloc, Data));
                if (fd.Value is string)
                {
                    il.Append(il.Create(OpCodes.Ldstr, (string)fd.Value));
                }
                if (fd.Value is int)
                {
                    il.Append(il.Create(OpCodes.Ldc_I4, (int)fd.Value));
                }
                if (fd.Value is float)
                {
                    il.Append(il.Create(OpCodes.Ldc_R4, (float)fd.Value));
                }
                if (fd.Value is bool)
                {
                    il.Append(il.Create(OpCodes.Ldc_I4, ((bool)fd.Value ? 1 : 0)));
                }
                il.Append(il.Create(OpCodes.Stfld, mod.mod.Import(fd.Key)));
            }

            il.Append(il.Create(OpCodes.Ldloc, Data));
            il.Append(il.Create(OpCodes.Newobj, mod.mod.Import(typeof(Colin.Gimbal.Physics).GetConstructor(new Type[] { }))));
            il.Append(il.Create(OpCodes.Stloc, Physics));
            il.Append(il.Create(OpCodes.Ldloc, Physics));
            il.Append(il.Create(OpCodes.Stfld, mod.mod.Import(typeof(Colin.Gimbal.ComponentStaticData).GetField("OriginalPhysics"))));

            il.Append(il.Create(OpCodes.Ldloc, Physics));
            il.Append(il.Create(OpCodes.Ldc_R4, Mass));
            il.Append(il.Create(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Physics).GetProperty("Mass").GetSetMethod())));
            il.Append(il.Create(OpCodes.Ldloc, Physics));
            il.Append(il.Create(OpCodes.Ldc_R4, Moment));
            il.Append(il.Create(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Physics).GetProperty("Moment").GetSetMethod())));

            if(CoM != null)
            {
                il.Append(il.Create(OpCodes.Ldloc, Physics));
                il.Append(il.Create(OpCodes.Ldc_R4, CoM.Get("x", 0f)));
                il.Append(il.Create(OpCodes.Ldc_R4, CoM.Get("y", 0f)));
                il.Append(il.Create(OpCodes.Newobj, mod.mod.Import(typeof(Microsoft.Xna.Framework.Vector2).GetConstructor(new Type[] { typeof(float), typeof(float) }))));
                il.Append(il.Create(OpCodes.Call, mod.mod.Import(typeof(Colin.Gimbal.Physics).GetProperty("ImageCenterOfMass").GetSetMethod())));
            }

            if (Texture != "")
            {
                if (Texture.Contains("."))
                {
                    var loader = mod.mod.Import(typeof(Ultra.TextureLoader).GetMethod("LoadTexture"));
                    il.Append(il.Create(OpCodes.Ldloc, Data));
                    il.Append(il.Create(OpCodes.Ldstr, Texture));
                    il.Append(il.Create(OpCodes.Call, loader));
                } else
                {
                    var loader = mod.mod.Import(typeof(Colin.Gimbal.Component).GetMethod("LoadBaseTexture"));
                    il.Append(il.Create(OpCodes.Ldarg_0));
                    il.Append(il.Create(OpCodes.Ldstr, Texture));
                    il.Append(il.Create(OpCodes.Call, loader));
                }
            }

            il.Append(il.Create(OpCodes.Ldarg_0));
            MethodReference baseMethod = null;
            TypeDefinition baseType = t;
            while (baseMethod == null)
            {
                baseType = baseType.BaseType.Resolve();
                baseMethod = baseType.Resolve().Methods.FirstOrDefault(m => m.Name == "LoadStaticData");
            }
            var baseM = mod.mod.Import(baseMethod);

            il.Append(il.Create(OpCodes.Call, baseM));

            il.Append(il.Create(OpCodes.Ret));

        }
    }
}
