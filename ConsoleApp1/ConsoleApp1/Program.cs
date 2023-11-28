List<string> Atier = new List<string>();
List<string> BTier = new List<string>();
List<string> Ctier = new List<string>();
List<string> DTier = new List<string>();
List<string> FTier = new List<string>();


string manqk = Console.ReadLine();
string ocenka = Console.ReadLine();

while(manqk != "krai")
{
    if(ocenka == "A")
    {
        Atier.Add(manqk);
    }
    else if(ocenka == "B")
    {
        BTier.Add(manqk);
    }
    else if(ocenka == "C")
    {
        Ctier.Add(manqk);
    }
    else if (ocenka == "D")
    {
        DTier.Add(manqk);
    }
    else
    {
        FTier.Add(manqk);
    }
    manqk = Console.ReadLine();
}
Console.Write("A tier: ");
Console.WriteLine(String.Join(" ", Atier));
Console.Write("B tier: ");
Console.WriteLine(String.Join(" ", BTier));
Console.Write("C tier: ");
Console.WriteLine(String.Join(" ", Ctier));
Console.Write("D tier: ");
Console.WriteLine(String.Join(" ", DTier));
Console.Write("F tier: ");
Console.WriteLine(String.Join(" ", FTier));

