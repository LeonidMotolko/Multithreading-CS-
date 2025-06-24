using TablesLibrary;
using MyProject.Services;

namespace MyProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var manufacturers = new List<Manufacturer>();
            for (int i = 1; i <= 20; i++)
            {
                manufacturers.Add(Manufacturer.Create(
                    $"Manufacturer{i}",
                    $"Address{i}",
                    i % 2 == 0));
            }

            var serializer = new ParallelSerializer();

            var firstBatch = manufacturers.GetRange(0, 10);
            var secondBatch = manufacturers.GetRange(10, 10);

            Console.WriteLine("Starting parallel serialization...");
            await serializer.ParallelSerializeAsync(
                firstBatch, "manufacturers1.json",
                secondBatch, "manufacturers2.json");

            Console.WriteLine("Done!");
        }
    }
}