using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

//https://www.cnblogs.com/cgzl/p/11736011.html

namespace Aksl.Csv
{
    public class CsvTextProvider
    {
        public CsvTextProvider()
        {
        }

        public async IAsyncEnumerable<string[]> ReadEmbeddedCsvTextAsync(string fileName)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string fileFullName = $"{assembly.GetName().Name}.{fileName}";

           var allLines = await  File.ReadAllLinesAsync(fileName, CancellationToken.None);
            {
                foreach (var line in  allLines)
                {
                    List<string> values = line.Split(',').ToList();
                    for (int i = values.Count - 1; i > 0; i--)
                    {
                        if (values[i].EndsWith("\"") && values[i - 1].StartsWith("\""))
                        {
                            values[i - 1] = values[i - 1].Trim('"') + values[i].Trim('"');
                            values.RemoveAt(i);
                        }
                    }

                    yield return values.ToArray();
                }
            }
        }

        public IEnumerable<string[]> ReadEmbeddedCsvText(string fileName)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            string fileFullName = $"{assembly.GetName().Name}.{fileName}";

         //   using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            using (Stream stream = assembly.GetManifestResourceStream(fileFullName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        List<string> values = line.Split(',').ToList();
                        for (int i = values.Count - 1; i > 0; i--)
                        {
                            if (values[i].EndsWith("\"") && values[i - 1].StartsWith("\""))
                            {
                                values[i - 1] = values[i - 1].Trim('"') + values[i].Trim('"');
                                values.RemoveAt(i);
                            }
                        }

                        yield return values.ToArray();
                    }
                }
            }
        }

        private Stream GetCsvStream(string fileName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
        }

        //Windows-appsample-lunch-scheduler-master-2.0\LunchScheduler.Repository\LunchDemoRepository.cs
        //private async Task<T> ReadEmbeddedResourcesAsync<T>(string resource)
        //{
        //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
        //    {
        //        using (var reader = new StreamReader(stream))
        //        {
        //            string json = await reader.ReadToEndAsync();
        //            T data = JsonConvert.DeserializeObject<T>(json);
        //            return data;
        //        }
        //    }
        //}
    }
}
