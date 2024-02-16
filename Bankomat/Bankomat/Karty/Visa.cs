namespace Bankomat.Karty
{
    public class Visa : Karta
    {
        public Visa(string numerKarty) : base(numerKarty)
        {
            TypKarty = "Visa";
        }
    }
}