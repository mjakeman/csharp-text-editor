using System;
using Gtk;

namespace Bluetype.Application
{
    using Document;

    public class AppWindow : Gtk.Window
    {
        const string WindowTitle = "Editor";

        private Paned contentPaned;
        private Entry insertEntry;
        private SpinButton spinButton;
        private DocumentView documentView;

        public Document Document { get; set; }

        public AppWindow()
        {
            this.DefaultWidth = 800;
            this.DefaultHeight = 600;

            // Create UI
            var headerBar = HeaderBar.New();
            headerBar.ShowCloseButton = true;
            headerBar.Title = WindowTitle;
            this.SetTitlebar(headerBar);
            this.SetDecorated(true);

            contentPaned = Paned.New(Orientation.Vertical);
            this.Child = contentPaned;

            // Start by creating a document
            Document = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            documentView = new DocumentView(Document);

            var display = new PieceTableDisplay(Document);
            
            // Paned
            contentPaned.Add1(documentView);
            contentPaned.Add2(display);
            contentPaned.Position = 400;
            
            // Set visible
            headerBar.ShowAll();
            contentPaned.ShowAll();
        }

        void UpdateCursor()
        {
            var value = Math.Clamp((int)spinButton.GetValue(), 0, Int32.MaxValue-1);
            documentView.SetCursorIndex(value);
        }

        void Insert(Button button, EventArgs args)
        {
            var index = spinButton.GetValue();
            var text = insertEntry.GetText();
            Document.Insert((int)index, text);

            // Move cursor to end of insertion
            spinButton.Value += text.Length;
        }

        void Delete(Button button, EventArgs args)
        {
            var index = (int)spinButton.GetValue();
            Document.Delete(index, 1);
        }
    }
}