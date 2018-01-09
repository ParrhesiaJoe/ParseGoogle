using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Parse
{
   public partial class MainWindow
   {
      public string StatusText { get => (string)GetValue(StatusTextProperty); set => SetValue(StatusTextProperty, value); }
      public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register("StatusText",
         typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));
      public string CurrentFile { get => (string)GetValue(CurrentFileProperty); set => SetValue(CurrentFileProperty, value); }
      public static readonly DependencyProperty CurrentFileProperty = DependencyProperty.Register("CurrentFile",
         typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

      public string DataPath { get => (string)GetValue(DataPathProperty); set => SetValue(DataPathProperty, value); }
      public static readonly DependencyProperty DataPathProperty = DependencyProperty.Register("DataPath",
         typeof(string), typeof(MainWindow), new PropertyMetadata(@"e:\GoogleNGrams\"));

      public int MinYear { get => (int)GetValue(MinYearProperty); set => SetValue(MinYearProperty, value); }
      public static readonly DependencyProperty MinYearProperty = DependencyProperty.Register("MinYear",
         typeof(int), typeof(MainWindow), new PropertyMetadata(1980));
      public int MaxYear { get => (int)GetValue(MaxYearProperty); set => SetValue(MaxYearProperty, value); }
      public static readonly DependencyProperty MaxYearProperty = DependencyProperty.Register("MaxYear",
         typeof(int), typeof(MainWindow), new PropertyMetadata(2020));

      public bool CanWriteOutput { get => (bool)GetValue(CanWriteOutputProperty); set => SetValue(CanWriteOutputProperty, value); }
      public static readonly DependencyProperty CanWriteOutputProperty = DependencyProperty.Register("CanWriteOutput",
         typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

      public int MinWords { get => (int)GetValue(MinWordsProperty); set => SetValue(MinWordsProperty, value); }
      public static readonly DependencyProperty MinWordsProperty = DependencyProperty.Register("MinWords",
         typeof(int), typeof(MainWindow), new PropertyMetadata(100000));
      public int MinBooks { get => (int)GetValue(MinBooksProperty); set => SetValue(MinBooksProperty, value); }
      public static readonly DependencyProperty MinBooksProperty = DependencyProperty.Register("MinBooks",
         typeof(int), typeof(MainWindow), new PropertyMetadata(30000));

      private Dictionary<string, WordRecord> _words;

      public MainWindow()
      {
         InitializeComponent();
      }

      private async void OnParseClick(object sender, RoutedEventArgs e)
      {
         var dataPath = DataPath;
         var minYear = MinYear;
         var maxYear = MaxYear;
         try
         {
            _words = await Task.Run(() => Parser.Go(dataPath, minYear, maxYear, ReportStatus));
            CanWriteOutput = true;
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
            throw;
         }
      }

      private void OnOutputClick(object sender, RoutedEventArgs e)
      {
         if (_words == null) return;
         try
         {
            Parser.GenerateOutputFiles(DataPath, "output", MinWords, MinBooks, MinYear, MaxYear, _words);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
            throw;
         }
      }
      private void ReportStatus(string status)
      {
         Dispatcher.BeginInvoke(new Action(() =>
         {
            StatusText += status + "\n";
            if (!status.StartsWith("Line"))
            {
               CurrentFile = status;
            }
            else
            {
               StatusText = status;
            }
         }));
      }
   }
}
