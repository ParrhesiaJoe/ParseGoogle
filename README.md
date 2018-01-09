# ParseGoogle
Parses google English word n-grams to create a word frequency list with a much smaller size, to utilize in word prediction.

# Use
Go to the google n-grams repository, and download the American English 2012 1-grams for letters a-z.
googlebooks-eng-us-all-1gram-20120701-[a-z]
Note the us in the file name. Also note the 2012.
http://storage.googleapis.com/books/ngrams/books/datasetsv2.html

Download the files, and unzip them all into a directory. The files will take about 20GB in total.
Next, run the program and enter the min and max year you wish to scrape for word frequency.

Press Scan, and watch it go. It would be trivial to add parallel execution if you are impatient for it to finish. It only take 10-20 minutes.

After the scanning is complete, the "Write Output" button will be enabled. Select the number of times each word must occur to be written into the output file.
Also, you may select the minimum number of unique books the word must appear in.

The program outputs four files when you hit "Write Output".
One file for the minimum words, and one for the minimum books.
For each of these, there is one list output that sorts by frequency (descending), and one that sorts alphabetically.

Included in the repo is the results of a 1m word and 300k book frequency list for the years from 1980-current.
