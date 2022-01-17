using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Project
{
    internal class ExtraFunctions
    {
        private readonly string[] yesAnswers = { "t", "tak", "y", "yes" };
        public bool IsAnswerTrue(string answer)
        {
            if(yesAnswers.Contains(answer, StringComparer.OrdinalIgnoreCase)) { return true; } // pozwala na wpisywanie duzych i malych liter z listy
            else { return false; }
        }
    }
}
