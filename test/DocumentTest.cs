using System;
using Xunit;

namespace Bluetype.Tests
{
    using Bluetype.Document;

    public class DocumentTest
    {
        [Fact]
        public void TestInsertAtZero()
        {
            var doc = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            doc.Insert(0, "Hello! ");

            Assert.True(doc.GetContents() == "Hello! The quick brown fox jumps over the lazy dog.");
        }
    }
}
