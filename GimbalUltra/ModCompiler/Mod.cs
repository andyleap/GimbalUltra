using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace GimbalUltra.ModCompiler
{
    class Mod
    {
        internal ModuleDefinition mod;
        internal string Namespace;
        internal ModuleDefinition gimbal;

        public Mod(string name, ModuleDefinition gimbal)
        {
            mod = ModuleDefinition.CreateModule(name, ModuleKind.Dll);
            Namespace = name;
            this.gimbal = gimbal;
        }

        public TypeReference GetType(string type)
        {
            return mod.Import(gimbal.Types.First(t => t.Name == type));
        }

        public MethodReference GetMethod(string type, string method, int paramCount)
        {
            return mod.Import(GetType(type).Resolve().Methods.First(m => m.Name == method && m.Parameters.Count == paramCount));
        }

        public FieldReference GetField(string type, string field)
        {
            return mod.Import(GetType(type).Resolve().Fields.First(f => f.Name == field));
        }

        public MethodReference GetGetMethod(string type, string property)
        {
            return mod.Import(GetType(type).Resolve().Properties.First(p => p.Name == property).GetMethod);
        }

        public MethodReference GetSetMethod(string type, string property)
        {
            return mod.Import(GetType(type).Resolve().Properties.First(p => p.Name == property).SetMethod);
        }
    }
}
