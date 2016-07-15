using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler.Parser
{
    public static partial class Parse
    {
        public static IResult<U> IfSuccess<T, U>(this IResult<T> result, Func<IResult<T>, IResult<U>> next)
        {
            if (result == null) throw new ArgumentNullException("result");

            if (result.WasSuccessful)
                return next(result);

            return Result.Failure<U>(result.Remainder, result.Message, result.Expectations);
        }

        public static IResult<T> IfFailure<T>(this IResult<T> result, Func<IResult<T>, IResult<T>> next)
        {
            if (result == null) throw new ArgumentNullException("result");

            return result.WasSuccessful
                ? result
                : next(result);
        }

        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }

        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (convert == null) throw new ArgumentNullException("convert");

            return parser.Then(t => Return(convert(t)));
        }

        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (selector == null) throw new ArgumentNullException("selector");
            if (projector == null) throw new ArgumentNullException("projector");

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        public static Parser<T> Ref<T>(Func<Parser<T>> reference)
        {
            Parser<T> p = null;
            return i =>
            {
                if (p == null)
                    p = reference();
                var result = p(i);
                return result;
            };
        }
    }
}
