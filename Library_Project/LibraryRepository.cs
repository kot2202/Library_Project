﻿using System;
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

        // moze zamiast tej metody robic petle bezposrednio w program
        public void ShowBooks()
        {
            var aktualnaListaKsiazek = GetBooks();
            for (int i = 0; i < aktualnaListaKsiazek.Count; i++)
            {
                Console.WriteLine($"{aktualnaListaKsiazek[i].GetInfo(1)}");
            }
        }

        public List<data.Czytelnicy>GetReadersWithName(string readerInfo)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Czytelnicy.Where(x => x.czytelnik_imie.Contains(readerInfo)
                                                    || x.czytelnik_nazwisko.Contains(readerInfo)).ToList();
            }
        }

        public List<data.Ksiazki> GetBooksWithName(string bookName) // TODO moze powinno usuwac znaki diakrytyczne dla prostszego wyszukiwania
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Ksiazki.Where(x => x.nazwa.Contains(bookName)).ToList();
            }
        }

        private void ReduceBook(int idKsiazki)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                data.Ksiazki ksiazkaDoZredukowania = libraryDb.Ksiazki.SingleOrDefault(x => x.id_ksiazki == idKsiazki);
                if (ksiazkaDoZredukowania != null)
                {
                    ksiazkaDoZredukowania.ilosc_ksiazek = ksiazkaDoZredukowania.ilosc_ksiazek - 1;
                    libraryDb.SaveChanges();
                }
            }
        }

        public void AddNewBook(data.Ksiazki ksiazkaDoDodania)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                libraryDb.Ksiazki.Add(ksiazkaDoDodania); // TODO dodac obsluge wyjatkow, uzytkownik mogl naklepac wiecej znakow niz pozwala kolumna
                libraryDb.SaveChanges();
            }
        }

        /// <summary>
        /// Redukuje ksiazki, dodaje rekord do wypozyczen
        /// </summary>
        public void RentBook(int idKsiazki, int idCzytelnika)
        {
#if DEBUG
            Console.WriteLine($"ksiazka:{idKsiazki} czytelnik:{idCzytelnika}");
#endif
            ReduceBook(idKsiazki);


            data.Wypozyczenia noweWypozyczenie = new data.Wypozyczenia
            {
                id_czytelnika = idCzytelnika,
                id_ksiazki = idKsiazki,
                aktywne = 1,
                data_wypozyczenia_od = DateTime.Now,
                data_ostatni_update = DateTime.Now
            };

            using (var libraryDb = new data.library_projectEntities())
            {
                libraryDb.Wypozyczenia.Add(noweWypozyczenie);
                libraryDb.SaveChanges();
            }
        }
    }
}
