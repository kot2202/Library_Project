﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Project
{
    internal class Program
    {
        static void Main(string[] args)
        {

            #region settings
            ////////////////// SETTINGS //////////////////

            Console.Title = "Library Project";

            /// General Colors ///
            ConsoleColor generalTextColor = ConsoleColor.White;
            ConsoleColor errorTextColor = ConsoleColor.Red;
            ConsoleColor questionTextColor = ConsoleColor.Yellow;
            ConsoleColor informationTextColor = ConsoleColor.Green;
            Console.ForegroundColor = generalTextColor; // TODO: https://stackoverflow.com/a/34866857

            
            const string menuLoopTextHead = "Wybierz opcję i zatwierdź przy użyciu <ENTER>\n";
            const string menuLoopTextBody = "1. Wyświetl stan książek\n" +
                                            "2. Sprawdź stan wypożyczeń\n" +
                                            "3. Wypożycz książkę\n" +
                                            "4. Zwróć wypożyczoną książkę\n" +
                                            "5. Dodaj nową książkę\n" +
                                            "6. Usuń dodaną książkę";

            /// Show Books Menu ///
            ConsoleColor menuShowBookTextColor = informationTextColor;
            const string menuShowBookHead = "Dostępne książki:";

            /// Rent Book Menu ///
            ConsoleColor menuBookRentColor = questionTextColor;
            const string menuBookRentBody = "Podaj część tytułu książki, jeśli jest dostępna, zostanie wypożyczona";

            int daysWithoutReturnWarningMedium = 14;
            int daysWithoutReturnWarningImportant = 30;

            /// Add Book Menu ///
            ConsoleColor menuAddBookColor = questionTextColor;

            //////////////////////////////////////////////
            #endregion

            Console.OutputEncoding = Encoding.UTF8; // Allows the usage of polish and other characters

            //////////////////////////////////////////////
            
            // TODO dodac wychodzenie z zapetlonego menu

            // TODO dodac publiczna klase z ustawieniami m.in. kolorków, żeby LibraryRepository moglo korzystac z kolorkow,
            // wtedy bedzie mniej syfu w Program

            LibraryRepository libraryRepo = new LibraryRepository();
            ushort wybor;
            string komenda;

            while (true)
            {
                Console.ForegroundColor = generalTextColor;
                Console.WriteLine(menuLoopTextHead + menuLoopTextBody);

                //wybor = Convert.ToUInt16(Console.ReadLine()); // wysypuje sie przy wpisaniu tekstu zamiast liczb
                ushort.TryParse(Console.ReadLine(), out wybor); // nie wysypuje sie, zwraca 0 po wpisaniu tekstu

                switch (wybor)
                {
                    case 1:
                    #region Stan ksiazek
                        Console.WriteLine(menuShowBookHead);
                        Console.ForegroundColor = menuShowBookTextColor;
                        libraryRepo.ShowBooks();
                        Console.ForegroundColor = generalTextColor;
                        break;
                    #endregion
                    case 2:
                    #region Stan wypozyczen
                        List<data.Wypozyczenia> listaWypozyczen = libraryRepo.GetRents(); // moze zwrocic pusta liste TODO obsluga wyjatkow
                        for(int i = 0; i < listaWypozyczen.Count; i++)
                        {
                            double roznicaDni = (listaWypozyczen[i].data_wypozyczenia_od - DateTime.Now).TotalDays;
                            roznicaDni = Math.Abs(roznicaDni);  //Math.Abs bo zwraca wartosci ujemne dla starych dat

                            if (roznicaDni > daysWithoutReturnWarningImportant) { Console.ForegroundColor = ConsoleColor.Red; }
                            else if (roznicaDni > daysWithoutReturnWarningMedium) { Console.ForegroundColor = ConsoleColor.Yellow; }
                            else { Console.ForegroundColor = ConsoleColor.Green; }
                            Console.WriteLine($"{listaWypozyczen[i].id_czytelnika} {listaWypozyczen[i].id_ksiazki} {listaWypozyczen[i].data_wypozyczenia_od}");
                        }
                        break;
                    #endregion
                    case 3:
                    #region Wypozycz ksiazke
                        // TODO jakos zamknac to w metodzie
                        Console.ForegroundColor = menuBookRentColor;
                        Console.WriteLine(menuBookRentBody);
                        komenda = Console.ReadLine();
                        var ksiazki = libraryRepo.GetBooksWithName(komenda);

                        if (ksiazki.Count == 0)
                        {
                            Console.ForegroundColor = errorTextColor;
                            Console.WriteLine("Nie znaleziono książek o takiej nazwie");
                            Console.ForegroundColor = generalTextColor;
                            break;
                        }
                        for (int nrKsiazki = 0; nrKsiazki < ksiazki.Count; nrKsiazki++)
                        {
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine($"Czy chodzi o {ksiazki[nrKsiazki].GetInfo()}?\nT/N"); // TODO dodac slownik odpowiedzi (t,tak,y,yes)
                            komenda = Console.ReadLine();
                            Console.ForegroundColor = generalTextColor; // TODO sprawdzic czy da sie zbugowac kolor
                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase)) // pozwala na wpisywanie duzych i malych liter
                            {
                                if (ksiazki[nrKsiazki].ilosc_ksiazek > 0)
                                {
                                    Console.ForegroundColor = questionTextColor;
                                    Console.WriteLine("Podaj imie / nazwisko czytelnika");

                                    komenda = Console.ReadLine();
                                    var czytelnicy = libraryRepo.GetReadersWithName(komenda);
                                    if (czytelnicy.Count == 0)
                                    {
                                        Console.ForegroundColor = errorTextColor;
                                        Console.WriteLine("Nie znaleziono czytelnika");
                                        Console.ForegroundColor = generalTextColor;
                                        goto default; // używanie goto to ostatecznosc
                                    }
                                    else
                                    {
                                        for (int nrCzytelnika = 0; nrCzytelnika < czytelnicy.Count; nrCzytelnika++)
                                        {
                                            Console.ForegroundColor = questionTextColor;
                                            Console.WriteLine($"Czy chodzi o {czytelnicy[nrCzytelnika].GetInfo()}?\nT/N"); // TODO dodac slownik odpowiedzi (t,tak,y,yes)
                                            komenda = Console.ReadLine();
                                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase)) // dobry czytelnik, dobra ksiazka wczytujemy ich id
                                            {
                                                libraryRepo.RentBook(ksiazki[nrKsiazki].id_ksiazki, czytelnicy[nrCzytelnika].id_czytelnika);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Niestety na stanie jest 0 tych książek");
                                }
                                goto default; // używanie goto to ostatecznosc
                            }
                        }
                        Console.ForegroundColor = errorTextColor;
                        Console.WriteLine("Nie ma na stanie więcej książek o tej nazwie");
                        break;
                    #endregion
                    case 4:
                        throw new NotImplementedException();
                    case 5:
                    #region Dodaj nowa ksiazke
                        data.Ksiazki nowaKsiazka = new data.Ksiazki();
                        komenda = "n";
                        while (komenda != "t")
                        {
                            Console.ForegroundColor = menuAddBookColor;
                            Console.WriteLine("Podaj nazwę książki którą chcesz dodać:");
                            nowaKsiazka.nazwa = Console.ReadLine();
                            Console.WriteLine("imię autora:");
                            nowaKsiazka.autor_imie = Console.ReadLine();
                            Console.WriteLine("nazwisko autora:");
                            nowaKsiazka.autor_nazwisko = Console.ReadLine();
                            Console.WriteLine("ilość książek:");
                            int.TryParse(Console.ReadLine(), out int il);
                            nowaKsiazka.ilosc_ksiazek = il;

                            Console.WriteLine("Zweryfikuj poprawność:");
                            Console.ForegroundColor = informationTextColor;
                            Console.WriteLine(nowaKsiazka.GetInfo(0, 1));
                            Console.ForegroundColor = menuAddBookColor;
                            Console.WriteLine("Czy wygląda poprawnie?\nT/N");
                            komenda = Console.ReadLine();
                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase))
                            {
                                libraryRepo.AddNewBook(nowaKsiazka);
                            }
                        }
                        break;
                    #endregion
                    case 6:
                        throw new NotImplementedException();
                    default:
                        break;
                }
            }
        }
    }
}
