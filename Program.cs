using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Dump2Cs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Do you want to start the process? (y/n): ");
            if (Console.ReadLine().Trim().ToLower() != "y")
            {
                Console.WriteLine("Process cancelled.");
                WaitForEnterKey();
                return;
            }

            Console.Write("Do you want to remove comments? (y/n): ");
            bool removeComments = Console.ReadLine().Trim().ToLower() == "y";

            Console.WriteLine("Read input file.");
            string inputFileContent = File.ReadAllText("dump.cs");

            inputFileContent = Regex.Replace(inputFileContent, @"//.*|/\*(.|\n)*?\*/", "");

            MatchCollection classMatches = Regex.Matches(inputFileContent, @"class\s+[A-Za-z_]\w*\s*{[^}]*}");

            if (classMatches.Count == 0)
            {
                Console.WriteLine("No classes found.");
                WaitForEnterKey();
                return;
            }

            Directory.CreateDirectory("DumpedScripts"); // Create directory if it doesn't exist

            int classNumber = 1;
            foreach (Match match in classMatches)
            {
                string classContent = match.Value;
                string className = Regex.Match(classContent, @"class\s+([A-Za-z_]\w*)").Groups[1].Value;

                GenerateClassFile(classContent, className, removeComments);
                Console.WriteLine($"Generated DumpedScripts/{className}.cs");
                classNumber++;
            }

            Console.WriteLine("Files generated successfully.");
            WaitForEnterKey();
        }

        static void WaitForEnterKey()
        {
            Console.WriteLine("Press Enter to exit...");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        static void GenerateClassFile(string classContent, string className, bool removeComments)
        {
            if (!string.IsNullOrEmpty(classContent))
            {
                string newClassContent = classContent;
                if (removeComments)
                {
                    newClassContent = Regex.Replace(classContent, @"//.*|/\*(.|\n)*?\*/", "");
                }

                string filePath = Path.Combine("DumpedScripts", $"{className}.cs");
                string newClassFileContent = $@"
using System;

namespace {className}
{{
    {newClassContent}
}}
";

                File.WriteAllText(filePath, newClassFileContent);
                Console.WriteLine($"Generated {filePath}");
            }
        }

    }
}
