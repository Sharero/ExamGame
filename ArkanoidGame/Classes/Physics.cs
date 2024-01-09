using System.Collections.Generic;

namespace ArkanoidGame
{
    public class Physics
    {
        private int tickRateValue = new int();
        private List<CartesianPosition> baseTopRight = new List<CartesianPosition>();
        private List<CartesianPosition> baseTopLeft = new List<CartesianPosition>();
        private List<CartesianPosition> baseBottomRight = new List<CartesianPosition>();
        private List<CartesianPosition> baseBottomLeft = new List<CartesianPosition>();
        public int NumberOfMoves; 
        public Physics(int tickRate)
        {
            tickRateValue = tickRate;
            NumberOfMoves = tickRateValue * 2 - 1;
            InitializeValues();
        }
        private void InitializeValues()
        {
            GenerateBaseTopRightValue();
            GenerateBaseTopLeftValue();
            GenerateBaseBottomRightValue();
            GenerateBaseBottomLeftValue();
        }
        private void GenerateBaseTopRightValue()
        {
            baseTopRight.Add(new CartesianPosition(0, -10));
            //Sharp angle
            for (int i = 1; i <= tickRateValue; i++)
            {
                baseTopRight.Add(new CartesianPosition(tickRateValue, -i));
            }
            //Для сохранения позиций в двух циклах
            //Obtuse angle
            for (int i = tickRateValue; i > 0; i--)
            {
                baseTopRight.Add(new CartesianPosition(i, -tickRateValue));
            }
            //There is dupilcate on tickRateValue and tickRateValue+1 index
            //So, we have to delete one
            baseTopRight.RemoveAt(tickRateValue);
        }
        private void GenerateBaseTopLeftValue()
        {
            //We just have to verticaly reflect baseTopRight values
            for (int i = 0; i < baseTopRight.Count; i++)
            {
                baseTopLeft.Add(new CartesianPosition(-baseTopRight[i].HorizontalPosition, baseTopRight[i].VerticalPosition));
            }
        }
        private void GenerateBaseBottomRightValue()
        {
            //We just have to horizontaly reflect baseTopRight values
            for (int i = 0; i < baseTopRight.Count; i++)
            {
                baseBottomRight.Add(new CartesianPosition(baseTopRight[i].HorizontalPosition, -baseTopRight[i].VerticalPosition));
            }
        }
        private void GenerateBaseBottomLeftValue()
        {
            //We just have to verticaly reflect baseBottomRight values
            for (int i = 0; i < baseTopRight.Count; i++)
            {
                baseBottomLeft.Add(new CartesianPosition(-baseBottomRight[i].HorizontalPosition, baseBottomRight[i].VerticalPosition));
            }
        }
        public CartesianPosition ExtractValue(bool HorizontalPosition, bool VerticalPosition, int position)
        {
            float X = new float();
            float Y = new float();
            if (HorizontalPosition)
            {
                if (VerticalPosition)
                {
                    X = baseTopLeft[position].HorizontalPosition;
                    Y = baseTopLeft[position].VerticalPosition;
                }
                else
                {
                    X = baseBottomLeft[position].HorizontalPosition;
                    Y = baseBottomLeft[position].VerticalPosition;
                }
            }
            else
            {
                if (VerticalPosition)
                {
                    X = baseTopRight[position].HorizontalPosition;
                    Y = baseTopRight[position].VerticalPosition;
                }
                else
                {
                    X = baseBottomRight[position].HorizontalPosition;
                    Y = baseBottomRight[position].VerticalPosition;
                }
            }
            X /= tickRateValue;
            Y /= tickRateValue;
            return new CartesianPosition(X, Y);
        }   
    }
}