using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArkanoidGame
{
    public class Ball
    {
        public int rad { get; set; } // радиус шара
        public int speed { get; set; }
        public double posX { get; set; } // - вверх, + вниз
        public double posY { get; set; } // - влево, + вправо
        public double trajectoryX { get; set; } // - влево, + вправо
        public double trajectoryY { get; set; } // - влево, + вправо
        public int position { get; set; }
        public bool top { get; set; } // top = true; bottom = false;
        public bool left { get; set; } // left = true; right = false;

        public bool stop { get; set; }
        public bool iAmShoot { get; set; }
        public bool iAmBossShoot { get; set; }

        public void InitBall(Ellipse ballEclipse, int firstPosition)
        {
            stop = true;
            position = firstPosition;
            rad = Convert.ToInt32(ballEclipse.Height) / 2; // диаметр / 2
            posX = Convert.ToInt32(Canvas.GetLeft(ballEclipse));
            posY = Convert.ToInt32(Canvas.GetTop(ballEclipse));

            ballEclipse.Fill = new SolidColorBrush(Color.FromRgb(139, 164, 223));
            trajectoryX = 1;
            trajectoryY = 1;

            top = true;
            left = true;
            iAmShoot = false;
            iAmBossShoot = false;
        }
    }
}
