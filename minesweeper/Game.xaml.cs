using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace minesweeper
{

    public partial class Game : Window
    {
        private int MinesToSpawn;
		private int ButtonSize;
        private int BarHeight;
        private int Size;
        private int MinesRemaing;
        DispatcherTimer MyTimer = new DispatcherTimer();
        bool GameStarted = false;
        private int timeElapsed = 0;

        ButtonMine[,] Buttons;

		public Game()
        {
            InitializeComponent();

            ButtonSize = 50;
            BarHeight = 200;
            Size = 15;
            MinesToSpawn = 20;
            MyTimer.Interval = TimeSpan.FromSeconds(1);
            MyTimer.Tick += UpadateTime;

            NewGame();
        }
        public void NewGame()
        {
            Width = Size * ButtonSize;
            Height = Size * ButtonSize + BarHeight;

            Buttons = new ButtonMine[Size, Size];

            timeElapsed = 0;
            GameStarted = false;
            timer.Text = timeElapsed.ToString();

            // odstran childy pokud existuji
            if (grid.Children.Count > 0)
            {
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }

            for (int x = 0; x < Size; x++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                for (int y = 0; y < Size; y++)
                {
                    ButtonMine btn = new ButtonMine(x, y);

                    btn.MouseRightButtonUp += RightClick;
                    btn.Click += LeftClick;
                    grid.Children.Add(btn);
                    Buttons[x, y] = btn;

                    Grid.SetRow(btn, x);
                    Grid.SetColumn(btn, y);
                }
            }
            MinesRemaing = PlaceMines(Buttons, Size, MinesToSpawn);
            mineCounter.Text = MinesRemaing.ToString();

            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    int mineCountAround = 0;

                    /*
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (x + dx < 0 || y + dy < 0 || x + dx >= Size || y + dy >= Size) continue;

                            if (Buttons[dx, dy].IsMine)
                            {
                                mineCountAround++;
                            }
                        }
                    }*/
                    
                    void funkce(int dx, int dy)
                    {
                        if (Buttons[dx, dy].IsMine)
                        {
                            mineCountAround++;
                        }
                    }

                    /*Action<int, int> lambda = (int dx, int dy) =>
                    {
                        if (Buttons[dx, dy].IsMine)
                        {
                            mineCountAround++;
                        }
                    }*/

                    RunOnNeighbors(x, y, funkce);
                    Buttons[x, y].MineCountArround = mineCountAround;
                }
        }

        private void UpadateTime(object sender, EventArgs e)
        {
            timeElapsed++;
            timer.Text = TimeSpan.FromSeconds(timeElapsed).ToString("mm':'ss");
        }

        int PlaceMines(ButtonMine[,] buttons, int size, int desiredMineCount)
        {
            int currentMineCount = 0;
            Random r = new Random();
            if (desiredMineCount >= size * size)  desiredMineCount = size * size - 1;

            while (currentMineCount < desiredMineCount) {
                int x = r.Next(size);
                int y = r.Next(size);

                if (!buttons[x, y].IsMine){
                    buttons[x, y].IsMine = true;
                    currentMineCount++;                    
                }
            }
            return currentMineCount;
        }

        private void RightClick(object sender, MouseButtonEventArgs e)
        {
            ButtonMine sdr = sender as ButtonMine;

            // oznac / odoznac policko
            if (sdr.IsHidden)
            {
                sdr.IsMarked = !sdr.IsMarked;                
                sdr.Refresh();
                if (sdr.IsMarked) MinesRemaing--;
                else MinesRemaing++;

                mineCounter.Text = MinesRemaing.ToString();
            }
            CheckWin();
        }

        

        private void CheckWin() {
            int revealed = 0;
            int mines = 0;
            int marked = 0;
            int count = Buttons.Length;

            foreach (var btn in Buttons) {
                if (btn.IsMine) mines++;
                if (btn.IsMarked) marked++;
                if (!btn.IsHidden) revealed++;
            }

            //Trace.WriteLine($"{count} {revealed} {mines} {marked}");

            if (revealed + mines == count) EndGame(true);

        }


        private void LeftClick( object sender, RoutedEventArgs e ) {

            if (!GameStarted)
            {
                MyTimer.Start();
                GameStarted = true;
            }
            
			ButtonMine btn = sender as ButtonMine;
            if (btn.IsMarked) return;

            var posX = btn.PosX;
            var posY = btn.PosY;
            //Trace.WriteLine();

            if (btn.IsMine)
            { // konec hry
                btn.IsHidden = false;
                btn.Refresh();
                EndGame(false);                
            }

            // odhal prazdne policko + okolo
            if (btn.MineCountArround == 0 && btn.IsHidden) {
                btn.IsHidden = false;
                RevealAround(posX, posY); 
            }

            // odhal policko s cislem
            else if (btn.MineCountArround > 0 && btn.IsHidden) {
                btn.IsHidden = false;
            }
          
            // na odhalenem policku: odhal okolni policka
            if (btn.MineCountArround == CountFlags(posX, posY) && !btn.IsHidden) {
                RevealAround(posX, posY);
            }

			btn.Refresh();
            CheckWin();
		}


        private int CountFlags(int posX, int posY)
        {
            int counter = 0;

            RunOnNeighbors(posX, posY, (dx, dy) => {
                var btn = Buttons[dx, dy];
                if (btn.IsMarked)counter++;
            });


            return counter;
		}

        private void RunOnNeighbors(int posX, int posY, Action<int, int> triggerFn) {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++) {
                    if (posX + dx < 0 || posY + dy < 0 || posX + dx >= Size || posY + dy >= Size) continue;

                    triggerFn(posX + dx, posY + dy);
                }
        }


        private void RevealAround(int posX, int posY)
        {
            RunOnNeighbors(posX, posY, (dx, dy) => { 
                var btn = Buttons[dx, dy];

                if (btn.IsMine && !btn.IsMarked) { 
                    btn.IsHidden = false;
                    btn.Refresh();
                    EndGame(false); 
                }

				if (btn.IsHidden && !btn.IsMarked) {
                    btn.IsHidden = false;
                    btn.Refresh();

                    if (btn.MineCountArround == 0)
                        RevealAround(dx, dy);
                }
            });
		}

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void EndGame(bool victory)
        {
            MyTimer.Stop();
            string time = TimeSpan.FromSeconds(timeElapsed).ToString();
            EndWindow endWindow = new EndWindow();            
            endWindow.EndText.Text = victory? "Vyhrál jsi!": "Prohrál jsi!";
            endWindow.TextTimer.Text = $"Tvůj čas je: {time}";
            endWindow.ShowDialog();

            if (endWindow.result) {
                NewGame();
            } else Close();
        }
    }
}
