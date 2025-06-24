using System.Runtime.Serialization.Json;

namespace MyProject.Services
{
    public class ParallelSerializer
    {
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

        public async Task ParallelSerializeAsync<T>(List<T> firstBatch, string firstFile,
                                                 List<T> secondBatch, string secondFile)
        {
            var task1 = SerializeToFileAsync(firstBatch, firstFile);
            var task2 = SerializeToFileAsync(secondBatch, secondFile);

            await Task.WhenAll(task1, task2);
        }
    }
}