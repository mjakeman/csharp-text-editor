using cairo;
using Gtk;

namespace Bluetype.Application
{
    using Document;
    
    public class PieceTableDisplay : Bin
    {
        private Document doc;
        private DrawingArea drawingArea;
        public PieceTableDisplay(Document document)
        {
            doc = document;
            doc.DocumentChanged += (_,_) => QueueDraw();

            drawingArea = DrawingArea.New();
            drawingArea.OnDraw += Render;
            Child = drawingArea;
            ShowAll();
        }

        private void Render(object sender, DrawSignalArgs args)
        {
            Context cr = args.Cr;
            
            double xPos = 40;
            double yPos = 60;
            int padding = 5;
            
            cr.MoveTo(xPos, 30);
            cr.SetFontSize(16);
            cr.ShowText("Piece Table Visualisation");

            cr.FontExtents(out FontExtents fontExtents);
            cr.SetFontSize(14);

            double position = xPos;
            foreach (Node node in doc.Contents)
            {
                // Get text for node
                string text = doc.RenderNode(node);
                
                // DRAW: Buffer Rectangle
                cr.MoveTo(position, yPos);
                
                cr.TextExtents(text, out TextExtents lineExtents);
                var length = lineExtents.xAdvance;
                var height = fontExtents.Height;

                // Set colour
                if (node.location == BufferType.File)
                    cr.SetSourceRgba(1, 0, 0, 1);
                else
                    cr.SetSourceRgba(0, 0, 1, 1);
                
                cr.Rectangle(position, yPos + fontExtents.Descent, length, height);
                cr.Fill();
                
                // DRAW: Buffer Text
                cr.MoveTo(position, yPos);
                cr.SetSourceRgba(0,0,0,1);
                cr.ShowText(text);
                
                // Move to next position - TODO: line wrap?
                position += length + padding;
            }
        }
    }
}