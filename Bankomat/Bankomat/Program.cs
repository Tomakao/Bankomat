using Bankomat.BazaDanych;
using Bankomat.Konfiguracja;
using Bankomat.SystemBankowy;

class Program
{
    static void Main(string[] args)
    {
        var menadzerBazyDanych = new MenadzerBazyDanych();
        var menadzerKonfiguracji = new MenadzerKonfiguracji(menadzerBazyDanych);
        var systemBankowy = new SystemBankowy(menadzerBazyDanych);
        var bankomat = new Bankomat.Bankomat.Bankomat(menadzerBazyDanych, menadzerKonfiguracji, systemBankowy);
        while (true)
        {
            Console.WriteLine("Witaj w Symulatorze Bankomatu!");
            Console.WriteLine("1. Uruchom Bankomat\n2. Tryb Serwisowy\n3. Wyjście");
            var wybor = Console.ReadLine();
            Console.Clear();

            switch (wybor)
            {
                case "1":
                    UruchomBankomat(bankomat);
                    break;
                case "2":
                    menadzerKonfiguracji.TrybSerwisowy();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Nieznane polecenie.");
                    break;
            }
        }
    }

    static void UruchomBankomat(Bankomat.Bankomat.Bankomat bankomat)
    {
        Console.WriteLine("Bankomat gotowy do użycia.");
        bankomat.Uruchom();
    }
}