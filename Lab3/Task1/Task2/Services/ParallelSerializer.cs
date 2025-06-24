using System.Runtime.Serialization.Json;
using TablesLibrary;

namespace MyProject.Services
{
    public class ParallelSerializer
    {
        // Метод для слияния двух JSON файлов
        public async Task MergeJsonFiles(string file1Path, string file2Path, string outputPath)
        {
            // Читаем оба файла параллельно
            var readTask1 = Task.Run(() => ReadJsonFile<List<Manufacturer>>(file1Path));
            var readTask2 = Task.Run(() => ReadJsonFile<List<Manufacturer>>(file2Path));

            await Task.WhenAll(readTask1, readTask2);

            var list1 = await readTask1;
            var list2 = await readTask2;

            // Объединяем списки поочередно
            var mergedList = new List<Manufacturer>();
            for (int i = 0; i < Math.Max(list1.Count, list2.Count); i++)
            {
                if (i < list1.Count) mergedList.Add(list1[i]);
                if (i < list2.Count) mergedList.Add(list2[i]);
            }

            // Записываем результат
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