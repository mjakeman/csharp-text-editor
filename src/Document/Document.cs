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

        public IEnumerable<Span> Contents => pieceTable;
        
        public event EventHandler DocumentChanged;


        private PieceTable pieceTable;
        private ReadOnlyBuffer fileBuffer;
        private AppendBuffer addBuffer;

        private (Span desc, int baseIndex) FromIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();

            var curIndex = 0;

            foreach (Span span in pieceTable)
            {
                curIndex += span.length;

                if (curIndex > index)
                    return (span, curIndex - span.length);
            }

            // We might be the final element -> in which case, create a new descriptor?

            return (null, -1); // throw new IndexOutOfRangeException();
        }

        private Buffer GetBuffer(Span span)
            => span.dest == BufferType.AddBuffer
                ? (Buffer)addBuffer
                : (Buffer)fileBuffer;

        public string SpanToString(Span span)
            => GetBuffer(span).GetString(span.offset, span.length);

        public string GetContents()
        {
            var builder = new StringBuilder();

            foreach (Span piece in pieceTable)
                builder.Append(SpanToString(piece));

            return builder.ToString();
        }

        private Span CreateSpan(string text)
        {
            var index = addBuffer.Append(text);
            return new Span(BufferType.AddBuffer, index, text.Length);
        }

        public void Insert(int index, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (index < 0)
                throw new IndexOutOfRangeException();

            // Create a new entry into the add buffer
            var insertSpan = CreateSpan(text);

            // Find the piece which contains the cursor index
            var (currentSpan, baseIndex) = FromIndex(index);
            

            // Case 1: If the current span is null, append to the end
            // of the document and return.
            if (currentSpan == null)
            {
                pieceTable.AddLast(insertSpan);
                DocumentChanged(this, EventArgs.Empty);
                return;
            }

            // Case 2: If we are at the start boundary, we can simply
            // insert at the beginning and return.
            else if (index - baseIndex == 0)
            {
                pieceTable.AddBefore(currentSpan, insertSpan);
                DocumentChanged(this, EventArgs.Empty);
                return;
            }

            // Case 3: We split the current span into three. The contents of the
            // span up to the insertion point, the insertion itself, and
            // the remainder of the span after the insertion.
            else
            {
                // Find the index at which to split, relative to the
                // start of the current piece.
                var insertionOffset = index - baseIndex;

                // We have one piece |current|
                var startLength = insertionOffset;
                var endLength = currentSpan.length - startLength;

                // Split into |start| |end|
                var startSpan = currentSpan with {length = startLength};
                var endSpan = currentSpan with {length = endLength, offset = startLength};

                // Insert so we have three pieces: |start| |insertion| |end|
                pieceTable.AddAfter(currentSpan, endSpan);
                pieceTable.AddAfter(currentSpan, insertSpan);
                pieceTable.Replace(currentSpan, startSpan);

                DocumentChanged(this, EventArgs.Empty);
            }
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
                    var newDesc = new Span(BufferType.AddBuffer, addOffset, newLength);
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

            // Emit document-changed event
            DocumentChanged(this, EventArgs.Empty);
        }

        private Document(string data)
        {
            // Initialise
            fileBuffer = new ReadOnlyBuffer(data);
            addBuffer = new AppendBuffer();
            pieceTable = new();

            pieceTable.AddFirst(new Span(BufferType.FileBuffer, 0, data.Length));
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