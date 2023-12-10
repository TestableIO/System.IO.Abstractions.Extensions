#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Abstractions
{
    internal sealed class AsyncLineEnumerable : IAsyncEnumerable<string>
    {
        private readonly IFileInfo _file;
        private readonly Encoding _encoding;

        public AsyncLineEnumerable(IFileInfo file, Encoding encoding)
        {
            _file = file;
            _encoding = encoding;
        }

        public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncLineEnumerator(_file, _encoding, cancellationToken);
        }
    }

    internal sealed class AsyncLineEnumerator : IAsyncEnumerator<string>
    {
        private Stream _stream;
        private StreamReader _reader;
        private readonly CancellationToken _cancellationToken;
        private string _current;

        public AsyncLineEnumerator(IFileInfo file, Encoding encoding, CancellationToken cancellationToken)
        {
            _stream = file.FileSystem.FileStream.New(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, useAsync: true);
            _reader = new StreamReader(_stream, encoding);
            _cancellationToken = cancellationToken;
        }

        public string Current => _current;

        public async ValueTask DisposeAsync()
        {
            if (_stream == null)
                return;

            _reader.Dispose();
            _reader = null;
            await _stream.DisposeAsync();
            _stream = null;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            _cancellationToken.ThrowIfCancellationRequested();
            _current = await _reader.ReadLineAsync();
            return _current != null;
        }
    }
}
#endif