using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RemoveGEOLocation
{
    
    internal class RunParameters
    {
        public bool QuietMode { get; set; }
        public bool TestMode { get; set; }
        public string UserEnteredFile { get; set; } = string.Empty;
        public string OutputFolder { get; set; } = @"modified"; 
        public bool InvalidParameters { get; set; }
        internal bool RemoveAllExif { get; set; }
        internal bool RemoveGeoData { get; set; }
        internal bool ShowGeoData { get; set; }

        public static List<string> ImageFormats
        { get { List<string> results = imageFormats.ToList(); return results; }
        }

        private static readonly List<string> imageFormats = new List<string>
            {
                ".jpg", ".jpeg", ".jpe", ".bmp", ".gif", ".png", ".tiff", ".tif"
            };
        public List<string> filesToProcess
        { get { return _filesToProcess; } }

        private List<string> _filesToProcess = new List<string>();
        
        private bool IfFileTypeAnImage( string filename)
        {
            filename = filename.ToLower(); 
            foreach( string type in imageFormats)
            {
                if (filename.EndsWith(type)) return true; 
            }
            return false; 
        }


        public bool BuildFileListToProcess()
        {
            _filesToProcess.Clear();
                        
            if (UserEnteredFile == null || UserEnteredFile.Length == 0 || UserEnteredFile[0] == ' ' || UserEnteredFile == string.Empty)
            {
                return false; 
            }
            UserEnteredFile = Path.GetFullPath(UserEnteredFile);

            if (File.Exists(UserEnteredFile))
            {
                if (IfFileTypeAnImage(UserEnteredFile)) _filesToProcess.Add(UserEnteredFile); 

            }
            else 
            {
                string fileName;
                string dirName; 
                if ( Directory.Exists(UserEnteredFile))
                {
                    fileName = "*.*";
                    dirName = UserEnteredFile; 
                } else
                {
                    fileName = Path.GetFileName(UserEnteredFile);
                    dirName = UserEnteredFile.Replace(fileName, string.Empty);
                }
                if (Directory.Exists(dirName))
                {
                    string[] files = Directory.GetFiles(dirName, fileName);
                    foreach( string file in files )
                    {
                        if (IfFileTypeAnImage(file)) _filesToProcess.Add(file); 
                    }
                }
            }

            if (_filesToProcess.Count > 0) return true;
            return false; 
        }

        public static RunParameters BuildRunParameters(string[] args)
        {
            RunParameters parms = new RunParameters();
            if (args.Length < 1)
            {
                ShowHelp();
                parms.InvalidParameters = true;
                return parms;
            }

            foreach (string a in args)
            {
                if ((a[0] == '-' || a[0] == '/') && a.Length > 1)
                {
                    char code = Char.ToLower(a[1]);
                    switch (code)
                    {
                        case 'q':
                            parms.QuietMode = true;
                            break;
                        case 'h':
                        case '?':
                            parms.QuietMode = true;
                            break;
                        case 't':
                            parms.TestMode = true;
                            break;
                        case 'a':
                            parms.RemoveAllExif = true;
                            parms.RemoveGeoData = false;
                            parms.ShowGeoData = false;
                            break;
                        case 'g':
                            parms.RemoveGeoData = true;
                            parms.RemoveAllExif = false;
                            parms.ShowGeoData = false;
                            break;
                        case 's':
                            parms.ShowGeoData = true;
                            parms.RemoveGeoData = false;
                            parms.RemoveAllExif = false;
                            break;

                        case 'o':
                            if (a.Length > 3)
                            {
                                parms.OutputFolder = a.Substring(3);
                            }
                            else
                            {
                                parms.InvalidParameters = true;
                            }
                            break;
                        default:
                            parms.InvalidParameters = true;
                            break;
                    }
                }
                else
                {
                    parms.UserEnteredFile = a;
                }
            }
            return parms;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("RemoveGPSLocation - remove All or GeoLocation (GPS) metadata from photos");
            Console.WriteLine("RemoveGPSLocation filename|path -a|-g|-s [-o:outfolder] [-?] [t]");
            Console.WriteLine(" ");
            Console.WriteLine("   filename = name of file or folder (for all files in folder) ");
            Console.WriteLine("   -a = remove all Exif data, not just GPS ");
            Console.WriteLine("   -g = remove only GeoLocation metadata, leave camera and other data");
            Console.WriteLine("   -s = Show GeoLocation metadata, don't delete or copy anything ");
            Console.WriteLine(@"   -o:outfolder, default is .\Modified under the input folder");
            Console.WriteLine(" ");
            Console.WriteLine(" For removing all or geo metadata, the new file will be placed in the ");
            Console.WriteLine(" outfolder location with the same name.  If the named file exist in the ");
            Console.WriteLine(" folder, it will be renamed. ");
            Console.WriteLine(" Example: RemoveGPSLocation c:\\junk\\pic.jpg -g ");
            Console.WriteLine(" Results: c:\\junk\\Modified\\pic.jpg  ");
            Console.WriteLine(" ");
            Console.WriteLine("   -q = quiet mode, no console output");
            Console.WriteLine("   -? = Help ");
            Console.WriteLine("   -t = test mode, show what would happen, without applying results ");
            Console.WriteLine(" ");
            Console.WriteLine("Can only process these filetypes: ");
            Console.WriteLine($"  {string.Join(" ", RunParameters.ImageFormats)} ");
            Console.WriteLine("");
        }

    }
}

