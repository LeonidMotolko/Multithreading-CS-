using System;
using System.IO;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            string dllPath = Path.Combine(Directory.GetCurrentDirectory(), "TabletsLibrary.dll");

            if (!File.Exists(dllPath))
            {
                Console.WriteLine("DLL not found in path: " + dllPath);
                return;
            }

            Assembly libAssembly = Assembly.LoadFrom(dllPath);

            Console.WriteLine("Loaded classes:\n");

            var classes = libAssembly.GetTypes()
                .Where(t => t.IsClass && t.GetMethod("Create") != null);

            foreach (var type in classes)
            {
                Console.WriteLine($"Class: {type.FullName}");

                var createMethod = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
                if (createMethod == null)
                {
                    Console.WriteLine("  - Create Method not found.");
                    continue;
                }

                var parameters = createMethod.GetParameters();
                object[] values = parameters.Select(p =>
                {
                    Console.Write($"  Enter a value for {p.Name} ({p.ParameterType.Name}): ");
                    string input = Console.ReadLine();
                    return Convert.ChangeType(input, p.ParameterType);
                }).ToArray();

                object instance = createMethod.Invoke(null, values);

                var printMethod = type.GetMethod("PrintObject", BindingFlags.Public | BindingFlags.Instance);
                if (printMethod == null)
                {
                    Console.WriteLine("  - PrintObject method not found.");
                    continue;
                }

                string result = printMethod.Invoke(instance, null)?.ToString();
                Console.WriteLine($"  >> Obj: {result}\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

