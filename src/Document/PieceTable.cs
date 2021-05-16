using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Bluetype.Document
{
    public class PieceTable : IEnumerable<Span>
    {
        // TODO: Using Find() is O(n) -> We should store the information in the descriptor
        // itself and avoid lookup entirely. Consider using a custom LinkedList implementation.

        private LinkedList<Span> table = new();

        public void AddAfter(Span span, Span insert)
        {
            LinkedListNode<Span> node = table.Find(span);
            table.AddAfter(node, insert);
        }

        public void AddBefore(Span span, Span insert)
        {
            LinkedListNode<Span> node = table.Find(span);
            table.AddBefore(node, insert);
        }

        public void AddFirst(Span insert)
            => table.AddFirst(insert);

        public void AddLast(Span insert)
            => table.AddLast(insert);

        public void Replace(Span old, Span replace)
            => table.Find(old).Value = replace;

        public void Remove(Span remove)
            => table.Remove(remove);

        // Implement IEnumerable
        public IEnumerator<Span> GetEnumerator()
            => table.GetEnumerator();
    
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}