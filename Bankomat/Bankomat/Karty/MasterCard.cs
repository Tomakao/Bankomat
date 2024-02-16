namespace Bankomat.Karty
{
    public class MasterCard : Karta
    {
        public MasterCard(string numerKarty) : base(numerKarty)
        {
            TypKarty = "MasterCard";
        }
    }
}