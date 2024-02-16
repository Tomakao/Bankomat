using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Bankomat.BazaDanych
{
    public class MenadzerBazyDanych
    {
        private const string plikBazyDanych = "bazaDanych.json";
        private List<Uzytkownik> uzytkownicy;

        public MenadzerBazyDanych()
        {
            uzytkownicy = new List<Uzytkownik>(); // Inicjalizujemy listę użytkowników
            WczytajDaneZPliku();
        }

        private void ZapiszDaneDoPliku()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(uzytkownicy, options);
            File.WriteAllText(plikBazyDanych, json);
        }

        private void WczytajDaneZPliku()
        {
            if (File.Exists(plikBazyDanych))
            {
                string json = File.ReadAllText(plikBazyDanych);
                uzytkownicy = JsonSerializer.Deserialize<List<Uzytkownik>>(json);
            }
        }

        public bool WeryfikujPin(string numerKarty, string pin)
        {
            var hashPin = HashujPin(pin);
            Console.WriteLine($"Hashpin wprowadzony: {hashPin}");

            var uzytkownik = uzytkownicy.FirstOrDefault(u => u.NumerKarty == numerKarty);
            if (uzytkownik != null)
            {
                Console.WriteLine($"Hashpin w bazie: {uzytkownik.HashPin}");

                return uzytkownik.HashPin == hashPin;
            }
    
            // Jeśli nie znaleziono użytkownika z podanym numerem karty, zwróć false.
            return false;
        }


        public decimal PobierzStanKonta(string numerKarty)
        {
            return uzytkownicy.Where(u => u.NumerKarty == numerKarty).Select(u => u.StanKonta).FirstOrDefault();
        }

        public bool AktualizujStanKonta(string numerKarty, decimal kwota)
        {
            var uzytkownik = uzytkownicy.FirstOrDefault(u => u.NumerKarty == numerKarty);
            if (uzytkownik != null && uzytkownik.StanKonta >= kwota)
            {
                uzytkownik.StanKonta -= kwota;
                ZapiszDaneDoPliku();
                return true; // Transakcja zakończona sukcesem
            }
            return false; // Brak wystarczających środków
        }
        
        private string HashujPin(string pin)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
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
        
        public void DodajUzytkownika(string numerKarty, string pin, decimal stanKonta, string typKarty)
        {
            // Sprawdzenie, czy użytkownik z danym numerem karty już istnieje
            var istnieje = uzytkownicy.Any(u => u.NumerKarty == numerKarty);
            if (!istnieje)
            {
                uzytkownicy.Add(new Uzytkownik
                {
                    NumerKarty = numerKarty,
                    HashPin = HashujPin(pin),
                    StanKonta = stanKonta,
                    TypKarty = RozpoznajTypKarty(numerKarty)
                });
                ZapiszDaneDoPliku();
            }
        }

        public bool EdytujUzytkownika(string numerKarty, string pin, decimal? stanKonta, string typKarty)
        {
            var uzytkownik = uzytkownicy.FirstOrDefault(u => u.NumerKarty == numerKarty);
            if (uzytkownik != null)
            {
                uzytkownik.HashPin = HashujPin(pin);
                if (stanKonta.HasValue)
                {
                    uzytkownik.StanKonta = stanKonta.Value;
                }
                uzytkownik.TypKarty = typKarty;
                ZapiszDaneDoPliku();
                return true;
            }
            return false;
        }

        public bool UsunUzytkownika(string numerKarty)
        {
            var uzytkownik = uzytkownicy.FirstOrDefault(u => u.NumerKarty == numerKarty);
            if (uzytkownik != null)
            {
                uzytkownicy.Remove(uzytkownik);
                ZapiszDaneDoPliku();
                return true;
            }
            return false;
        }
        
        public void WyswietlWszystkichUzytkownikow()
        {
            foreach (var uzytkownik in uzytkownicy)
            {
                Console.WriteLine($"Karta: {uzytkownik.NumerKarty}, Hash PIN: {uzytkownik.HashPin}, Stan konta: {uzytkownik.StanKonta}, Typ karty: {uzytkownik.TypKarty}");
            }
        }
        
        public bool ZmienPin(string numerKarty, string nowyPin)
        {
            var uzytkownik = uzytkownicy.FirstOrDefault(u => u.NumerKarty == numerKarty);
            if (uzytkownik != null)
            {
                uzytkownik.HashPin = HashujPin(nowyPin);
                ZapiszDaneDoPliku();
                return true;
            }
            return false;
        }

    }

    // Klasa pomocnicza do symulacji rekordu użytkownika w bazie danych
    public class Uzytkownik
    {
        public string NumerKarty { get; set; }
        public string HashPin { get; set; }
        public decimal StanKonta { get; set; }
        public string TypKarty { get; set; }
    }
}