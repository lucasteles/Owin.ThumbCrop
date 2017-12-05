using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace Owin.ThumbCrop
{
    internal static class Thumbnail
    {
        internal static Image<Rgba32> Create(
            Image<Rgba32> image,
            Size cropArea,
            ReferencePoint refPoint = ReferencePoint.MiddleCenter,
            bool fitInside = false
        )
        {
            var newSize = GetProportionalSize(new Size(image.Width, image.Height), cropArea, fitInside);
            var p = GetStartPosition(cropArea, newSize, refPoint);
            var img = new Image<Rgba32>(image.Width, image.Height);
            img.Mutate(x => x.Resize(cropArea).DrawImage(image, 1, newSize, new Point(-(p.X), -(p.Y))));
            return img;
        }

        internal static Point GetStartPosition(
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

        internal static Image<Rgba32> Resize(Image<Rgba32> img, Size size) => SetImageAttr(img, m => m.Resize(size));
        internal static Image<Rgba32> Alpha(Image<Rgba32> img, float percent) => SetImageAttr(img, m => m.Alpha(percent));
        internal static Image<Rgba32> Flip(Image<Rgba32> img, FlipType flip) => SetImageAttr(img, m => m.Flip(flip));
        internal static Image<Rgba32> Rotate(Image<Rgba32> img, int degrees) => SetImageAttr(img, m => m.Rotate(degrees));

        internal static Image<Rgba32> SetImageAttr(Image<Rgba32> img, Action<IImageProcessingContext<Rgba32>> action)
        {
            var newImage = img;
            newImage.Mutate(action);
            return newImage;
        }

        internal static MemoryStream Convert(Image<Rgba32> image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, new PngEncoder());
            memoryStream.Position = 0;
            return memoryStream;
        }


        internal static Dictionary<string, Action<IImageProcessingContext<Rgba32>>> GetImageBoolProcessors() =>
             new Dictionary<string, Action<IImageProcessingContext<Rgba32>>>
             {
                 ["blur"] = m => m.GaussianBlur(),
                 ["sharpening"] = m => m.GaussianSharpen(),
                 ["invert"] = m => m.Invert(),
                 ["glow"] = m => m.Glow(),
                 ["BlackWhite"] = m => m.BlackWhite(),
                 ["GrayscaleBT709"] = m => m.Grayscale(GrayscaleMode.Bt709),
                 ["Grayscale"] = m => m.Grayscale(GrayscaleMode.Bt601),
                 ["Lomograph"] = m => m.Lomograph(),
                 ["Polaroid"] = m => m.Polaroid(),
                 ["Kodachrome"] = m => m.Kodachrome(),
                 ["Sepia"] = m => m.Sepia(),
                 ["Vignette"] = m => m.Vignette(),

             };


        internal static Dictionary<string, Func<float, Action<IImageProcessingContext<Rgba32>>>> GetImageFloatProcessors() =>
             new Dictionary<string, Func<float, Action<IImageProcessingContext<Rgba32>>>>
             {
                 ["rotate"] = v => m => m.Rotate(v == 0 ? 90 : v),
                 ["hue"] = v => m => m.Hue(v == 0 ? 10 : v),
                 ["opacity"] = v => m => m.Alpha((v == 0 ? 50 : v) / 100),
                 ["oil"] = v => m => m.OilPaint((int)(v <= 0 ? 10 : v), 15),
                 ["pixelate"] = v => m => m.Pixelate((int)(v <= 0 ? 4 : v)),
                 ["contrast"] = v => m => m.Contrast((int)(v == 0 ? 10 : v)),
                 ["brightness"] = v => m => m.Brightness((int)(v == 0 ? 10 : v)),
             };
    }
}
