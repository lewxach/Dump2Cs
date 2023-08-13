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
                string namespaceName = Regex.Match(classContent, @"namespace\s+([A-Za-z_]\w*)").Groups[1].Value;
                string className = Regex.Match(classContent, @"class\s+([A-Za-z_]\w*)").Groups[1].Value;

                if (string.IsNullOrEmpty(namespaceName))
                {
                    namespaceName = "NoNamespace";
                }

                GenerateClassFile(classContent, namespaceName, className, true);
                Console.WriteLine($"Generated {namespaceName}.{className}.cs");
                classNumber++;
            }

            Console.WriteLine("Files generated successfully.");
            WaitForEnterKey();
        }
        static void ProcessNamespace(string namespaceName, string namespaceContent, bool removeComments)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = "NoNamespace";
            }

            string[] lines = namespaceContent.Split('\n');
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("// Namespace:"))
                {
                    namespaceName = line.Trim().Replace("// Namespace:", "").Trim();
                    break;
                }
            }

            MatchCollection classMatches = Regex.Matches(namespaceContent, @"class\s+[A-Za-z_]\w*\s*{[^}]*}");

            if (classMatches.Count == 0)
            {
                Console.WriteLine($"No class content found for namespace: {namespaceName}");
                return;
            }

            foreach (Match classMatch in classMatches)
            {
                string classContent = classMatch.Value;
                string className = Regex.Match(classContent, @"class\s+([A-Za-z_]\w*)").Groups[1].Value;

                GenerateClassFile(classContent, namespaceName, className, removeComments);
            }
        }

        static void GenerateClassFile(string classContent, string namespaceName, string className, bool removeComments)
        {
            if (!string.IsNullOrEmpty(classContent))
            {
                string newClassContent = classContent;
                if (removeComments)
                {
                    newClassContent = Regex.Replace(classContent, @"//.*|/\*(.|\n)*?\*/", "");
                }

                string fullNamespace = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}." : string.Empty;
                string fileName = $"{fullNamespace}{className}.cs";

                string newClassFileContent = $@"
using System;

namespace {fullNamespace}{{
    {newClassContent}
}}
";

                File.WriteAllText(fileName, newClassFileContent);
                Console.WriteLine($"Generated {fileName}");
            }
        }


        static void WaitForEnterKey()
        {
            Console.WriteLine("Press Enter to exit...");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
