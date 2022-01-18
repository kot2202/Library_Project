using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Console.ForegroundColor = generalTextColor;

            const string menuLoopTextHead = "Wybierz opcję i zatwierdź przy użyciu <ENTER>\n";
            const string menuLoopTextBody = "1. Wyświetl stan książek\n" +
                                            "2. Wyświetl wypożyczenia\n" +
                                            "3. Wyświetl czytelników\n" +
                                            "4. Wypożycz książkę\n" +
                                            "5. Zwróć wypożyczoną książkę\n" +
                                            "6. Dodaj nową książkę\n" +
                                            "7. Usuń dodaną książkę\n" +
                                            "8. Dodaj czytelnika\n" +
                                            "9. Usuń czytelnika\n";

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
            const string imienazwiskoRegexFormat = "[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$";
            const string peselRegexFormat = "^\\d{11}$";

            //////////////////////////////////////////////
            #endregion

            Console.OutputEncoding = Encoding.UTF8; // Pozwala na polskie znaki

            //////////////////////////////////////////////

            ExtraFunctions extraFunctions = new ExtraFunctions();
            LibraryRepository libraryRepo = new LibraryRepository();
            ushort wybor;
            string komenda;

            while (true)
            {
                Console.ForegroundColor = generalTextColor;
                Console.WriteLine(menuLoopTextHead + menuLoopTextBody);

                ushort.TryParse(Console.ReadLine(), out wybor); // nie wysypuje sie, zwraca 0 po wpisaniu tekstu

                try
                {
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
                            Console.WriteLine(menuShowRentsHead);
                            List<data.WypozyczeniaView> listaWypozyczen = libraryRepo.GetRents();
                            if (listaWypozyczen.Count == 0)
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Brak wypożyczeń");
                            }
                            for (int i = 0; i < listaWypozyczen.Count; i++)
                            {
                                double roznicaDni = (listaWypozyczen[i].data_wypozyczenia_od - DateTime.Now).TotalDays;
                                roznicaDni = Math.Abs(roznicaDni);  //Math.Abs bo zwraca wartosci ujemne dla starych dat

                                if (roznicaDni > daysWithoutReturnWarningImportant) { Console.ForegroundColor = ConsoleColor.Red; }
                                else if (roznicaDni > daysWithoutReturnWarningMedium) { Console.ForegroundColor = ConsoleColor.Yellow; }
                                else { Console.ForegroundColor = ConsoleColor.Green; }
                                Console.WriteLine(listaWypozyczen[i].GetInfo());
                            }
                            break;
                        #endregion
                        case 3:
                            #region Wyswietl czytelnikow
                            Console.ForegroundColor = menuShowReadersColor;
                            List<data.Czytelnicy> listaCzytelnikow = libraryRepo.GetReaders();
                            if (listaCzytelnikow.Count == 0)
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Brak czytelników");
                            }
                            for (int i = 0; i < listaCzytelnikow.Count; i++)
                            {
                                Console.WriteLine($"{listaCzytelnikow[i].czytelnik_imie} {listaCzytelnikow[i].czytelnik_nazwisko} {listaCzytelnikow[i].czytelnik_adres} {listaCzytelnikow[i].czytelnik_pesel}");
                            }
                            break;
                        #endregion
                        case 4:
                            #region Wypozycz ksiazke
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
                                Console.WriteLine($"Czy chodzi o {ksiazki[nrKsiazki].GetInfo()}?\nT/N");
                                komenda = Console.ReadLine();
                                Console.ForegroundColor = generalTextColor;
                                if (extraFunctions.IsAnswerTrue(komenda))
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
                                                Console.WriteLine($"Czy chodzi o {czytelnicy[nrCzytelnika].GetInfo()}?\nT/N");
                                                komenda = Console.ReadLine();
                                                if (extraFunctions.IsAnswerTrue(komenda)) // dobry czytelnik, dobra ksiazka wczytujemy ich id
                                                {
                                                    libraryRepo.RentBook(ksiazki[nrKsiazki].id_ksiazki, czytelnicy[nrCzytelnika].id_czytelnika);
                                                    Console.ForegroundColor = informationTextColor;
                                                    Console.WriteLine("Wypożyczono książkę");
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
                        case 5:
                            #region Zwroc ksiazke
                            Console.ForegroundColor = questionTextColor;
                            string infoCzytelnik;
                            string infoKsiazka;
                            Console.WriteLine("Podaj imie / nazwisko czytelnika");
                            infoCzytelnik = Console.ReadLine();

                            Console.WriteLine("Podaj nazwę książki do zwrotu");
                            infoKsiazka = Console.ReadLine();
                            var listaWypozyczenDoZwrotu = libraryRepo.GetRentsForReturn(infoCzytelnik, infoKsiazka);

                            for (int i = 0; i < listaWypozyczenDoZwrotu.Count; i++)
                            {
                                Console.WriteLine($"Czy chodzi o {listaWypozyczenDoZwrotu[i].czytelnik_imie_nazwisko} {listaWypozyczenDoZwrotu[i].nazwa} {listaWypozyczenDoZwrotu[i].autor_nazwisko} Data wyp: {listaWypozyczenDoZwrotu[i].data_wypozyczenia_od}\nT/N");
                                komenda = Console.ReadLine();
                                if (extraFunctions.IsAnswerTrue(komenda))
                                {
                                    libraryRepo.ReturnBook(listaWypozyczenDoZwrotu[i].id_wypozyczenia);
                                    Console.ForegroundColor = informationTextColor;
                                    Console.WriteLine("Zwrócono książkę.");
                                    goto default; // używanie goto to ostatecznosc
                                }
                            }
                            Console.ForegroundColor = errorTextColor;
                            Console.WriteLine("Nie znaleziono takiego wypożyczenia");
                            break;
                        #endregion
                        case 6:
                            #region Dodaj nowa ksiazke
                            data.Ksiazki nowaKsiazka = new data.Ksiazki();
                            komenda = "n";
                            Console.ForegroundColor = menuAddBookColor;
                            Console.WriteLine("Podaj nazwę książki którą chcesz dodać:");
                            nowaKsiazka.nazwa = Console.ReadLine();
                            Console.WriteLine("imię autora:");
                            nowaKsiazka.autor_imie = Console.ReadLine();
                            Console.WriteLine("nazwisko autora:");
                            nowaKsiazka.autor_nazwisko = Console.ReadLine();
                            Console.WriteLine("ilość książek:");
                            int.TryParse(Console.ReadLine(), out int il);
                            if (il < 1)
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Ilość książek musi być większa niż 0");
                                break;
                            }
                            nowaKsiazka.ilosc_ksiazek = il;

                            Console.WriteLine("Zweryfikuj poprawność:");
                            Console.ForegroundColor = informationTextColor;
                            Console.WriteLine(nowaKsiazka.GetInfo(0, 1));
                            Console.ForegroundColor = menuAddBookColor;
                            Console.WriteLine("Czy wygląda poprawnie?\nT/N");
                            komenda = Console.ReadLine();
                            if (extraFunctions.IsAnswerTrue(komenda))
                            {
                                try
                                {
                                    libraryRepo.AddNewBook(nowaKsiazka);
                                }
                                catch
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Wystąpił błąd przy dodawaniu książki");
                                    break;
                                }
                                Console.ForegroundColor = informationTextColor;
                                Console.WriteLine("Pomyślnie dodano książkę");
                            }
                            break;
                        #endregion
                        case 7:
                            #region Usun ksiazke
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine("Podaj nazwę książki którą chcesz usunąć");
                            komenda = Console.ReadLine();
                            var ksiazkiDoUsuniecia = libraryRepo.GetBooksWithName(komenda);

                            if (ksiazkiDoUsuniecia.Count == 0)
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Nie znaleziono takiej książki");
                                break;
                            }
                            for (int nrKsiazki = 0; nrKsiazki < ksiazkiDoUsuniecia.Count; nrKsiazki++)
                            {
                                Console.ForegroundColor = questionTextColor;
                                Console.WriteLine($"Czy chodzi o {ksiazkiDoUsuniecia[nrKsiazki].GetInfo(1, 0)}?\nT/N");
                                komenda = Console.ReadLine();
                                if (extraFunctions.IsAnswerTrue(komenda))
                                {
                                    try
                                    {
                                        libraryRepo.TryToDeleteBook(ksiazkiDoUsuniecia[nrKsiazki].id_ksiazki);
                                    }
                                    catch (System.Data.Entity.Infrastructure.DbUpdateException)
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
                                    goto default; // uzywanie goto to ostatecznosc
                                }
                            }
                            Console.ForegroundColor = errorTextColor;
                            Console.WriteLine("Nie znaleziono więcej książek o tej nazwie");
                            break;
                        #endregion
                        case 8:
                            #region Dodaj czytelnika
                            data.Czytelnicy nowyCzytelnik = new data.Czytelnicy();
                            Regex peselRegex = new Regex(peselRegexFormat);
                            Regex imienazwiskoRegex = new Regex(imienazwiskoRegexFormat);

                            Console.ForegroundColor = menuAddReaderColor;
                            Console.WriteLine("Podaj imię czytelnika, którego chcesz dodać");
                            nowyCzytelnik.czytelnik_imie = Console.ReadLine();
                            Console.WriteLine("nazwisko:");
                            nowyCzytelnik.czytelnik_nazwisko = Console.ReadLine();

                            if (!imienazwiskoRegex.IsMatch(nowyCzytelnik.czytelnik_imie) || !imienazwiskoRegex.IsMatch(nowyCzytelnik.czytelnik_nazwisko))
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Imie i nazwisko muszą składać się z samych liter");
                                break;
                            }
                            Console.WriteLine("adres:");
                            nowyCzytelnik.czytelnik_adres = Console.ReadLine();
                            Console.WriteLine("pesel:");
                            nowyCzytelnik.czytelnik_pesel = Console.ReadLine();

                            if (!peselRegex.IsMatch(nowyCzytelnik.czytelnik_pesel))
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Zły pesel");
                                break;
                            }

                            Console.WriteLine("Zweryfikuj poprawność:");
                            Console.ForegroundColor = informationTextColor;
                            Console.WriteLine(nowyCzytelnik.GetInfo());
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine("Czy wygląda poprawnie?\nT/N");
                            komenda = Console.ReadLine();
                            if (extraFunctions.IsAnswerTrue(komenda))
                            {
                                try
                                {
                                    libraryRepo.AddNewReader(nowyCzytelnik);
                                }
                                catch
                                {
                                    Console.ForegroundColor = errorTextColor;
                                    Console.WriteLine("Wystąpił błąd przy dodawaniu czytelnika");
                                    break;
                                }
                                Console.ForegroundColor = informationTextColor;
                                Console.WriteLine("Pomyślnie dodano czytelnika");
                            }
                            break;
                        #endregion
                        case 9:
                            #region Usun czytelnika
                            Console.ForegroundColor = questionTextColor;
                            Console.WriteLine("Podaj imię / nazwisko czytelnika którego chcesz usunąć");
                            komenda = Console.ReadLine();
                            var czytelnicyDoUsuniecia = libraryRepo.GetReadersWithName(komenda);

                            if (czytelnicyDoUsuniecia.Count == 0)
                            {
                                Console.ForegroundColor = errorTextColor;
                                Console.WriteLine("Nie znaleziono takiego czytelnika");
                                break;
                            }
                            for (int nrCzytelnika = 0; nrCzytelnika < czytelnicyDoUsuniecia.Count; nrCzytelnika++)
                            {
                                Console.WriteLine($"Czy chodzi o {czytelnicyDoUsuniecia[nrCzytelnika].GetInfo()}?\nT/N");
                                komenda = Console.ReadLine();
                                if (extraFunctions.IsAnswerTrue(komenda))
                                {
                                    try
                                    {
                                        libraryRepo.TryToDeleteReader(czytelnicyDoUsuniecia[nrCzytelnika].id_czytelnika);
                                    }
                                    catch
                                    {
                                        Console.ForegroundColor = errorTextColor;
                                        Console.WriteLine("Błąd usuwania, ten czytelnik prawdopodobnie nie oddał wypożyczonej książki");
                                        goto default; // uzywanie goto to ostatecznosc
                                    }
                                    Console.ForegroundColor = informationTextColor;
                                    Console.WriteLine("Pomyślnie usunięto czytelnika");
                                    goto default; // uzywanie goto to ostatecznosc
                                }
                            }
                            Console.ForegroundColor = errorTextColor;
                            Console.WriteLine("Nie znaleziono więcej czytelników");
                            break;
                        #endregion
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor= errorTextColor;
                    Console.WriteLine("Błąd łączenia z bazą danych");
                }
            }
        }
    }
}
