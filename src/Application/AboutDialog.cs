using System;
using System.Reflection;
using GdkPixbuf;
using GObject;

namespace Bluetype.Application
{
    public class AboutDialog : Gtk.AboutDialog
    {
        public AboutDialog()
        {
            this.Authors = new[] { "Matthew Jakeman <mjak923@aucklanduni.ac.nz>" };
            this.Comments = "Bluetype is a word processor for writing long-form documents.";
            this.Copyright = "© Matthew Jakeman 2021-present";
            this.License = "Mozilla Public License 2.0";
            this.Logo = LoadFromResource("Bluetype.bluetype.png");
            this.Version = "0.1.0";
            this.Website = "https://www.mattjakeman.com";
            this.LicenseType = Gtk.License.Custom;
            this.ProgramName = $"Bluetype";
        }
        
        private static Pixbuf LoadFromResource(string resourceName)
        {
            try
            {
                var bytes = Assembly.GetExecutingAssembly().ReadResourceAsByteArray(resourceName);
                return PixbufLoader.FromBytes(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to load image resource '{resourceName}': {e.Message}");
                return null;
            }
        }
    }
}