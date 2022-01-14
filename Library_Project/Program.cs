using System;
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

            /// General Colors ///
            ConsoleColor generalTextColor = ConsoleColor.White;
            ConsoleColor errorTextColor = ConsoleColor.Red;
            ConsoleColor questionTextColor = ConsoleColor.Yellow;
            ConsoleColor informationTextColor = ConsoleColor.Green;
            Console.ForegroundColor = generalTextColor; // TODO: https://stackoverflow.com/a/34866857

            Console.Title = "Library Project";
            const string menuLoopTextHead = "Wybierz opcję i zatwierdź przy użyciu <ENTER>\n";
            const string menuLoopTextBody = "1. Wyświetl stan książek\n" +
                                            "2. Wypożycz książkę\n" +
                                            "3. Dodaj książkę\n" +
                                            "4. Usuń książkę";

            /// Show Books Menu ///
            ConsoleColor menuShowBookTextColor = informationTextColor;
            const string menuShowBookHead = "Dostępne książki:";

            /// Rent Book Menu ///
            ConsoleColor menuBookRentColor = questionTextColor;
            const string menuBookRentBody = "Podaj część tytułu książki, jeśli jest dostępna, zostanie Ci wypożyczona";

            /// Add Book Menu ///
            ConsoleColor menuAddBookColor = questionTextColor;

            //////////////////////////////////////////////
            #endregion

            Console.OutputEncoding = Encoding.UTF8; // Allows the usage of polish and other characters

            //////////////////////////////////////////////
            
            // TODO dodac wychodzenie z zapetlonego menu

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
                        Console.WriteLine(menuShowBookHead);
                        Console.ForegroundColor = menuShowBookTextColor;
                        libraryRepo.ShowBooks();
                        Console.ForegroundColor = generalTextColor;

                        break;
                    case 2:
                    #region opcja2
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
                                    Console.WriteLine("Podaj imie / nazwisko czytelnika"); // TODO dodac nr tel

                                    komenda = Console.ReadLine();
                                    var czytelnicy = libraryRepo.GetReadersWithName(komenda);
                                    if(czytelnicy.Count == 0)
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
                    case 3:
                        data.Ksiazki nowaKsiazka = new data.Ksiazki();
                        komenda = "n";
                        while(komenda != "t")
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
                            if(String.Equals(komenda, "t", StringComparison.OrdinalIgnoreCase))
                            {
                                libraryRepo.AddNewBook(nowaKsiazka);
                            }
                        }
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
