using Corporate.Web.Cms.ContentTypes.Media;
using EPiServer;

namespace TinyfyEpiImages
{
    public class ContentEvents
    {
        public static void PublishedContent(object sender, ContentEventArgs e)
        {
            var image = e.Content as ImageFile;
            if (image != null && !image.IsCompressed)
            {
                Tinyfy.CompressImage(image);
            }
        }
    }
}
