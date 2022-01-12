using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Project
{
    internal class LibraryRepository
    {
        private List<data.Ksiazki> GetBooks()
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Ksiazki.ToList(); // moze zwrocic pusta liste TODO obsluga wyjatkow
            }
        }

        public void ShowBooks()
        {
            var aktualnaListaKsiazek = GetBooks();
            for (int i = 0; i < aktualnaListaKsiazek.Count; i++)
            {
                Console.WriteLine($"{aktualnaListaKsiazek[i].nazwa} {aktualnaListaKsiazek[i].autor_imie} {aktualnaListaKsiazek[i].autor_nazwisko} Ilość:{aktualnaListaKsiazek[i].ilosc_ksiazek}");
            }
        }

        public List<data.Ksiazki> GetBooksWithName(string bookName) // TODO moze powinno usuwac znaki diakrytyczne dla prostszego wyszukiwania
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Ksiazki.Where(x => x.nazwa.Contains(bookName)).ToList(); // wiele ksiazek moze miec podobne nazwy TODO iteracja po nich
            }
        }

        public void ReduceBook(int idKsiazki)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                data.Ksiazki ksiazkaDoZredukowania = libraryDb.Ksiazki.SingleOrDefault(x => x.id_ksiazki == idKsiazki);
                if (ksiazkaDoZredukowania != null)
                {
                    ksiazkaDoZredukowania.ilosc_ksiazek = ksiazkaDoZredukowania.ilosc_ksiazek - 1;
                    libraryDb.SaveChanges();
                    Console.WriteLine("TODO: DODAJ REKORD DO TABELI WYPOZYCZENIA"); // TODO
                }
            }
        }
    }
}
