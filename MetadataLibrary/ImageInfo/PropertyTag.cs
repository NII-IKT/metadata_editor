//http://www.codeproject.com/Articles/7888/A-library-to-simplify-access-to-image-metadata

using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;

namespace MetadataLibrary.ImageInfo
{
    ///<summary>This class is designed to transform from PropertyItem received by calling 
    /// Image.GetPropertyItem() method. It transforms also the data that are stored in PropertyItem.Value byte array to ordinary .NET types.
    ///</summary>
    public class PropertyTag
    {
        PropertyItem _prop;

        ///<summary></summary>
        /// <param name="aPropertyItem"></param>
        public PropertyTag(PropertyItem aPropertyItem)
        {
            _prop = aPropertyItem;
        }

        ///<summary>Specifies the data type of the values stored in the value of that PropertyItem object. Note that this are not ordinary .NET types.</summary>
        public PropertyTagType Type
        {
            get { return (PropertyTagType)_prop.Type; }
            //set {_prop.Type = (short)value;}
        }

        ///<summary>Id of a Property Tag</summary>
        public PropertyTagId Id
        {
            get { return (PropertyTagId)_prop.Id; }
            //set {_prop.Id =(int)value;}
        }

        ///<summary>Display Name of a Property Tag</summary>
        public string TagName
        {
            get { return getNameFromId(Id); }
        }

        ///<summary>Transformed value of the PropertyItem.Value byte array.</summary>
        public Object Value
        {
            get { return getValue(_prop); }
        }

        private static System.Text.ASCIIEncoding _encoding = new System.Text.ASCIIEncoding();
        ///<summary>Transformes value of the PropertyItem.Value byte array to apropriate .NET Framework type.</summary>
        /// <param name="aPropertyItem"></param>
        public static Object getValue(PropertyItem aPropertyItem)
        {
            if (aPropertyItem == null) return null;
            switch ((PropertyTagType)aPropertyItem.Type)
            {

                case PropertyTagType.Byte:
                    if (aPropertyItem.Value.Length == 1) return aPropertyItem.Value[0];
                    return aPropertyItem.Value;

                case PropertyTagType.ASCII:
                    return _encoding.GetString(aPropertyItem.Value, 0, aPropertyItem.Len - 1);

                case PropertyTagType.Short:
                    ushort[] _resultUShort = new ushort[aPropertyItem.Len / (16 / 8)];
                    for (int i = 0; i < _resultUShort.Length; i++)
                        _resultUShort[i] = BitConverter.ToUInt16(aPropertyItem.Value, i * (16 / 8));
                    if (_resultUShort.Length == 1) return _resultUShort[0];
                    return (_resultUShort);

                case PropertyTagType.Long:
                    uint[] _resultUInt32 = new uint[aPropertyItem.Len / (32 / 8)];
                    for (int i = 0; i < _resultUInt32.Length; i++)
                        _resultUInt32[i] = BitConverter.ToUInt32(aPropertyItem.Value, i * (32 / 8));
                    if (_resultUInt32.Length == 1) return _resultUInt32[0];
                    return _resultUInt32;

                case PropertyTagType.Rational:
                    Fraction[] _resultRational = new Fraction[aPropertyItem.Len / (64 / 8)];
                    uint uNominator;
                    uint uDenominator;
                    for (int i = 0; i < _resultRational.Length; i++)
                    {
                        uNominator = 1;
                        uNominator = BitConverter.ToUInt32(aPropertyItem.Value, i * (64 / 8));
                        uDenominator = BitConverter.ToUInt32(aPropertyItem.Value, (i * (64 / 8)) + (32 / 8));
                        _resultRational[i] = new Fraction(uNominator, uDenominator);
                        if (_resultRational[i] == null) MessageBox.Show("Null");
                    }
                    if (_resultRational.Length == 1) return _resultRational[0];
                    return _resultRational;

                case PropertyTagType.Undefined:
                    if (aPropertyItem.Value.Length == 1) return aPropertyItem.Value[0];
                    return aPropertyItem.Value;

                case PropertyTagType.SLONG:
                    int[] _resultInt32 = new int[aPropertyItem.Len / (32 / 8)];
                    for (int i = 0; i < _resultInt32.Length; i++)
                        _resultInt32[i] = BitConverter.ToInt32(aPropertyItem.Value, i * (32 / 8));
                    if (_resultInt32.Length == 1) return _resultInt32[0];
                    return _resultInt32;

                case PropertyTagType.SRational:
                    Fraction[] _resultSRational = new Fraction[aPropertyItem.Len / (64 / 8)];
                    int sNominator;
                    int sDenominator;
                    for (int i = 0; i < _resultSRational.Length; i++)
                    {
                        sNominator = BitConverter.ToInt32(aPropertyItem.Value, i * (64 / 8));
                        sDenominator = BitConverter.ToInt32(aPropertyItem.Value, i * (64 / 8) + (32 / 8));
                        _resultSRational[i] = new Fraction(sNominator, sDenominator);
                    }
                    if (_resultSRational.Length == 1) return _resultSRational[0];
                    return _resultSRational;

                default:
                    if (aPropertyItem.Value.Length == 1) return aPropertyItem.Value[0];
                    return aPropertyItem.Value;
            }
        }

        ///<summary>Returns the Display Name of a Property Item. The current imlementation will return a name of the Enumeration member.</summary>
        ///<param name="aId">PropertyId to get description for.</param>
        private string getNameFromId(PropertyTagId aId)
        {
            return aId.ToString();
        }

        //выдаем тип по id
        public static short GetTypeFromId(int id)
        {
            switch (id)
            {
                //string
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ImageDescription: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.EquipMake: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.EquipModel: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.SoftwareUsed: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.DateTime: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.Artist: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.Copyright: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTOrig: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTDigitized: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTSubsec: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTOrigSS: return 2;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTDigSS: return 2;
                //short
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.Orientation: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ResolutionUnit: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.YCbCrPositioning: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifExposureProg: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifISOSpeed: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifMeteringMode: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifLightSource: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifFlash: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifColorSpace: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifPixXDim: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifPixYDim: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifSensingMethod: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailCompression: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailResolutionUnit: return 3;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailYCbCrPositioning: return 3;
                //long
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.JPEGInterFormat: return 4;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.JPEGInterLength: return 4;
                //rational
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.XResolution: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.YResolution: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifExposureTime: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifFNumber: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifCompBPP: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifMaxAperture: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ExifFocalLength: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailResolutionX: return 5;
                case (int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailResolutionY: return 5;


            }
            return 0;
        }
    }
}