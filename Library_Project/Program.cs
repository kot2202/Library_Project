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
            ////////////////// SETTINGS //////////////////

            Console.ForegroundColor = ConsoleColor.Red; // TODO: https://stackoverflow.com/a/34866857

            Console.Title = "Library Project";
            const string menuLoopTextHead = "Wybierz opcję i zatwierdź przy użyciu <ENTER>\n";
            const string menuLoopTextBody = "1. Wyświetl stan książek\n2. Wypożycz książkę\n3. Dodaj książkę\n4. Usuń książkę";

            //////////////////////////////////////////////

            Console.OutputEncoding = Encoding.UTF8; // Allows the usage of polish and other characters

            //////////////////////////////////////////////

            ushort wybor;

            while(true)
            {
                Console.WriteLine(menuLoopTextHead + menuLoopTextBody);

                //wybor = Convert.ToUInt16(Console.ReadLine()); // wysypuje sie przy wpisaniu tekstu zamiast liczb
                ushort.TryParse(Console.ReadLine(), out wybor); // nie wysypuje sie, zwraca 0 po wpisaniu tekstu

                switch (wybor)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
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
