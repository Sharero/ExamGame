
namespace ArkanoidGame
{
    public class SimpleBrick : Brick
    {
        private int _value;
        private int _timesToBreak;
        public SimpleBrick(string color, int value) : base(color, true)
        {
            _value = value;
            _timesToBreak = 1;
        }
        public override int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public override int TimesToBreak
        {
            get
            {
                return _timesToBreak;
            }
            set
            {
                _timesToBreak = value;
            }
        }
    }
}