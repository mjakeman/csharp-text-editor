using System;
using System.Text;
using System.Diagnostics;
using Gtk;

namespace Bluetype.Application
{
    using Document;

    public class DocumentView : Gtk.Bin
    {
        private Document doc;
        private int cursorIndex;

        private string cachedText;

        private Label textLayout; // <-- replace with something more sophisticated

        public DocumentView(Document document)
        {
            textLayout = Label.New(string.Empty);
            Child = textLayout;
            SetDocument(document);

            ShowAll();
        }

        public void SetDocument(Document document)
        {
            doc = document;
            doc.DocumentChanged += OnDocumentChanged;

            UpdateCache();
            Redraw();

            cursorIndex = 0;
        }

        public void SetCursorIndex(int index)
        {
            cursorIndex = Math.Clamp(index, 0, cachedText.Length);
            Redraw();
        }

        public void CloseDocument()
        {
            doc = null;
            cursorIndex = 0;
            cachedText = null;
        }

        private void OnDocumentChanged(object sender, EventArgs e)
        {
            // The document has been modified - redraw
            // TODO: Redraw the currently on-screen section only
            Debug.Assert(sender == doc);
            UpdateCache();
            Redraw();
        }

        private void UpdateCache()
        {
            var builder = new StringBuilder();

            // Assemble the document
            foreach (Node textSpan in doc.Contents)
                builder.Append(doc.RenderNode(textSpan));

            cachedText = builder.ToString();
        }

        private void Redraw()
        {
            textLayout.LabelProp = cachedText.Insert(cursorIndex, "|");
        }
    }
}