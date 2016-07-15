using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler
{
    class Armor : Part
    {
        public Armor(Config config) : base(config)
        {
        }

        public override TypeDefinition AddPart(Mod mod)
        {
            return AddType(mod, typeof(Colin.Gimbal.Component), Colin.Gimbal.PartsCategories.Armor);
        }
    }
}
