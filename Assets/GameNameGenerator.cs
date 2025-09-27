using System;

public static class GameNameGenerator
{

    private static readonly string[] prefixes = new string[]
    {
        "Shadow", "Dragon", "Mysti", "Iron", "Silver",
        "Fire", "Night", "Storm", "Thund", "Frost",
        "Goldn", "Crims", "Silen", "Dark", "Brigh",
        "Ghost", "Steel", "Wind", "Stone", "Moon"
    };


    private static int HashString(string input)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in input)
            {
                hash = hash * 31 + c;
            }
            return Math.Abs(hash);
        }
    }

    // Adresten oyun ismi üret
    public static string GenerateName(string address)
    {
        if (string.IsNullOrEmpty(address))
            return "Play0";

        int hash = HashString(address);


        string prefix = prefixes[hash % prefixes.Length];


        int number = hash % 10;

        return $"{prefix}{number}";
    }
}
