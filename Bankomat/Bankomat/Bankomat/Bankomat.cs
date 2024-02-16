using Bankomat.BazaDanych;
using Bankomat.Konfiguracja;
using Bankomat.Karty;

namespace Bankomat.Bankomat
{
    public class Bankomat
    {
        private List<string> akceptowaneTypyKart;
        private Karta wprowadzonaKarta;
        private SystemBankowy.SystemBankowy bankingSystem;
        private MenadzerBazyDanych menadzerBazyDanych;
        private MenadzerKonfiguracji menadzerKonfiguracji;
        
        public Bankomat(MenadzerBazyDanych menadzerBazyDanych, MenadzerKonfiguracji menadzerKonfiguracji, SystemBankowy.SystemBankowy bankingSystem)
        {
            this.menadzerKonfiguracji = menadzerKonfiguracji;
            this.akceptowaneTypyKart = menadzerKonfiguracji.WczytajKonfiguracje();
            this.bankingSystem = bankingSystem;
            this.menadzerBazyDanych = menadzerBazyDanych;
        }

        public bool WprowadzPin(string pin)
        {
            if (wprowadzonaKarta != null && bankingSystem.WeryfikujPin(wprowadzonaKarta.NumerKarty, pin))
            {
                Console.WriteLine("PIN poprawny.");
                return true;
            }
            else
            {
                Console.WriteLine("Niepoprawny PIN.");
                return false;
            }
        }

        public void KwotaWyplaty(decimal kwota)
        {
            if (wprowadzonaKarta == null)
            {
                Console.WriteLine("Najpierw wprowadź kartę.");
                return;
            }

            if (bankingSystem.SprawdzStanKonta(wprowadzonaKarta.NumerKarty, kwota) && bankingSystem.RealizujWyplate(wprowadzonaKarta.NumerKarty, kwota))
            {
                Console.WriteLine($"Wypłacano {kwota}.");
            }
            else
            {
                Console.WriteLine("Nie można zrealizować wypłaty.");
            }
        }
        
        private string RozpoznajTypKarty(string numerKarty)
        {
            int prefix = int.Parse(numerKarty.Substring(0, 4));
            if (prefix >= 1000 && prefix <= 2499) return "Visa";
            if (prefix >= 2500 && prefix <= 4999) return "VisaElectron";
            if (prefix >= 5000 && prefix <= 7499) return "MasterCard";
            if (prefix >= 7500 && prefix <= 9999) return "AmericanExpress";
            return null;
        }
        
        private Karta StworzKarte(string typKarty, string numerKarty)
        {
            switch (typKarty)
            {
                case "Visa":
                    return new Visa(numerKarty);
                case "VisaElectron":
                    return new VisaElectron(numerKarty);
                case "MasterCard":
                    return new MasterCard(numerKarty);
                case "AmericanExpress":
                    return new AmericanExpress(numerKarty);
                default:
                    return null;
            }
        }
        
        private string WeryfikujIZamienNumerKarty(string numerKarty)
        {
            var oczyszczonyNumerKarty = numerKarty.Replace(" ", "");
            if (!ulong.TryParse(oczyszczonyNumerKarty, out _))
            {
                throw new ArgumentException("Numer karty zawiera nieprawidłowe znaki. Numer karty powinien zawierać tylko cyfry.");
            }
            return oczyszczonyNumerKarty;
        }

        private void WeryfikujPin(string pin)
        {
            if (!pin.All(char.IsDigit))
            {
                throw new ArgumentException("PIN powinien zawierać tylko cyfry.");
            }
            if (pin.Length != 4)
            {
                throw new ArgumentException("PIN powinien składać się z 4 cyfr.");
            }
        }


        public void Uruchom()
        {
            Console.WriteLine("\nSymulacja działania bankomatu.");
            try
            {
                Console.WriteLine("Proszę wprowadzić numer karty:");
                var numerKartyInput = Console.ReadLine();
                var numerKarty = WeryfikujIZamienNumerKarty(numerKartyInput);

                var typKarty = RozpoznajTypKarty(numerKarty);
                if (typKarty == null || !akceptowaneTypyKart.Contains(typKarty))
                {
                    Console.WriteLine($"Karta typu {typKarty} nie jest akceptowana.");
                    return;
                }

                this.wprowadzonaKarta = StworzKarte(typKarty, numerKarty);
                Console.Write("Karta zaakceptowana.\nProszę wprowadzić PIN: ");

                var pin = Console.ReadLine();
                WeryfikujPin(pin);
                if (!WprowadzPin(pin)) return;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            bool dzialaj = true;
            while (dzialaj)
            {
                Console.WriteLine("\nWybierz opcję:\n1. Sprawdź stan konta\n2. Wypłać środki\n3. Zmień PIN\n4. Wyjście");
                var wybor = Console.ReadLine();
                switch (wybor)
                {
                    case "1":
                        SprawdzStanKonta();
                        break;
                    case "2":
                        Console.WriteLine("Podaj kwotę do wypłaty:");
                        var kwota = decimal.Parse(Console.ReadLine());
                        KwotaWyplaty(kwota);
                        break;
                    case "3":
                        ZmienPin();
                        break;
                    case "4":
                        dzialaj = false;
                        break;
                    default:
                        Console.WriteLine("Nieznane polecenie.\n");
                        break;
                }
            }
        }

        private void SprawdzStanKonta()
        {
            if (wprowadzonaKarta == null)
            {
                Console.WriteLine("Nie wprowadzono karty.\n");
                return;
            }
    
            var stanKonta = bankingSystem.SprawdzStanKonta(wprowadzonaKarta.NumerKarty);
            Console.WriteLine($"Stan konta: {stanKonta}");
        }


        private void ZmienPin()
        {
            if (wprowadzonaKarta == null)
            {
                Console.WriteLine("Nie wprowadzono karty.\n");
                return;
            }

            Console.WriteLine("Proszę wprowadzić obecny PIN:");
            var obecnyPin = Console.ReadLine();
            if (!bankingSystem.WeryfikujPin(wprowadzonaKarta.NumerKarty, obecnyPin))
            {
                Console.WriteLine("Niepoprawny PIN.\n");
                return;
            }

            Console.WriteLine("Proszę wprowadzić nowy PIN:");
            var nowyPin = Console.ReadLine();
            if (bankingSystem.ZmienPin(wprowadzonaKarta.NumerKarty, obecnyPin, nowyPin))
            {
                Console.WriteLine("PIN został zmieniony.\n");
            }
            else
            {
                Console.WriteLine("Nie udało się zmienić PIN.\n");
            }
        }
    }
}
