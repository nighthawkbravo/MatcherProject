using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.IO;
using Matcher.Classes;

namespace Matcher
{
    public partial class MainWindow : Window
    {
        private BrushConverter bc = new BrushConverter();
        private string lightPurple = "#383660";
        private string mylightRed = "#FF5555";
        private string mylightGreen = "#42f548";

        private string PlayersListStrings;
        private List<Player> Players = new List<Player>();
        private List<Player> tmpPlayers = new List<Player>();

        private int numOfRounds = 1;
        private int numOfPlayers = 0;
        private int counterLimit;
        private bool canIRun = false;

        private string currentSetting = "Random";

        private Random rnd;

        ComboBoxItem cbi;

        public MainWindow()
        {
            InitializeComponent();

            cbi = (ComboBoxItem)AssignmentBox.SelectedItem;
            rnd = new Random(Guid.NewGuid().GetHashCode());
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void UploadButton(object sender, RoutedEventArgs e)
        {
            if (Players.Count != 0)
            {
                Players.Clear();
                numOfPlayers = 0;
            }
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a text file";
            op.Filter = "Text file (*.txt)|*.txt;";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UpFileName.Text = $"{op.FileName}";
                PlayersListStrings = File.ReadAllText(op.FileName);
            }
            else return;

            //StringBuilder sb;
            StringReader sr = new StringReader(PlayersListStrings);

            string line = sr.ReadLine();
            while(line != null)
            {
                int firstSpace = line.IndexOf(" ", 0);
                int SecondSpace = line.IndexOf(" ", firstSpace+1);
                int firstPar = SecondSpace + 1;
                int SecondPar = line.IndexOf(")", firstPar+1);

                Player p = new Player(line.Substring(0, firstSpace), line.Substring(firstSpace + 1, SecondSpace - firstSpace), line.Substring(firstPar + 1, SecondPar - SecondSpace - 2));
                Players.Add(p);
                ++numOfPlayers;
                line = sr.ReadLine();
            }
            
            CheckNumOfRounds(null, null);

            PlayerCount.Text = numOfPlayers.ToString();

            string all = $"List of players ({numOfPlayers}):\n";
            int count = 0;
            foreach (var p in Players)
            {
                all += ("\t" + p.Print() + "\n");
                count++;
                if (count % 4 == 0) all += "\n";
            }
            all += "\nPlease confirm that the list is correct.";
            MessageBox.Show(all);
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (!canIRun)
            {
                MessageBox.Show("ERROR");
                return;
            }
            counterLimit = numOfPlayers * numOfRounds;
            foreach (var p in Players)
            {
                p.SetOpponents(numOfRounds);
            }

            switch (currentSetting)
            {
                case "Random":
                    AssignRandomly();
                    break;
                case "Global faction":

                    break;
                case "Sub faction":

                    break;
            }
        }
        private void ButtonClose(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void CheckNumOfRounds(object sender, TextChangedEventArgs e)
        {
            int num;
            bool b = Int32.TryParse(NumberOfRoundsBox.Text, out num);

            if (b && num > 0 && (numOfPlayers > num))
            {
                NumberOfRoundsBox.Background = (Brush)bc.ConvertFrom(lightPurple);
                numOfRounds = num;
                canIRun = true;
            }
            else
            {
                NumberOfRoundsBox.Background = (Brush)bc.ConvertFrom(mylightRed);
                canIRun = false;
            }
        }
        private void AssignmentChange(object sender, SelectionChangedEventArgs e)
        {
            if(cbi != null) currentSetting = cbi.Content.ToString();
        }

        private void AssignRandomly()
        {
            int counter = 0;

            tmpPlayers.Clear();
            foreach (var p in Players)
            {
                tmpPlayers.Add(p);
            }

            for (int i = 0; i < numOfRounds; ++i)
            {
                if(tmpPlayers.Count == 0)
                {
                    foreach(var p in Players)
                    {
                        tmpPlayers.Add(p);
                    }
                }
                int count = Players.Count;
                for (int j = 0; j < count / 2; ++j)
                {
                    int r1;
                    int r2;
                    bool b;
                    
                    do
                    {
                        r1 = rnd.Next(0, tmpPlayers.Count - 1);
                        r2 = rnd.Next(0, tmpPlayers.Count - 1);
                        while (r2 == r1)
                        {
                            r2 = rnd.Next(0, tmpPlayers.Count);
                        }
                        b = !(tmpPlayers[r1].TryAssigning(tmpPlayers[r2]) && tmpPlayers[r2].TryAssigning(tmpPlayers[r1]));
                        ++counter;
                        if (counter > counterLimit) break;
                    } while (b);

                    if (counter > counterLimit) break;

                    Player p1 = tmpPlayers[r1];
                    Player p2 = tmpPlayers[r2];

                    tmpPlayers.Remove(p1);
                    tmpPlayers.Remove(p2);

                }
                if (counter > counterLimit) break;
            }
            if (counter > counterLimit)
            {
                ClearPairings();
                AssignRandomly();
            }
            else SavePairings();
        }

        private void ClearPairings()
        {
            foreach(var p in Players)
            {
                p.opponents.Clear();
            }
        }
        private void PrintPairings()
        {
            string all = "Pairings:\n";

            foreach(var p in Players)
            {
                all += $"\t{p.PrintWithOpponents()}\n";
            }
            all += "\nDONE";
            MessageBox.Show(all);
        }
        private void SavePairings()
        {
            string all = "Pairings:\n";

            foreach (var p in Players)
            {
                all += $"\t{p.PrintWithOpponents()}\n";
            }
            all += "\nDONE";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) File.WriteAllText(saveFileDialog.FileName, all);
        }


    }
}
