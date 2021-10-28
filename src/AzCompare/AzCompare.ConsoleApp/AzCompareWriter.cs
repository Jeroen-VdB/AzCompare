using DiffPlex.DiffBuilder.Model;
using System;

namespace AzCompare.ConsoleApp
{
    public static class AzCompareWriter
    {        
        // Sample from: https://github.com/mmanela/diffplex#sample-code
        public static void WriteDiff(DiffPaneModel diff)
        {
            var savedColor = Console.ForegroundColor;
            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("+ ");
                        break;
                    case ChangeType.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("- ");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("  ");
                        break;
                }

                Console.WriteLine(line.Text);
            }
            Console.ForegroundColor = savedColor;
            Console.ReadLine();
        }
    }
}
