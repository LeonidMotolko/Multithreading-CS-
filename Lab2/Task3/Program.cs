using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TablesLibrary
{
    public class Tablet
    {
        private int ID { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string OSType { get; set; }
    }
}

namespace TabletLibrary
{
    public class Manufacturer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        private bool IsAChildCompany { get; set; }
    }
}

class Program
{
    static string tabletsXmlFile = "tablets.xml";
    static string manufacturersXmlFile = "manufacturers.xml";

    static void Main()
    {
        List<TablesLibrary.Tablet> tablets = new List<TablesLibrary.Tablet>();
        List<TabletLibrary.Manufacturer> manufacturers = new List<TabletLibrary.Manufacturer>();

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1 - Create and display 10 Tablets");
            Console.WriteLine("2 - Serialize Tablets to XML");
            Console.WriteLine("3 - Display Tablets from XML file");
            Console.WriteLine("4 - Create and display 10 Manufacturers");
            Console.WriteLine("5 - Serialize Manufacturers to XML");
            Console.WriteLine("6 - Display Manufacturers from XML file");
            Console.WriteLine("7 - Open and display objects from XML");
            Console.WriteLine("8 - Find all 'Model' attributes with XDocument");
            Console.WriteLine("9 - Find all 'Model' attributes with XmlDocument");
            Console.WriteLine("10 - Modify attribute using XDocument");
            Console.WriteLine("11 - Modify attribute using XmlDocument");
            Console.WriteLine("0 - Exit");
            Console.Write("Choose option: ");

            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1": tablets = CreateTablets(10); PrintTablets(tablets); break;
                    case "2": SerializeToXml(tablets, tabletsXmlFile); break;
                    case "3": PrintTablets(DeserializeFromXml<List<TablesLibrary.Tablet>>(tabletsXmlFile)); break;
                    case "4": manufacturers = CreateManufacturers(10); PrintManufacturers(manufacturers); break;
                    case "5": SerializeToXml(manufacturers, manufacturersXmlFile); break;
                    case "6": PrintManufacturers(DeserializeFromXml<List<TabletLibrary.Manufacturer>>(manufacturersXmlFile)); break;
                    case "7":
                        Console.Write("Enter XML filename: ");
                        string fname = Console.ReadLine();
                        if (fname == tabletsXmlFile) PrintTablets(DeserializeFromXml<List<TablesLibrary.Tablet>>(fname));
                        else if (fname == manufacturersXmlFile) PrintManufacturers(DeserializeFromXml<List<TabletLibrary.Manufacturer>>(fname));
                        else Console.WriteLine("Unknown file.");
                        break;
                    case "8":
                        Console.Write("Enter XML filename: ");
                        UsingXDocumentFindModelAttributes(Console.ReadLine());
                        break;
                    case "9":
                        Console.Write("Enter XML filename: ");
                        UsingXmlDocumentFindModelAttributes(Console.ReadLine());
                        break;
                    case "10": ModifyAttributeXDocument(); break;
                    case "11": ModifyAttributeXmlDocument(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static List<TablesLibrary.Tablet> CreateTablets(int count)
    {
        var list = new List<TablesLibrary.Tablet>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(new TablesLibrary.Tablet
            {
                Model = $"Model{i}",
                SerialNumber = $"SN{i:000}",
                OSType = (i % 2 == 0) ? "Android" : "iOS"
            });
        }
        return list;
    }

    static void PrintTablets(List<TablesLibrary.Tablet> tablets)
    {
        Console.WriteLine("\nTablets:");
        foreach (var t in tablets)
            Console.WriteLine($"Model: {t.Model}, SN: {t.SerialNumber}, OS: {t.OSType}");
    }

    static List<TabletLibrary.Manufacturer> CreateManufacturers(int count)
    {
        var list = new List<TabletLibrary.Manufacturer>();
        for (int i = 1; i <= count; i++)
        {
            list.Add(new TabletLibrary.Manufacturer
            {
                Name = $"Company{i}",
                Address = $"Street {i}"
            });
        }
        return list;
    }

    static void PrintManufacturers(List<TabletLibrary.Manufacturer> list)
    {
        Console.WriteLine("\nManufacturers:");
        foreach (var m in list)
            Console.WriteLine($"Name: {m.Name}, Address: {m.Address}");
    }

    static void SerializeToXml<T>(T obj, string path)
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Create))
            ser.Serialize(fs, obj);
    }

    static T DeserializeFromXml<T>(string path)
    {
        XmlSerializer ser = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Open))
            return (T)ser.Deserialize(fs);
    }

    static void UsingXDocumentFindModelAttributes(string filename)
    {
        XDocument doc = XDocument.Load(filename);
        var models = doc.Descendants().Attributes("Model");
        foreach (var m in models)
            Console.WriteLine(m.Value);
    }

    static void UsingXmlDocumentFindModelAttributes(string filename)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filename);
        XmlNodeList nodes = doc.SelectNodes("//*[@Model]");
        foreach (XmlNode node in nodes)
            Console.WriteLine(node.Attributes["Model"].Value);
    }

    static void ModifyAttributeXDocument()
    {
        Console.Write("Enter XML filename: ");
        string file = Console.ReadLine();
        if (!File.Exists(file)) { Console.WriteLine("File not found."); return; }

        Console.Write("Enter element index (0-based): ");
        int idx = int.Parse(Console.ReadLine());

        Console.Write("Enter attribute name: ");
        string attr = Console.ReadLine();

        Console.Write("Enter new value: ");
        string value = Console.ReadLine();

        XDocument doc = XDocument.Load(file);
        var elements = doc.Descendants().Where(e => e.Attribute(attr) != null).ToList();
        if (idx >= 0 && idx < elements.Count)
        {
            elements[idx].Attribute(attr).Value = value;
            doc.Save(file);
            Console.WriteLine("Attribute updated successfully.");
        }
        else Console.WriteLine("Index out of range.");
    }

    static void ModifyAttributeXmlDocument()
    {
        Console.Write("Enter XML filename: ");
        string file = Console.ReadLine();
        if (!File.Exists(file)) { Console.WriteLine("File not found."); return; }

        Console.Write("Enter element index (0-based): ");
        int idx = int.Parse(Console.ReadLine());

        Console.Write("Enter attribute name: ");
        string attr = Console.ReadLine();

        Console.Write("Enter new value: ");
        string value = Console.ReadLine();

        XmlDocument doc = new XmlDocument();
        doc.Load(file);
        XmlNodeList nodes = doc.SelectNodes($"//*[@{attr}]");
        if (idx >= 0 && idx < nodes.Count)
        {
            nodes[idx].Attributes[attr].Value = value;
            doc.Save(file);
            Console.WriteLine("Attribute updated successfully.");
        }
        else Console.WriteLine("Index out of range.");
    }
}
