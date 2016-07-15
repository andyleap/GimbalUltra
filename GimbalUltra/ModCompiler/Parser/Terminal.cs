using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler.Parser
{
    public static partial class Parse
    {
        public static Parser<char> Any()
        {
            return i =>
            {
                Input nexti = i;
                if (nexti.AtEnd)
                {
                    return Result.Failure<char>(i, "At EOF", null);
                }
                return Result.Success(i.Current, nexti);
            };
        }

        public static Parser<string> Literal(string Match)
        {
            return i =>
            {
                Input nexti = i;
                foreach (var letter in Match)
                {
                    if (nexti.AtEnd)
                    {
                        return Result.Failure<string>(i, "Did not match \"" + Match + "\"", null);
                    }
                    if (letter == nexti.Current)
                    {
                        nexti = nexti.Advance();
                    }
                    else
                    {
                        return Result.Failure<string>(i, "Did not match \"" + Match + "\"", null);
                    }
                }
                return Result.Success(Match, nexti);
            };
        }

        internal struct CharRange
        {
            internal char Lower;
            internal char Upper;
            internal CharRange(char Lower, char Upper)
            {
                this.Lower = Lower;
                this.Upper = Upper;
            }
        }

        public static Parser<char> Set(string Set)
        {
            List<char> rawTrueSet = new List<char>();
            List<CharRange> rawRanges = new List<CharRange>();
            for (int l1 = 0; l1 < Set.Length; l1++)
            {
                if (Set[l1] == '-')
                {
                    if (l1 > 0)
                    {
                        rawTrueSet.RemoveAt(rawTrueSet.Count - 1);
                        rawRanges.Add(new CharRange(Set[l1 - 1], Set[l1 + 1]));
                        l1++;
                    }
                    else
                    {
                        rawTrueSet.Add(Set[l1]);
                    }
                }
                else if (Set[l1] == '\\')
                {
                    rawTrueSet.Add(Set[l1 + 1]);
                    l1++;
                }
                else
                {
                    rawTrueSet.Add(Set[l1]);
                }
            }
            return i =>
            {
                if (i.AtEnd)
                {
                    return Result.Failure<char>(i, "EOF", null);
                }
                foreach (char c in rawTrueSet)
                {
                    if (c == i.Current)
                    {
                        Input nexti = i.Advance();
                        return Result.Success(i.Current, nexti);
                    }
                }
                foreach (var crange in rawRanges)
                {
                    if (i.Current >= crange.Lower && i.Current <= crange.Upper)
                    {
                        Input nexti = i.Advance();
                        return Result.Success(i.Current, nexti);
                    }
                }
                return Result.Failure<char>(i, "Did not match \"" + Set + "\"", null);
            };
        }

        public static Parser<char> NotSet(string Set)
        {
            List<char> rawTrueSet = new List<char>();
            List<CharRange> rawRanges = new List<CharRange>();
            for (int l1 = 0; l1 < Set.Length; l1++)
            {
                if (Set[l1] == '-')
                {
                    if (l1 > 0)
                    {
                        rawTrueSet.RemoveAt(rawTrueSet.Count - 1);
                        rawRanges.Add(new CharRange(Set[l1 - 1], Set[l1 + 1]));
                        l1++;
                    }
                    else
                    {
                        rawTrueSet.Add(Set[l1]);
                    }
                }
                else if (Set[l1] == '\\')
                {
                    rawTrueSet.Add(Set[l1 + 1]);
                    l1++;
                }
                else
                {
                    rawTrueSet.Add(Set[l1]);
                }
            }
            return i =>
            {
                if (i.AtEnd)
                {
                    return Result.Failure<char>(i, "EOF", null);
                }
                foreach (char c in rawTrueSet)
                {
                    if (c == i.Current)
                    {
                        return Result.Failure<char>(i, "Did match \"" + Set + "\"", null);
                    }
                }
                foreach (var crange in rawRanges)
                {
                    if (i.Current >= crange.Lower && i.Current <= crange.Upper)
                    {
                        return Result.Failure<char>(i, "Did match \"" + Set + "\"", null);
                    }
                }
                Input nexti = i.Advance();
                return Result.Success(i.Current, nexti);
            };
        }
    }
}
