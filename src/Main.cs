using System;
using GObject;
using Gtk;

namespace Bluetype
{
    using Application;
    
    public class App : Gtk.Application
    {
        const string AppName = "com.mattjakeman.Bluetype";

        public App()
        {
            this.ApplicationId = AppName;
            this.OnActivate += Activate;
        }

        private void Activate(object app, EventArgs args)
        {
            var window = new AppWindow();
            window.Application = this;
            window.Present();
        }

        static int Main(string[] args)
            => new App().Run();
    }
}