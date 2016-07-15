using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler.Parser
{
    public interface IResult<out T>
    {
        /// <summary>
        /// Gets the resulting value.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Gets a value indicating whether wether parsing was successful.
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the parser expectations in case of error.
        /// </summary>
        IEnumerable<string> Expectations { get; }

        /// <summary>
        /// Gets the remainder of the input.
        /// </summary>
        Input Remainder { get; }
    }

    public static class Result
    {
        public static IResult<T> Success<T>(T value, Input remainder)
        {
            return new Result<T>(value, remainder);
        }
        public static IResult<T> Failure<T>(Input remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    public class Result<T> : IResult<T>
    {


        private readonly T _value;
        private readonly Input _remainder;
        private readonly bool _wasSuccessful;
        private readonly string _message;
        private readonly IEnumerable<string> _expectations;

        public Result(T value, Input remainder)
        {
            _value = value;
            _remainder = remainder;
            _wasSuccessful = true;
            _message = null;
            _expectations = Enumerable.Empty<string>();
        }

        public Result(Input remainder, string message, IEnumerable<string> expectations)
        {
            _value = default(T);
            _remainder = remainder;
            _wasSuccessful = false;
            _message = message;
            _expectations = expectations;
        }

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new InvalidOperationException("No value can be computed.");

                return _value;
            }
        }

        public bool WasSuccessful { get { return _wasSuccessful; } }

        public string Message { get { return _message; } }

        public IEnumerable<string> Expectations { get { return _expectations; } }

        public Input Remainder { get { return _remainder; } }

        public override string ToString()
        {
            if (WasSuccessful)
                return string.Format("Successful parsing of {0}.", Value);

            var expMsg = "";

            if (Expectations != null && Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);

            return string.Format("Parsing failure: {0};{1} ({2});", Message, expMsg, Remainder);
        }
    }
}
