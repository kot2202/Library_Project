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
                return libraryDb.Ksiazki.ToList();
            }
        }

        // mozna zamiast tej metody robic petle bezposrednio w program
        public void ShowBooks()
        {
            var aktualnaListaKsiazek = GetBooks();
            if( aktualnaListaKsiazek.Count == 0)
            {
                Console.WriteLine("Brak książek");
                return;
            }
            for (int i = 0; i < aktualnaListaKsiazek.Count; i++)
            {
                Console.WriteLine($"{aktualnaListaKsiazek[i].GetInfo(1)}");
            }
        }

        public List<data.WypozyczeniaView> GetRents()
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                List<data.WypozyczeniaView> wypozyczeniaView = libraryDb.WypozyczeniaView.Where(x => x.aktywne == 1).ToList();
                List<data.Wypozyczenia> wypozyczenia = libraryDb.Wypozyczenia.Where(x => x.aktywne == 1).ToList(); // widok jest read only
                wypozyczenia.ForEach(x => x.data_ostatni_update = DateTime.Now);
                libraryDb.SaveChanges();
                return wypozyczeniaView;

            }

        }

        public List<data.CzytelnicyView>GetReadersWithName(string readerInfo)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.CzytelnicyView.Where(x => x.czytelnik_imie_nazwisko.Contains(readerInfo)).ToList();
            }
        }

        public List<data.Ksiazki> GetBooksWithName(string bookName)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Ksiazki.Where(x => x.nazwa.Contains(bookName)).ToList();
            }
        }

        /// <summary>
        /// Zmienia stan książki
        /// </summary>
        /// <param name="idKsiazki"></param>
        /// <param name="amount">Przyjmuje wartości pozytywne dla dodawania i negatywne dla odejmowania</param>
        private void ModifyBookCount(int idKsiazki, int amount)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                data.Ksiazki ksiazkaDoZredukowania = libraryDb.Ksiazki.SingleOrDefault(x => x.id_ksiazki == idKsiazki);
                if (ksiazkaDoZredukowania != null)
                {
                    ksiazkaDoZredukowania.ilosc_ksiazek = ksiazkaDoZredukowania.ilosc_ksiazek + amount;
                    libraryDb.SaveChanges();
                }
            }
        }

        public void AddNewBook(data.Ksiazki ksiazkaDoDodania)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                libraryDb.Ksiazki.Add(ksiazkaDoDodania);
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
            ModifyBookCount(idKsiazki, -1);


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

        public List<data.WypozyczeniaView> GetRentsForReturn(string infoCzytelnik, string infoKsiazka)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                List<data.WypozyczeniaView> listaWypozyczen = libraryDb.WypozyczeniaView.Where(x => x.czytelnik_imie_nazwisko.Contains(infoCzytelnik) && x.nazwa.Contains(infoKsiazka) && x.aktywne == 1).ToList();
                return listaWypozyczen;
            }
        }

        public void ReturnBook(int idWypozyczeniaDoZwrotu)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                data.Wypozyczenia wypozyczenieDoSkonczenia = libraryDb.Wypozyczenia.SingleOrDefault(x => x.id_wypozyczenia == idWypozyczeniaDoZwrotu);
                if (wypozyczenieDoSkonczenia != null)
                {
                    wypozyczenieDoSkonczenia.aktywne = 0;
                    wypozyczenieDoSkonczenia.data_ostatni_update = DateTime.Now;

                    ModifyBookCount(wypozyczenieDoSkonczenia.id_ksiazki, 1);

                    libraryDb.SaveChanges();
                }
            }
        }

        public void TryToDeleteBook(int idKsiazkiDoUsuniecia)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                data.Ksiazki ksiazkaDoUsuniecia = libraryDb.Ksiazki.SingleOrDefault(x => x.id_ksiazki == idKsiazkiDoUsuniecia);
                libraryDb.Ksiazki.Remove(libraryDb.Ksiazki.Find(idKsiazkiDoUsuniecia));
                libraryDb.SaveChanges();
            }
        }

        public List<data.Czytelnicy> GetReaders()
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                return libraryDb.Czytelnicy.ToList();
            }
        }

        public void AddNewReader(data.Czytelnicy nowyCzytelnik)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                libraryDb.Czytelnicy.Add(nowyCzytelnik);
                libraryDb.SaveChanges();
            }
        }

        internal void TryToDeleteReader(int id_czytelnika)
        {
            using (var libraryDb = new data.library_projectEntities())
            {
                libraryDb.Czytelnicy.Remove(libraryDb.Czytelnicy.Find(id_czytelnika));
                libraryDb.SaveChanges();
            }
        }
    }
}
