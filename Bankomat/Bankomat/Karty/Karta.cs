namespace Bankomat.Karty
{
    public abstract class Karta
    {
        public string NumerKarty { get; }
        public string TypKarty { get; protected set; }

        protected Karta(string numerKarty)
        {
            NumerKarty = numerKarty;
        }
    }
}