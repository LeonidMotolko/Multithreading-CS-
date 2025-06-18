using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TablesLibrary;

class Program
{
    static string tabletsFile = "tablets.xml";
    static string manufacturersFile = "manufacturers.xml";

    static void Main()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1 - Create and display 10 Tablets");
            Console.WriteLine("2 - Serialize Tablets to XML");
            Console.WriteLine("3 - Display Tablets from XML file");
            Console.WriteLine("4 - Create and display 10 Manufacturers");
            Console.WriteLine("5 - Serialize Manufacturers to XML");
            Console.WriteLine("6 - Display Manufacturers from XML file");
            Console.WriteLine("7 - Open and display objects from XML");
            Console.WriteLine("8 - Find all 'Model' elements with XDocument");
            Console.WriteLine("9 - Find all 'Model' elements with XmlDocument");
            Console.WriteLine("10 - Modify attribute using XDocument");
            Console.WriteLine("11 - Modify attribute using XmlDocument");
            Console.WriteLine("0 - Exit");
            Console.Write("Choose option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    var tablets = CreateTablets(10);
                    DisplayPrintable("Tablets:", tablets);
                    break;
                case "2":
                    var tabletsToSerialize = CreateTablets(10);
                    SerializeToXml(tabletsToSerialize, tabletsFile);
                    Console.WriteLine($"Tablets serialized to {tabletsFile}");
                    break;
                case "3":
                    DisplayFromFile(tabletsFile);
                    break;
                case "4":
                    var manufacturers = CreateManufacturers(10);
                    DisplayPrintable("Manufactures:", manufacturers);
                    break;
                case "5":
                    var manufacturersToSerialize = CreateManufacturers(10);
                    SerializeToXml(manufacturersToSerialize, manufacturersFile);
                    Console.WriteLine($"Manufacturers serialized to {manufacturersFile}");
                    break;
                case "6":
                    DisplayFromFile(manufacturersFile);
                    break;
                case "7":
                    DisplayObjectsFromXml();
                    break;
                case "8":
                    FindModelValuesXDocument();
                    break;
                case "9":
                    FindModelValuesXmlDocument();
                    break;
                case "10":
                    ModifyXmlAttributeXDocumentMenu();
                    break;
                case "11":
                    ModifyXmlAttributeXmlDocumentMenu();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    #region Create and Display

    //1
    static List<Tablet> CreateTablets(int count)
    {
        var list = new List<Tablet>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(Tablet.Create(i, $"Model{i}", $"SN{i}", i % 2 == 0 ? "Android" : "iOS"));
        }
        return list;
    }


    static void DisplayFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            Console.WriteLine("File content: ");
            Console.WriteLine(content);
        }
        else
        {
            Console.WriteLine("File not found: " + filePath);
        }
    }
    
    // 
    static void DisplayPrintable(string header, IEnumerable<IPrintable> printables)
    {
        Console.WriteLine("\n" + header);
        foreach (var printable in printables)
        {
            printable.Print();
        }   
    }
    

    //4
    static List<Manufacturer> CreateManufacturers(int count)
    {
        var list = new List<Manufacturer>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(Manufacturer.Create($"Name{i}", $"Address{i}", i % 2 == 0));
        }
        return list;
    }

    #endregion

    #region Serialization and Deserialization

    //5
    static void SerializeToXml<T>(T obj, string filename)
    {
        try
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File {filename} not found");
                return ;
            }
            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, obj);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serialization error: {ex.Message}");
        }
    }

    //
    static T DeserializeFromXml<T>(string filename) where T : class
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filename))
            {
                return serializer.Deserialize(reader) as T;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Deserialization error: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region XML Reading and Searching

    static void DisplayObjectsFromXml()
    {
        string filename = InputFilename("Enter XML filename: ");
        string name = Path.GetFileName(filename).ToLower();
    
        switch (name)
        {
            case "manufacturers.xml":
                var manufactures = DeserializeFromXml<List<Manufacturer>>(filename);
                DisplayPrintable("Manufactures:", manufactures);
                break;
    
            case "tablets.xml":
                var tablets = DeserializeFromXml<List<Tablet>>(filename);
                DisplayPrintable("Tablets:", tablets);
                break;
    
            default:
                Console.WriteLine($"Unsupported XML file: {name}");
                break;
        }
    }


    // Find all <Model> elements and print their values using XDocument
    static void FindModelValuesXDocument()
    {
        try
        {
            string filename = InputFilename("Enter XML filename: ");
            
            XDocument xdoc = XDocument.Load(filename);
            var models = xdoc.Descendants().Attributes("Model").Select(x => x.Value).Distinct();

            Console.WriteLine("Model values found with XDocument:");
            foreach (var model in models)
            {
                Console.WriteLine(model);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    // Find all <Model> elements and print their values using XmlDocument
    static void FindModelValuesXmlDocument()
    {
        try
        {
            string filename = InputFilename("Enter XML filename: ");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            
            var nodes = xmlDoc.SelectNodes("//*[@Model]");
            

            Console.WriteLine("Model values found with XmlDocument:");
            foreach (XmlNode node in nodes)
            {
                Console.WriteLine(((XmlElement)node).GetAttribute("Model"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    #endregion

    #region Modify XML Attribute


    
    // Menu and logic for modifying XML using XDocument
    static void ModifyXmlAttributeXDocumentMenu()
    {
        string filename = InputFilename();

        Console.Write("Enter attribute name to modify: ");
        string attrName = Console.ReadLine();

        Console.Write("Enter element number (1-based): ");
        if (!int.TryParse(Console.ReadLine(), out int elementNumber) || elementNumber < 1)
        {
            Console.WriteLine("Invalid element number");
            return;
        }

        Console.Write("Enter new attribute value: ");
        string newValue = Console.ReadLine();

        ModifyAttributeUsingXDocument(filename, attrName, elementNumber, newValue);
    }

    // Modify attribute value using XDocument without rewriting whole file (modifies in memory, then saves)
    static void ModifyAttributeUsingXDocument(string filename, string attributeName, int elementIndex, string newValue)
    {
        try
        {
            
            XDocument xdoc = XDocument.Load(filename);
            // Get all elements that have the attribute
            var elementsWithAttr = xdoc.Descendants()
                .Where(e => e.Attribute(attributeName) != null)
                .ToList();

            if (elementsWithAttr.Count == 0)
            {
                Console.WriteLine($"No elements with attribute '{attributeName}' found");
                return;
            }

            if (elementIndex > elementsWithAttr.Count)
            {
                Console.WriteLine($"Element number {elementIndex} exceeds number of elements with '{attributeName}' attribute ({elementsWithAttr.Count})");
                return;
            }

            // Modify the attribute value
            elementsWithAttr[elementIndex - 1].SetAttributeValue(attributeName, newValue);
            xdoc.Save(filename);
            Console.WriteLine($"Attribute '{attributeName}' updated for element #{elementIndex}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error modifying XML: {ex.Message}");
        }
    }

    // Menu and logic for modifying XML using XmlDocument
    static void ModifyXmlAttributeXmlDocumentMenu()
    {
        string filename = InputFilename();

        Console.Write("Enter attribute name to modify: ");
        string attrName = Console.ReadLine();

        Console.Write("Enter element number (1-based): ");
        if (!int.TryParse(Console.ReadLine(), out int elementNumber) || elementNumber < 1)
        {
            Console.WriteLine("Invalid element number");
            return;
        }

        Console.Write("Enter new attribute value: ");
        string newValue = Console.ReadLine();

        ModifyAttributeUsingXmlDocument(filename, attrName, elementNumber, newValue);
    }

    // Modify attribute value using XmlDocument
    static void ModifyAttributeUsingXmlDocument(string filename, string attributeName, int elementIndex, string newValue)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            // Select all elements that have the attribute
            XmlNodeList nodesWithAttr = xmlDoc.SelectNodes($"//*[@{attributeName}]");

            if (nodesWithAttr == null || nodesWithAttr.Count == 0)
            {
                Console.WriteLine($"No elements with attribute '{attributeName}' found");
                return;
            }

            if (elementIndex > nodesWithAttr.Count)
            {
                Console.WriteLine($"Element number {elementIndex} exceeds number of elements with '{attributeName}' attribute ({nodesWithAttr.Count})");
                return;
            }

            XmlElement element = (XmlElement)nodesWithAttr[elementIndex - 1];
            element.SetAttribute(attributeName, newValue);

            xmlDoc.Save(filename);
            Console.WriteLine($"Attribute '{attributeName}' updated for element #{elementIndex}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error modifying XML: {ex.Message}");
        }
    }

    #endregion
    
    static string InputFilename(string message="Enter XML filename: ")
    {
        Console.Write(message);
        string filename = Console.ReadLine();
        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found");
            return InputFilename();
        }

        return filename;
    }
}
