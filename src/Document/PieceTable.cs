using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Bluetype.Document
{
    internal class PieceTable : IEnumerable<Node>
    {
        // TODO: Using Find() is O(n) -> We should store the information in the descriptor
        // itself and avoid lookup entirely. Consider using a custom LinkedList implementation.

        // Also: https://code.visualstudio.com/blogs/2018/03/23/text-buffer-reimplementation

        private LinkedList<Node> table = new();

        public void AddAfter(Node span, Node insert)
        {
            LinkedListNode<Node> node = table.Find(span);
            table.AddAfter(node, insert);
        }

        public void AddBefore(Node span, Node insert)
        {
            LinkedListNode<Node> node = table.Find(span);
            table.AddBefore(node, insert);
        }

        public void AddFirst(Node insert)
            => table.AddFirst(insert);

        public void AddLast(Node insert)
            => table.AddLast(insert);

        public void Replace(Node old, Node replace)
            => table.Find(old).Value = replace;

        public void Remove(Node remove)
            => table.Remove(remove);

        // Implement IEnumerable
        public IEnumerator<Node> GetEnumerator()
            => table.GetEnumerator();
    
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}