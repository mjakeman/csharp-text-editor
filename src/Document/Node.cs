using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Bluetype.Document
{
    public enum BufferType : byte
    {
        File,
        Add
    };

    // Also called: Span, Piece, Descriptor, etc
    public record Node
    {
        internal BufferType location { get; init; }
        internal int offset { get; init; }
        internal int length { get; init; }

        public Node(BufferType location, int offset, int length)
        {
            this.location = location;
            this.offset = offset;
            this.length = length;
        }
    }
}