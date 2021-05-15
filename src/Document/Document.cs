using System;
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

        // Also called: Piece, Span, etc
        public record Descriptor(bool addBuffer, int offset, int length);

        private LinkedList<Descriptor> _pieceTable = new();

        private readonly string _file;
        private string _add;

        private (LinkedListNode<Descriptor> desc, int baseIndex) FromIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();

            var curIndex = 0;

            for (var desc = _pieceTable.First; desc != null; desc = desc.Next)
            {
                curIndex += desc.Value.length;

                if (curIndex > index)
                    return (desc, curIndex - desc.Value.length);
            }

            // We might be the final element -> in which case, create a new descriptor?

            return (null, -1); // throw new IndexOutOfRangeException();
        }

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

        public void Insert(int index, string text)
        {
            if (text.Length == 0)
                return;

            if (index < 0)
                throw new IndexOutOfRangeException();

            // TODO: Use a better buffer data structure?
            var newOffset = _add.Length;
            var newLength = text.Length;
            
            var newDesc = new Descriptor(true, newOffset, newLength);
            _add += text;

            var (existingPieceNode, baseIndex) = FromIndex(index);

            if (existingPieceNode == null)
            {
                // Node is null - assume we're appending to the end
                _pieceTable.AddLast(newDesc);
                return;
            }

            Descriptor existingDesc = existingPieceNode.Value;

            // If we are at a piece boundary, we can simply
            // insert at the beginning and return.
            if (index == existingDesc.offset)
            {
                _pieceTable.AddBefore(existingPieceNode, newDesc);
                return;
            }

            // We split the above piece into three, for the
            // first part, the newly inserted part, and the
            // remaning last part.

            // Find the point within the piece in which to split
            var insertionOffset = index - baseIndex;

            // Calculate new size/offset for start and end pieces
            var startLength = insertionOffset;
            var endLength = existingDesc.length - startLength;
            var endOffset = startLength;

            if (endLength != 0)
            {
                var endDesc = new Descriptor(existingDesc.addBuffer, endOffset, endLength);
                _pieceTable.AddAfter(existingPieceNode, endDesc);
            }
                
            _pieceTable.AddAfter(existingPieceNode, newDesc);

            // Finally: resize the existing piece to go up until the
            // point of insertion, or alternatively, remove the piece if
            // length is zero.
            if (insertionOffset != 0)
                existingPieceNode.Value = existingDesc with { length = startLength };
            else
                _pieceTable.Remove(existingDesc);
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