using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArkanoidGame
{
    public abstract class Brick
    {
        private string _color;
        private bool _canBreak;

        public Brick(string color, bool canBreak)
        {
            _color = color;
            _canBreak = canBreak;
        }
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public bool CanBreak
        {
            get
            {
                return _canBreak;
            }
            set
            {
                _canBreak = value;
            }
        }
        public abstract int Value { get; set; }
        public abstract int TimesToBreak { get; set; }

        public static void GenerateElements(ref Canvas myCanvas, ref Brick[,] bricks, int width, int height)
        {
            int top = 0;
            int left = 0;
            for (int i = 0; i < 13; i++)
            { //x
                for (int j = 0; j < 21; j++) //y
                {

                    if (bricks[i, j] != null)
                    {
                        // Create brick
                        Rectangle rec = new Rectangle()
                        {
                            Width = width / 13,
                            Height = height / 26,
                            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bricks[i, j].Color)),
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                            Tag = "brick"
                        };

                        // Add to a canvas
                        myCanvas.Children.Add(rec);
                        Canvas.SetTop(rec, top);
                        Canvas.SetLeft(rec, left);
                    }
                    top += (height / 26);
                }
                left += (width / 13);
                top = 0;
            }
        }
    }
}