using System.Net;
using System.Text;
using Newtonsoft.Json;

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
    
    // Кодування тексту за допомогою таблиці кодів Хафмана
    static byte[] EncodeText(string text, Dictionary<char, string> huffmanCodes)
    {
        var binaryString = new StringBuilder();
        // Для кожного символу в тексті додавання його коду Хафмана до двійкового рядка
        foreach (var symbol in text)
            binaryString.Append(huffmanCodes[symbol]);

        // Визначення кількості байтів, необхідних для зберігання двійкового рядка
        int numBytes = (binaryString.Length + 7) / 8;
        byte[] result = new byte[numBytes];
        // Заповнення масиву байтів значеннями, що відповідають 8-бітним фрагментам двійкового рядка
        for (int i = 0; i < numBytes; i++)
            result[i] = Convert.ToByte(binaryString.ToString(i * 8, Math.Min(8, binaryString.Length - i * 8)), 2);

        return result;
    }

    // Читання закодованого тексту із файлу та його розшифрування
        static string DecodeText(string inputFilePath, Dictionary<char, string> huffmanCodes)
        {
            // Створення зворотної таблиці для розшифрування (ключі і значення міняються місцями)
            var reverseHuffmanCodes = huffmanCodes.ToDictionary(pair => pair.Value, pair => pair.Key);

            // Завантаження закодованого тексту з файлу у вигляді байтів
            byte[] encodedBytes = File.ReadAllBytes(inputFilePath);

            // Перетворення масиву байтів у двійковий рядок
            var binaryString = new StringBuilder();
            foreach (var byteValue in encodedBytes)
            {
                binaryString.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
            }

            // Розшифрування двійкового рядка за допомогою зворотної таблиці кодів Хафмана
            var decodedText = new StringBuilder();
            var currentCode = "";
            foreach (var bit in binaryString.ToString())
            {
                currentCode += bit;
                if (reverseHuffmanCodes.ContainsKey(currentCode))
                {
                    decodedText.Append(reverseHuffmanCodes[currentCode]);
                    currentCode = "";
                }
            }

            // Повернення розшифрованого тексту
            return decodedText.ToString();
        }


        static void Main()
    {
        string inputFilePath = "https://raw.githubusercontent.com/kse-ua/algorithms/main/res/sherlock.txt";
        string outputFilePath = "encoded_text.bin";

        // Зчитування тексту з файлу
        string text = new WebClient().DownloadString(inputFilePath);
        // Підрахунок частоти символів
        var frequencies = CountSymbols(text);
        // Побудова дерева Хафмана
        var huffmanTree = BuildHuffmanTree(frequencies);
        // Генерація таблиці кодів Хафмана
        var huffmanCodes = new Dictionary<char, string>();
        GenerateHuffmanCodes(huffmanTree, "", huffmanCodes);
        
        File.WriteAllText("HuffmanCodes.txt", JsonConvert.SerializeObject(huffmanCodes));
        var huffmanCodesfromFILE = JsonConvert.DeserializeObject<Dictionary<char, string>>
            (File.ReadAllText("HuffmanCodes.txt"));

        // Виведення таблиці кодів Хафмана
        foreach (var code in huffmanCodesfromFILE)
            Console.WriteLine($"Symbol: {code.Key}, Code: {code.Value}");

        // Кодування тексту за допомогою таблиці кодів Хафмана
        byte[] encodedBytes = EncodeText(text, huffmanCodesfromFILE);
        // Запис закодованого тексту у файл у форматі байтів
        File.WriteAllBytes(outputFilePath, encodedBytes);
        
        Console.WriteLine("Text was encoded and saved in file: encoded_text.bin.");

        // Читання закодованого тексту, його розшифрування та виведення на екран
        string decodedText = DecodeText("encoded_text.bin", huffmanCodesfromFILE);
        Console.WriteLine("\nDecoded text:");
        Console.WriteLine(decodedText);
    }
}