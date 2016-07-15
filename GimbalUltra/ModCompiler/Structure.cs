using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace GimbalUltra.ModCompiler
{
    class Structure : Part
    {
        public Structure(Config config) : base(config)
        {
        }

        public override TypeDefinition AddPart(Mod mod)
        {
            return AddType(mod, typeof(Colin.Gimbal.Component), Colin.Gimbal.PartsCategories.Structure);
        }
    }
}
