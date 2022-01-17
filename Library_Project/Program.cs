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

                                            // TODO do zmiany kolejnosc
            const string menuLoopTextHead = "Wybierz opcję i zatwierdź przy użyciu <ENTER>\n";
            const string menuLoopTextBody = "1. Wyświetl stan książek\n" +
                                            "2. Wyświetl wypożyczenia\n" +
                                            "3. Wypożycz książkę\n" +
                                            "4. Zwróć wypożyczoną książkę\n" +
                                            "5. Dodaj nową książkę\n" +
                                            "6. Usuń dodaną książkę\n" +
                                            "7. Wyświetl listę czytelników\n" +
                                            "8. Dodaj czytelnika\n" +
                                            "9. Usuń czytelnika";

            /// Show Books Menu ///
            ConsoleColor menuShowBookTextColor = informationTextColor;
            const string menuShowBookHead = "Dostępne książki:";

            /// Show Rents Menu ///
            const string menuShowRentsHead = "Obecne wypożyczenia:";

            /// Rent Book Menu ///
            ConsoleColor menuBookRentColor = questionTextColor;
            const string menuBookRentBody = "Podaj część tytułu książki, jeśli jest dostępna, zostanie wypożyczona";

            int daysWithoutReturnWarningMedium = 14;
            int daysWithoutReturnWarningImportant = 30;

            /// Add Book Menu ///
            ConsoleColor menuAddBookColor = questionTextColor;

            /// Show Readers Menu ///
            ConsoleColor menuShowReadersColor = ConsoleColor.Blue;

            /// Add Reader Menu ///
            ConsoleColor menuAddReaderColor = ConsoleColor.Blue;

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
                        // TODO zrobic update wersje z nowym view
                        Console.WriteLine(menuShowRentsHead);
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
                    #region Zwroc ksiazke
                        Console.ForegroundColor = questionTextColor;
                        string infoCzytelnik;
                        string infoKsiazka;
                        Console.WriteLine("Podaj imie / nazwisko czytelnika"); // TODO przypisac te 2 teksty do zmiennych w settings
                        infoCzytelnik = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę książki do zwrotu");
                        infoKsiazka = Console.ReadLine();
                        var listaWypozyczenDoZwrotu = libraryRepo.GetRentsForReturn(infoCzytelnik, infoKsiazka);

                        for (int i = 0; i < listaWypozyczenDoZwrotu.Count; i++)
                        {
                            Console.WriteLine($"Czy chodzi o {listaWypozyczenDoZwrotu[i].czytelnik_imie_nazwisko} {listaWypozyczenDoZwrotu[i].nazwa} {listaWypozyczenDoZwrotu[i].autor_nazwisko} Data wyp: {listaWypozyczenDoZwrotu[i].data_wypozyczenia_od} T/N");
                            komenda = Console.ReadLine();
                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase)) // pozwala na wpisywanie duzych i malych liter
                            {
                                libraryRepo.ReturnBook(listaWypozyczenDoZwrotu[i].id_wypozyczenia);
                                Console.ForegroundColor = informationTextColor;
                                Console.WriteLine("Zwrócono książkę.");
                                goto default; // goto to ostatecznosc
                            }
                        }
                        Console.ForegroundColor = errorTextColor;
                        Console.WriteLine("Nie znaleziono takiego wypożyczenia");
                        break;
                    #endregion
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
                                try
                                {
                                    libraryRepo.AddNewBook(nowaKsiazka);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Wystąpił błąd przy dodawaniu książki");
                                    break;
                                }
                                finally
                                {
                                    Console.ForegroundColor = informationTextColor;
                                    Console.WriteLine("Pomyślnie dodano książkę");
                                }
                                
                            }
                        }
                        break;
                    #endregion
                    case 6:
                    #region Usun ksiazke
                        Console.ForegroundColor = questionTextColor;
                        Console.WriteLine("Podaj nazwę książki którą chcesz usunąć");
                        komenda = Console.ReadLine();
                        var ksiazkiDoUsuniecia = libraryRepo.GetBooksWithName(komenda);

                        for (int nrKsiazki = 0; nrKsiazki < ksiazkiDoUsuniecia.Count; nrKsiazki++)
                        {
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine($"Czy chodzi o {ksiazkiDoUsuniecia[nrKsiazki].GetInfo(1,0)}?\nT/N"); // TODO dodac slownik odpowiedzi (t,tak,y,yes)
                            komenda = Console.ReadLine();
                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase) ) // pozwala na wpisywanie duzych i malych liter
                            {
                                // TODO zrobic wiecej testow do tego
                                try
                                {
                                    libraryRepo.TryToDeleteBook(ksiazkiDoUsuniecia[nrKsiazki].id_ksiazki);
                                }
                                catch(System.Data.Entity.Infrastructure.DbUpdateException) // problem przy updatowaniu bazy
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Błąd usuwania, prawdopodobnie ta książka jest obecnie przez kogoś wypożyczona");
                                    goto default; // uzywanie goto to ostatecznosc
                                }
                                finally
                                {
                                    Console.ForegroundColor = informationTextColor;
                                    Console.WriteLine("Pomyślnie usunięto książkę");
                                }
                            }
                        }
                        Console.ForegroundColor = errorTextColor;
                        Console.WriteLine("Nie znaleziono takiego wypożyczenia");
                        break;
                    #endregion
                    case 7:
                    #region Wyswietl czytelnikow
                        Console.ForegroundColor = menuShowReadersColor;
                        List<data.Czytelnicy> listaCzytelnikow = libraryRepo.GetReaders();
                        for(int i = 0;i < listaCzytelnikow.Count;i++)
                        {
                            Console.WriteLine($"{listaCzytelnikow[i].czytelnik_imie} {listaCzytelnikow[i].czytelnik_nazwisko} {listaCzytelnikow[i].czytelnik_adres} {listaCzytelnikow[i].czytelnik_pesel}");
                        }
                        break;
                    #endregion
                    case 8:
                    #region Dodaj czytelnika
                        data.Czytelnicy nowyCzytelnik = new data.Czytelnicy();
                        komenda = "n";
                        while (komenda != "t")
                        {
                            Console.ForegroundColor = menuAddReaderColor;
                            Console.WriteLine("Podaj imię czytelnika, którego chcesz dodać");
                            nowyCzytelnik.czytelnik_imie = Console.ReadLine();
                            Console.WriteLine("nazwisko:");
                            nowyCzytelnik.czytelnik_nazwisko = Console.ReadLine();
                            Console.WriteLine("adres:");
                            nowyCzytelnik.czytelnik_adres = Console.ReadLine();
                            Console.WriteLine("pesel:");
                            nowyCzytelnik.czytelnik_pesel = Console.ReadLine();

                            Console.WriteLine("Zweryfikuj poprawność:");
                            Console.ForegroundColor = informationTextColor;
                            Console.WriteLine(nowyCzytelnik.GetInfo());
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine("Czy wygląda poprawnie?\nT/N");
                            komenda = Console.ReadLine();
                            if (String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    libraryRepo.AddNewReader(nowyCzytelnik);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Wystąpił błąd przy dodawaniu czytelnika");
                                    break;
                                }
                                finally
                                {
                                    Console.ForegroundColor = informationTextColor;
                                    Console.WriteLine("Pomyślnie dodano czytelnika");
                                }
                            }
                        }
                        break;
                    #endregion
                    case 9:
                        throw new NotImplementedException();
                    default:
                        break;
                }
            }
        }
    }
}
