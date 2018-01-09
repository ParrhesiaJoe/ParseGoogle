namespace Parse
{
   public class WordRecord
   {
      public string Raw { get; set; }
      public int WordCount { get; set; }
      public int BookCount { get; set; }
      public override string ToString()
      {
         return $"W:{WordCount} B:{BookCount}";
      }
   }
}