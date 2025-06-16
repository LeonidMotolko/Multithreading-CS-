using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TablesLibrary;

class Program
{
    // File paths for XML serialization
    static string tabletsXmlFile = "tablets.xml";
    static string manufacturersXmlFile = "manufacturers.xml";

    static void Main()
    {
        // Lists to hold created objects in memory
        List<Tablet> tablets = new List<Tablet>();
        List<Manufacturer> manufacturers = new List<Manufacturer>();

        while (true)
        {
            // Display the menu options to the user
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1 - Create 10 Tablet objects and display");
            Console.WriteLine("2 - Serialize Tablet objects to XML");
            Console.WriteLine("3 - Display contents of Tablet XML file");
            Console.WriteLine("4 - Create 10 Manufacturer objects and display");
            Console.WriteLine("5 - Serialize Manufacturer objects to XML");
            Console.WriteLine("6 - Display contents of Manufacturer XML file");
            Console.WriteLine("7 - Open XML file, deserialize objects and display");
            Console.WriteLine("8 - Using XDocument find and display all 'Model' attribute values");
            Console.WriteLine("9 - Using XmlDocument find and display all 'Model' attribute values");
            Console.WriteLine("0 - Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        // Create 10 tablets and display them
                        tablets = CreateTablets(10);
                        PrintTablets(tablets);
                        break;

                    case "2":
                        // Serialize tablet list to XML file if it exists
                        if (tablets.Count == 0)
                            Console.WriteLine("Create Tablet objects first (option 1).");
                        else
                        {
                            SerializeToXml(tablets, tabletsXmlFile);
                            Console.WriteLine($"Tablet objects serialized to {tabletsXmlFile}");
                        }
                        break;

                    case "3":
                        // Deserialize tablets from XML and display
                        if (!File.Exists(tabletsXmlFile))
                            Console.WriteLine($"File {tabletsXmlFile} not found.");
                        else
                        {
                            var loadedTablets = DeserializeFromXml<List<Tablet>>(tabletsXmlFile);
                            PrintTablets(loadedTablets);
                        }
                        break;

                    case "4":
                        // Create 10 manufacturers and display them
                        manufacturers = CreateManufacturers(10);
                        PrintManufacturers(manufacturers);
                        break;

                    case "5":
                        // Serialize manufacturer list to XML file if it exists
                        if (manufacturers.Count == 0)
                            Console.WriteLine("Create Manufacturer objects first (option 4).");
                        else
                        {
                            SerializeToXml(manufacturers, manufacturersXmlFile);
                            Console.WriteLine($"Manufacturer objects serialized to {manufacturersXmlFile}");
                        }
                        break;

                    case "6":
                        // Deserialize manufacturers from XML and display
                        if (!File.Exists(manufacturersXmlFile))
                            Console.WriteLine($"File {manufacturersXmlFile} not found.");
                        else
                        {
                            var loadedManufacturers = DeserializeFromXml<List<Manufacturer>>(manufacturersXmlFile);
                            PrintManufacturers(loadedManufacturers);
                        }
                        break;

                    case "7":
                        // Open any XML file (tablets or manufacturers), deserialize and display
                        Console.Write("Enter XML filename to open and display objects: ");
                        string filename = Console.ReadLine();
                        if (!File.Exists(filename))
                        {
                            Console.WriteLine($"File {filename} not found.");
                            break;
                        }
                        if (filename == tabletsXmlFile)
                        {
                            var loaded = DeserializeFromXml<List<Tablet>>(filename);
                            PrintTablets(loaded);
                        }
                        else if (filename == manufacturersXmlFile)
                        {
                            var loaded = DeserializeFromXml<List<Manufacturer>>(filename);
                            PrintManufacturers(loaded);
                        }
                        else
                        {
                            Console.WriteLine("Unknown file. Supported: tablets.xml or manufacturers.xml");
                        }
                        break;

                    case "8":
                        // Use LINQ to XML (XDocument) to find all 'Model' attributes in given XML file and print them
                        Console.Write("Enter XML filename to search for 'Model' attribute with XDocument: ");
                        string xdocFile = Console.ReadLine();
                        if (!File.Exists(xdocFile))
                        {
                            Console.WriteLine($"File {xdocFile} not found.");
                            break;
                        }
                        UsingXDocumentFindModelAttributes(xdocFile);
                        break;

                    case "9":
                        // Use XmlDocument to find all 'Model' attributes in given XML file and print them
                        Console.Write("Enter XML filename to search for 'Model' attribute with XmlDocument: ");
                        string xmldocFile = Console.ReadLine();
                        if (!File.Exists(xmldocFile))
                        {
                            Console.WriteLine($"File {xmldocFile} not found.");
                            break;
                        }
                        UsingXmlDocumentFindModelAttributes(xmldocFile);
                        break;

                    case "0":
                        // Exit the program
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Catch and display any unexpected errors
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Creates a list of Tablet objects with sample data.
    /// </summary>
    static List<Tablet> CreateTablets(int count)
    {
        var list = new List<Tablet>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(new Tablet
            {
                Model = $"Model{i}",
                SerialNumber = $"SN{i:0000}",
                OSType = (i % 2 == 0) ? "Android" : "iOS"
            });
        }
        return list;
    }

    /// <summary>
    /// Prints a list of Tablet objects to the console.
    /// </summary>
    static void PrintTablets(List<Tablet> tablets)
    {
        Console.WriteLine("\nTablets:");
        foreach (var t in tablets)
            Console.WriteLine($"Model: {t.Model}, SerialNumber: {t.SerialNumber}, OSType: {t.OSType}");
    }

    /// <summary>
    /// Creates a list of Manufacturer objects with sample data.
    /// </summary>
    static List<Manufacturer> CreateManufacturers(int count)
    {
        var list = new List<Manufacturer>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(new Manufacturer
            {
                Name = $"Manufacturer{i}",
                Address = $"Address{i}"
            });
        }
        return list;
    }

    /// <summary>
    /// Prints a list of Manufacturer objects to the console.
    /// </summary>
    static void PrintManufacturers(List<Manufacturer> manufacturers)
    {
        Console.WriteLine("\nManufacturers:");
        foreach (var m in manufacturers)
            Console.WriteLine($"Name: {m.Name}, Address: {m.Address}");
    }

    /// <summary>
    /// Serializes an object to XML and saves it to the specified file path.
    /// </summary>
    static void SerializeToXml<T>(T obj, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(fs, obj);
        }
    }

    /// <summary>
    /// Deserializes XML content from a file into an object of type T.
    /// </summary>
    static T DeserializeFromXml<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return (T)serializer.Deserialize(fs);
        }
    }

    /// <summary>
    /// Uses LINQ to XML (XDocument) to find and print all 'Model' attribute values in the XML file.
    /// </summary>
    static void UsingXDocumentFindModelAttributes(string filename)
    {
        // Load the XML document
        XDocument xdoc = XDocument.Load(filename);

        // Find all 'Model' attributes anywhere in the document
        var modelAttributes = xdoc.Descendants()
                                  .Attributes("Model");

        Console.WriteLine($"Found {modelAttributes.Count()} 'Model' attribute(s) in {filename}:");
        foreach (var attr in modelAttributes)
        {
            Console.WriteLine(attr.Value);
        }
    }

    /// <summary>
    /// Uses XmlDocument to find and print all 'Model' attribute values in the XML file.
    /// </summary>
    static void UsingXmlDocumentFindModelAttributes(string filename)
    {
        // Load the XML document
        XmlDocument doc = new XmlDocument();
        doc.Load(filename);

        // Select all nodes that have a 'Model' attribute
        XmlNodeList nodesWithModelAttr = doc.SelectNodes("//*[@Model]");

        Console.WriteLine($"Found {nodesWithModelAttr.Count} 'Model' attribute(s) in {filename}:");
        foreach (XmlNode node in nodesWithModelAttr)
        {
            var modelAttr = node.Attributes["Model"];
            if (modelAttr != null)
                Console.WriteLine(modelAttr.Value);
        }
    }
}
