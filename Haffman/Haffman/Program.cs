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
        return result;}

// Побудова дерева Хафмана
    static Node BuildHuffmanTree(Dictionary<char, int> frequencies)
    {
        // Створення списку вузлів дерева на основі частот символів
        var nodes = new List<Node>(frequencies.Select(f => new Node { Symbol = f.Key, Frequency = f.Value }));

        // Цикл виконується, доки в списку не залишиться тільки один вузол (корінь дерева Хафмана)
        while (nodes.Count > 1)
        {
            // Впорядкування вузлів за частотою (по зростанню)
            var orderedNodes = nodes.OrderBy(node => node.Frequency).ToList();

            // Якщо в списку є принаймні два 
            if (orderedNodes.Count >= 2)
            {
                // Створення нового батьківського вузла
                var parentNode = new Node
                {
                    // Частота батьківського вузла дорівнює сумі частот двох найменших вузлів
                    Frequency = orderedNodes[0].Frequency + orderedNodes[1].Frequency,
                    // Перший найменший вузол стає лівим нащадком
                    Left = orderedNodes[0],
                    // Другий найменший вузол стає правим нащадком
                    Right = orderedNodes[1]
                };

                // Видалення двох найменших вузлів зі списку
                nodes.Remove(orderedNodes[0]);
                nodes.Remove(orderedNodes[1]);
                // Додавання нового батьківського вузла до списку
                nodes.Add(parentNode);
            }
        }

        // Повернення кореня дерева Хафмана (перший елемент списку, якщо він існує, або null)
        return nodes.FirstOrDefault();
    }
    
    // Генерація кодів Хафмана
    static void GenerateHuffmanCodes(Node node, string code, Dictionary<char, string> codes)
    {
        if (node == null)
            return;

        if (node.Left == null && node.Right == null)
            codes[node.Symbol] = code;

        GenerateHuffmanCodes(node.Left, code + "0", codes);
        GenerateHuffmanCodes(node.Right, code + "1", codes);
    }