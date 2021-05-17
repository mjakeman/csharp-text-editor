using System;
using Gtk;

namespace Bluetype.Application
{
    using Document;

    public class AppWindow : Gtk.Window
    {
        const string WindowTitle = "Editor";

        private Box contentBox;
        private Entry insertEntry;
        private SpinButton spinButton;
        private DocumentView documentView;

        public Document Document { get; set; }

        string cachedDocContents = string.Empty;

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

            contentBox = Box.New(Orientation.Vertical, 0);
            this.Child = contentBox;

            // Start by creating a document
            Document = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            documentView = new DocumentView(Document);
            contentBox.PackStart(documentView, true, true, 0);
            // cachedDocContents = Document.GetContents();
            // label.SetText(cachedDocContents);
            // UpdateCursor();

            // label = Label.New(string.Empty);
            // label.SetSingleLineMode(false);
            // var docView = new DocumentView();
            // contentBox.PackStart(label, true, true, 0);

            var hbox = Box.New(Orientation.Horizontal, 0);
            contentBox.PackEnd(hbox, false, false, 0);

            // Text to insert
            insertEntry = Entry.New();
            hbox.PackStart(insertEntry, true, true, 0);

            // Index to insert
            spinButton = SpinButton.NewWithRange(0, Int32.MaxValue, 1);
            spinButton.OnValueChanged += (_,_) => UpdateCursor();
            spinButton.SetDigits(0);
            hbox.PackStart(spinButton, false, false, 0);

            var insertButton = new Button("Insert");
            insertButton.Label = "Insert"; // <-- TODO: While construct args are broken
            insertButton.OnClicked += Insert;
            hbox.PackEnd(insertButton, false, false, 0);

            var deleteButton = new Button("Delete");
            deleteButton.Label = "Delete"; // <-- TODO: While construct args are broken
            deleteButton.OnClicked += Delete;
            hbox.PackEnd(deleteButton, false, false, 0);

            // Set visible
            headerBar.ShowAll();
            contentBox.ShowAll();
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