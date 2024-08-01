# ImageGeoLocation

This is a windows command line program to Show or Delete image's geolocation, or delete all metadata in an image.

## usage

- In a folder where this is copied at, enter ...
- - ImageGeoLocation c:\somewhere\yourimage.jpg -g
- - This will create a copy of yourimage.jpb as c:\somewhere\modified\yourimage.jpg without geo location data but with the rest of the EXIF metadata intact.

- This, of course, assumes that you want to keep your camera settings in the metadata but remove the GPS data.
- Other options are to set the output folder, remove all metadata data, or just show the GPS data.
- example to get all options:
- - ImageGeoLocation -?

## Why is this useful?

When you upload a photo to Facebook or most other social medias someone else looks at that photo. Facebook, at least, does not show the metadata and GPS location. But that does not mean they don't have it themselves. Do you want that company to know where the picture was taken?

Do you know if your social media company of choice even blocks this information people browsing your account?

If you are sending photos by email, or sharing on a fileshare, then there is no automatic blocking of this metadata.

## Word meaning for the curious

| Word        | Desc                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Metadata    | The picture is the data, but information about the picture                                                                                                                                                                                                                                                                                                                                                                                            |
| GeoLocation | this is about the location                                                                                                                                                                                                                                                                                                                                                                                                                            |
| GPS         | Global Positioning System, the coordinates gained from satellites about a location                                                                                                                                                                                                                                                                                                                                                                    |
| EXIF        | Exchangeable image file format, this is where and how the metadata is stored.                                                                                                                                                                                                                                                                                                                                                                         |
|             | It’s a standard that most companies use. This is why my Canon camera and my Android phone can save metadata in a form that Window’s image viewer, PicPick’s image viewer, GIMP, PaintNET, Google’s photos and dozens of other applications can all show you the metadata. In the case of my Canon, it’s just camera settings, from my Android phone, it’s camera settings and geolocation. (This last sentence was foreshadowing for my next project) |

## Limits

- This uses .NET 8.x
- All error handling pathways have not bee tested, if this crashes let me know.
- JPEGs come out larger after removing the EXIF than before
- This works on a single file, or all files in a folder
- This only works on filetypes ".jpg", ".jpeg", ".jpe", ".bmp", ".gif", ".png", ".tiff", ".tif"

## Todos

- Find a way to make this one file, instead of 4 files.
- Fix JPEGS to save in the same resolution as when opened, instead of growing
- Enhance input file selection to include wildcards
- Create install package
-

## Credits

- https://github.com/exiftool/exiftool
- https://github.com/SixLabors/ImageSharp
