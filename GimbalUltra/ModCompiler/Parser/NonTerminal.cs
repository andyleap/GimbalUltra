using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler.Parser
{
    public static partial class Parse
    {
        public static Parser<List<T>> And<T>(params Parser<T>[] P)
        {
            return i =>
            {
                var nexti = i;
                List<T> resses = new List<T>();
                foreach (var parser in P)
                {
                    var res = parser(nexti);
                    if (res.WasSuccessful)
                    {
                        resses.Add(res.Value);
                    }
                    else
                    {
                        return Result.Failure<List<T>>(nexti, res.Message, null);
                    }
                    nexti = res.Remainder;
                }
                return Result.Success(resses, nexti);
            };
        }

        public static Parser<T> Or<T>(params Parser<T>[] P)
        {
            return i =>
            {
                foreach (var parser in P)
                {
                    var res = parser(i);
                    if (res.WasSuccessful)
                    {
                        return res;
                    }
                }
                return Result.Failure<T>(i, "Or Failed", null);
            };
        }

        public static Parser<List<T>> Mult<T>(this Parser<T> P, int n = 0, int m = int.MaxValue)
        {
            return i =>
            {
                var nexti = i;
                List<T> resses = new List<T>();
                for (int l1 = 0; l1 < m; l1++)
                {
                    var res = P(nexti);
                    if (!res.WasSuccessful)
                    {
                        if (l1 >= n)
                        {
                            return Result.Success(resses, nexti);
                        }
                        else
                        {
                            return Result.Failure<List<T>>(nexti, string.Format("Only got {0}, needed {1}", l1, n), null);
                        }
                    }
                    resses.Add(res.Value);
                    nexti = res.Remainder;
                }
                return Result.Success(resses, nexti);
            };
        }

        public static Parser<T> Optional<T>(this Parser<T> P)
        {
            return i => P(i).IfFailure(_ => Result.Success(default(T), i));
        }

        public static Parser<T> Optional<T>(this Parser<T> P, T def)
        {
            return i => P(i).IfFailure(_ => Result.Success(def, i));
        }

        public static Parser<List<T>> Optional<T>(this Parser<List<T>> P)
        {
            return i => P(i).IfFailure(_ => Result.Success(new List<T>(), i));
        }

        public static Parser<string> String(this Parser<List<char>> P)
        {
            return i =>
            {
                var res = P(i);
                if (res.WasSuccessful)
                {
                    return Result.Success(new string(res.Value.ToArray()), res.Remainder);
                }
                return Result.Failure<string>(res.Remainder, res.Message, res.Expectations);
            };
        }

        public static List<T> Concat<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }
    }
}
