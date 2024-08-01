using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata.Ecma335;

namespace RemoveGEOLocation
{
    internal class ExifGeoData
    {
        // note: DMS = degrees, minutes, seconds  DD = Direct Decimal 
        /*
         * Empire State Building in New York City are as follows:

            In Degrees, Minutes, and Seconds (DMS):
            Latitude: 40° 44’ 54.3" N
            Longitude: 73° 59’ 9" W
            In Decimal Degrees (DD):
            Latitude: 40.748417
            Longitude: -73.9858332
         */
        public double Latitude { get; set; } = 0.0;
        public string LatitudeRef { get; set; } = string.Empty;
        public uint LatitudeDegrees { get; set; } = 0;
        public double LatitudeMinutes { get; set; } = 0;
        public double LatitudeSeconds { get; set; } = 0;
        public string LatitudeDMS { 
            get
            {
                if (Latitude == 0.0 && Longitude == 0.0)
                {
                    return $"No Geo Locations data present";
                }
                else
                {
                    return $"{LatitudeDegrees}°{LatitudeMinutes}’{LatitudeSeconds}\"{LatitudeRef} ";
                }
            }
        }

        public double Longitude { get; set; } = 0.0;
        public string LongitudeRef { get; set; } = string.Empty;
        public uint LongitudeDegrees { get; set; } = 0;
        public double LongitudeMinutes { get; set; } = 0;
        public double LongitudeSeconds { get; set; } = 0;
        public string LongitudeDMS
        {
            get
            {
                if (Latitude == 0.0 && Longitude == 0.0)
                {
                    return $"No Geo Locations data present";
                }
                else
                {
                    return $"{LongitudeDegrees}°{LongitudeMinutes}’{LongitudeSeconds}\"{LongitudeRef} ";
                }
            }            
        }

    }
}
