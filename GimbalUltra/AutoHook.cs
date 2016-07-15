using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;
using System.Xml.Serialization;

namespace GimbalUltra
{
    public class AutoHook
    {
        ModuleDefinition Gimbal;

        public AutoHook(ModuleDefinition Gimbal)
        {
            this.Gimbal = Gimbal;

            Patches();

        }

        private void Patches()
        {
            Log.Debug("Patching RefreshListing");
            PatchRefreshListing();
            Log.Debug("Patching ComponentFromTypeID");
            PatchGetComponentFromTypeID();
            Log.Debug("Patching GrabCallback");
            PatchGrabCallback();
            Log.Debug("Patching MirrorCallback");
            PatchMirrorCallback();
            Log.Debug("Making everything possible public");
            MakeAllPublic();
        }

        private void PatchRefreshListing()
        {
            var refreshListing = Gimbal.Types.First(td => td.Name == "PartsBin").Methods.First(md => md.Name == "RefreshListing");

            refreshListing.Body.SimplifyMacros();
            ILProcessor iLProcessor = refreshListing.Body.GetILProcessor();
            int l1 = 0;
            for (l1 = 0; l1 < refreshListing.Body.Instructions.Count; l1++)
            {
                if (refreshListing.Body.Instructions[l1].OpCode == OpCodes.Callvirt && refreshListing.Body.Instructions[l1 + 1].OpCode == OpCodes.Callvirt)
                {
                    break;
                }
            }
            l1 -= 2;
            refreshListing.Body.Instructions.RemoveAt(l1);
            refreshListing.Body.Instructions.RemoveAt(l1);
            refreshListing.Body.Instructions.RemoveAt(l1);
            refreshListing.Body.Instructions.RemoveAt(l1);
            var inst = refreshListing.Body.Instructions[l1];
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Ldloc, 1));
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }))));
            refreshListing.Body.OptimizeMacros();
        }

        private void PatchGetComponentFromTypeID()
        {
            var refreshListing = Gimbal.Types.First(td => td.Name == "NetworkVehicleSerializer").Methods.First(md => md.Name == "GetComponentFromTypeID");

            ILProcessor iLProcessor = refreshListing.Body.GetILProcessor();
            refreshListing.Body.Instructions.RemoveAt(0);
            refreshListing.Body.Instructions.RemoveAt(3);
            refreshListing.Body.Instructions.RemoveAt(3);
            var inst = refreshListing.Body.Instructions[3];
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }))));
            refreshListing.Body.OptimizeMacros();
        }

        private void PatchGrabCallback()
        {
            var refreshListing = Gimbal.Types.First(td => td.Name == "DesignerScreen").Methods.First(md => md.Name == "GrabCallback");

            refreshListing.Body.SimplifyMacros();
            ILProcessor iLProcessor = refreshListing.Body.GetILProcessor();
            int l1 = 0;
            for (l1 = 0; l1 < refreshListing.Body.Instructions.Count; l1++)
            {
                if (refreshListing.Body.Instructions[l1].OpCode == OpCodes.Callvirt && refreshListing.Body.Instructions[l1 + 1].OpCode == OpCodes.Callvirt)
                {
                    break;
                }
            }
            l1 -= 3;
            refreshListing.Body.Instructions.RemoveAt(l1);
            l1+= 2;
            refreshListing.Body.Instructions.RemoveAt(l1);
            refreshListing.Body.Instructions.RemoveAt(l1);
            var inst = refreshListing.Body.Instructions[l1];
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }))));
            refreshListing.Body.OptimizeMacros();
        }

        private void PatchMirrorCallback()
        {
            var refreshListing = Gimbal.Types.First(td => td.Name == "DesignerScreen").Methods.First(md => md.Name == "GetMirrorComponent");

            refreshListing.Body.SimplifyMacros();
            ILProcessor iLProcessor = refreshListing.Body.GetILProcessor();
            int l1 = 0;
            bool found = false;
            for (l1 = 0; l1 < refreshListing.Body.Instructions.Count; l1++)
            {
                if (refreshListing.Body.Instructions[l1].OpCode == OpCodes.Callvirt && refreshListing.Body.Instructions[l1 + 1].OpCode == OpCodes.Callvirt)
                {
                    if (found)
                    {
                        break;
                    }
                    found = true;
                }
            }
            refreshListing.Body.Instructions.RemoveAt(l1-4);
            refreshListing.Body.Instructions.RemoveAt(l1);
            var inst = refreshListing.Body.Instructions[l1];
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Ultra.ModLoader).GetMethod("CreateInstance", new Type[] { typeof(string) }))));
            for (; l1 < refreshListing.Body.Instructions.Count; l1++)
            {
                if (refreshListing.Body.Instructions[l1].OpCode == OpCodes.Callvirt && refreshListing.Body.Instructions[l1 + 1].OpCode == OpCodes.Callvirt)
                {
                    if (found)
                    {
                        break;
                    }
                    found = true;
                }
            }
            refreshListing.Body.Instructions.RemoveAt(l1 - 4);
            refreshListing.Body.Instructions.RemoveAt(l1);
            inst = refreshListing.Body.Instructions[l1];
            iLProcessor.InsertBefore(inst, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Ultra.ModLoader).GetMethod("CreateInstance", new Type[] { typeof(string) }))));
            refreshListing.Body.OptimizeMacros();
        }

        private void MakeAllPublic()
        {
            foreach (var t in Gimbal.Types)
            {
                t.IsPublic = true;
                foreach (var e in t.Events)
                {
                    if (e.AddMethod != null && !e.AddMethod.IsPublic)
                    {
                        if (t.IsSerializable)
                        {
                            e.CustomAttributes.Add(new CustomAttribute(Gimbal.Import(typeof(XmlIgnoreAttribute).GetConstructor(new Type[] { }))));
                        }
                    }
                }

                foreach (var p in t.Properties)
                {
                    bool blockSerialize = false;
                    if (p.GetMethod != null)
                    {
                        blockSerialize = blockSerialize && !p.GetMethod.IsPublic;
                        p.GetMethod.IsPublic = true;
                    }
                    if (p.SetMethod != null)
                    {
                        blockSerialize = blockSerialize && !p.SetMethod.IsPublic;
                        p.SetMethod.IsPublic = true;
                    }
                    if(blockSerialize)
                    {
                        p.CustomAttributes.Add(new CustomAttribute(Gimbal.Import(typeof(XmlIgnoreAttribute).GetConstructor(new Type[] { }))));
                    }
                }

                foreach (var m in t.Methods)
                {
                    m.IsPublic = true;
                }
                
                foreach (var f in t.Fields)
                {
                    if (!f.IsPublic && ((f.IsNotSerialized && t.IsSerializable) || !t.IsSerializable))
                    {
                        f.IsPublic = true;
                    }
                }

                
            }
        }

        public void HookMod(ModuleDefinition mod, string Filename)
        {
            var ggmInit = Gimbal.Types.First(td => td.Name == "GimbalGameManager").GetConstructors().First(c => c.Parameters.Count == 0);

            Log.Debug("Adding mod load to GimbalGameManager Constructor");
            var modLoad = Gimbal.Import(typeof(Ultra.ModLoader).GetMethod("LoadMod"));
            ggmInit.Body.SimplifyMacros();
            ILProcessor iLProcessor = ggmInit.Body.GetILProcessor();
            Instruction target = ggmInit.Body.Instructions.First<Instruction>();
            iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldstr, Filename));
            iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, modLoad));
            MethodBodyRocks.OptimizeMacros(ggmInit.Body);


            foreach (var t in mod.Types)
            {
                foreach(var m in t.Methods)
                {
                    foreach(var a in m.CustomAttributes)
                    {
                        if(a.AttributeType.Name == "AutoHookAttribute")
                        {
                            string ClassName = (string)a.ConstructorArguments[0].Value;
                            string MethodName = (string)a.ConstructorArguments[1].Value;
                            Console.WriteLine("Hooking " + ClassName + "." + MethodName);
                            Log.Debug("Hooking " + ClassName + "." + MethodName);

                            bool Skip = false;
                            if (a.Properties.Any(na => na.Name == "Skip"))
                            {
                                Skip = (bool)a.Properties.First(na => na.Name == "Skip").Argument.Value;
                            }
                            var Class = Gimbal.Types.First((TypeDefinition td) => td.FullName == ClassName);
                            var Method = Class.Methods.First((MethodDefinition md) =>
                            {
                                if (md.Name == MethodName)
                                {
                                    if (md.IsStatic)
                                    {
                                        if (md.Parameters.Count != m.Parameters.Count)
                                        {
                                            return false;
                                        }
                                        for (int l1 = 0; l1 < md.Parameters.Count; l1++)
                                        {
                                            if (!md.Parameters[l1].ParameterType.FullName.Equals(m.Parameters[l1].ParameterType.FullName))
                                            {
                                                return false;
                                            }
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        if (md.Parameters.Count + 1 != m.Parameters.Count)
                                        {
                                            return false;
                                        }
                                        if(m.Parameters[0].ParameterType.FullName != Class.FullName)
                                        {
                                            return false;
                                        }
                                        for (int l1 = 0; l1 < md.Parameters.Count; l1++)
                                        {
                                            if (!md.Parameters[l1].ParameterType.FullName.Equals(m.Parameters[l1+1].ParameterType.FullName))
                                            {
                                                return false;
                                            }
                                        }
                                        return true;
                                    }
                                }
                                return false;
                            });

                            var mr = Gimbal.Import(m);
                            Method.Body.SimplifyMacros();
                            iLProcessor = Method.Body.GetILProcessor();
                            target = Method.Body.Instructions.First<Instruction>();
                            for (int i = 0; i < Method.Parameters.Count; i++)
                            {
                                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldarg, i));
                            }
                            if(!Method.IsStatic)
                            {
                                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldarg, Method.Parameters.Count));
                            }
                            iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, mr));
                            if (Skip)
                            {
                                iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ret));
                            }
                            MethodBodyRocks.OptimizeMacros(Method.Body);
                        }
                    }
                }
                foreach (var a in t.CustomAttributes)
                {
                    if (a.AttributeType.Name == "CustomPartAttribute")
                    {
                        Console.WriteLine("Adding Custom Part: " + t.Name);
                        Log.Debug("Adding Custom Part: " + t.Name);
                        int val = (int)a.ConstructorArguments[0].Value;
                        
                        var partlisting = Gimbal.Types.First(td => td.Name == "PartsBin").Methods.First(md => md.Name == "GetTypesForCategory");
                        partlisting.Body.SimplifyMacros();
                        iLProcessor = partlisting.Body.GetILProcessor();
                        target = partlisting.Body.Instructions[partlisting.Body.Instructions.Count - 1];
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldloc, 0));
                        target = partlisting.Body.Instructions[partlisting.Body.Instructions.Count - 3];
                        iLProcessor.Replace(target, iLProcessor.Create(OpCodes.Ldarg, 0));
                        target = partlisting.Body.Instructions[partlisting.Body.Instructions.Count - 2];
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldc_I4, val));
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Bne_Un, target));

                        bool MastermindOnly = false;
                        if (a.Properties.Any(na => na.Name == "MastermindOnly"))
                        {
                            MastermindOnly = (bool)a.Properties.First(na => na.Name == "MastermindOnly").Argument.Value;
                        }

                        if(MastermindOnly)
                        {
                            iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldarg, 1));
                            iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Brfalse, target));
                        }

                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldloc, 0));
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Ldtoken, Gimbal.Import(t)));
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Call, Gimbal.Import(typeof(Type).GetMethod("GetTypeFromHandle"))));
                        iLProcessor.InsertBefore(target, iLProcessor.Create(OpCodes.Callvirt, Gimbal.Import(typeof(List<Type>).GetMethod("Add"))));
                        partlisting.Body.OptimizeMacros();
                    }
                }
            }
        }
    }
}
