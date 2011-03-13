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
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using Gumshoe.Models;
using System.ComponentModel;
using Gumshoe.Views;

namespace Gumshoe
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        /*
        TODOS:
         * We need to create a dictionary<int level, int queuesize> to populate the queue on each level
          and determine the number of flowers for each level. 
         * We need to randomly select the colours of the gum and
         * and we need to check after a tile is placed if it creates a pattern to delete
         * */
        private ObservableCollection<Gum> _gQueue;
        private int _curLevel;
        private int _totalScore;
        private int _curLevelScore;
        private int boardX = 6;
        private int boardY = 8;
        private Dictionary<int, int> levelQueueAmount = new Dictionary<int, int>();

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;
            //Initialize the level info
            //(lvl,queue amount
            levelQueueAmount.Add(1, 15);
            levelQueueAmount.Add(2, 15);
            levelQueueAmount.Add(3, 15);
            //inner gums
            levelQueueAmount.Add(4, 20);
            levelQueueAmount.Add(5, 20);
            levelQueueAmount.Add(6, 20);
            //introduce stick
            levelQueueAmount.Add(7, 20);
            levelQueueAmount.Add(8, 25);
            levelQueueAmount.Add(9, 25);
            //introduce something else
            levelQueueAmount.Add(10, 35);

            // Initilizes the playing queue
            this.GQueue = new ObservableCollection<Gum>();
            this._curLevelScore = 0;
            this._curLevel = 1;
            this.InitializeNextLevel();
        }

        private void InitializeNextLevel()
        {
            this.GQueue.Clear();
            //this.ResetBoard();
            this._totalScore += this._curLevelScore;
            this._curLevelScore = 0;
            this.InitializeGQueue(this._curLevel);
            this._curLevel++;
        }

        public void InitializeGQueue(int lvl)
        {
            //set this number based on level
            int numElements = levelQueueAmount[lvl];
            Random random = new Random();
            int colourNum;
            int innerColourNum;
            for (int i = 0; i < numElements; i++)
            {
                //randomly pick a colour
                colourNum = (int)Math.Floor(random.NextDouble() * 4);

                Gum innerGum = null;
                // TODO, set back to 4
                if (lvl >= 1)
                {
                    innerColourNum = (int)Math.Floor(random.NextDouble() * 4);

                    if (colourNum != innerColourNum)
                    {
                        innerGum = new Gum((GumColours)innerColourNum, null);
                    }
                }

                this.GQueue.Add(new Gum((GumColours)colourNum, innerGum));
            }

        }

        private void ResetBoard()
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this.Board.LayoutRoot); i++)
            {
                (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).ResetGum();
            }
        }

        public ObservableCollection<Gum> GQueue
        {
            get { return this._gQueue; }
            set
            {
                if (value != this._gQueue)
                {
                    this._gQueue = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("GQueue"));
                }
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        private void PhoneApplicationPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<UIElement> hits = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(this), this) as List<UIElement>;

            Tile targetTile = null;

            foreach (UIElement el in hits)
            {
                if (el is Tile)
                {
                    targetTile = el as Tile;
                    break;
                }
            }

            if (null != targetTile && this.IsValidTile(targetTile))
            {
                this.AddGumToTile(targetTile);
            }
        }

        private bool IsValidTile(Tile t)
        {
            return null == t.Gum;
        }

        private void matching()
        {
            //check for pairs
            List<int> matchings = new List<int>();
            bool anyMatching = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this.Board.LayoutRoot) - 2; i++)
            {
                //horizontally
                int j = i + 1;
                int k = i + 2;
                if (j % boardX > i % boardX && k % boardX > i % boardX &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, j) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, k) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum.Colour == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, j) as Tile).Gum.Colour &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum.Colour == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, k) as Tile).Gum.Colour)
                {
                    anyMatching = true;
                    matchings.Add(i);
                    matchings.Add(j);
                    matchings.Add(k);
                }
                //vertically
                j = i + boardX;
                k = i + boardX * 2;
                if (k < boardX * boardY &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, j) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, k) as Tile).Gum != null &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum.Colour == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, j) as Tile).Gum.Colour &&
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum.Colour == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, k) as Tile).Gum.Colour)
                {
                    anyMatching = true;
                    matchings.Add(i);
                    matchings.Add(j);
                    matchings.Add(k);
                }
            }
            if (anyMatching)
            {
                MessageBox.Show("You Got a Matching!");
            }

            //remove pairs
            for (int i = 0; i < matchings.Count; i++)
            {
                // TODO: I think this should be the opposite,
                // EX: You need to remove the inner gum before you remove the outter gum
                // this behaviour is the opposite of flowers, copying flower behvaiour for now
                // if we do this, it will change the matching alg you used too, lets keep it this way for now
                if (null == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, matchings[i]) as Tile).Gum.InnerGum)
                {
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, matchings[i]) as Tile).ResetGum();
                }
                else
                {
                    // remove outter gum, show inner gum
                    Gum iGum = (VisualTreeHelper.GetChild(this.Board.LayoutRoot, matchings[i]) as Tile).Gum.InnerGum;
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, matchings[i]) as Tile).ResetGum();
                    (VisualTreeHelper.GetChild(this.Board.LayoutRoot, matchings[i]) as Tile).AddGum(iGum);
                    // TODO: re-eval the matchings since we now have inner gums to match with
                    // should move the matching stuff to a helper function
                }
                this._curLevelScore += 10;
            }
        }

        private void AddGumToTile(Tile t)
        {
            Gum g = this._gQueue.ElementAt(0);
            this._gQueue.RemoveAt(0);

            t.AddGum(g);
            
            // TODO: Check for points
            matching();

            // Is Level Complete?
            if (this._gQueue.Count == 0)
            {
                //Show score for level
                int temp = this._curLevelScore + this._totalScore;
                MessageBox.Show("Level Complete. " + this._curLevelScore + " Points this level. " + temp + " Points total.");
                this.InitializeNextLevel();
            }

            // Is there still open tiles, or is it game over?
            bool gameOver = true;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this.Board.LayoutRoot); i++)
            {
                if (null == (VisualTreeHelper.GetChild(this.Board.LayoutRoot, i) as Tile).Gum)
                {
                    gameOver = false;
                    break;
                }
            }

            if (gameOver)
            {
                int temp = this._curLevelScore + this._totalScore;
                MessageBox.Show("Game Over. " + temp + " Points Total.");
                //TODO: Show score, return to main menu
            }
            
        }
    }
}