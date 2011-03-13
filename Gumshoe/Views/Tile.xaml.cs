using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Gumshoe.Models;
using System.ComponentModel;

namespace Gumshoe.Views
{
    public partial class Tile : UserControl, INotifyPropertyChanged
    {
        public Gum Gum { get; private set; }
        public bool Enabled { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public Tile()
        {
            this.Gum = null;
            this.Enabled = true;
            InitializeComponent();
            this.DataContext = this;
            this.OnPropertyChanged(new PropertyChangedEventArgs("Gum"));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public void AddGum(Gum g)
        {
            this.Gum = g;
            this.OnPropertyChanged(new PropertyChangedEventArgs("Gum"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Source"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("InnerSource"));
        }

        public void ResetGum()
        {
            this.Gum = null;
            this.OnPropertyChanged(new PropertyChangedEventArgs("Gum"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("Source"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("InnerSource"));
        }

        public void IsTileEnabled(bool flag)
        {
            this.Enabled = flag;
            this.OnPropertyChanged(new PropertyChangedEventArgs("Source"));
            this.OnPropertyChanged(new PropertyChangedEventArgs("InnerSource"));
        }

        public string Source
        {
            get
            {
                if (this.Enabled)
                {
                    return this.Gum.Source;
                }
                else
                {
                    return "/Gumshoe;component/GFX/Disabled.png";
                }
            }
        }

        public string InnerSource
        {
            get
            {
                if (this.Enabled && null != this.Gum.InnerGum)
                {
                    return this.Gum.InnerGum.Source;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}