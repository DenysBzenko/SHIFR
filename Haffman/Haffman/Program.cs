using System.Net;
using System.Text;

class Node
{
    public char Symbol;
    public int Frequency;
    public Node Left;
    public Node Right;
}

class Program
{
    // Підрахунок частоти символів у тексті
    static Dictionary<char, int> CountSymbols(string text)
    {
        var result = new Dictionary<char, int>();
        foreach (var symbol in text)
        {
            if (result.ContainsKey(symbol))
                result[symbol]++;
            else
                result[symbol] = 1;
        }
        return result;