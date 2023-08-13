using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Dump2Cs
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFileContent = File.ReadAllText("dump.cs");

            inputFileContent = Regex.Replace(inputFileContent, @"//.*|/\*(.|\n)*?\*/", "");

            MatchCollection classMatches = Regex.Matches(inputFileContent, @"class\s+[A-Za-z_]\w*\s*{[^}]*}");

            if (classMatches.Count == 0)
            {
                Console.WriteLine("No classes found.");
                WaitForEnterKey();
                return;
            }

            int classNumber = 1;
            foreach (Match match in classMatches)
            {
                string classContent = match.Value;
                string className = Regex.Match(classContent, @"class\s+([A-Za-z_]\w*)").Groups[1].Value;

                string newClassContent = $@"
using System;
//Dump2Cs go crazy
namespace Dump2Cs
{{
    {classContent}
}}
";

                File.WriteAllText($"{className}.cs", newClassContent);
                Console.WriteLine($"Generated {className}.cs");
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
    }
}
