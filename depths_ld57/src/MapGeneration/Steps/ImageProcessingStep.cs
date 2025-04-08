using Godot;

namespace depths_ld57.MapGeneration.Steps;

public class ImageProcessingStep : IMapGenerationStep
{
    private readonly Vector2I _mapSize;

    private readonly Image _dilatedImage;
    private readonly Image _erodedImage;

    public ImageProcessingStep(Vector2I mapSize)
    {
        _mapSize = mapSize;

        _dilatedImage = Image.CreateEmpty(_mapSize.X, _mapSize.Y, false, Image.Format.Rgbaf);
        _erodedImage = Image.CreateEmpty(_mapSize.X, _mapSize.Y, false, Image.Format.Rgbaf);
    }

    public void Generate(MapGenerationContext ctx)
    {
        ApplyTransformation(ctx);
        CopyToFinal(ctx);
    }

    private void ApplyTransformation(MapGenerationContext ctx)
    {
        if (ctx.Dilated) ImageProcessing.Dilate(_dilatedImage, ctx.WorkingImage, ctx.DilationKernel);
        if (ctx.Eroded && !ctx.Dilated) ImageProcessing.Erode(_erodedImage, ctx.WorkingImage, ctx.ErosionKernel);
        if (ctx.Eroded && ctx.Dilated) ImageProcessing.Erode(_erodedImage, _dilatedImage, ctx.ErosionKernel);
    }

    private void CopyToFinal(MapGenerationContext ctx)
    {
        if (ctx.Dilated && !ctx.Eroded)
            ctx.WorkingImage
                .SetData(_mapSize.X, _mapSize.Y, false, Image.Format.Rgbaf, _dilatedImage.GetData());

        if (ctx.Eroded)
            ctx.WorkingImage
                .SetData(_mapSize.X, _mapSize.Y, false, Image.Format.Rgbaf, _erodedImage.GetData());
    }
}