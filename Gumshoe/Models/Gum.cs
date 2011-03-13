using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Gumshoe.Models
{
    public enum GumColours { Red, Blue, Green, Orange };

    public class Gum
    {
        public string Source { get; private set; }
        public GumColours Colour { get; private set; }
        public Gum InnerGum { get; private set; }

        public Gum(GumColours colour, Gum innerGum)
        {
            this.Colour = colour;
            this.InnerGum = innerGum;

            // Sets the image source
                switch (this.Colour)
                {
                    case GumColours.Red:
                        this.Source = "/Gumshoe;component/GFX/RedGum.png";
                        break;
                    case GumColours.Blue:
                        this.Source = "/Gumshoe;component/GFX/BlueGum.png";
                        break;
                    case GumColours.Green:
                        this.Source = "/Gumshoe;component/GFX/GreenGum.png";
                        break;
                    case GumColours.Orange:
                        this.Source = "/Gumshoe;component/GFX/OrangeGum.png";
                        break;
                    default:
                        break;
                }
        }
    }
}
