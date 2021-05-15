using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Bluetype.Document
{
    /// <summary>
    /// A document is a representation of one single text buffer. It
    /// may be up to 2GB in size (approximately 1 billion characters),
    /// which is the maximum limit of the C# String data type.
    /// </summary>
    public class Document
    {
        // See: https://web.archive.org/web/20180223071931/https://www.cs.unm.edu/~crowley/papers/sds.pdf
        // This paper demonstrates the piece-table based data structure used to store the
        // document's text and subsequent modifications.

        public record Descriptor(bool addBuffer, int offset, int length);

        private LinkedList<Descriptor> _pieceTable = new();

        private readonly string _file;
        private string _add;

        public string GetContents()
        {
            var builder = new StringBuilder();
            foreach (Descriptor piece in _pieceTable)
            {
                var text = piece.addBuffer
                    ? _add.Substring(piece.offset, piece.length)
                    : _file.Substring(piece.offset, piece.length);

                builder.Append(text);
            }

            return builder.ToString();
        }

        private Document(string data)
        {
            // Initialise
            _file = data;
            _add = string.Empty;
            _pieceTable = new();

            _pieceTable.AddFirst(new Descriptor(false, 0, data.Length));
        }

        public static Document New() => new Document(string.Empty);
        public static Document NewFromString(string data) => new Document(data);

        public static Document NewFromFile(FileInfo fileInfo)
        {
            var contents = fileInfo.OpenRead().ToString();

            return new Document(contents);
        }
    }
}