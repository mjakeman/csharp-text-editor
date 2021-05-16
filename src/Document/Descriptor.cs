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

    // Also called: Piece, Span, etc
    public record Descriptor(BufferType dest, int offset, int length);
}