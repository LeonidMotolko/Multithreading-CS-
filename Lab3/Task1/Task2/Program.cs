using System.Runtime.Serialization.Json;
using TablesLibrary;

namespace Lab3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // 1. Генерация тестовых данных (20 производителей)
                var manufacturers = GenerateTestData(20);

                // 2. Разделение на два списка по 10 элементов
                var firstBatch = manufacturers.GetRange(0, 10);
                var secondBatch = manufacturers.GetRange(10, 10);

                // 3. Параллельная сериализация в два файла
                var serializer = new ParallelSerializer();
                await serializer.ParallelSerializeAsync(
                    firstBatch, "manufacturers1.json",
                    secondBatch, "manufacturers2.json");

                Console.WriteLine("1. Сериализация завершена. Созданы файлы:");
                Console.WriteLine("   - manufacturers1.json");
                Console.WriteLine("   - manufacturers2.json");

                // 4. Слияние файлов с поочерёдной записью
                await serializer.MergeJsonFiles(
                    "manufacturers1.json",
                    "manufacturers2.json",
                    "merged_manufacturers.json");

                Console.WriteLine("\n2. Файлы успешно объединены:");
                Console.WriteLine("   - Результат в merged_manufacturers.json");

                // 5. Вывод информации о результате
                Console.WriteLine("\nСодержимое объединённого файла:");
                Console.WriteLine(await File.ReadAllTextAsync("merged_manufacturers.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static List<Manufacturer> GenerateTestData(int count)
        {
            var result = new List<Manufacturer>();
            for (int i = 1; i <= count; i++)
            {
                result.Add(Manufacturer.Create(
                    $"Manufacturer{i}",
                    $"Address{i}",
                    i % 2 == 0));
            }
            return result;
        }
    }

    public class ParallelSerializer
    {
        public async Task ParallelSerializeAsync<T>(List<T> firstBatch, string firstFile,
                                                 List<T> secondBatch, string secondFile)
        {
            var task1 = SerializeToFileAsync(firstBatch, firstFile);
            var task2 = SerializeToFileAsync(secondBatch, secondFile);
            await Task.WhenAll(task1, task2);
        }

        public async Task SerializeToFileAsync<T>(List<T> objects, string filePath)
        {
            await Task.Run(() =>
            {
                var serializer = new DataContractJsonSerializer(typeof(List<T>));
                using (var stream = File.Create(filePath))
                {
                    serializer.WriteObject(stream, objects);
                }
            });
        }

        public async Task MergeJsonFiles(string file1Path, string file2Path, string outputPath)
        {
            // Чтение обоих файлов параллельно
            var readTask1 = Task.Run(() => ReadJsonFile<List<Manufacturer>>(file1Path));
            var readTask2 = Task.Run(() => ReadJsonFile<List<Manufacturer>>(file2Path));

            await Task.WhenAll(readTask1, readTask2);

            var list1 = await readTask1;
            var list2 = await readTask2;

            // Поочерёдное объединение
            var mergedList = new List<Manufacturer>();
            for (int i = 0; i < Math.Max(list1.Count, list2.Count); i++)
            {
                if (i < list1.Count) mergedList.Add(list1[i]);
                if (i < list2.Count) mergedList.Add(list2[i]);
            }

            // Запись результата
            await WriteJsonFile(outputPath, mergedList);
        }

        private T ReadJsonFile<T>(string filePath)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = File.OpenRead(filePath))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        private async Task WriteJsonFile<T>(string filePath, T data)
        {
            await Task.Run(() =>
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = File.Create(filePath))
                {
                    serializer.WriteObject(stream, data);
                }
            });
        }
    }
}