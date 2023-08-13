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

            Console.WriteLine("Do you want to remove comments from generated files? (y/n)");
            bool removeComments = Console.ReadLine().ToLower() == "y";

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

namespace Dump2Cs
{{
    {RemoveCommentsFromCode(classContent, removeComments)}
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

        static string RemoveCommentsFromCode(string code, bool removeComments)
        {
            if (removeComments)
            {
                code = Regex.Replace(code, @"//.*", "");
                code = Regex.Replace(code, @"/\*(.|\n)*?\*/", "");
            }
            return code;
        }
    }
}
