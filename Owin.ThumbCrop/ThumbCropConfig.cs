namespace Owin.ThumbCrop
{
    public class ThumbCropConfig
    {
        internal string BasePath { get; set; }
        public int DefaultWidth { get; set; } = 100;
        public int DefaultHeight { get; set; } = 100;

        public string UrlPattern { get; set; } = ".thumb.axd";

        public string NotFoundFile { get; set; }
    }
}
