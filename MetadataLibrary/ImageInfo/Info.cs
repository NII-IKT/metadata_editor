//http://www.codeproject.com/Articles/7888/A-library-to-simplify-access-to-image-metadata

using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace MetadataLibrary.ImageInfo
{
    ///<summary>This Class retrives Image Properties using Image.GetPropertyItem() method 
    /// and gives access to some of them trough its public properties. Or to all of them
    /// trough its public property PropertyItems.
    ///</summary>
    public class Info
    {
        ///<summary>Wenn using this constructor the Image property must be set before accessing properties.</summary>
        public Info()
        {
        }

        ///<summary>Creates Info Class to read properties of an Image given from a file.</summary>
        /// <param name="imageFileName">A string specifiing image file name on a file system.</param>
        public Info(string imageFileName)
        {
            _imageFileName = imageFileName;
            _image = System.Drawing.Image.FromFile(imageFileName);
        }

        ///<summary>Creates Info Class to read properties of a given Image object.</summary>
        /// <param name="anImage">An Image object to analise.</param>
        public Info(System.Drawing.Image anImage)
        {
            _image = anImage;
        }

        System.Drawing.Image _image;
        string _imageFileName;
        ///<summary>Sets or returns the current Image object.</summary>
        public System.Drawing.Image Image
        {
            set { _image = value; }
            get { return _image; }
        }

        ///<summary>
        /// Type is PropertyTagTypeShort or PropertyTagTypeLong
        ///Information specific to compressed data. When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file.
        /// </summary>
        public uint? PixXDim
        {
            get
            {
                try
                {
                    object tmpValue = PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifPixXDim));
                    if (tmpValue.GetType().ToString().Equals("System.UInt16")) return (uint)(ushort)tmpValue;
                    return (uint)tmpValue;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifPixXDim);
            }
        }
        ///<summary>
        /// Type is PropertyTagTypeShort or PropertyTagTypeLong
        /// Information specific to compressed data. When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file. Because data padding is unnecessary in the vertical direction, the number of lines recorded in this valid image height tag will be the same as that recorded in the SOF.
        /// </summary>
        public uint? PixYDim
        {
            get
            {
                try
                {
                    object tmpValue = PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifPixYDim));
                    if (tmpValue.GetType().ToString().Equals("System.UInt16")) return (uint)(ushort)tmpValue;
                    return (uint)tmpValue;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifPixYDim);
            }
        }

        ///<summary>
        ///Number of pixels per unit in the image width (x) direction. The unit is specified by PropertyTagResolutionUnit
        ///</summary>
        public Fraction XResolution
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.XResolution));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.XResolution);
            }
        }

        ///<summary>
        ///Number of pixels per unit in the image height (y) direction. The unit is specified by PropertyTagResolutionUnit.
        ///</summary>
        public Fraction YResolution
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.YResolution));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.YResolution);
            }
        }

        ///<summary>
        ///Unit of measure for the horizontal resolution and the vertical resolution.
        ///2 - inch 3 - centimeter
        ///</summary>
        public ResolutionUnit? ResolutionUnit
        {
            get
            {
                try
                {
                    return (ResolutionUnit)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ResolutionUnit));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue((ushort)value, (int)PropertyTagId.YResolution);
            }

        }

        ///<summary>
        ///Brightness value. The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
        ///</summary>
        public Fraction Brightness
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifBrightness));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifBrightness);
            }
        }

        ///<summary>
        /// The manufacturer of the equipment used to record the image.
        ///</summary>
        public string EquipMake
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.EquipMake));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.EquipMake);
            }
        }

        ///<summary>
        /// The model name or model number of the equipment used to record the image.
        /// </summary>
        public string EquipModel
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.EquipModel));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.EquipModel);
            }
        }

        ///<summary>
        ///Copyright information.
        ///</summary>
        public string Copyright
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.Copyright));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.Copyright);
            }
        }


        ///<summary>
        ///Date and time the image was created.
        ///</summary>		
        public string DateTime
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.DateTime));
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                SetValue(value, (int)PropertyTagId.DateTime);
            }

        }

        //The format is YYYY:MM:DD HH:MM:SS with time shown in 24-hour format and the date and time separated by one blank character (0x2000). The character string length is 20 bytes including the NULL terminator. When the field is empty, it is treated as unknown.
        private static DateTime ExifDTToDateTime(string exifDT)
        {
            exifDT = exifDT.Replace(' ', ':');
            string[] ymdhms = exifDT.Split(':');
            int years = int.Parse(ymdhms[0]);
            int months = int.Parse(ymdhms[1]);
            int days = int.Parse(ymdhms[2]);
            int hours = int.Parse(ymdhms[3]);
            int minutes = int.Parse(ymdhms[4]);
            int seconds = int.Parse(ymdhms[5]);
            return new DateTime(years, months, days, hours, minutes, seconds);
        }

        ///<summary>
        ///Date and time when the original image data was generated. For a DSC, the date and time when the picture was taken. 
        ///</summary>
        public DateTime? ExifDTOrig
        {
            get
            {
                try
                {
                    string tmpStr = (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifDTOrig));
                    return ExifDTToDateTime(tmpStr);
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                SetValue(value, (int)PropertyTagId.ExifDTOrig);
            }

        }

        ///<summary>
        ///Date and time when the image was stored as digital data. If, for example, an image was captured by DSC and at the same time the file was recorded, then DateTimeOriginal and DateTimeDigitized will have the same contents.
        ///</summary>
        public DateTime? ExifDTDigitized
        {
            get
            {
                try
                {
                    string tmpStr = (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifDTDigitized));
                    return ExifDTToDateTime(tmpStr);
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                SetValue(value, (int)PropertyTagId.ExifDTDigitized);
            }
        }


        ///<summary>
        ///ISO speed and ISO latitude of the camera or input device as specified in ISO 12232.
        ///</summary>		
        public ushort? ExifISOSpeed
        {
            get
            {
                try
                {
                    return (ushort)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifISOSpeed));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifISOSpeed);
            }
        }

        ///<summary>
        ///Image orientation viewed in terms of rows and columns.
        ///</summary>				
        public Orientation? Orientation
        {
            get
            {
                try
                {
                    return (Orientation)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.Orientation));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue((ushort)value, (int)PropertyTagId.Orientation);
            }
        }

        ///<summary>
        ///Actual focal length, in millimeters, of the lens. Conversion is not made to the focal length of a 35 millimeter film camera.
        ///</summary>						
        public Fraction FocalLength
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifFocalLength));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifFocalLength);
            }
        }

        ///<summary>
        ///F number.
        ///</summary>						
        public Fraction FNumber
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifFNumber));
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                SetValue(value, (int)PropertyTagId.ExifFNumber);
            }

        }

        ///<summary>
        ///Class of the program used by the camera to set exposure when the picture is taken.
        ///</summary>						
        public ExposureProg? ExifExposureProg
        {
            get
            {
                try
                {
                    return (ExposureProg)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifExposureProg));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue((ushort)value, (int)PropertyTagId.ExifExposureProg);
            }
        }

        ///<summary>
        ///Metering mode.
        ///</summary>						
        public MeteringMode? ExifMeteringMode
        {
            get
            {
                try
                {
                    return (MeteringMode)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifMeteringMode));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue((ushort)value, (int)PropertyTagId.ExifMeteringMode);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public string ImageDescription
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ImageDescription));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ImageDescription);
            }
        }

        public string SoftwareUsed
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.SoftwareUsed));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.SoftwareUsed);
            }
        }

        public string Artist
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.Artist));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.Artist);
            }
        }

        public string ExifDTSubsec
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifDTSubsec));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifDTSubsec);
            }
        }

        public string ExifDTOrigSS
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifDTOrigSS));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifDTOrigSS);
            }
        }

        public string ExifDTDigSS
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifDTDigSS));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifDTDigSS);
            }
        }

        public string GpsLatitudeRef
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.GpsLatitudeRef));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.GpsLatitudeRef);
            }
        }

        public string GpsLongitudeRef
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.GpsLongitudeRef));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.GpsLongitudeRef);
            }
        }

        public string GpsGpsMeasureMode
        {
            get
            {
                try
                {
                    return (string)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.GpsGpsMeasureMode));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.GpsGpsMeasureMode);
            }
        }

        public long? JPEGInterFormat
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.JPEGInterFormat));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.JPEGInterFormat);
            }
        }

        public long? JPEGInterLength
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.JPEGInterLength));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.JPEGInterLength);
            }
        }

        public ushort? YCbCrPositioning
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.YCbCrPositioning));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.YCbCrPositioning);
            }
        }

        public ushort? ExifLightSource
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifLightSource));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifLightSource);
            }
        }

        public ushort? ExifFlash
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifFlash));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifFlash);
            }
        }

        public ushort? ExifColorSpace
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifColorSpace));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifColorSpace);
            }
        }

        public ushort? ExifPixXDim
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifPixXDim));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifPixXDim);
            }
        }

        public ushort? ExifPixYDim
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifPixYDim));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifPixYDim);
            }
        }

        public ushort? ExifSensingMethod
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifSensingMethod));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifSensingMethod);
            }
        }

        public ushort? ThumbnailCompression
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ThumbnailCompression));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ThumbnailCompression);
            }
        }

        public ushort? ThumbnailResolutionUnit
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ThumbnailResolutionUnit));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ThumbnailResolutionUnit);
            }
        }

        public ushort? ThumbnailYCbCrPositioning
        {
            get
            {
                try
                {
                    return (byte)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ThumbnailYCbCrPositioning));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ThumbnailYCbCrPositioning);
            }
        }

        public Fraction ExifExposureTime
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifExposureTime));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifExposureTime);
            }
        }

        public Fraction ExifFNumber
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifFNumber));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifFNumber);
            }
        }

        public Fraction ExifCompBPP
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifCompBPP));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifCompBPP);
            }
        }

        public Fraction ExifMaxAperture
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifMaxAperture));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifMaxAperture);
            }
        }

        public Fraction ExifFocalLength
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ExifFocalLength));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ExifFocalLength);
            }
        }

        public Fraction ThumbnailResolutionX
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ThumbnailResolutionX));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ThumbnailResolutionX);
            }
        }

        public Fraction ThumbnailResolutionY
        {
            get
            {
                try
                {
                    return (Fraction)PropertyTag.getValue(_image.GetPropertyItem((int)PropertyTagId.ThumbnailResolutionY));
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                SetValue(value, (int)PropertyTagId.ThumbnailResolutionY);
            }
        }

        private Hashtable _propertyItems;
        ///<summary>
        /// Returns a Hashtable of all available Properties of a gieven Image. Keys of this Hashtable are
        /// Display names of the Property Tags and values are transformed (typed) data.
        ///</summary>
        /// <example>
        /// <code>
        /// if (openFileDialog.ShowDialog()==DialogResult.OK)
        ///	{
        ///		Info inf=new Info(Image.FromFile(openFileDialog.FileName));
        ///		listView.Items.Clear();
        ///		foreach (string propertyname in inf.PropertyItems.Keys)
        ///		{
        ///			ListViewItem item1 = new ListViewItem(propertyname,0);
        ///		    item1.SubItems.Add((inf.PropertyItems[propertyname]).ToString());
        ///			listView.Items.Add(item1);
        ///		}
        ///	}
        /// </code>
        ///</example>
        public Hashtable PropertyItems
        {
            get
            {
                if (_propertyItems == null)
                {
                    _propertyItems = new Hashtable();
                    foreach (int id in _image.PropertyIdList)
                        _propertyItems[((PropertyTagId)id).ToString()] = PropertyTag.getValue(_image.GetPropertyItem(id));

                }
                return _propertyItems;
            }
        }


        /// <summary>
        /// Устанавливает значения тега
        /// </summary>
        /// <param name="value">Строковое значение тега</param>
        /// <param name="id">ID тега</param>
        public void SetValue(string value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Id = id;
                pi.Type = PropertyTag.GetTypeFromId(id);
            }

            //устанавливаем значение
            byte[] val = Encoding.ASCII.GetBytes(value);
            pi.Value = new byte[val.Length + 1];
            pi.Len = val.Length + 1;
            Array.Copy(val, pi.Value, val.Length);
            _image.SetPropertyItem(pi);//запись в image
            //GDI+ это странная библиотека
            //_image.Save(_imageFileName+".tmp");
            //_image.Dispose();
            //File.Delete(_imageFileName);
            //File.Move(_imageFileName + ".tmp", _imageFileName);               
        }

        public void SetValue(ushort? value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Type = PropertyTag.GetTypeFromId(id);
            }


            //переделаем их ushort? в ushort
            ushort myvalue = (ushort)(value != null ? value : 0);
            //получаемм массив ьайт
            var res = BitConverter.GetBytes(myvalue);

            pi.Len = res.Length;
            pi.Value = res;
            _image.SetPropertyItem(pi);//запись в image
        }

        public void SetValue(DateTime? value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Type = PropertyTag.GetTypeFromId(id);
            }


            ////переделаем их ushort? в ushort

            DateTime myvalue = (DateTime)(value != null ? value : System.DateTime.Now);
            //"2016:02:17 15:22:33" - как должно быть
            string res = myvalue.Year + ":" + addSpace(myvalue.Month) + ":" + addSpace(myvalue.Day) + " " + addSpace(myvalue.Hour) + ":" + addSpace(myvalue.Minute) + ":" + addSpace(myvalue.Second);

            SetValue(res, id);
            ////получаемм массив ьайт
            //var res = BitConverter.GetBytes(myvalue);

            //pi.Len = res.Length;
            //pi.Value = res;
            //_image.SetPropertyItem(pi);//запись в image
        }

        public void SetValue(uint? value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Type = PropertyTag.GetTypeFromId(id);
            }


            //переделаем их ushort? в ushort
            uint myvalue = (uint)(value != null ? value : 0);
            //получаемм массив ьайт
            var res = BitConverter.GetBytes(myvalue);

            pi.Len = res.Length;
            pi.Value = res;
            _image.SetPropertyItem(pi);//запись в image
        }

        public void SetValue(long? value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Type = PropertyTag.GetTypeFromId(id);
            }


            //переделаем их ushort? в ushort
            long myvalue = (long)(value != null ? value : 0);
            //получаемм массив ьайт
            var res = BitConverter.GetBytes(myvalue);

            pi.Len = res.Length;
            pi.Value = res;
            _image.SetPropertyItem(pi);//запись в image
        }

        public void SetValue(Fraction value, int id)
        {
            PropertyItem pi; //получаем элемент
            try
            {
                pi = _image.GetPropertyItem(id); //элемент либо есть
            }
            catch
            {
                pi = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem)); //либо его нет и мы его создаем
                pi.Type = PropertyTag.GetTypeFromId(id);
            }



            //получаемм массив ьайт
            var num = BitConverter.GetBytes(value.Numerator);
            var den = BitConverter.GetBytes(value.Denumerator);

            int count = Math.Max(num.Length, den.Length);
            count = ((count % 4) + 1) * 4;

            pi.Len = count * 2;
            byte[] res = new byte[pi.Len];
            Array.Copy(num, 0, res, 0, count);
            Array.Copy(den, 0, res, count, count);
            pi.Value = res;

            _image.SetPropertyItem(pi);//запись в image
        }

        private string addSpace(int dat)
        {
            var date = dat.ToString();
            if (date.Length == 1)
                return "0" + date;
            return date;
        }

    }
}
