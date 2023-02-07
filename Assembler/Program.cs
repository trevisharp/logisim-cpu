using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

int pc = 0;
var labels = new Dictionary<string, int>();

if (args.Length == 0)
{
    Console.WriteLine("Você precisa passar um parâmetro para o arquivo a ser montado.");
    return;
}

var filePath = args[0];

if (!File.Exists(filePath))
{
    Console.WriteLine("O arquivo especifiado não existe.");
    return;
}

StreamWriter writer = null;
StreamReader reader = null;
try
{
    writer = new StreamWriter("memory");
    writer.WriteLine("v2.0 raw");
    reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        line = processLine(line);
        writer.Write(line);
        writer.Write(" ");
    }
}
catch (Exception ex)
{
    Console.WriteLine("O seguinte erro ocorreu durante o processo:");
    Console.WriteLine(ex.Message);
}
finally
{
    reader.Close();
    writer.Close();
}

string processLine(string line)
{
    if (line.Contains(":"))
    {
        labels.Add(line.Trim().Replace(":", ""), pc);
        return "";
    }
    pc++;

    var lnargs = line.Trim().Split(" ", 
        StringSplitOptions.RemoveEmptyEntries);

    switch (lnargs[0])
    {
        default:
        case "nop":
            return "0000";
        
        case "add":
            return $"10{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "sub":
            return $"11{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "mul":
            return $"12{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "div":
            return $"13{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "shl":
            return $"14{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "shr":
            return $"15{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "neg":
            return $"16{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "and":
            return $"17{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "or":
            return $"18{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "not":
            return $"19{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "xor":
            return $"1a{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "inc":
            return $"1b{hex(lnargs[1])}0";
        
        case "dec":
            return $"1c{hex(lnargs[1])}0";
        
        case "push":
            return $"23{hex(lnargs[1])}0";
        
        case "pop":
            return $"24{hex(lnargs[1])}0";

        case "cmp":
            if (lnargs[1].Contains("$") && lnargs[2].Contains("$"))
                return $"40{hex(lnargs[1])}{hex(lnargs[2])}";
            else return $"5{hex(lnargs[1])}{hex(lnargs[2])}";
        
        case "mov":
            if (lnargs[1].Contains("$") && lnargs[2].Contains("["))
                return $"21{hex(lnargs[1])}{hex(lnargs[2])}";
            else if (lnargs[1].Contains("[") && lnargs[2].Contains("$"))
                return $"22{hex(lnargs[1])}{hex(lnargs[2])}";
            else if (lnargs[1].Contains("$") && lnargs[2].Contains("$"))
                return $"20{hex(lnargs[1])}{hex(lnargs[2])}";
            else return $"2{hex(lnargs[1])}{hex(lnargs[2])}";
            
        case "call":
            return $"e{hex(lnargs[1])}";
        
        case "ret":
            return $"ffff";
    }

    string hex(string str)
    {
        str = str
            .Replace("$", "")
            .Replace(",", "")
            .Replace("[", "")
            .Replace("]", "");
        var value = int.Parse(str);
        var r = "";
        while (value > 0)
        {
            r += inttohex(value % 16);
            value /= 16;
        }
        if (r == "")
            r = "0";
        return r;
    }

    string inttohex(int value)
        => value < 10 ? value.ToString() :
        ((char)(value - 10 + 'a')).ToString();
}
