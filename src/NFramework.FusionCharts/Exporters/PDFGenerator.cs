using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace NSoft.NFramework.FusionCharts {
    // TODO: 새로 만들어야 한다.

    /// <summary>
    /// FusionCharts Exporter - 'PDF Resource' handles 
    /// FusionCharts (since v3.1) Server Side Export feature that
    /// helps FusionCharts exported as PDF file.
    /// </summary>
    public class PDFGenerator {
        //Array - Stores multiple chart export data
        private readonly ArrayList arrExportData = new ArrayList();
        //stores number of pages = length of $arrExportData array
        private int numPages;

        /// <summary>
        /// Generates a PDF file with the given parameters
        /// The imageData_FCFormat parameter is the FusionCharts export format data
        /// width, height are the respective width and height of the original image
        /// bgcolor determines the default background colour
        /// </summary>
        public PDFGenerator(string imageData_FCFormat, string width, string height, string bgcolor) {
            setBitmapData(imageData_FCFormat, width, height, bgcolor);
        }

        /// <summary>
        /// Gets the binary data stream of the image
        /// The passed parameter determines the file format of the image
        /// to be exported
        /// </summary>
        public MemoryStream getBinaryStream(string strFormat) {
            byte[] exportObj = getPDFObjects(true);

            var outStream = new MemoryStream();

            outStream.Write(exportObj, 0, exportObj.Length);

            return outStream;
        }

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color should also be specified
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

        //create image PDF object containing the chart image 
        private byte[] addImageToPDF(int id, bool isCompressed) {
            var imgObj = new MemoryStream();

            //PDF Object number
            int imgObjNo = 6 + id * 3;

            //Get chart Image binary
            byte[] imgBinary = getBitmapData24(id, isCompressed);

            //get the length of the image binary
            int len = imgBinary.Length;

            string width = getMeta("width", id);
            string height = getMeta("height", id);

            //Build PDF object containing the image binary and other formats required
            //string strImgObjHead = imgObjNo.ToString() + " 0 obj\n<<\n/Subtype /Image /ColorSpace /DeviceRGB /BitsPerComponent 8 /HDPI 72 /VDPI 72 " + (isCompressed ? "" : "") + "/Width " + width + " /Height " + height + " /Length " + len.ToString() + " >>\nstream\n";
            string strImgObjHead = imgObjNo +
                                   " 0 obj\n<<\n/Subtype /Image /ColorSpace /DeviceRGB /BitsPerComponent 8 /HDPI 72 /VDPI 72 " +
                                   (isCompressed ? "/Filter /RunLengthDecode " : "") + "/Width " + width + " /Height " + height +
                                   " /Length " +
                                   len + " >>\nstream\n";

            imgObj.Write(stringToBytes(strImgObjHead), 0, strImgObjHead.Length);
            imgObj.Write(imgBinary, 0, imgBinary.Length);

            const string strImgObjEnd = "endstream\nendobj\n";
            imgObj.Write(stringToBytes(strImgObjEnd), 0, strImgObjEnd.Length);

            imgObj.Close();
            return imgObj.ToArray();
        }

        private byte[] addImageToPDF(int id) {
            return addImageToPDF(id, true);
        }

        private byte[] addImageToPDF() {
            return addImageToPDF(0, true);
        }

        //Main PDF builder function
        private byte[] getPDFObjects() {
            return getPDFObjects(true);
        }

        private byte[] getPDFObjects(bool isCompressed) {
            var PDFBytes = new MemoryStream();

            //Store all PDF objects in this temporary string to be written to ByteArray

            //start xref array
            var xRefList = new ArrayList
                           {
                               "xref\n0 ",
                               "0000000000 65535 f \n"
                           };

            //Build PDF objects sequentially
            //version and header
            var strTmpObj = "%PDF-1.3\n%{FC}\n";
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 1 : info (optional)
            strTmpObj = "1 0 obj<<\n/Author (FusionCharts)\n/Title (FusionCharts)\n/Creator (FusionCharts)\n>>\nendobj\n";
            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 1
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 2 : Starts with Pages Catalogue
            strTmpObj = "2 0 obj\n<< /Type /Catalog /Pages 3 0 R >>\nendobj\n";
            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 2
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 3 : Page Tree (reference to pages of the catalogue)
            strTmpObj = "3 0 obj\n<<  /Type /Pages /Kids [";
            for(int i = 0; i < numPages; i++) {
                strTmpObj += (((i + 1) * 3) + 1) + " 0 R\n";
            }
            strTmpObj += "] /Count " + numPages + " >>\nendobj\n";

            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 3
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //Each image page
            for(int itr = 0; itr < numPages; itr++) {
                string iWidth = getMeta("width", itr);
                string iHeight = getMeta("height", itr);
                //OBJECT 4..7..10..n : Page config
                strTmpObj = (((itr + 2) * 3) - 2) + " 0 obj\n<<\n/Type /Page /Parent 3 0 R \n/MediaBox [ 0 0 " + iWidth + " " +
                            iHeight +
                            " ]\n/Resources <<\n/ProcSet [ /PDF ]\n/XObject <</R" + (itr + 1) + " " + ((itr + 2) * 3) +
                            " 0 R>>\n>>\n/Contents [ " +
                            (((itr + 2) * 3) - 1) + " 0 R ]\n>>\nendobj\n";
                xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 4,7,10,13,16...
                PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

                //OBJECT 5...8...11...n : Page resource object (xobject resource that transforms the image)
                xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 5,8,11,14,17...
                string xObjR = getXObjResource(itr);
                PDFBytes.Write(stringToBytes(xObjR), 0, xObjR.Length);

                //OBJECT 6...9...12...n : Binary xobject of the page (image)
                byte[] imgBA = addImageToPDF(itr, isCompressed);
                xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 6,9,12,15,18...
                PDFBytes.Write(imgBA, 0, imgBA.Length);
            }

            //xrefs	compilation
            xRefList[0] += ((xRefList.Count - 1) + "\n");

            //get trailer
            string trailer = getTrailer((int)PDFBytes.Length, xRefList.Count - 1);

            //write xref and trailer to PDF
            string strXRefs = string.Join("", (string[])xRefList.ToArray(typeof(string)));
            PDFBytes.Write(stringToBytes(strXRefs), 0, strXRefs.Length);
            //
            PDFBytes.Write(stringToBytes(trailer), 0, trailer.Length);

            //write EOF
            const string strEOF = "%%EOF\n";
            PDFBytes.Write(stringToBytes(strEOF), 0, strEOF.Length);

            PDFBytes.Close();
            return PDFBytes.ToArray();
        }

        //Build Image resource object that transforms the image from First Quadrant system to Second Quadrant system
        private string getXObjResource() {
            return getXObjResource(0);
        }

        private string getXObjResource(int itr) {
            string width = getMeta("width", itr);
            string height = getMeta("height", itr);
            return (((itr + 2) * 3) - 1) + " 0 obj\n<< /Length " + (24 + (width + height).Length) + " >>\nstream\nq\n" + width +
                   " 0 0 " + height +
                   " 0 0 cm\n/R" + (itr + 1) + " Do\nQ\nendstream\nendobj\n";
        }

        private static string calculateXPos(int posn) {
            return posn.ToString().PadLeft(10, '0') + " 00000 n \n";
        }

        private string getTrailer(int xrefpos) {
            return getTrailer(xrefpos, 7);
        }

        private static string getTrailer(int xrefpos, int numxref) {
            return "trailer\n<<\n/Size " + numxref + "\n/Root 2 0 R\n/Info 1 0 R\n>>\nstartxref\n" + xrefpos + "\n";
        }

        private byte[] getBitmapData24() {
            return getBitmapData24(0, true);
        }

        private byte[] getBitmapData24(int id, bool isCompressed) {
            string rawImageData = getMeta("imagedata", id);
            string bgColor = getMeta("bgcolor", id);

            var imageData24 = new MemoryStream();

            // Split the data into rows using ; as separator
            string[] rows = rawImageData.Split(';');

            for(int yPixel = 0; yPixel < rows.Length; yPixel++) {
                //Split each row into 'color_count' columns.			
                string[] color_count = rows[yPixel].Split(',');

                for(int col = 0; col < color_count.Length; col++) {
                    //Now, if it's not empty, we process it				
                    //Split the 'color_count' into color and repeat factor
                    string[] split_data = color_count[col].Split('_');

                    //Reference to color
                    string hexColor = split_data[0] != "" ? split_data[0] : bgColor;
                    //If the hexadecimal code is less than 6 characters, pad with 0
                    hexColor = hexColor.Length < 6 ? hexColor.PadLeft(6, '0') : hexColor;

                    //refer to repeat factor
                    int fRepeat = int.Parse(split_data[1]);

                    // convert color string to byte[] array
                    byte[] rgb = hexToBytes(hexColor);

                    // Set the repeated pixel in MemoryStream
                    for(int cRepeat = 0; cRepeat < fRepeat; cRepeat++) {
                        imageData24.Write(rgb, 0, 3);
                    }
                }
            }

            var len = (int)imageData24.Length;
            imageData24.Close();

            //Compress image binary
            if(isCompressed) {
                return new PDFCompress(imageData24.ToArray()).RLECompress();
            }
            return imageData24.ToArray();
        }

        // converts a hexadecimal colour string to it's respective byte value
        private static byte[] hexToBytes(string strHex) {
            if(strHex == null || strHex.Trim().Length == 0)
                strHex = "00";
            strHex = Regex.Replace(strHex, @"[^0-9a-fA-f]", "");
            if(strHex.Length % 2 != 0)
                strHex = "0" + strHex;

            int len = strHex.Length / 2;
            var bytes = new byte[len];

            for(int i = 0; i < len; i++) {
                string hex = strHex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        private string getMeta(string metaName) {
            return getMeta(metaName, 0);
        }

        private string getMeta(string metaName, int id) {
            if(metaName == null)
                metaName = "";
            var chartData = (Hashtable)arrExportData[id];
            return (chartData[metaName] == null ? "" : chartData[metaName].ToString());
        }

        private static byte[] stringToBytes(string str) {
            if(str == null)
                str = "";
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }
}