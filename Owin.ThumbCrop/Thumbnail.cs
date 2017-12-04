using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.Primitives;
using System.IO;

namespace Owin.ThumbCrop
{
    public static class Thumbnail
    {
        public static Image<Rgba32> Create(
            Image<Rgba32> image,
            Size cropArea,
            ReferencePoint refPoint = ReferencePoint.MiddleCenter,
            bool fitInside = false
        )
        {
            var img = image.Clone();

            var newSize = GetProportionalSize(new Size(img.Width, img.Height), cropArea, fitInside);
            var p = GetStartPosition(cropArea, newSize, refPoint);

            img.Mutate(x => x.Resize(cropArea).DrawImage(img, 1, newSize, new Point(-(p.X), -(p.Y))));

            return img;
        }

        public static Point GetStartPosition(
            Size requested,
            Size proportional,
            ReferencePoint refPoint
        )
        {
            Point result;

            switch (refPoint)
            {
                case ReferencePoint.TopLeft:
                    result = GetStartPositionOnTopLeft(requested, proportional);
                    break;
                case ReferencePoint.TopCenter:
                    result = GetStartPositionOnTopCenter(requested, proportional);
                    break;
                case ReferencePoint.TopRight:
                    result = GetStartPositionOnTopRight(requested, proportional);
                    break;
                case ReferencePoint.MiddleLeft:
                    result = GetStartPositionOnMiddleLeft(requested, proportional);
                    break;
                case ReferencePoint.MiddleCenter:
                    result = GetStartPositionOnMiddleCenter(requested, proportional);
                    break;
                case ReferencePoint.MiddleRight:
                    result = GetStartPositionOnMiddleRight(requested, proportional);
                    break;
                case ReferencePoint.BottomLeft:
                    result = GetStartPositionOnBottomLeft(requested, proportional);
                    break;
                case ReferencePoint.BottomCenter:
                    result = GetStartPositionOnBottomCenter(requested, proportional);
                    break;
                case ReferencePoint.BottomRight:
                    result = GetStartPositionOnBottomRight(requested, proportional);
                    break;
                default:
                    result = new Point(0, 0);
                    break;
            }

            return result;
        }

        private static Point GetStartPositionOnBottomRight(Size requested, Size proportional)
        {
            var x = proportional.Width - requested.Width;
            var y = proportional.Height - requested.Height;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnBottomCenter(Size requested, Size proportional)
        {
            var x = (proportional.Width - requested.Width) / 2;
            var y = proportional.Height - requested.Height;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnBottomLeft(Size requested, Size proportional)
        {
            var x = 0;
            var y = proportional.Height - requested.Height;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnMiddleRight(Size requested, Size proportional)
        {
            var x = proportional.Width - requested.Width;
            var y = (proportional.Height - requested.Height) / 2;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnMiddleCenter(Size requested, Size proportional)
        {
            var x = (proportional.Width - requested.Width) / 2;
            var y = (proportional.Height - requested.Height) / 2;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnMiddleLeft(Size requested, Size proportional)
        {
            var x = 0;
            var y = (proportional.Height - requested.Height) / 2;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnTopRight(Size requested, Size proportional)
        {
            var x = proportional.Width - requested.Width;
            var y = 0;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnTopCenter(Size requested, Size proportional)
        {
            var x = (proportional.Width - requested.Width) / 2;
            var y = 0;
            return new Point(x, y);
        }

        private static Point GetStartPositionOnTopLeft(Size requested, Size proportional)
        {
            var x = 0;
            var y = 0;
            return new Point(x, y);
        }

        private static Size GetProportionalSize(Size original, Size requested, bool fitInside)
        {
            var result = requested;
            var propW = (double)requested.Width / original.Width;
            var propH = (double)requested.Height / original.Height;

            double newProp;

            if (propH != propW)
            {
                if (fitInside)
                {
                    newProp = (propW < propH) ? propW : propH;
                }
                else
                {
                    newProp = (propW > propH) ? propW : propH;
                }

                result.Width = (int)(original.Width * newProp);
                result.Height = (int)(original.Height * newProp);
            }

            return result;
        }

        public static Image<Rgba32> Resize(Image<Rgba32> img, Size size)
        {
            var newImage = img.Clone();
            newImage.Mutate(x => x.Resize(size));
            return newImage;
        }

        public static MemoryStream Convert(Image<Rgba32> image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
