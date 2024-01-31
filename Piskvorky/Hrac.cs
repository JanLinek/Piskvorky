using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class Hrac
    {
        public string Jmeno;
        public string Symbol;
        public List<Pozice> ZadanePozice;
        public Hrac(string jmeno, string symbol)
        {
            Jmeno = jmeno;
            ZadanePozice = new List<Pozice>();
            Symbol = symbol;
        }
    }
}
