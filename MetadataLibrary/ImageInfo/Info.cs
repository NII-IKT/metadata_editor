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
            _image = System.Drawing.Image.FromFile(imageFileName);
        }

        ///<summary>Creates Info Class to read properties of a given Image object.</summary>
        /// <param name="anImage">An Image object to analise.</param>
        public Info(System.Drawing.Image anImage)
        {
            _image = anImage;
        }

        System.Drawing.Image _image;
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
        public DateTime? DTOrig
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
        }

        ///<summary>
        ///Date and time when the image was stored as digital data. If, for example, an image was captured by DSC and at the same time the file was recorded, then DateTimeOriginal and DateTimeDigitized will have the same contents.
        ///</summary>
        public DateTime? DTDigitized
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
        }


        ///<summary>
        ///ISO speed and ISO latitude of the camera or input device as specified in ISO 12232.
        ///</summary>		
        public ushort? ISOSpeed
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
            //TODO реализоовать
            //set
            //{
            //    SetValue(value, (int)PropertyTagId.ExifISOSpeed);
            //}
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
        }

        ///<summary>
        ///Class of the program used by the camera to set exposure when the picture is taken.
        ///</summary>						
        public ExposureProg? ExposureProg
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
        }

        ///<summary>
        ///Metering mode.
        ///</summary>						
        public MeteringMode? MeteringMode
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
                pi.Type = PropertyTag.GetTypeFromId(id);
            }
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
           
            char[] str= value.ToString().ToCharArray();
            byte[] val = Encoding.ASCII.GetBytes(str);
            //encoding.GetString(val, 0, val.Length);

            pi.Value = val;
            pi.Len = pi.Value.Length + 1;
            _image.SetPropertyItem(pi);//запись в image
        }

        public void SetValue(short value, int id)
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
               //case PropertyTagType.Short:
               //     ushort[] _resultUShort = new ushort[aPropertyItem.Len / (16 / 8)];
               //     for (int i = 0; i < _resultUShort.Length; i++)
               //         _resultUShort[i] = BitConverter.ToUInt16(aPropertyItem.Value, i * (16 / 8));
               //     if (_resultUShort.Length == 1) return _resultUShort[0];
               //     return (_resultUShort);

            byte[] val= BitConverter.GetBytes(value);

            pi.Value = val;
            pi.Len = pi.Value.Length + 1;
            _image.SetPropertyItem(pi);//запись в image
        }
    }
}
