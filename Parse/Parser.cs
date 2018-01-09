using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Parse
{
   public class Parser
   {
      public static Dictionary<string, WordRecord> Go(string path, int minYear = 0, int maxYear = 2100, Action<string> reportStatus = null)
      {
         var words = new Dictionary<string, WordRecord>();
         const char a = 'a';
         const char z = 'z';
         for (var i = a; i <= z; i++)
         {
            var file = $"{path}googlebooks-eng-us-all-1gram-20120701-" + i;
            if (!File.Exists(file)) continue;
            var lineIndex = 0;
            reportStatus?.Invoke($"Starting file {file}");
            using (var ts = File.OpenText(file))
            {
               var line = ts.ReadLine();
               while (!string.IsNullOrWhiteSpace(line))
               {
                  if (reportStatus != null && lineIndex++ % 500000 == 0)
                  {
                     reportStatus($"Line: {ts.BaseStream.Position}/{ts.BaseStream.Length}");
                  }
                  var tabs = line.Split('\t');
                  var word = tabs[0];

                  // Filter by year min and max year are trimmed here because it decreases the memory footprint significantly.
                  // We cannot filter by count until we have counted all instances in the n-gram file. We aggregate all years into
                  // one count to process the data, so there is already data loss on this field. I'm taking advantage of that to
                  // further trim the in-memory data set. The entire corpus generally fits in about a GB of memory.
                  var year = tabs[1];
                  var intYear = int.Parse(year);
                  if (intYear < minYear || intYear > maxYear)
                  {
                     line = ts.ReadLine();
                     continue;
                  }

                  var overallCount = tabs[2];
                  var bookCount = tabs[3];
                  // Words are often in the form [wordRoot].[number]_[partOfSpeech].
                  // desk.13_NOUN is combined with
                  // desk, desk.3 or desk.13
                  var rawRoot = word.Split('_')[0].Split('.')[0];

                  // Here, because we are generating a frequency list, we are preserving the case of the input.
                  // If you wish to build a case insensitive list, ToLower all the roots.
                  // rawRoot = rawRoot.ToLower();

                  // If the record exists, increment... else, add a new record for this word variant.
                  if (words.ContainsKey(rawRoot))
                  {
                     var record = words[rawRoot];
                     record.BookCount += int.Parse(bookCount);
                     record.WordCount += int.Parse(overallCount);
                  }
                  else
                  {
                     var newRecord = new WordRecord
                     {
                        Raw = rawRoot,
                        BookCount = int.Parse(bookCount),
                        WordCount = int.Parse(overallCount)
                     };
                     words.Add(rawRoot, newRecord);
                  }
                  line = ts.ReadLine();
               }
            }
         }
         return words;
      }

      public static void GenerateOutputFiles(string path, string outputFileName, int minWordCount, int minBookCount, int startYear, int endYear, Dictionary<string, WordRecord> words)
      {
         var wordTrimmed = words.Where(w => w.Value.WordCount > minWordCount).Select(w => w.Value).ToList();
         WriteFile(wordTrimmed.OrderByDescending(w => w.WordCount), path, $"{outputFileName}{startYear}-{endYear}w{minWordCount}f.csv");
         WriteFile(wordTrimmed.OrderBy(w => w.Raw), path, $"{outputFileName}{startYear}-{endYear}w{minWordCount}a.csv");
         var bookTrimmed = words.Where(w => w.Value.BookCount > minBookCount).Select(w => w.Value).ToList();
         WriteFile(bookTrimmed.OrderByDescending(w => w.WordCount), path, $"{outputFileName}{startYear}-{endYear}b{minBookCount}f.csv");
         WriteFile(bookTrimmed.OrderBy(w => w.Raw), path, $"{outputFileName}{startYear}-{endYear}b{minBookCount}a.csv");
      }

      private static void WriteFile(IEnumerable<WordRecord> list, string path, string fileName)
      {
         using (var writer = File.CreateText($"{path}{fileName}"))
         {
            foreach (var pair in list)
            {
               writer.WriteLine($"{pair.Raw},{pair.WordCount}");
            }
         }
      }
   }
}
