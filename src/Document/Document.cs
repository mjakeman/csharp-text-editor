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

        private PieceTable pieceTable;

        private ReadOnlyBuffer fileBuffer;
        private AppendBuffer addBuffer;

        private (Descriptor desc, int baseIndex) FromIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();

            var curIndex = 0;

            foreach (Descriptor span in pieceTable)
            {
                curIndex += span.length;

                if (curIndex > index)
                    return (span, curIndex - span.length);
            }

            // We might be the final element -> in which case, create a new descriptor?

            return (null, -1); // throw new IndexOutOfRangeException();
        }

        private Buffer GetBuffer(Descriptor span)
            => span.dest == BufferType.AddBuffer
                ? (Buffer)addBuffer
                : (Buffer)fileBuffer;

        private string DescriptorToString(Descriptor span)
            => GetBuffer(span).GetString(span.offset, span.length);

        public string GetContents()
        {
            var builder = new StringBuilder();
            foreach (Descriptor piece in pieceTable)
                builder.Append(DescriptorToString(piece));

            return builder.ToString();
        }

        public void Insert(int index, string text)
        {
            if (text.Length == 0)
                return;

            if (index < 0)
                throw new IndexOutOfRangeException();

            var newLength = text.Length;
            var newOffset = addBuffer.Append(text);
            var newDesc = new Descriptor(BufferType.AddBuffer, newOffset, newLength);

            var (existingDesc, baseIndex) = FromIndex(index);

            if (existingDesc == null)
            {
                // Node is null - assume we're appending to the end
                pieceTable.AddLast(newDesc);
                return;
            }

            // If we are at a piece boundary, we can simply
            // insert at the beginning and return.
            if (index == existingDesc.offset)
            {
                pieceTable.AddBefore(existingDesc, newDesc);
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
                var endDesc = new Descriptor(existingDesc.dest, endOffset, endLength);
                pieceTable.AddAfter(existingDesc, endDesc);
            }
                
            pieceTable.AddAfter(existingDesc, newDesc);

            // Finally: resize the existing piece to go up until the
            // point of insertion, or alternatively, remove the piece if
            // length is zero.
            if (insertionOffset != 0)
                pieceTable.Replace(existingDesc, existingDesc with { length = startLength });
            else
                pieceTable.Remove(existingDesc);
        }

        public void Delete(int index, int length)
        {

            var (deleteStartDesc, deleteStartNodeBaseIndex) = FromIndex(index);

            // Cannot delete from the end of the sequence
            if (deleteStartDesc == null)
                return;

            if (length < deleteStartDesc.length)
            {
                var internalOffset = index - deleteStartNodeBaseIndex;
                if (internalOffset == 0)
                {
                    // Simple case
                    var newLength = (deleteStartDesc.length - length);
                    var newOffset = deleteStartDesc.offset + length;
                    pieceTable.Replace(deleteStartDesc, deleteStartDesc with { offset = newOffset, length = newLength});
                }
                else
                {
                    // Split into two

                    // Create new
                    var newLength = deleteStartDesc.length - length - internalOffset;
                    var addOffset = addBuffer.Append(GetBuffer(deleteStartDesc).GetString(internalOffset + length, newLength));
                    var newDesc = new Descriptor(BufferType.AddBuffer, addOffset, newLength);
                    pieceTable.AddAfter(deleteStartDesc, newDesc);

                    // Resize original
                    var resizeLength = internalOffset;
                    pieceTable.Replace(deleteStartDesc, deleteStartDesc with { length = resizeLength});
                }
            }
            else
            {
                // Complex case - not supported yet
                throw new NotImplementedException("Cannot delete across piece boundaries yet");
            }
        }

        private Document(string data)
        {
            // Initialise
            fileBuffer = new ReadOnlyBuffer(data);
            addBuffer = new AppendBuffer();
            pieceTable = new();

            pieceTable.AddFirst(new Descriptor(BufferType.FileBuffer, 0, data.Length));
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