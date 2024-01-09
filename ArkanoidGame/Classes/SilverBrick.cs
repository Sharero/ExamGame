
namespace ArkanoidGame
{
    public class SilverBrick : SimpleBrick
    {
        public SilverBrick(int value, int timesToBreak) : base("#626161", value)
        {
            TimesToBreak = timesToBreak;
        }
    }
}