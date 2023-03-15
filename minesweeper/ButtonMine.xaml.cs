using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace minesweeper
{

    public partial class ButtonMine : Button
    {
        public int MineCountArround { get; set; } = 0;
        public bool IsMine { get; set; } = false;
        public bool IsHidden { get; set; } = true;
        public bool IsMarked { get; set; } = false;

        public int PosX { get; private set; }
        public int PosY { get; private set; }

        public ButtonMine(int posX, int posY)
        {
            PosX = posX;
            PosY = posY;

            InitializeComponent();
            Refresh();
        }

        public void Refresh() {
            img.Source = MineImage();

            if (!IsHidden && !IsMine && !IsMarked && MineCountArround != 0)
            {
                Content = $"{MineCountArround}";
            }

            if (!IsHidden && MineCountArround == 0)
            {
                Background = new SolidColorBrush(Colors.LightGray);
            }

        }

        BitmapImage MineImage()
        {
            if (IsMarked)
            {
               return new BitmapImage(new Uri("images/Flag2.png", UriKind.Relative));
            }

            if (IsMine && !IsHidden)
            {
                return  new BitmapImage(new Uri("images/Mine.png", UriKind.Relative));
            }

            return null;
        }
    }
}
