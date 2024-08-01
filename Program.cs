// See https://aka.ms/new-console-template for more information

using RemoveGEOLocation;
using System.IO;
using System.Drawing;
using System.Net.Sockets;
using System.Security.Cryptography;
using SixLabors.ImageSharp.Formats;

internal class Program
{
    private static void Main(string[] args)
    {
        RunParameters runParameters = RunParameters.BuildRunParameters(args);
        if (runParameters.InvalidParameters || runParameters.ShowHelpMode )
        {
            RunParameters.ShowHelp();
            return; 
        }

        runParameters.BuildFileListToProcess();

        if (runParameters.filesToProcess.Count < 1)
        {
            if (!runParameters.QuietMode)
            {
                Console.WriteLine("No files to process");
            }
            return; 
        }

        // make output folder 
        string? outputDir = CreateOutputLocation(runParameters.filesToProcess[0], runParameters.OutputFolder, runParameters.QuietMode, runParameters.TestMode); 
        if (outputDir == null )
        {
            if (!runParameters.QuietMode)
            {
                Console.WriteLine($"Unable to create or access the putput folder");
            }
            return; 
        }

        if (!runParameters.QuietMode)
        {
            Console.WriteLine($"Output Files will be placed at {outputDir}");
        }
        
        foreach (string file in runParameters.filesToProcess)
        {
            if ( runParameters.RemoveAllExif )
            {
                ExifAccess.RemoveAllExif(file, outputDir, runParameters.QuietMode, runParameters.TestMode); 
            } else if (runParameters.RemoveGeoData)
            {
                ExifAccess.RemoveGEOLocation(file, outputDir, runParameters.QuietMode, runParameters.TestMode);
            } else if (runParameters.ShowGeoData)
            {
                ExifAccess.ShowGEOLocation(file, runParameters.QuietMode);
            }

        }

    }


    // the output folder make bbe full path 
    // otherwise create 
    private static string? CreateOutputLocation(string firstInputFilename, string outputPathOrName, bool silentMode, bool testMode)
    {
        string outputDirectory; 
        if ( Path.IsPathRooted(outputPathOrName))
        {
            outputDirectory = outputPathOrName; 
        }

        // make a rooted path 
        string dir = Path.GetFullPath(firstInputFilename);  
        dir = Path.GetDirectoryName(dir);

        outputDirectory = Path.Combine(dir, outputPathOrName);
        try
        {
            if (!testMode)
            {
                Directory.CreateDirectory(outputDirectory);
            }
            if (!silentMode)
            {
                string action = testMode ? "would have created" : "Created";
                Console.WriteLine($"{ action} or used output folder { outputDirectory}");
            }                   
        }
        catch (Exception)
        {
            if (!silentMode)
            {
                string action = testMode ? "would have created" : "Created";
                Console.WriteLine($"Failed to create or access output dir {outputDirectory}");
            }

            return null;
        }
        return outputDirectory;
    }

 
}