using System;
using Xunit;

namespace Bluetype.Tests
{
    using Bluetype.Document;

    public class DocumentTest
    {
        [Fact]
        public void TestInsertAtStart()
        {
            var doc = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            doc.Insert(0, "Hello! ");

            Assert.True(doc.GetContents() == "Hello! The quick brown fox jumps over the lazy dog.");
        }

        [Fact]
        public void TestInsertAtEnd()
        {
            var doc = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            doc.Insert(44, " Bye!");

            Assert.True(doc.GetContents() == "The quick brown fox jumps over the lazy dog. Bye!");
        }

        [Fact]
        public void TestInsertOutOfRange()
        {
            var doc = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            doc.Insert(10000, " Bye!");

            // Default behaviour is to append at the end
            Assert.True(doc.GetContents() == "The quick brown fox jumps over the lazy dog. Bye!");
        }

        [Fact]
        public void TestNegativeIndex()
        {
            Assert.Throws<IndexOutOfRangeException>(() => {
                var doc = Document.NewFromString("Make sure not to use a negative index when inserting");
                doc.Insert(-5, "Oops!");
            });
        }
    }
}
