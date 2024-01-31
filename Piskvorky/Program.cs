using Piskvorky;
using System.Security.Cryptography;
using System.Xml.Serialization;


string SymbolyHracu = "OX+#&";
bool PokracovatVeHre = true;
string PoziceVstup = "";
int StranaHrPlochyDefault = 20;
int StranaHrPlochy;
int PocetHracu = 2;
List<Hrac> Hraci;

//zacatek hry
while (PokracovatVeHre)
{
    Hraci = new List<Hrac>();
    bool JeKonec = false;
    Console.Clear();
    StranaHrPlochy = StranaHrPlochyDefault;
    string Vyherce = "";
    Console.WriteLine("Vítejte ve hře PIŠKVORKY!\n-------------------------");
    ZadaniPredStartem();

    // start
    Console.WriteLine("Tak začínáme, klávesu...");
    Console.ReadKey();
    Console.Clear();
    VykresliHraciPlochu();
    while (!JeKonec)
    {
        foreach (Hrac hrac in Hraci)
        {
            Console.SetCursorPosition(0, 6 + StranaHrPlochy);
            Console.Write("Na tahu je {0} (symbol {1})                            \nZadej pozici: ",
                hrac.Jmeno, hrac.Symbol);
            string Vstup = ZadaniPozice();
            int VstupY = (int)Vstup[0] - 65;
            int VstupX = Math.Abs(int.Parse(Vstup.Remove(0, 1).ToString()));
            hrac.ZadanePozice.Add(new Pozice(VstupX, VstupY));
            VykresliPozici(VstupX, VstupY, " " + hrac.Symbol);
            JeKonec = TestVyhry(hrac, VstupX, VstupY);
            Vyherce = hrac.Jmeno;
            if (JeKonec) break;
        }
    }
    Console.SetCursorPosition(0, StranaHrPlochy + 6);
    Console.WriteLine("------------------------------------------------");
    Console.WriteLine("Konec hry! Vyhrál hráč " + Vyherce);
    Console.WriteLine("------------------------------------------------");
    Console.Write("Chcete hrát znovu (A/N)?");
    string HratOdpoved = Console.ReadKey(true).KeyChar.ToString().ToUpper();
    PokracovatVeHre = (HratOdpoved == "A");
}
bool TestVyhry(Hrac hrac, int TestX, int TestY)
{
    int Nascitano = 0;
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 0; j++)
        {
            if (i > -1 && j > -1) break;
            Nascitano = NascitejRadu(hrac, TestX, TestY, i, j, 1);
            if (Nascitano < 5) Nascitano = NascitejRadu(hrac, TestX, TestY, -i, -j, Nascitano);
            if (Nascitano > 4) break;
        }
        if (Nascitano > 4) break;
    }
    return (Nascitano > 4);
}

int NascitejRadu(Hrac hrac, int x, int y, int PosunX, int PosunY, int Nascitano)
{
    int pocitadlo = 0;
    Pozice TestovanaPozice = new Pozice(x, y);
    while (pocitadlo <= 5)
    {
        TestovanaPozice.X = TestovanaPozice.X + PosunX;
        TestovanaPozice.Y = TestovanaPozice.Y + PosunY;
        if (TestovanaPozice.X < 0 || TestovanaPozice.X > StranaHrPlochy) break;
        pocitadlo++;
        bool Nalezeno = false;
        if (PoziceObsazenaHracem(hrac, TestovanaPozice))
        {
            Nalezeno = true;
            Nascitano++;
        }
        if (!Nalezeno) break;
    }
    return Nascitano;
}

string ZadaniPozice()
{
    bool ZadaniPoziceOk = false;
    while (!ZadaniPoziceOk)
    {
        Console.SetCursorPosition(14, StranaHrPlochy + 7);
        Console.ForegroundColor = ConsoleColor.Black; Console.Write(PoziceVstup); Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(14, StranaHrPlochy + 7);
        PoziceVstup = Console.ReadLine().ToUpper();
        if (PoziceVstup.Length < 2 || PoziceVstup.Length > 3) Console.WriteLine("Příliš krátké nebo dlouhé zadání!              ");
        else if ((int)PoziceVstup[0] < 65 || (int)PoziceVstup[0] > 90 || !int.TryParse(PoziceVstup.Remove(0, 1), out int abc))
            Console.WriteLine("Nesprávný formát zadání!                ");
        else if ((int)PoziceVstup[0] - 64 > StranaHrPlochy || Math.Abs(abc) >= StranaHrPlochy)
            Console.WriteLine("Zadání mimo rozsah hrací plochy!               ");
        else if (PoziceObsazena(PoziceVstup)) Console.WriteLine("Pozice " + PoziceVstup + " je již obsazena!               ");
        else ZadaniPoziceOk = true;
    }
    Console.WriteLine("                                                     ");
    return PoziceVstup;
}

bool PoziceObsazena(string VstupText)
{
    Pozice NovaPozice = TextNaPozici(VstupText);
    bool Obsazeno = false;
    foreach (Hrac hrac in Hraci)
    {
        Obsazeno = (PoziceObsazenaHracem(hrac, NovaPozice));
        if (Obsazeno) break;
    }
    return Obsazeno;
}

bool PoziceObsazenaHracem(Hrac hrac, Pozice KontrolovanaPozice)
{
    bool Obsazeno = false;
    foreach (Pozice pozice in hrac.ZadanePozice)
    {
        if (pozice.X == KontrolovanaPozice.X && pozice.Y == KontrolovanaPozice.Y) Obsazeno = true;
        if (Obsazeno) break;
    }
    return Obsazeno;
}

void VykresliHraciPlochu()
{
    for (int i = 0; i < StranaHrPlochy; i++)
    {
        for (int j = 0; j < StranaHrPlochy; j++)
        {
            if (j == 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(4 + 3 * i, 3);
                Console.Write(" {0} ", i);
                Console.SetCursorPosition(4 + 3 * i, 4 + StranaHrPlochy);
                Console.Write(" {0} ", i);
                Console.SetCursorPosition(1, 4 + i);
                char c = (char)(65 + i);
                Console.Write(" {0} ", c);
                Console.SetCursorPosition(4 + StranaHrPlochy * 3, 4 + i);
                Console.Write(" {0} ", c);
            }
            VykresliPozici(j, i, "   ");
        }
    }
}
void ZadaniPredStartem()
{
    bool ZadaniOK = false;
    while (!ZadaniOK)
    {
        StranaHrPlochy = StranaHrPlochyDefault;
        Console.SetCursorPosition(0, 2);
        Console.WriteLine("Délka strany hracího čtverce (5 - 20, ENTER = 20 políček):                     ");
        Console.SetCursorPosition(59, 2);
        string Vstup = Console.ReadLine();
        if (Vstup == "") ZadaniOK = true;
        else if (!int.TryParse(Vstup, out StranaHrPlochy)) Console.WriteLine("Zadej číslo nebo ENTER!             ");
        else if (StranaHrPlochy < 5 || StranaHrPlochy > 20) Console.WriteLine("Zadání mimo rozsah, zadej znovu!");
        else ZadaniOK = true;
    }
    Console.WriteLine("Chcete zadat jména hráčů (A/N)?             ");
    string PojmenovatHrace = Console.ReadKey(true).KeyChar.ToString().ToLower();
    for (int i = 0; i < PocetHracu; i++)
    {
        string JmenoHrace = "Hráč č. " + (i + 1).ToString();
        if (PojmenovatHrace == "a")
        {
            bool JmenoExistuje;
            do
            {
                Console.Write("Zadej jméno " + (i + 1).ToString() + ". hráče: ");
                JmenoHrace = Console.ReadLine().Trim();
                JmenoExistuje = false;
                foreach (Hrac hrac in Hraci)
                {
                    if (JmenoHrace.ToLower() == hrac.Jmeno.ToLower())
                    {
                        JmenoExistuje = true;
                        Console.WriteLine("Hráč tohoto jména již existuje!");
                        break;
                    }
                }
                if (JmenoHrace == "")
                {
                    Console.WriteLine("Nelze zadat prázdné jméno!");
                    JmenoExistuje = true;
                }
            }
            while (JmenoExistuje);
        }
        Hraci.Add(new Hrac(JmenoHrace, SymbolyHracu[i].ToString()));
    }
    Console.WriteLine("\nZadávání hotovo!\n----------------");
    for (int i = 0; i < PocetHracu; i++)
    {
        Console.WriteLine("Hráč {0}: {1}", i + 1, Hraci[i].Jmeno);
    }
    Console.WriteLine("Strana hrací plochy: " + StranaHrPlochy);

}
void VykresliPozici(int x, int y, string text)
{
    Console.BackgroundColor = ConsoleColor.Black;
    if ((x + y) % 2 == 1) Console.BackgroundColor = ConsoleColor.DarkBlue;
    Console.SetCursorPosition(4 + 3 * x, 4 + y);
    Console.WriteLine(text); Console.BackgroundColor = ConsoleColor.Black;
}

Pozice TextNaPozici(string text)
{
    return new Pozice(int.Parse(text.Remove(0, 1)), (int)text[0] - 65);
}

