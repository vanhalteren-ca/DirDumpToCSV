// This app takes in a txt dump of random bull shit and attempts to turn it into a csv file.
// The file we are processing is just a saved text dump from calling >dir.
// We want it to be a nice csv file.
using System.Text;
using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.WriteLine("Error: Please provide a file path.");
    return 1;
}
if (!File.Exists(args[0]))
{
    Console.WriteLine("Error: File does not exist.");
    return 2;
}

var lines = File.ReadLines(args[0]);
var currentDir = "";

// Magical regex, my brain cant begin to comprehend.
var regex = new Regex(@"^(\d{4}-\d{2}-\d{2})\s+(\d{2}:\d{2}\s[APM]{2})\s+([\d,]+)\s+(.+)$");

// This string builder is used to create the csv file.
var csvBuilder = new StringBuilder();
csvBuilder.AppendLine("Data,Time,Size,Filename");

foreach (var line in lines)
{
    if (line.Contains("Directory of "))
    {
        currentDir = line.Replace("Directory of ", "").Trim() + "\\";
        continue;
    }

    // I need to learn regex because fuck its good.
    var match = regex.Match(line);
    if (match.Success)
    {
        string date = match.Groups[1].Value;
        string time = match.Groups[2].Value;
        string size = match.Groups[3].Value;
        string filename = currentDir + match.Groups[4].Value;

        // Enclose fields in double quotes if they contain commas.
        if (date.Contains(",")) date = $"\"{date}\"";
        if (time.Contains(",")) time = $"\"{time}\"";
        if (size.Contains(",")) size = $"\"{size}\"";
        if (filename.Contains(",")) filename = $"\"{filename}\"";

        csvBuilder.AppendLine($"{date},{time},{size},{filename}");
    }
}
File.WriteAllText(args[0] + ".csv", csvBuilder.ToString());
return 0;