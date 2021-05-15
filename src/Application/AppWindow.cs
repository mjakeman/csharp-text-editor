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
        private Label label;
        private SpinButton spinButton;

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

            contentBox = Box.New(Orientation.Vertical, 0);
            this.Child = contentBox;

            label = Label.New(string.Empty);
            label.SetSingleLineMode(false);
            contentBox.PackStart(label, true, true, 0);

            var hbox = Box.New(Orientation.Horizontal, 0);
            contentBox.PackEnd(hbox, false, false, 0);

            // Text to insert
            insertEntry = Entry.New();
            hbox.PackStart(insertEntry, true, true, 0);

            // Index to insert
            spinButton = SpinButton.NewWithRange(0, Int32.MaxValue, 1);
            spinButton.SetDigits(0);
            hbox.PackStart(spinButton, false, false, 0);

            var button = new Button("Insert");
            button.Label = "Insert"; // <-- TODO: While construct args are broken
            button.OnClicked += Insert;
            hbox.PackEnd(button, false, false, 0);

            // Start by creating a document
            Document = Document.NewFromString("The quick brown fox jumps over the lazy dog.");
            label.LabelProp = Document.GetContents();

            // Set visible
            headerBar.ShowAll();
            contentBox.ShowAll();
        }

        void Insert(Button button, EventArgs args)
        {
            var index = spinButton.GetValue();
            var text = insertEntry.GetText();
            Document.Insert((int)index, text);

            // Update label
            label.SetText(Document.GetContents());
        }
    }
}