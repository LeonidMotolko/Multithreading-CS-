using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using TablesLibrary;

class Program
{
    // File paths for XML serialization
    static string tabletsXmlFile = "tablets.xml";
    static string manufacturersXmlFile = "manufacturers.xml";

    static void Main()
    {
        List<Tablet> tablets = new List<Tablet>();
        List<Manufacturer> manufacturers = new List<Manufacturer>();

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1 - Create 10 Tablet objects and display them");
            Console.WriteLine("2 - Serialize Tablet objects to XML");
            Console.WriteLine("3 - Display contents of Tablet XML file");
            Console.WriteLine("4 - Create 10 Manufacturer objects and display them");
            Console.WriteLine("5 - Serialize Manufacturer objects to XML");
            Console.WriteLine("6 - Display contents of Manufacturer XML file");
            Console.WriteLine("0 - Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    tablets = CreateTablets(10);
                    PrintTablets(tablets);
                    break;
                case "2":
                    if (tablets.Count == 0)
                        Console.WriteLine("Please create Tablet objects first (option 1).");
                    else
                    {
                        SerializeToXml(tablets, tabletsXmlFile);
                        Console.WriteLine($"Tablet objects serialized to file {tabletsXmlFile}");
                    }
                    break;
                case "3":
                    if (!File.Exists(tabletsXmlFile))
                        Console.WriteLine($"File {tabletsXmlFile} not found. Please serialize Tablet objects first.");
                    else
                    {
                        var loadedTablets = DeserializeFromXml<List<Tablet>>(tabletsXmlFile);
                        PrintTablets(loadedTablets);
                    }
                    break;
                case "4":
                    manufacturers = CreateManufacturers(10);
                    PrintManufacturers(manufacturers);
                    break;
                case "5":
                    if (manufacturers.Count == 0)
                        Console.WriteLine("Please create Manufacturer objects first (option 4).");
                    else
                    {
                        SerializeToXml(manufacturers, manufacturersXmlFile);
                        Console.WriteLine($"Manufacturer objects serialized to file {manufacturersXmlFile}");
                    }
                    break;
                case "6":
                    if (!File.Exists(manufacturersXmlFile))
                        Console.WriteLine($"File {manufacturersXmlFile} not found. Please serialize Manufacturer objects first.");
                    else
                    {
                        var loadedManufacturers = DeserializeFromXml<List<Manufacturer>>(manufacturersXmlFile);
                        PrintManufacturers(loadedManufacturers);
                    }
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    // Creates a list of Tablet objects with sample data
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

    // Prints list of Tablets to console
    static void PrintTablets(List<Tablet> tablets)
    {
        Console.WriteLine("\nList of Tablets:");
        foreach (var tablet in tablets)
        {
            Console.WriteLine($"Model: {tablet.Model}, SerialNumber: {tablet.SerialNumber}, OSType: {tablet.OSType}");
        }
    }

    // Creates a list of Manufacturer objects with sample data
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

    // Prints list of Manufacturers to console
    static void PrintManufacturers(List<Manufacturer> manufacturers)
    {
        Console.WriteLine("\nList of Manufacturers:");
        foreach (var m in manufacturers)
        {
            Console.WriteLine($"Name: {m.Name}, Address: {m.Address}");
        }
    }

    // Serializes an object to XML file
    static void SerializeToXml<T>(T obj, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(fs, obj);
        }
    }

    // Deserializes an object from XML file
    static T DeserializeFromXml<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            return (T)serializer.Deserialize(fs);
        }
    }
}
