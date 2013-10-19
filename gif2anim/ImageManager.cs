using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace Gif2Anim
{
    public class ImageParts {
        public string Drawable { get; set; }
        public int Duration { get; set; }
    }

    public class ImageManager
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameCount { get; set; }
        public string OutputFileName { get; set; }
        public bool IsAnimated { get; set; }
        public bool IsLooped { get; set; }
        public int AnimationLength { get; set; } // In milliseconds
        public List<ImageParts> ImageParts { get; set; }

        public static ImageManager ProcessImage(Options option)
        {
            ImageManager info = new ImageManager();

            String path = option.InputFile;

            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                info.Height = image.Height;
                info.Width = image.Width;
                info.OutputFileName = Path.GetFileNameWithoutExtension(path);
                info.ImageParts = new List<ImageParts>();

                if (image.RawFormat.Equals(ImageFormat.Gif))
                {
                    if (System.Drawing.ImageAnimator.CanAnimate(image))
                    {
                        FrameDimension frameDimension = new FrameDimension(image.FrameDimensionsList[0]);

                        int frameCount = image.GetFrameCount(frameDimension);
                        int delay = 0;
                        int this_delay = 0;
                        int index = 0;

                        if (!Directory.Exists(option.OutputFolder))
                        {
                            Directory.CreateDirectory(option.OutputFolder);
                        }

                        String drawablePath = Path.Combine(option.OutputFolder, "drawable-" + option.Density);

                        if (!Directory.Exists(drawablePath))
                        {
                            Directory.CreateDirectory(drawablePath);
                        }

                        ImageManipulation.OctreeQuantizer quantizer = null;

                        for (int f = 0; f < frameCount; f++)
                        {
                            this_delay = BitConverter.ToInt32(image.GetPropertyItem(20736).Value, index) * 10;
                            delay += (this_delay < 100 ? 100 : this_delay);  // Minimum delay is 100 ms
                            index += 4;

                            image.SelectActiveFrame(frameDimension, f);

                            quantizer = new ImageManipulation.OctreeQuantizer(255, 8);

                            using (Bitmap quantized = quantizer.Quantize(image))
                            {
                                String drawable = info.OutputFileName + "_" + f.ToString();
                                String fileName = drawable + ".png";
                                info.ImageParts.Add(new ImageParts { Drawable = drawable, Duration = this_delay });
                                quantized.Save(Path.Combine(drawablePath, fileName), ImageFormat.Png);
                            }
                        }

                        info.FrameCount = frameCount;
                        info.AnimationLength = delay;
                        info.IsAnimated = true;
                        info.IsLooped = BitConverter.ToInt16(image.GetPropertyItem(20737).Value, 0) != 1;
                    }
                }
            }

            return info;
        }
    }

}
