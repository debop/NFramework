using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NSoft.NFramework.FusionCharts {
    // TODO: 새로 만들어야 한다.

    /// <summary>
    /// FusionCharts Image Generator Class
    /// FusionCharts Exporter - 'Image Resource' handles 
    /// FusionCharts (since v3.1) Server Side Export feature that
    /// helps FusionCharts exported as Image files in various formats. 
    /// </summary>
    public class ImageGenerator {
        //Array - Stores multiple chart export data
        private readonly ArrayList arrExportData = new ArrayList();
        //stores number of pages = length of $arrExportData array
        private int numPages;

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color can also be specified
        /// </summary>
        public ImageGenerator(string imageData_FCFormat, string width, string height, string bgcolor) {
            setBitmapData(imageData_FCFormat, width, height, bgcolor);
        }

        /// <summary>
        /// Gets the binary data stream of the image
        /// The passed parameter determines the file format of the image
        /// to be exported
        /// </summary>
        public MemoryStream getBinaryStream(string strFormat) {
            // the image object 
            Bitmap exportObj = getImageObject();

            // initiates a new binary data sream
            var outStream = new MemoryStream();

            // determines the image format
            switch(strFormat) {
                case "jpg":
                case "jpeg":
                    exportObj.Save(outStream, ImageFormat.Jpeg);
                    break;
                case "png":
                    exportObj.Save(outStream, ImageFormat.Png);
                    break;
                case "gif":
                    exportObj.Save(outStream, ImageFormat.Gif);
                    break;
                case "tiff":
                    exportObj.Save(outStream, ImageFormat.Tiff);
                    break;
                default:
                    exportObj.Save(outStream, ImageFormat.Bmp);
                    break;
            }
            exportObj.Dispose();

            return outStream;
        }

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color can also be specified
        /// </summary>
        private void setBitmapData(string imageData_FCFormat, string width, string height, string bgcolor) {
            var chartExportData = new Hashtable();
            chartExportData["width"] = width;
            chartExportData["height"] = height;
            chartExportData["bgcolor"] = bgcolor;
            chartExportData["imagedata"] = imageData_FCFormat;
            arrExportData.Add(chartExportData);
            numPages++;
        }

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color should also be specified
        /// </summary>
        private Bitmap getImageObject(int id) {
            var rawImageData = (Hashtable)arrExportData[id];

            // create blank bitmap object which would store image pixel data
            var image = new Bitmap(Convert.ToInt16(rawImageData["width"]), Convert.ToInt16(rawImageData["height"]),
                                   System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // drwaing surface
            Graphics gr = Graphics.FromImage(image);

            // set background color
            gr.Clear(ColorTranslator.FromHtml("#" + rawImageData["bgcolor"]));

            string[] rows = rawImageData["imagedata"].ToString().Split(';');

            for(int yPixel = 0; yPixel < rows.Length; yPixel++) {
                //Split each row into 'color_count' columns.			
                String[] color_count = rows[yPixel].Split(',');
                //Set horizontal row index to 0
                int xPixel = 0;

                for(int col = 0; col < color_count.Length; col++) {
                    //Now, if it's not empty, we process it				
                    //Split the 'color_count' into color and repeat factor
                    String[] split_data = color_count[col].Split('_');

                    //Reference to color
                    string hexColor = split_data[0];
                    //refer to repeat factor
                    int fRepeat = int.Parse(split_data[1]);

                    //If color is not empty (i.e. not background pixel)
                    if(hexColor != "") {
                        //If the hexadecimal code is less than 6 characters, pad with 0
                        hexColor = hexColor.Length < 6 ? hexColor.PadLeft(6, '0') : hexColor;
                        for(int k = 1; k <= fRepeat; k++) {
                            //draw pixel with specified color
                            image.SetPixel(xPixel, yPixel, ColorTranslator.FromHtml("#" + hexColor));
                            //Increment horizontal row count
                            xPixel++;
                        }
                    }
                    else {
                        //Just increment horizontal index
                        xPixel += fRepeat;
                    }
                }
            }
            gr.Dispose();
            return image;
        }

        /// <summary>
        /// Retreives the bitmap image object
        /// </summary>
        private Bitmap getImageObject() {
            return getImageObject(0);
        }
    }
}