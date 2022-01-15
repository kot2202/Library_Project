//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library_Project.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Czytelnicy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Czytelnicy()
        {
            this.Wypozyczenia = new HashSet<Wypozyczenia>();
        }
    
        public int id_czytelnika { get; set; }
        public string czytelnik_imie { get; set; }
        public string czytelnik_nazwisko { get; set; }
        public string czytelnik_adres { get; set; }
        public string czytelnik_pesel { get; set; }

        /// <summary>
        /// Zwraca pelne info o czytelniku
        /// </summary>
        public string GetInfo()
        {
            string info = string.Empty;
            info = $"{czytelnik_imie} {czytelnik_nazwisko} {czytelnik_adres} {czytelnik_pesel}";
            return info;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Wypozyczenia> Wypozyczenia { get; set; }
    }
}
