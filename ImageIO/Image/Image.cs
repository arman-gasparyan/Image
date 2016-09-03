using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace ImageIO
{
    public class Image
    {
        public enum ImageIOResult
        {
            Uploaded,
            ResizedAndUploaded,
            Deleted,
            Replaced,
            Faild
        }

        public ImageIOResult ResizeAndUpload(List<HttpPostedFileBase> images,string path, int maxWidth = 600, int maxHeight = 600, params string[] imageNames)
        {
            int i = 0;

            if (images == null || images.Count <= imageNames.Length)
            {
                return ImageIOResult.Faild;
            }

  
                var fileName = "";

                if (images != null)
                {
                    foreach (var item in images)
                    {
                        if (item != null)
                        {
                            fileName = Guid.NewGuid().ToString();
                            item.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                            ImageBuilder.Current.Build(
                                new ImageJob(item.InputStream,path + fileName, 
                                new Instructions("maxwidth=" + maxWidth + "&" + "maxheight=" + maxHeight + "&" + "format=jpg"),
                                true, true));
                            item.InputStream.Dispose();
                            imageNames[i] = fileName + ".jpg";
                            i++;
                        }
                    }
                }

            return ImageIOResult.ResizedAndUploaded;

            }
        
        public ImageIOResult Upload(List<HttpPostedFileBase> images, string[] imageNames, string path)
        {
            int i = 0;

            if (images != null && images.Count <= imageNames.Length)
            {
                foreach (var item in images)
                {
                    if (item != null)
                    {
                        var fileName = Guid.NewGuid().ToString();
                        item.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                        ImageBuilder.Current.Build(
                            new ImageJob(
                                item.InputStream,
                                path + fileName,
                                new Instructions(),
                                true,
                                true));
                        item.InputStream.Dispose();
                        imageNames[i] = fileName + ".jpg";
                        i++;
                    }
                }

                return ImageIOResult.Uploaded;
            }

            return ImageIOResult.Faild;
        }

        public string Upload(HttpPostedFileBase image, string path)
        {

            if (image != null)
            {
                string imageName = Guid.NewGuid().ToString();
                image.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                ImageBuilder.Current.Build(
                    new ImageJob(
                        image.InputStream,
                        path + imageName,
                        new Instructions(),
                        true,
                        true));
                image.InputStream.Dispose();
                imageName = imageName + ".jpg";
                return imageName;
            }

            return null;
        }
        
        public ImageIOResult Delete(string path, params string[] imageNames)
        {
            try
            {
                for (int i = 0; i < imageNames.Length; i++)
                {
                    string iamgePath = "";
                    if (!String.IsNullOrWhiteSpace(imageNames[i]))
                    {
                        iamgePath = Path.Combine(path, imageNames[i]);
                        if (File.Exists(iamgePath.ToString()))
                        {
                            File.Delete(iamgePath);
                        }
                    }
                }

                return ImageIOResult.Deleted;
            }

            catch (Exception)
            {
                return ImageIOResult.Faild;
            }
        }

        public ImageIOResult Replace(List<HttpPostedFileBase> images, string path, params string[] imageNames)
        {
            if (images == null || images.Count <= imageNames.Length)
            {
                return ImageIOResult.Faild;
            }
            
            Delete(path, imageNames);

            Upload(images, imageNames, path);

            return ImageIOResult.Replaced;
        }
    }
}
