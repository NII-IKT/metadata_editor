using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;

namespace MetadataLibrary
{
    /// <summary>
    /// Класс для работы с метаданными картинок
    /// </summary>
    public class MetaData
    {
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

            img.Save(pathFile+".tmp", Encoder, EncoderParams);
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

        public Dictionary<string,string> ReadAll(string pathFile)//TODO нехороший метод
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            MetadataLibrary.ImageInfo.Info info = new ImageInfo.Info(pathFile);
            res.Add("PixXDim", info.PixXDim == 1123456789 ? "n/a" : info.PixXDim.ToString());
            res.Add("PixYDim", info.PixYDim == 1123456789? "n/a" :info.PixYDim.ToString());
            res.Add("EquipModel", String.Copy(info.EquipModel));
            res.Add("EquipMake", String.Copy(info.EquipMake));
            res.Add("Copyright", String.Copy(info.Copyright));
            res.Add("DateTime", String.Copy(info.DateTime));
            res.Add("ISOSpeed", info.ISOSpeed == 12345 ? "n/a" : info.ISOSpeed.ToString());
            info.Image.Dispose();
            return res;
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
                case "jpeg":return "image/jpeg";                  
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

    }
}
