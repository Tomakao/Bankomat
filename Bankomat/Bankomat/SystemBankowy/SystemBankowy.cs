using Bankomat.BazaDanych;

namespace Bankomat.SystemBankowy
{
    public class SystemBankowy
    {
        private MenadzerBazyDanych menadzerBazyDanych;

        public SystemBankowy(MenadzerBazyDanych menadzerBazyDanych)
        {
            this.menadzerBazyDanych = menadzerBazyDanych;
        }

        public decimal SprawdzStanKonta(string numerKarty)
        {
            return menadzerBazyDanych.PobierzStanKonta(numerKarty);
        }

        public bool ZmienPin(string numerKarty, string obecnyPin, string nowyPin)
        {
            // Logika zmiany PIN-u, weryfikując najpierw obecny PIN
            if (WeryfikujPin(numerKarty, obecnyPin))
            {
                return menadzerBazyDanych.ZmienPin(numerKarty, nowyPin);
            }
            return false;
        }

        public bool WeryfikujPin(string numerKarty, string pin)
        {
            return menadzerBazyDanych.WeryfikujPin(numerKarty, pin);
        }

        public bool SprawdzStanKonta(string numerKarty, decimal kwota)
        {
            // Pobranie stanu konta i sprawdzenie, czy wystarcza środków
            var stanKonta = menadzerBazyDanych.PobierzStanKonta(numerKarty);
            return stanKonta >= kwota;
        }

        public bool RealizujWyplate(string numerKarty, decimal kwota)
        {
            // Sprawdzenie stanu konta
            if (!SprawdzStanKonta(numerKarty, kwota))
            {
                Console.WriteLine("Niewystarczające środki na koncie.");
                return false;
            }

            // Aktualizacja stanu konta po wypłacie
            var wynik = menadzerBazyDanych.AktualizujStanKonta(numerKarty, kwota);
            if (wynik)
            {
                Console.WriteLine($"Pomyślnie wypłacono {kwota}.");
            }
            else
            {
                Console.WriteLine("Wystąpił problem podczas realizacji wypłaty.");
            }

            return wynik;
        }
    }
}