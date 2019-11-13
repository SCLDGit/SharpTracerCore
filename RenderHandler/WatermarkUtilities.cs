using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace RenderHandler
{
    internal static class WatermarkUtilities
    {
        public static void ApplyScalingWaterMark<TPixel>(this IImageProcessingContext<TPixel> p_processingContext,
                                                         Font p_font,
                                                         string p_text, TPixel p_fontColor,
                                                         TPixel p_backgroundColor,
                                                         float p_padding, bool p_wordwrap,
                                                         float p_targetSize)
           where TPixel : struct, IPixel<TPixel>
        {
            if (p_wordwrap)
            {
                p_processingContext.ApplyScalingWaterMarkWordWrap(p_font, p_text, p_fontColor, p_padding);
            }
            else
            {
                p_processingContext.ApplyScalingWaterMarkSimpleTopLeft(p_font, p_text, p_fontColor, p_backgroundColor, p_targetSize);
            }
        }

        /*
                public static IImageProcessingContext<TPixel> ApplyScalingWaterMarkSimple<TPixel>(this IImageProcessingContext<TPixel> processingContext, Font font, string text, TPixel color, float padding)
                    where TPixel : struct, IPixel<TPixel>
                {
                    return processingContext.Apply(img =>
                    {
                        float targetWidth = img.Width - (padding * 2);
                        float targetHeight = img.Height - (padding * 2);

                        // measure the text size
                        SizeF size = TextMeasurer.Measure(text, new RendererOptions(font));

                        //find out how much we need to scale the text to fill the space (up or down)
                        float scalingFactor = Math.Min(img.Width / size.Width, img.Height / size.Height);

                        //create a new font
                        Font scaledFont = new Font(font, scalingFactor * font.Size);

                        var center = new PointF(img.Width / 2, img.Height / 2);
                        var textGraphicOptions = new TextGraphicsOptions(true)
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        img.Mutate(i => i.DrawText(textGraphicOptions, text, scaledFont, color, center));
                    });
                }
        */

        private static void ApplyScalingWaterMarkSimpleTopLeft<TPixel>(
            this IImageProcessingContext<TPixel> p_processingContext, Font p_font, string p_text, TPixel p_fontColor,
            TPixel p_backgroundColor, float p_targetSize)
            where TPixel : struct, IPixel<TPixel>
        {
            p_processingContext.Apply(p_img =>
            {
                var targetSizePercent = 100 / p_targetSize;

                // measure the text size
                var size = TextMeasurer.Measure(p_text, new RendererOptions(p_font));

                //find out how much we need to scale the text to fill the space (up or down)
                var scalingFactor = Math.Min(p_img.Width / targetSizePercent / size.Width,
                                               p_img.Height / targetSizePercent / size.Height);

                //create a new font
                var scaledFont = new Font(p_font, scalingFactor * p_font.Size);

                size = TextMeasurer.Measure(p_text, new RendererOptions(scaledFont));

                var center = new PointF(p_img.Width / 75.0f + size.Width / 2,
                                        p_img.Height / 75.0f + size.Height / 2);
                var textGraphicOptions = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                p_img.Mutate(p_i =>
                               p_i.FillPolygon(new GraphicsOptions(true, PixelColorBlendingMode.Multiply, 0.5f),
                                             p_backgroundColor, new PointF(0, 0), new PointF(0, size.Height * 2), new
                                                 PointF(size.Width + p_img.Width / 75 * 2,
                                                        size.Height * 2), new
                                                 PointF(size.Width + p_img.Width / 75 * 2,
                                                        0)));
                p_img.Mutate(p_i => p_i.DrawText(textGraphicOptions, p_text, scaledFont, p_fontColor,
                                           center));
            });
        }

        private static void ApplyScalingWaterMarkWordWrap<TPixel>(
            this IImageProcessingContext<TPixel> p_processingContext, Font p_font, string p_text, TPixel p_color,
            float p_padding)
            where TPixel : struct, IPixel<TPixel>
        {
            p_processingContext.Apply(p_img =>
            {
                var targetWidth = p_img.Width - p_padding * 2;
                var targetHeight = p_img.Height - p_padding * 2;

                var targetMinHeight = p_img.Height - p_padding * 3; // must be with in a margin width of the target height

                // now we are working i 2 dimensions at once and can't just scale because it will cause the text to
                // reflow we need to just try multiple times

                var scaledFont = p_font;
                var s = new SizeF(float.MaxValue, float.MaxValue);

                var scaleFactor = scaledFont.Size / 2; // Every time we change direction we half this size
                var trapCount = (int)scaledFont.Size * 2;
                if (trapCount < 10)
                {
                    trapCount = 10;
                }

                var isTooSmall = false;

                while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0)
                {
                    if (s.Height > targetHeight)
                    {
                        if (isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }

                        scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                        isTooSmall = false;
                    }

                    if (s.Height < targetMinHeight)
                    {
                        if (!isTooSmall)
                        {
                            scaleFactor = scaleFactor / 2;
                        }
                        scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                        isTooSmall = true;
                    }
                    trapCount--;

                    s = TextMeasurer.Measure(p_text, new RendererOptions(scaledFont)
                    {
                        WrappingWidth = targetWidth
                    });
                }

                var center = new PointF(p_padding, p_img.Height / 2.0f);
                var textGraphicOptions = new TextGraphicsOptions(true)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    WrapTextWidth = targetWidth
                };
                p_img.Mutate(p_i => p_i.DrawText(textGraphicOptions, p_text, scaledFont, p_color, center));
            });
        }
    }
}
