using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultra
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class CustomPartAttribute : Attribute
    {
        public readonly Colin.Gimbal.PartsCategories Category;
        public CustomPartAttribute(Colin.Gimbal.PartsCategories Category)
        {
            this.Category = Category;
        }

        public bool MastermindOnly
        {
            get;
            set;
        }
    }
}
