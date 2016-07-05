using System;
using System.IO;
using System.Net;
using System.Text;
using Corporate.Web.Cms.ContentTypes.Media;
using EPiServer;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using NuGet;

namespace TinyfyEpiImages
{
    public class Tinyfy
    {
        private static Injected<IContentRepository> _contentRepository;
        private static Injected<BlobFactory> _blobFactory;
        private static readonly string Key = WebConfigurationManager.AppSettings["tinyfykey"];
        private const string Url = "https://api.tinify.com/shrink";
        private static byte[] CompressedImage { get; set; }

        public static void CompressImage(ImageFile image)
        {
            using (var client = new WebClient())
            {
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + Key));
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

                //Read data from image
                var imageBytes = image.BinaryData.OpenRead().ReadAllBytes();

                try
                {
                    //Upload to TinyPNG for compression
                    client.UploadData(Url, imageBytes);

                    //Compression was successful, retrieve output from Location header
                    CompressedImage = client.DownloadData(client.ResponseHeaders["Location"]);
                }

                catch (WebException we)
                {
                    var m = we.Message;
                    return;
                }
            }

            //Create writable clone
            var writableImageClone = image.CreateWritableClone() as ImageFile;
            if (writableImageClone == null) return;

            //Get file extension
            var fileType = Path.GetExtension(image.Name);

            //Upload new blob
            var blob = _blobFactory.Service.CreateBlob(image.BinaryDataContainer, fileType);

            //Write new data to blob
            using (var s = blob.OpenWrite())
            {
                if (s.CanWrite)
                {
                    s.Write(CompressedImage, 0, CompressedImage.Length);
                    s.Flush();
                }
                else
                {
                    return;
                }
            }

            //Save compressed image to content repository
            writableImageClone.BinaryData = blob;
            writableImageClone.IsCompressed = true;
            _contentRepository.Service.Save(writableImageClone, SaveAction.Publish);
        }
    }
}
