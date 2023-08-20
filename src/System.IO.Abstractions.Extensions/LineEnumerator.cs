using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.IO.Abstractions
{
    internal sealed class LineEnumerable : IEnumerable<string>
    {
        private readonly IFileInfo _file;
        private readonly Encoding _encoding;

        public LineEnumerable(IFileInfo file, Encoding encoding)
        {
            _file = file;
            _encoding = encoding;
        }

        public IEnumerator<string> GetEnumerator() => new LineEnumerator(_file, _encoding);

        IEnumerator IEnumerable.GetEnumerator() => new LineEnumerator(_file, _encoding);
    }

    internal sealed class LineEnumerator : IEnumerator<string>
    {
        private Stream _stream;
        private StreamReader _reader;
        private string _current;

        public LineEnumerator(IFileInfo file, Encoding encoding)
        {
            _stream = file.OpenRead();
            _reader = encoding == null
                ? new StreamReader(_stream)
                : new StreamReader(_stream, encoding);
        }

        public string Current => _current;

        object IEnumerator.Current => _current;

        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;
            _stream?.Dispose();
            _stream = null;
        }

        public bool MoveNext()
        {
            _current = _reader.ReadLine();
            return _current != null;
        }

        public void Reset()
        {
            throw new InvalidOperationException();
        }
    }
}
