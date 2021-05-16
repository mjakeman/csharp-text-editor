using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Bluetype.Document
{
    public class PieceTable : IEnumerable<Descriptor>
    {
        // TODO: Using Find() is O(n) -> We should store the information in the descriptor
        // itself and avoid lookup entirely. Consider using a custom LinkedList implementation.

        private LinkedList<Descriptor> table = new();

        public void AddAfter(Descriptor span, Descriptor insert)
        {
            LinkedListNode<Descriptor> node = table.Find(span);
            table.AddAfter(node, insert);
        }

        public void AddBefore(Descriptor span, Descriptor insert)
        {
            LinkedListNode<Descriptor> node = table.Find(span);
            table.AddBefore(node, insert);
        }

        public void AddFirst(Descriptor insert)
            => table.AddFirst(insert);

        public void AddLast(Descriptor insert)
            => table.AddLast(insert);

        public void Replace(Descriptor old, Descriptor replace)
            => table.Find(old).Value = replace;

        public void Remove(Descriptor remove)
            => table.Remove(remove);

        // Implement IEnumerable
        public IEnumerator<Descriptor> GetEnumerator()
            => table.GetEnumerator();
    
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}