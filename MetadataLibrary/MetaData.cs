using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using MetadataLibrary.ImageInfo;

namespace MetadataLibrary
{
    /// <summary>
    /// Класс для работы с метаданными картинок
    /// </summary>
    public class MetaData
    {
        /*
         * Представить все в удобочитаемом виде 
        for (int type = 0; type < 11; type++)
            {

                richTextBox1.AppendText("type = " + type + " =" + (PropertyTagType)type + "\n");
                foreach (var p in bitmap.PropertyItems)
                {
                    if (p.Type == type)
                    {
                        string s = "";
                        s = BitConverter.ToString(BitConverter.GetBytes(Double.Parse(p.Id.ToString()))).Replace("-", "");
                        richTextBox1.AppendText(p.Id + " = " + s + " = " + (MetadataLibrary.ImageInfo.PropertyTagId)p.Id + " = " + PropertyTag.getValue(p) + "\n");
                    }
                }

                richTextBox1.AppendText("======================\n\n");
            }
         * 
         */


        /// <summary>
        /// Метод который удаляет все метаданные из файла картинки
        /// </summary>
        /// <param name="pathFile">Путь до файла</param>
        /// <returns>Истина - удаление прошло успешно, ЛОЖЬ - что то пошло не так</returns>
        public bool Remove(string pathFile) //TODO в этом коде нет try cath, нужно потестить
        {

            Image sourceImg = new Bitmap(pathFile);
            Image img = (Image)sourceImg.Clone();
            sourceImg.Dispose();

            PropertyItem[] propertyItemsList = img.PropertyItems;
            //к сожалниею PropertyItems доступен только для чтения а операции PropertyItems.Clear() нет, поэтому приходится удалять вручную, все так делают
            foreach (PropertyItem property in propertyItemsList)
            {
                img.RemovePropertyItem(property.Id);
            }

            string mime = GetMimeType(pathFile);
            ImageCodecInfo Encoder = GetEncoderInfo(mime);
            EncoderParameters EncoderParams = new EncoderParameters(2);
            EncoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (Int64)EncoderValue.CompressionNone);//TODO решить со сжатием
            EncoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            //string newfile = openFileDialog1.FileName.Replace(openFileDialog1.SafeFileName, "") + "Clear_" + openFileDialog1.SafeFileName;
            //if (System.IO.File.Exists(newfile))
            //{
            //    System.IO.File.Delete(newfile);
            //}

            //для удаоние файла нужно удалить img а в енм хранится картинка

            //HACK болт, но иначе не работает нашел здесь: http://stackoverflow.com/questions/19460149/c-sharp-remove-property-tag-items-from-tif-file 
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);

            img.Save(pathFile + ".tmp", Encoder, EncoderParams);
            img.Dispose();

            //к сожалению, после освобождения объекта sourceImg через Dispose файл pathFile нельзя удалить.
            //Операция sourceImg.Clone не делает клона, а передает ссылку на файл
            //файл pathFile можно удалить только после того как img высвободить черещ Dispose
            //Но тогда нечего будет сохранять
            //тупое решение - записать в другой файл, потом удалить оригинал и переименовать.

            File.Delete(pathFile);
            File.Move(pathFile + ".tmp", pathFile);

            return true;
        }

        /// <summary>
        /// Метод который удаляет все метаданные из файла картинки
        /// </summary>
        /// <param name="metadataPathFile">Путь до файла хранящего метаданные</param>
        /// <param name="imagePathFile">Путь до файла хранящего изображения</param>
        /// <returns>Истина - копирование прошло успешно, ЛОЖЬ - что то пошло не так</returns>
        public bool Copy(string metadataPathFile, string imagePathFile)
        {

            Image sourceImg = new Bitmap(metadataPathFile);
            Image img = new Bitmap(imagePathFile);

            //очищаем изображение от мета
            PropertyItem[] propertyItemsList = img.PropertyItems;
            //к сожалниею PropertyItems доступен только для чтения а операции PropertyItems.Clear() нет, поэтому приходится удалять вручную, все так делают
            foreach (PropertyItem property in propertyItemsList)
            {
                img.RemovePropertyItem(property.Id);
            }

            //заполняем метаданными от старого рисунка
            propertyItemsList = sourceImg.PropertyItems;
            foreach (PropertyItem property in propertyItemsList)
            {
                img.SetPropertyItem(property);
            }

            string mime = GetMimeType(imagePathFile);
            ImageCodecInfo Encoder = GetEncoderInfo(mime);
            EncoderParameters EncoderParams = new EncoderParameters(2);
            EncoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (Int64)EncoderValue.CompressionNone);//TODO решить со сжатием
            EncoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            //string newfile = openFileDialog2.FileName.Replace(openFileDialog2.SafeFileName, "") + "New_" + openFileDialog2.SafeFileName;
            //if (System.IO.File.Exists(newfile))
            //{
            //    System.IO.File.Delete(newfile);
            //}

            //TODO болт, но иначе не работает нашел здесь: http://stackoverflow.com/questions/19460149/c-sharp-remove-property-tag-items-from-tif-file 
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);
            img.RotateFlip(RotateFlipType.Rotate180FlipNone);

            img.Save(imagePathFile + ".tmp", Encoder, EncoderParams);

            EncoderParams.Dispose();
            img.Dispose();
            sourceImg.Dispose();

            File.Delete(imagePathFile);
            File.Move(imagePathFile + ".tmp", imagePathFile);

            return true;
        }

        public Hashtable Read(string pathFile)//TODO нехороший метод
        {
            //MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);
            // info.Image.Dispose();
            // return info.PropertyItems; //HACK вот здесб жопа
            return new Hashtable();
        }

        /// <summary>
        /// Читает все свойства что есть
        /// </summary>
        /// <param name="pathFile"></param>
        /// <returns></returns>
        public Dictionary<string, string> ReadAll2(string pathFile)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);

            foreach (var p in info.Image.PropertyItems)
            {
                res.Add((MetadataLibrary.ImageInfo.PropertyTagId)p.Id + "", PropertyTag.getValue(p).ToString());
                //richTextBox1.AppendText((MetadataLibrary.ImageInfo.PropertyTagId)p.Id + " = " + PropertyTag.getValue(p) + "\n");
            }
            return res;
        }

        //читает нужные свойства
        public Dictionary<string, string> ReadAll(string pathFile)//TODO нехороший метод
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);

            //выводим тут все свойства для удобства
            res.Add("Artist", info.Artist != null ? info.Artist : "n/a");
            res.Add("Brightness", info.Brightness != null ? info.Brightness.ToString() : "n/a");
            res.Add("Copyright", info.Copyright != null ? info.Copyright : "n/a");
            res.Add("DateTime", info.DateTime != null ? info.DateTime : "n/a");
            res.Add("EquipMake", info.EquipMake != null ? info.EquipMake : "n/a");
            res.Add("EquipModel", info.EquipModel != null ? info.EquipModel : "n/a");
            res.Add("ExifColorSpace", info.ExifColorSpace != null ? info.ExifColorSpace.ToString() : "n/a");
            res.Add("ExifCompBPP", info.ExifCompBPP != null ? info.ExifCompBPP.ToString() : "n/a");
            res.Add("ExifDTDigitized", info.ExifDTDigitized != null ? info.ExifDTDigitized.ToString() : "n/a");
            res.Add("ExifDTDigSS", info.ExifDTDigSS != null ? info.ExifDTDigSS : "n/a");
            res.Add("ExifDTOrig", info.ExifDTOrig != null ? info.ExifDTOrig.ToString() : "n/a");
            res.Add("ExifDTOrigSS", info.ExifDTOrigSS != null ? info.ExifDTOrigSS : "n/a");
            res.Add("ExifDTSubsec", info.ExifDTSubsec != null ? info.ExifDTSubsec : "n/a");
            res.Add("ExifExposureProg", info.ExifExposureProg != null ? info.ExifExposureProg.ToString() : "n/a");
            res.Add("ExifExposureTime", info.ExifExposureTime != null ? info.ExifExposureTime.ToString() : "n/a");
            res.Add("ExifFlash", info.ExifFlash != null ? info.ExifFlash.ToString() : "n/a");
            res.Add("ExifFNumber", info.ExifFNumber != null ? info.ExifFNumber.ToString() : "n/a");
            res.Add("ExifFocalLength", info.ExifFocalLength != null ? info.ExifFocalLength.ToString() : "n/a");
            res.Add("ExifISOSpeed", info.ExifISOSpeed != null ? info.ExifISOSpeed.ToString() : "n/a");
            res.Add("ExifLightSource", info.ExifLightSource != null ? info.ExifLightSource.ToString() : "n/a");
            res.Add("ExifMaxAperture", info.ExifMaxAperture != null ? info.ExifMaxAperture.ToString() : "n/a");
            res.Add("ExifMeteringMode", info.ExifMeteringMode != null ? info.ExifMeteringMode.ToString() : "n/a");
            res.Add("ExifPixXDim", info.ExifPixXDim != null ? info.ExifPixXDim.ToString() : "n/a");
            res.Add("ExifPixYDim", info.ExifPixYDim != null ? info.ExifPixYDim.ToString() : "n/a");
            res.Add("ExifSensingMethod", info.ExifSensingMethod != null ? info.ExifSensingMethod.ToString() : "n/a");
            res.Add("FNumber", info.FNumber != null ? info.FNumber.ToString() : "n/a");
            res.Add("FocalLength", info.FocalLength != null ? info.FocalLength.ToString() : "n/a");
            res.Add("GpsGpsMeasureMode", info.GpsGpsMeasureMode != null ? info.GpsGpsMeasureMode : "n/a");
            res.Add("GpsLatitudeRef", info.GpsLatitudeRef != null ? info.GpsLatitudeRef : "n/a");
            res.Add("GpsLongitudeRef", info.GpsLongitudeRef != null ? info.GpsLongitudeRef : "n/a");
            res.Add("ImageDescription", info.ImageDescription != null ? info.ImageDescription : "n/a");
            res.Add("JPEGInterFormat", info.JPEGInterFormat != null ? info.JPEGInterFormat.ToString() : "n/a");
            res.Add("JPEGInterLength", info.JPEGInterLength != null ? info.JPEGInterLength.ToString() : "n/a");
            res.Add("Orientation", info.Orientation != null ? info.Orientation.ToString() : "n/a");
            res.Add("PixXDim", info.PixXDim != null ? info.PixXDim.ToString() : "n/a");
            res.Add("PixYDim", info.PixYDim != null ? info.PixYDim.ToString() : "n/a");
            res.Add("ResolutionUnit", info.ResolutionUnit != null ? info.ResolutionUnit.ToString() : "n/a");
            res.Add("SoftwareUsed", info.SoftwareUsed != null ? info.SoftwareUsed : "n/a");
            res.Add("ThumbnailCompression", info.ThumbnailCompression != null ? info.ThumbnailCompression.ToString() : "n/a");
            res.Add("ThumbnailResolutionUnit", info.ThumbnailResolutionUnit != null ? info.ThumbnailResolutionUnit.ToString() : "n/a");
            res.Add("ThumbnailResolutionX", info.ThumbnailResolutionX != null ? info.ThumbnailResolutionX.ToString() : "n/a");
            res.Add("ThumbnailResolutionY", info.ThumbnailResolutionY != null ? info.ThumbnailResolutionY.ToString() : "n/a");
            res.Add("ThumbnailYCbCrPositioning", info.ThumbnailYCbCrPositioning != null ? info.ThumbnailYCbCrPositioning.ToString() : "n/a");
            res.Add("XResolution", info.XResolution != null ? info.XResolution.ToString() : "n/a");
            res.Add("YCbCrPositioning", info.YCbCrPositioning != null ? info.YCbCrPositioning.ToString() : "n/a");
            res.Add("YResolution", info.YResolution != null ? info.YResolution.ToString() : "n/a");

            /*
            res.Add("Brightness", info.Brightness != null ? info.Brightness.ToString() : "n/a");
            res.Add("Copyright", info.Copyright != null ? info.Copyright : "n/a");
            res.Add("DateTime", info.DateTime != null ? info.DateTime : "n/a");
            res.Add("ExifDTDigitized", info.ExifDTDigitized != null ? info.ExifDTDigitized.ToString() : "n/a");
            res.Add("ExifDTOrig", info.ExifDTOrig != null ? info.ExifDTOrig.ToString() : "n/a");
            res.Add("EquipMake", info.EquipMake != null ? info.EquipMake : "n/a");
            res.Add("EquipModel", info.EquipModel != null ? info.EquipModel : "n/a");
            res.Add("ExifDTDigSS", info.ExifDTDigSS != null ? info.ExifDTDigSS : "n/a");
            res.Add("ExifDTOrigSS", info.ExifDTOrigSS != null ? info.ExifDTOrigSS : "n/a");
            res.Add("ExifDTSubsec", info.ExifDTSubsec != null ? info.ExifDTSubsec : "n/a");
            res.Add("ExifExposureProg", info.ExifExposureProg != null ? info.ExifExposureProg.ToString() : "n/a");
            res.Add("FNumber", info.FNumber != null ? info.FNumber.ToString() : "n/a");
            res.Add("FocalLength", info.FocalLength != null ? info.FocalLength.ToString() : "n/a");
            res.Add("GpsGpsMeasureMode", info.GpsGpsMeasureMode != null ? info.GpsGpsMeasureMode : "n/a");
            res.Add("GpsLatitudeRef", info.GpsLatitudeRef != null ? info.GpsLatitudeRef : "n/a");
            res.Add("GpsLongitudeRef", info.GpsLongitudeRef != null ? info.GpsLongitudeRef : "n/a");
            res.Add("ImageDescription", info.ImageDescription != null ? info.ImageDescription : "n/a");
            res.Add("ISOSpeed", info.ExifISOSpeed != null ? info.ExifISOSpeed.ToString() : "n/a");
            res.Add("JPEGInterFormat", info.JPEGInterFormat != null ? info.JPEGInterFormat.ToString() : "n/a");
            res.Add("ExifMeteringMode", info.ExifMeteringMode != null ? info.ExifMeteringMode.ToString() : "n/a");
            res.Add("Orientation", info.Orientation != null ? info.Orientation.ToString() : "n/a");
            res.Add("PixXDim", info.PixXDim != null ? info.PixXDim.ToString() : "n/a");
            res.Add("PixYDim", info.PixYDim != null ? info.PixYDim.ToString() : "n/a");
            res.Add("ResolutionUnit", info.ResolutionUnit != null ? info.ResolutionUnit.ToString() : "n/a");
            res.Add("SoftwareUsed", info.SoftwareUsed != null ? info.SoftwareUsed : "n/a");
            res.Add("XResolution", info.XResolution != null ? info.XResolution.ToString() : "n/a");
            res.Add("YResolution", info.YResolution != null ? info.YResolution.ToString() : "n/a");
            */

            //res.Add("PixXDim", info.PixXDim == 1123456789 ? "n/a" : info.PixXDim.ToString());
            //res.Add("PixYDim", info.PixYDim == 1123456789? "n/a" :info.PixYDim.ToString());
            //res.Add("EquipModel", String.Copy(info.EquipModel));
            //res.Add("EquipMake", String.Copy(info.EquipMake));
            //res.Add("Copyright", String.Copy(info.Copyright));
            //res.Add("DateTime", String.Copy(info.DateTime));
            //res.Add("ISOSpeed", info.ISOSpeed == 12345 ? "n/a" : info.ISOSpeed.ToString());
            info.Image.Dispose();
            return res;
        }

        public Dictionary<string, string> ReadAllEdit(string pathFile)//TODO нехороший метод
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);

            //выводим тут все свойства для удобства
            res.Add("Artist", info.Artist != null ? info.Artist : "n/a");
            //res.Add("Brightness", info.Brightness != null ? info.Brightness.ToString() : "n/a");
            res.Add("Copyright", info.Copyright != null ? info.Copyright : "n/a");
            res.Add("DateTime", info.DateTime != null ? info.DateTime : "n/a");
            res.Add("EquipMake", info.EquipMake != null ? info.EquipMake : "n/a");
            res.Add("EquipModel", info.EquipModel != null ? info.EquipModel : "n/a");
            res.Add("ExifColorSpace", info.ExifColorSpace != null ? info.ExifColorSpace.ToString() : "n/a");
            //res.Add("ExifCompBPP", info.ExifCompBPP != null ? info.ExifCompBPP.ToString() : "n/a");
            res.Add("ExifDTDigitized", info.ExifDTDigitized != null ? info.ExifDTDigitized.ToString() : "n/a");
            res.Add("ExifDTDigSS", info.ExifDTDigSS != null ? info.ExifDTDigSS : "n/a");
            res.Add("ExifDTOrig", info.ExifDTOrig != null ? info.ExifDTOrig.ToString() : "n/a");
            res.Add("ExifDTOrigSS", info.ExifDTOrigSS != null ? info.ExifDTOrigSS : "n/a");
            res.Add("ExifDTSubsec", info.ExifDTSubsec != null ? info.ExifDTSubsec : "n/a");
            res.Add("ExifExposureProg", info.ExifExposureProg != null ? info.ExifExposureProg.ToString() : "n/a");
            //res.Add("ExifExposureTime", info.ExifExposureTime != null ? info.ExifExposureTime.ToString() : "n/a");
            res.Add("ExifFlash", info.ExifFlash != null ? info.ExifFlash.ToString() : "n/a");
            //res.Add("ExifFNumber", info.ExifFNumber != null ? info.ExifFNumber.ToString() : "n/a");
            //res.Add("ExifFocalLength", info.ExifFocalLength != null ? info.ExifFocalLength.ToString() : "n/a");
            res.Add("ExifISOSpeed", info.ExifISOSpeed != null ? info.ExifISOSpeed.ToString() : "n/a");
            res.Add("ExifLightSource", info.ExifLightSource != null ? info.ExifLightSource.ToString() : "n/a");
            //res.Add("ExifMaxAperture", info.ExifMaxAperture != null ? info.ExifMaxAperture.ToString() : "n/a");
            res.Add("ExifMeteringMode", info.ExifMeteringMode != null ? info.ExifMeteringMode.ToString() : "n/a");
            res.Add("ExifPixXDim", info.ExifPixXDim != null ? info.ExifPixXDim.ToString() : "n/a");
            res.Add("ExifPixYDim", info.ExifPixYDim != null ? info.ExifPixYDim.ToString() : "n/a");
            res.Add("ExifSensingMethod", info.ExifSensingMethod != null ? info.ExifSensingMethod.ToString() : "n/a");
            //res.Add("FNumber", info.FNumber != null ? info.FNumber.ToString() : "n/a");
            //res.Add("FocalLength", info.FocalLength != null ? info.FocalLength.ToString() : "n/a");
            //res.Add("GpsGpsMeasureMode", info.GpsGpsMeasureMode != null ? info.GpsGpsMeasureMode : "n/a");
            //res.Add("GpsLatitudeRef", info.GpsLatitudeRef != null ? info.GpsLatitudeRef : "n/a");
            //res.Add("GpsLongitudeRef", info.GpsLongitudeRef != null ? info.GpsLongitudeRef : "n/a");
            res.Add("ImageDescription", info.ImageDescription != null ? info.ImageDescription : "n/a");
            res.Add("JPEGInterFormat", info.JPEGInterFormat != null ? info.JPEGInterFormat.ToString() : "n/a");
            res.Add("JPEGInterLength", info.JPEGInterLength != null ? info.JPEGInterLength.ToString() : "n/a");
            res.Add("Orientation", info.Orientation != null ? info.Orientation.ToString() : "n/a");
            //res.Add("PixXDim", info.PixXDim != null ? info.PixXDim.ToString() : "n/a");
            //res.Add("PixYDim", info.PixYDim != null ? info.PixYDim.ToString() : "n/a");
            res.Add("ResolutionUnit", info.ResolutionUnit != null ? info.ResolutionUnit.ToString() : "n/a");
            res.Add("SoftwareUsed", info.SoftwareUsed != null ? info.SoftwareUsed : "n/a");
            res.Add("ThumbnailCompression", info.ThumbnailCompression != null ? info.ThumbnailCompression.ToString() : "n/a");
            res.Add("ThumbnailResolutionUnit", info.ThumbnailResolutionUnit != null ? info.ThumbnailResolutionUnit.ToString() : "n/a");
            //res.Add("ThumbnailResolutionX", info.ThumbnailResolutionX != null ? info.ThumbnailResolutionX.ToString() : "n/a");
            //res.Add("ThumbnailResolutionY", info.ThumbnailResolutionY != null ? info.ThumbnailResolutionY.ToString() : "n/a");
            res.Add("ThumbnailYCbCrPositioning", info.ThumbnailYCbCrPositioning != null ? info.ThumbnailYCbCrPositioning.ToString() : "n/a");
            //res.Add("XResolution", info.XResolution != null ? info.XResolution.ToString() : "n/a");
            res.Add("YCbCrPositioning", info.YCbCrPositioning != null ? info.YCbCrPositioning.ToString() : "n/a");
            //res.Add("YResolution", info.YResolution != null ? info.YResolution.ToString() : "n/a");

            info.Image.Dispose();
            return res;
        }

        public void WriteAll(string pathFile, Dictionary<string, string> Properties)
        {
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);

            #region stringtype
            if (Properties["ImageDescription"] == "n/a" || Properties["ImageDescription"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ImageDescription); }
                catch { }
            }
            else
                info.ImageDescription = Properties["ImageDescription"];

            if (Properties["EquipMake"] == "n/a" || Properties["EquipMake"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.EquipMake); }
                catch { }
            }
            else
                info.EquipMake = Properties["EquipMake"];

            if (Properties["EquipModel"] == "n/a" || Properties["EquipModel"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.EquipModel); }
                catch { }
            }
            else
                info.EquipModel = Properties["EquipModel"];

            if (Properties["SoftwareUsed"] == "n/a" || Properties["SoftwareUsed"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.SoftwareUsed); }
                catch { }
            }
            else
                info.SoftwareUsed = Properties["SoftwareUsed"];

            if (Properties["DateTime"] == "n/a" || Properties["DateTime"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.DateTime); }
                catch { }
            }
            else
                info.DateTime = Properties["DateTime"];

            if (Properties["Artist"] == "n/a" || Properties["Artist"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.Artist); }
                catch { }
            }
            else
                info.Artist = Properties["Artist"];

            if (Properties["Copyright"] == "n/a" || Properties["Copyright"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.Copyright); }
                catch { }
            }
            else
                info.Copyright = Properties["Copyright"];

            if (Properties["ExifDTOrig"] == "n/a" || Properties["ExifDTOrig"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTOrig); }
                catch { }
            }
            else
                try
                { info.ExifDTOrig = DateTime.Parse(Properties["ExifDTOrig"]); }
                catch { }

            if (Properties["ExifDTDigitized"] == "n/a" || Properties["ExifDTDigitized"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTDigitized); }
                catch { }
            }
            else
                try
                { info.ExifDTDigitized = DateTime.Parse(Properties["ExifDTDigitized"]); }
                catch { }

            if (Properties["ExifDTSubsec"] == "n/a" || Properties["ExifDTSubsec"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTSubsec); }
                catch { }
            }
            else
                info.ExifDTSubsec = Properties["ExifDTSubsec"];

            if (Properties["ExifDTOrigSS"] == "n/a" || Properties["ExifDTOrigSS"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTOrigSS); }
                catch { }
            }
            else
                info.ExifDTOrigSS = Properties["ExifDTOrigSS"];

            if (Properties["ExifDTDigSS"] == "n/a" || Properties["ExifDTDigSS"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifDTDigSS); }
                catch { }
            }
            else
                info.ExifDTDigSS = Properties["ExifDTDigSS"];

            #endregion

            #region short

            if (Properties["Orientation"] == "n/a" || Properties["Orientation"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.Orientation); }
                catch { }
            }
            else
            {
                try
                { info.Orientation = (MetadataLibrary.ImageInfo.Orientation)short.Parse(Properties["Orientation"]); }
                catch
                { }
            }

            if (Properties["ResolutionUnit"] == "n/a" || Properties["ResolutionUnit"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ResolutionUnit); }
                catch { }
            }
            else
            {
                try
                { info.ResolutionUnit = (MetadataLibrary.ImageInfo.ResolutionUnit)short.Parse(Properties["ResolutionUnit"]); }
                catch
                { }
            }

            if (Properties["YCbCrPositioning"] == "n/a" || Properties["YCbCrPositioning"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.YCbCrPositioning); }
                catch { }
            }
            else
            {
                try
                { info.YCbCrPositioning = ushort.Parse(Properties["YCbCrPositioning"]); }
                catch
                { }
            }

            if (Properties["ExifExposureProg"] == "n/a" || Properties["ExifExposureProg"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifExposureProg); }
                catch { }
            }
            else
            {
                try
                { info.ExifExposureProg = (MetadataLibrary.ImageInfo.ExposureProg)short.Parse(Properties["ExifExposureProg"]); }
                catch
                { }
            }

            if (Properties["ExifISOSpeed"] == "n/a" || Properties["ExifISOSpeed"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifISOSpeed); }
                catch { }
            }
            else
            {
                try
                { info.ExifISOSpeed = ushort.Parse(Properties["ExifISOSpeed"]); }
                catch
                { }
            }

            if (Properties["ExifMeteringMode"] == "n/a" || Properties["ExifMeteringMode"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifMeteringMode); }
                catch { }
            }
            else
            {
                try
                { info.ExifMeteringMode = (MetadataLibrary.ImageInfo.MeteringMode)short.Parse(Properties["ExifMeteringMode"]); }
                catch
                { }
            }

            if (Properties["ExifLightSource"] == "n/a" || Properties["ExifLightSource"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifLightSource); }
                catch { }
            }
            else
            {
                try
                { info.ExifLightSource = ushort.Parse(Properties["ExifLightSource"]); }
                catch
                { }
            }

            if (Properties["ExifFlash"] == "n/a" || Properties["ExifFlash"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifFlash); }
                catch { }
            }
            else
            {
                try
                { info.ExifFlash = ushort.Parse(Properties["ExifFlash"]); }
                catch
                { }
            }

            if (Properties["ExifColorSpace"] == "n/a" || Properties["ExifColorSpace"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifColorSpace); }
                catch { }
            }
            else
            {
                try
                { info.ExifColorSpace = ushort.Parse(Properties["ExifColorSpace"]); }
                catch
                { }
            }

            if (Properties["ExifPixXDim"] == "n/a" || Properties["ExifPixXDim"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifPixXDim); }
                catch { }
            }
            else
            {
                try
                { info.ExifPixXDim = ushort.Parse(Properties["ExifPixXDim"]); }
                catch
                { }
            }

            if (Properties["ExifPixYDim"] == "n/a" || Properties["ExifPixYDim"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifPixYDim); }
                catch { }
            }
            else
            {
                try
                { info.ExifPixYDim = ushort.Parse(Properties["ExifPixYDim"]); }
                catch
                { }
            }

            if (Properties["ExifSensingMethod"] == "n/a" || Properties["ExifSensingMethod"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ExifSensingMethod); }
                catch { }
            }
            else
            {
                try
                { info.ExifSensingMethod = ushort.Parse(Properties["ExifSensingMethod"]); }
                catch
                { }
            }

            if (Properties["ThumbnailCompression"] == "n/a" || Properties["ThumbnailCompression"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailCompression); }
                catch { }
            }
            else
            {
                try
                { info.ThumbnailCompression = ushort.Parse(Properties["ThumbnailCompression"]); }
                catch
                { }
            }

            if (Properties["ThumbnailResolutionUnit"] == "n/a" || Properties["ThumbnailResolutionUnit"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailResolutionUnit); }
                catch { }
            }
            else
            {
                try
                { info.ThumbnailResolutionUnit = ushort.Parse(Properties["ThumbnailResolutionUnit"]); }
                catch
                { }
            }

            if (Properties["ThumbnailYCbCrPositioning"] == "n/a" || Properties["ThumbnailYCbCrPositioning"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.ThumbnailYCbCrPositioning); }
                catch { }
            }
            else
            {
                try
                { info.ThumbnailYCbCrPositioning = ushort.Parse(Properties["ThumbnailYCbCrPositioning"]); }
                catch
                { }
            }

            #endregion

            #region long

            if (Properties["JPEGInterFormat"] == "n/a" || Properties["JPEGInterFormat"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.JPEGInterFormat); }
                catch { }
            }
            else
            {
                try
                { info.JPEGInterFormat = long.Parse(Properties["JPEGInterFormat"]); }
                catch
                { }
            }

            if (Properties["JPEGInterLength"] == "n/a" || Properties["JPEGInterLength"] == "")
            {
                try
                { info.Image.RemovePropertyItem((int)MetadataLibrary.ImageInfo.PropertyTagId.JPEGInterLength); }
                catch { }
            }
            else
            {
                try
                { info.JPEGInterLength = long.Parse(Properties["JPEGInterLength"]); }
                catch
                { }
            }

            #endregion


            /*
            if (Properties["Brightness"] != "n/a" || Properties["Brightness"] != "")
            {
                var res = stringToFraction(Properties["Brightness"]);
                if (res != null)
                    info.Brightness = res;
            }
            if (Properties["Copyright"] != "n/a" || Properties["Copyright"] != "") info.Copyright = Properties["Copyright"];
            if (Properties["DateTime"] != "n/a" || Properties["DateTime"] != "") info.DateTime = Properties["DateTime"];
            //   if (Properties["DTDigitized"] != "n/a" || Properties["DTDigitized"] != "") info.DTDigitized = Properties["DTDigitized"];
            //     if (Properties["DTOrig"] != "n/a" || Properties["DTOrig"] != "") info.DTOrig = Properties["DTOrig"];
            if (Properties["EquipMake"] != "n/a" || Properties["EquipMake"] != "") info.EquipMake = Properties["EquipMake"];
            if (Properties["EquipModel"] != "n/a" || Properties["EquipModel"] != "") info.EquipModel = Properties["EquipModel"];
            if (Properties["ExifDTDigSS"] != "n/a" || Properties["ExifDTDigSS"] != "") info.ExifDTDigSS = Properties["ExifDTDigSS"];
            if (Properties["ExifDTOrigSS"] != "n/a" || Properties["ExifDTOrigSS"] != "") info.ExifDTOrigSS = Properties["ExifDTOrigSS"];
            if (Properties["ExifDTSubsec"] != "n/a" || Properties["ExifDTSubsec"] != "") info.ExifDTSubsec = Properties["ExifDTSubsec"];
            //           if (Properties["ExposureProg"] != "n/a" || Properties["ExposureProg"] != "") info.ExposureProg = Properties["ExposureProg"];
            if (Properties["FNumber"] != "n/a" || Properties["FNumber"] != "")
            {
                var res = stringToFraction(Properties["FNumber"]);
                if (res != null)
                    info.FNumber = res;
            }
            if (Properties["FocalLength"] != "n/a" || Properties["FocalLength"] != "")
            {
                var res = stringToFraction(Properties["FocalLength"]);
                if (res != null)
                    info.FocalLength = res;
            }
            if (Properties["GpsGpsMeasureMode"] != "n/a" || Properties["GpsGpsMeasureMode"] != "") info.GpsGpsMeasureMode = Properties["GpsGpsMeasureMode"];
            if (Properties["GpsLatitudeRef"] != "n/a" || Properties["GpsLatitudeRef"] != "") info.GpsLatitudeRef = Properties["GpsLatitudeRef"];
            if (Properties["GpsLongitudeRef"] != "n/a" || Properties["GpsLongitudeRef"] != "") info.GpsLongitudeRef = Properties["GpsLongitudeRef"];
            if (Properties["ImageDescription"] != "n/a" || Properties["ImageDescription"] != "") info.ImageDescription = Properties["ImageDescription"];
            if (Properties["ExifISOSpeed"] != "n/a" || Properties["ExifISOSpeed"] != "")
            {
                ushort p = 0;
                ushort.TryParse(Properties["ExifISOSpeed"], out p);
                info.ExifISOSpeed = p;
            }
            if (Properties["JPEGInterFormat"] != "n/a" || Properties["JPEGInterFormat"] != "")
            {
                long p = 0;
                long.TryParse(Properties["JPEGInterFormat"], out p);
                info.JPEGInterFormat = p;
            }
            //    if (Properties["MeteringMode"] != "n/a" || Properties["MeteringMode"] != "") info.MeteringMode = Properties["MeteringMode"];
            //    if (Properties["Orientation"] != "n/a" || Properties["Orientation"] != "") info.Orientation = Properties["Orientation"];
            if (Properties["PixXDim"] != "n/a" || Properties["PixXDim"] != "")
            {
                uint p = 0;
                uint.TryParse(Properties["PixXDim"], out p);
                info.PixXDim = p;

            }
            if (Properties["PixYDim"] != "n/a" || Properties["PixYDim"] != "")
            {
                uint p = 0;
                uint.TryParse(Properties["PixYDim"], out p);
                info.PixYDim = p;
            }
            //  if (Properties["ResolutionUnit"] != "n/a" || Properties["ResolutionUnit"] != "") info.ResolutionUnit = Properties["ResolutionUnit"];
            if (Properties["SoftwareUsed"] != "n/a" || Properties["SoftwareUsed"] != "") info.SoftwareUsed = Properties["SoftwareUsed"];
            if (Properties["XResolution"] != "n/a" || Properties["XResolution"] != "")
            {
                var res = stringToFraction(Properties["XResolution"]);
                if (res != null)
                    info.XResolution = res;
            }
            if (Properties["YResolution"] != "n/a" || Properties["YResolution"] != "")
            {
                var res = stringToFraction(Properties["YResolution"]);
                if (res != null)
                    info.YResolution = res;
            }
            */

            info.Image.Save(pathFile + ".tmp");
            info.Image.Dispose();
            File.Delete(pathFile);
            File.Move(pathFile + ".tmp", pathFile);

        }

        public MetadataLibrary.ImageInfo.Info AllTestRead(string pathFile)
        {
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);
            return info;
        }

        public bool Write(string pathFile)
        {
            return true;
        }

        /// <summary>
        /// Возвращает mime файла в типе стринг основываюсь на расширение файла
        /// </summary>
        /// <param name="pathFileImg">Путь до файла</param>
        /// <returns>Тип mime в типе стринг</returns>
        private string GetMimeType(string pathFileImg)
        {
            var file = Path.GetFileName(pathFileImg).ToLower().Split('.');
            switch (file[file.LongLength - 1])
            {
                case "gif": return "image/gif";
                case "png": return "image/png";
                case "bmp": return "image/bmp";
                case "dib": return "image/bmp";
                case "jpe": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "jpeg": return "image/jpeg";
                case "jfif": return "image/jpeg";
                case "tif": return "image/tiff";
                case "tiff": return "image/tiff";
                default: return "ups_not_mime_type";
            }
        }

        /// <summary>
        /// Возвращает кодировку изображения (GDI+)
        /// </summary>
        /// <param name="mimeType">Название кодировки в текстовом виде</param>
        /// <returns>Кодировка изображения</returns>
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Получить кодеки изображения для всех форматов изображений
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Найти нужный кодек изображения
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        private Fraction stringToFraction(string oper)
        {
            var str = oper.Split('/');
            if (str.Length == 2)
            {
                uint num = 777;
                uint den = 777;

                uint.TryParse(str[0], out num);
                uint.TryParse(str[1], out den);
                if (num != 777 && den != 777)
                {
                    Fraction fr = new Fraction(num, den);
                    return fr;
                }
                return null;
            }
            return null;
        }

    }
}
