using GimbalUltra.ModCompiler.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler
{
    class CommonGrammar
    {
        public static Parser<string> whitespace =
            from ws in Parse.Set(" \r\n\t").Mult(1).String()
            from comment in Parse.And(Parse.Literal("//"), Parse.NotSet("\n").Mult().String(), Parse.Literal("\n")).Optional()
            select ws;

        public static Parser<string> optionalWhitespace =
            from ws in Parse.Set(" \r\n\t").Mult().String()
            from comment in Parse.And(Parse.Literal("//"), Parse.NotSet("\n").Mult().String(), Parse.Literal("\n")).Optional()
            select ws;
        public static Parser<string> identifier =
            from leadingWhitespace in optionalWhitespace
            from leading in Parse.Set("a-zA-Z")
            from rest in Parse.Set("a-zA-Z0-9_").Mult().String()
            from trailingWhitespace in optionalWhitespace
            select leading.ToString() + rest;

        public static Parser<object> BoolTrue =
            from True in Parse.Literal("true")
            select (object)true;
        public static Parser<object> BoolFalse =
            from False in Parse.Literal("false")
            select (object)false;
        public static Parser<object> Bool =
            Parse.Or(BoolTrue, BoolFalse);

        public static Parser<object> Int =
            from negative in Parse.Literal("-").Optional("")
            from number in Parse.Set("0-9").Mult(1).String()
            select (object)int.Parse(negative + number);

        public static Parser<object> Float =
            from negative in Parse.Literal("-").Optional("")
            from number in Parse.Set("0-9").Mult(1).String()
            from period in Parse.Literal(".")
            from fractional in Parse.Set("0-9").Mult(1).String()
            select (object)float.Parse(negative + number + "." + fractional);

        public static Parser<object> Color =
            from pound in Parse.Literal("#")
            from R in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from G in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from B in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from A in Parse.Set("0-9a-fA-F").Mult(2, 2).String().Optional("FF")
            select (object)(new Microsoft.Xna.Framework.Graphics.Color(Convert.ToByte(R, 16), Convert.ToByte(G, 16), Convert.ToByte(B, 16), Convert.ToByte(A, 16)));

        public static Parser<object> String =
            from openquote in Parse.Literal("\"")
            from content in Parse.Or(Parse.NotSet("\\\""), Parse.Literal("\\\"").Select(s => '"')).Mult().String()
            from closequote in Parse.Literal("\"")
            select content;

        public static Parser<object> Constant =
            Parse.Or(Bool, Float, Int, Color, String, Parse.Ref(() => Object));

        public static Parser<KeyValuePair<string, object>> FieldValue =
            from ws0 in optionalWhitespace
            from field in identifier
            from ws1 in optionalWhitespace
            from equal in Parse.Literal("=")
            from ws2 in optionalWhitespace
            from value in Constant
            from ws3 in optionalWhitespace
            select new KeyValuePair<string, object>(field, value);

        public static Parser<Config> Object =
            from ws1 in optionalWhitespace
            from start in Parse.Literal("{")
            from fields in FieldValue.Mult()
            from ws2 in optionalWhitespace
            from end in Parse.Literal("}")
            from ws3 in optionalWhitespace
            select new Config(fields.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }
}
