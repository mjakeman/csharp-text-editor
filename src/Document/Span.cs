using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Bluetype.Document
{
    public enum BufferType : byte
    {
        FileBuffer,
        AddBuffer
    };

    // Also called: Piece, Descriptor, etc
    public record Span(BufferType dest, int offset, int length);
}