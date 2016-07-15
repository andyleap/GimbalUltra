using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GimbalUltra.ModCompiler.Parser
{
    public class Input
    {
        private readonly string _source;
        private readonly int _position;
        private readonly int _line;
        private readonly int _column;

        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            _source = source;
            _position = position;
            _line = line;
            _column = column;
        }

        public Input Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new Input(_source, _position + 1, Current == '\n' ? _line + 1 : _line, Current == '\n' ? 1 : _column + 1);
        }

        public string Source { get { return _source; } }

        public char Current { get { return _source[_position]; } }

        public bool AtEnd { get { return _position == _source.Length; } }

        public int Position { get { return _position; } }

        public int Line { get { return _line; } }

        public int Column { get { return _column; } }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }
    }
}
