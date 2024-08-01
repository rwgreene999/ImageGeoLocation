using System;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Jpeg.Components;
using SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace RemoveGEOLocation
{
    class ExifAccess
    {
        public static void RemoveAllExif(string filename, string outputDir, bool silentMode, bool testMode)
        {
            string outputFile = Path.Combine(outputDir, Path.GetFileName(filename));
            using (Image image = SixLabors.ImageSharp.Image.Load(filename))
            {
                
                DateTime? picsDT = GetPicsDate(image);
                IImageFormat imageFormat = image.Metadata.DecodedImageFormat;
                image.Metadata.ExifProfile = null;
                image.Metadata.XmpProfile = null;                
                if ( !testMode)
                {
                    DealWithExistingFile(outputFile);
                    image.Save(outputFile);
                    SetFilesDateTime(outputFile, picsDT);
                }
                
                if (!silentMode) 
                {
                    Console .WriteLine($"Image {filename} all Exif data remove" + Environment.NewLine + "   saved as {outputFile}"); 
                }
                
            }
        }
        public static void RemoveGEOLocation(string filename, string outputDir, bool silentMode, bool testMode)
        {            
            string outputFile = Path.Combine( outputDir, Path.GetFileName(filename) );
            using (var image = SixLabors.ImageSharp.Image.Load(filename))
            {
                ExifGeoData geo = GetLongitudeAndLattitude(image);

                RemoveLongitudeAndLattitude(image); 

                DateTime? picsDT = GetPicsDate(image);
                IImageFormat imageFormat = image.Metadata.DecodedImageFormat;

                if (!testMode)
                {
                    DealWithExistingFile(outputFile ); 
                    image.Save(outputFile);
                    SetFilesDateTime(outputFile, picsDT); 
                }

                if (!silentMode)
                {
                    string runtype = testMode ? "Would have saved" : "saved"; 
                    Console.WriteLine($"Image {filename} Geo Locatoin metadata remove" + Environment.NewLine + "{runtype} as {outputFile}");
                }                
            }
        }

        private static void SetFilesDateTime(string outputFile, DateTime? picsDT)
        {
            if ( !picsDT.HasValue ) { return; }
            File.SetCreationTime(outputFile, picsDT.Value );
            File.SetLastWriteTime(outputFile, picsDT.Value);
            File.SetLastAccessTime(outputFile, picsDT.Value);
            return; 
        }

        private static void DealWithExistingFile(string outputFile)
        {
            if (File.Exists(outputFile))
            {
                string backName = outputFile + ".oldversion";
                File.Delete(backName);
                File.Move(outputFile, backName);
            }
        }

        private static DateTime? GetPicsDate(Image? image)
        {
            DateTime? picDT = null;
            var exif = image.Metadata.ExifProfile;            
            if (exif != null )
            {
                var dateTimeOriginalValue = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.DateTime);


                    if (dateTimeOriginalValue != null)
                {
                    string dateTaken = dateTimeOriginalValue.ToString();
                    string date = dateTaken.Substring(0, 10).Replace(":", @"/");
                    string time = dateTaken.Substring(11);
                    string dt = date + " " + time;
                    try
                    {
                        picDT = DateTime.Parse(dt);
                    }
                    catch (Exception)
                    {
                        picDT = null;
                    }
                }

            }

            return picDT; 

        }

        private static double ConvertToDecimalDegrees(Rational[]? dms)
        {
            if (dms.Length != 3)
                throw new ArgumentException("Expected three rational values for degrees, minutes, seconds.");

            double degrees = dms[0].ToDouble();
            double minutes = dms[1].ToDouble();
            double seconds = dms[2].ToDouble();

            return degrees + (minutes / 60) + (seconds / 3600);
        }


        private static void RemoveLongitudeAndLattitude( Image image)
        {
            var profile = image.Metadata.ExifProfile;
            profile.RemoveValue(ExifTag.GPSLatitudeRef);
            profile.RemoveValue(ExifTag.GPSLatitude);
            profile.RemoveValue(ExifTag.GPSLongitudeRef);
            profile.RemoveValue(ExifTag.GPSLongitude);
            profile.RemoveValue(ExifTag.GPSMapDatum);
            profile.RemoveValue(ExifTag.GPSVersionID);
            profile.RemoveValue(ExifTag.GPSAltitude);
            profile.RemoveValue(ExifTag.GPSAltitudeRef);
            profile.RemoveValue(ExifTag.GPSAreaInformation);
            profile.RemoveValue(ExifTag.GPSDateStamp);
            profile.RemoveValue(ExifTag.GPSDestBearing);
            profile.RemoveValue(ExifTag.GPSDestBearingRef);
            profile.RemoveValue(ExifTag.GPSDestDistance);
            profile.RemoveValue(ExifTag.GPSDestDistanceRef);
            profile.RemoveValue(ExifTag.GPSDestLatitude);
            profile.RemoveValue(ExifTag.GPSDestLatitudeRef);
            profile.RemoveValue(ExifTag.GPSDestLongitude);
            profile.RemoveValue(ExifTag.GPSDestLongitudeRef);
            profile.RemoveValue(ExifTag.GPSDifferential);
            profile.RemoveValue(ExifTag.GPSDOP);
            profile.RemoveValue(ExifTag.GPSHPositioningError);
            profile.RemoveValue(ExifTag.GPSIFDOffset);
            profile.RemoveValue(ExifTag.GPSImgDirection);
            profile.RemoveValue(ExifTag.GPSImgDirectionRef);
            profile.RemoveValue(ExifTag.GPSMeasureMode);
            profile.RemoveValue(ExifTag.GPSProcessingMethod);
            profile.RemoveValue(ExifTag.GPSSatellites);
            profile.RemoveValue(ExifTag.GPSSpeed);
            profile.RemoveValue(ExifTag.GPSSpeedRef);
            profile.RemoveValue(ExifTag.GPSStatus);
            profile.RemoveValue(ExifTag.GPSTimestamp);
            profile.RemoveValue(ExifTag.GPSTrack);
            profile.RemoveValue(ExifTag.GPSTrackRef);
            profile.RemoveValue(ExifTag.GPSVersionID);
        }

        static private ExifGeoData GetLongitudeAndLattitude( Image image )
        {
            // from: https://github.com/SixLabors/ImageSharp/discussions/1295
            ExifGeoData geo = new ExifGeoData();
            var profile = image.Metadata.ExifProfile;



            if (profile?.TryGetValue(ExifTag.GPSLatitude, out IExifValue<Rational[]>? latitudeParts) == true 
                && latitudeParts?.Value?.Length == 3)
            {
                geo.LatitudeDegrees = latitudeParts.Value[0].Numerator;
                geo.LatitudeMinutes = latitudeParts.Value[1].Numerator;
                geo.LatitudeSeconds = (latitudeParts.Value[2].Numerator /
                    (double)latitudeParts.Value[2].Denominator);
                geo.LatitudeRef = profile?.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLatitudeRef).ToString();

                double LatitudeDegrees = latitudeParts.Value[0].Numerator;
                double LatitudeMinutes = latitudeParts.Value[1].Numerator / 60D;
                double LatitudeSeconds= (latitudeParts.Value[2].Numerator / 
                    (double)latitudeParts.Value[2].Denominator) / 3600D;

                geo.Latitude = LatitudeDegrees + LatitudeMinutes + LatitudeSeconds;
                if (geo.LatitudeRef == "S") geo.Latitude = 0 - geo.Latitude;
                
            }

            if (profile?.TryGetValue(ExifTag.GPSLongitude, out IExifValue<Rational[]>? longitudeParts) == true
                && longitudeParts?.Value?.Length == 3)
            {

                geo.LongitudeDegrees = longitudeParts.Value[0].Numerator;
                geo.LongitudeMinutes = longitudeParts.Value[1].Numerator;
                geo.LongitudeSeconds = (longitudeParts.Value[2].Numerator /
                    (double)longitudeParts.Value[2].Denominator);

                geo.LongitudeRef = profile?.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLongitudeRef).ToString();

                double LongitudeDegrees = longitudeParts.Value[0].Numerator;
                double LongitudeMinutes = longitudeParts.Value[1].Numerator / 60D;
                double LongitudeSeconds = (longitudeParts.Value[2].Numerator /
                    (double)longitudeParts.Value[2].Denominator) / 3600D;
                geo.Longitude = LongitudeDegrees + LongitudeMinutes + LongitudeSeconds;
                
                if (geo.LongitudeRef == "W") geo.Longitude = 0 - geo.Longitude;

            }

            return geo; 


            /* 
             * comments here in case I ever decide to access these fields. 
            var exif = image.Metadata.ExifProfile;
            var geo0 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestDistance);
            var geo1 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestDistanceRef);
            var geo2 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestLatitude);
            var geo3 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestLatitudeRef);
            var geo4 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestLongitude);
            var geo5 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSDestLongitudeRef);
            var geo6 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSAltitude);
            var geo7 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSAltitudeRef);
            var geo8 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSMeasureMode);
            var geoArray9 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSMapDatum);
            var geo10 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSTimestamp);
            var geo11 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSProcessingMethod);
            var geo12 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLatitude);
            var geo13 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLatitudeRef);
            var geo14 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLongitude);
            var geo15 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLongitudeRef);
            var geo16 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSAltitude);
            var geo17 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSAltitudeRef);
            var geo18 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSMeasureMode);
            var geo19 = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSMapDatum);


            var latitudeValue = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLatitude);
            var longitudeValue = exif.Values.FirstOrDefault(v => v.Tag == ExifTag.GPSLongitude);
            */ 

        }

        internal static void ShowGEOLocation(string file, bool quietMode)
        {
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(file))
            {
                ExifGeoData geo = GetLongitudeAndLattitude(image);
                Console.WriteLine($"File: {file}");
                Console.WriteLine($"      {geo.Latitude} {geo.Longitude} ");
                Console.WriteLine($"      {geo.LatitudeDMS} {geo.LongitudeDMS} "); 
            }
            return; 

        }
    }

}
