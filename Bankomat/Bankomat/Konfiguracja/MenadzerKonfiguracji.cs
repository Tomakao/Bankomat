using System.Text.Json;
using Bankomat.BazaDanych;
using System;

namespace Bankomat.Konfiguracja
{
    public class MenadzerKonfiguracji
    {
        private const string plikKonfiguracyjny = "konfiguracja.json";
        
        private MenadzerBazyDanych menadzerBazyDanych;

        public MenadzerKonfiguracji(MenadzerBazyDanych menadzerBazyDanych)
        {
            this.menadzerBazyDanych = menadzerBazyDanych;
        }

        public void ZapiszKonfiguracje(List<string> akceptowaneTypyKart)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(akceptowaneTypyKart, options);
            File.WriteAllText(plikKonfiguracyjny, json);
        }

        public List<string> WczytajKonfiguracje()
        {
            if (File.Exists(plikKonfiguracyjny))
            {
                string json = File.ReadAllText(plikKonfiguracyjny);
                return JsonSerializer.Deserialize<List<string>>(json);
            }
            else
            {
                return ZapytajOUstawieniaKart();
            }
        }

        private List<string> ZapytajOUstawieniaKart()
        {
            Console.WriteLine(
                "Wprowadź typy kart (np. \"1,2,3\" dla AmericanExpress-1, MasterCard-2, Visa-3, VisaElectron-4):");
            var input = Console.ReadLine();
            var typyKart = input.Split(',').Select(x => x.Trim()).ToList();
            ZapiszKonfiguracje(typyKart);
            return typyKart;
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
        
        public void TrybSerwisowy()
        {
            Console.WriteLine("Wprowadź hasło do trybu serwisowego:");
            var haslo = Console.ReadLine();
            if (haslo == "tajneHaslo")
            {
                bool dzialaj = true;
                while (dzialaj)
                {
                    Console.WriteLine("\n1. Dodaj użytkownika\n2. Edytuj użytkownika\n3. Usuń użytkownika\n4. Konfiguruj akceptowane typy kart\n5. Wyświetl wszystkich użytkowników\n6. Wyjście");
                    var wybor = Console.ReadLine();
                    switch (wybor)
                    {
                        case "1":
                            Console.WriteLine("Wprowadź numer karty:");
                            var numerKarty = Console.ReadLine();
                            var typKarty = RozpoznajTypKarty(numerKarty); // Automatyczne rozpoznawanie typu karty
                            Console.WriteLine("Wprowadź PIN:");
                            var pin = Console.ReadLine();
                            Console.WriteLine("Wprowadź stan konta:");
                            var stanKonta = decimal.Parse(Console.ReadLine());

                            menadzerBazyDanych.DodajUzytkownika(numerKarty, pin, stanKonta, typKarty);
                            Console.WriteLine("Użytkownik dodany.");
                            break;
                        case "2":
                            Console.WriteLine("Wprowadź numer karty do edycji:");
                            numerKarty = Console.ReadLine();
                            typKarty = RozpoznajTypKarty(numerKarty); // Ponowne rozpoznanie typu karty
                            Console.WriteLine("Wprowadź nowy PIN:");
                            pin = Console.ReadLine();
                            Console.WriteLine("Wprowadź nowy stan konta (pozostaw puste, jeśli bez zmian):");
                            var inputStanKonta = Console.ReadLine();
                            decimal? stanKontaNullable = string.IsNullOrEmpty(inputStanKonta) ? (decimal?)null : decimal.Parse(inputStanKonta);

                            if (stanKontaNullable.HasValue)
                            {
                                if (menadzerBazyDanych.EdytujUzytkownika(numerKarty, pin, stanKontaNullable.Value, typKarty))
                                {
                                    Console.WriteLine("Użytkownik zaktualizowany.");
                                }
                                else
                                {
                                    Console.WriteLine("Nie znaleziono użytkownika.");
                                }
                            }
                            else
                            {
                                // Aktualizacja użytkownika bez zmiany stanu konta
                                if (menadzerBazyDanych.EdytujUzytkownika(numerKarty, pin, null, typKarty))
                                {
                                    Console.WriteLine("Użytkownik zaktualizowany (bez zmiany stanu konta).");
                                }
                                else
                                {
                                    Console.WriteLine("Nie znaleziono użytkownika.");
                                }
                            }
                            break;
                        case "3":
                            Console.WriteLine("Wprowadź numer karty do usunięcia:");
                            numerKarty = Console.ReadLine();

                            if (menadzerBazyDanych.UsunUzytkownika(numerKarty))
                            {
                                Console.WriteLine("Użytkownik usunięty.");
                            }
                            else
                            {
                                Console.WriteLine("Nie znaleziono użytkownika.");
                            }
                            break;
                        case "4":
                            Console.WriteLine("Aktualne typy kart: " + String.Join(", ", WczytajKonfiguracje()));
                            Console.WriteLine("Wprowadź nowe typy kart oddzielone przecinkiem (np. Visa,MasterCard):");
                            var noweTypyKart = Console.ReadLine().Split(',').Select(t => t.Trim()).ToList();
                            ZapiszKonfiguracje(noweTypyKart);
                            Console.WriteLine("Konfiguracja zaktualizowana.");
                            break;
                        case "5":
                            menadzerBazyDanych.WyswietlWszystkichUzytkownikow();
                            break;
                        case "6":
                            dzialaj = false;
                            break;
                        default:
                            Console.WriteLine("Nieznane polecenie.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowe hasło.");
            }
        }
    }
}