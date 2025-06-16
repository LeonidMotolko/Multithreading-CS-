using System.Reflection;

class program
{
    static void Main()
    {
        // loading the assembly in which the Tablet class is defined
        Assembly libraryAssembly = typeof(TablesLibrary.Tablet).Assembly;

        Console.WriteLine("Все классы из сборки:\n");

        // get all classes from the assembly, without filtering by namespace
        var types = libraryAssembly.GetTypes()
                    .Where(t => t.IsClass);

        foreach (var type in types)
        {
            Console.WriteLine($"Class: {type.FullName}");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                string access = prop.GetMethod.IsPublic ? "Public" : "Private";
                Console.WriteLine($"  - {access} property: {prop.Name} ({prop.PropertyType.Name})");
            }

            Console.WriteLine();
        }
    }
}