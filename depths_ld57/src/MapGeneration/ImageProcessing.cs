
using Godot;
using System;

namespace depths_ld57.MapGeneration;

public static class ImageProcessing 
{
    public static void Dilate(Image dilatedImage, Image sourceImage, int kernelSize)
    {
        var width = sourceImage.GetWidth();
        var height = sourceImage.GetHeight();

        dilatedImage.SetData(width, height, false, Image.Format.Rgbaf, sourceImage.GetData());

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var maxColor = new Color(0, 0, 0, 1);
                for (var ky = -kernelSize / 2; ky <= kernelSize / 2; ky++)
                {
                    for (var kx = -kernelSize / 2; kx <= kernelSize / 2; kx++)
                    {
                        var nx = x + kx;
                        var ny = y + ky;

                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            var neighborColor = sourceImage.GetPixel(nx, ny);
                            maxColor = new Color(
                                Math.Max(maxColor.R, neighborColor.R),
                                Math.Max(maxColor.G, neighborColor.G),
                                Math.Max(maxColor.B, neighborColor.B),
                                maxColor.A
                            );
                        }
                    }
                }
                dilatedImage.SetPixel(x, y, maxColor);
            }
        }
    }

    public static void Erode(Image erodedImage, Image sourceImage, int kernelSize)
    {
        var width = sourceImage.GetWidth();
        var height = sourceImage.GetHeight();
        
        erodedImage.SetData(width, height, false, Image.Format.Rgbaf, sourceImage.GetData());

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var minColor = new Color(1, 1, 1, 1);
                for (var ky = -kernelSize / 2; ky <= kernelSize / 2; ky++)
                {
                    for (var kx = -kernelSize / 2; kx <= kernelSize / 2; kx++)
                    {
                        var nx = x + kx;
                        var ny = y + ky;

                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            var neighborColor = sourceImage.GetPixel(nx, ny);
                            minColor = new Color(
                                Math.Min(minColor.R, neighborColor.R),
                                Math.Min(minColor.G, neighborColor.G),
                                Math.Min(minColor.B, neighborColor.B),
                                minColor.A
                            );
                        }
                    }
                }
                erodedImage.SetPixel(x, y, minColor);
            }
        }
    }
}
