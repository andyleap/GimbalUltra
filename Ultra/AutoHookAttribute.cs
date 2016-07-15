using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultra
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class AutoHookAttribute : Attribute
    {
        readonly string _Class;
        readonly string _Method;

        // This is a positional argument
        public AutoHookAttribute(string Class, string Method)
        {
            this._Class = Class;
            this._Method = Method;
        }

        public string Class
        {
            get { return _Class; }
        }

        public string Method
        {
            get { return _Method; }
        }

        public bool Skip
        {
            get;
            set;
        }
    }
}
