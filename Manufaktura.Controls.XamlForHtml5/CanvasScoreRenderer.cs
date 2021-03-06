﻿/*
 * Copyright 2018 Manufaktura Programów Jacek Salamon http://musicengravingcontrols.com/
 * MIT LICENCE
 
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Manufaktura.Controls.Audio;
using Manufaktura.Controls.Model;
using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Rendering;
using Manufaktura.Controls.Rendering.Implementations;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Manufaktura.Controls.XamlForHtml5
{
    public class CanvasScoreRenderer : ScoreRenderer<Canvas>
    {
        public CanvasScoreRenderer(Canvas canvas, HtmlScoreRendererSettings settings)
            : base(canvas, settings)
        {
            OwnershipDictionary = new Dictionary<FrameworkElement, MusicalSymbol>();
        }

        public Dictionary<FrameworkElement, MusicalSymbol> OwnershipDictionary { get; private set; }

        /// <summary>
        /// Settings cast to HtmlScoreRendererSettings
        /// </summary>
        public HtmlScoreRendererSettings TypedSettings { get { return Settings as HtmlScoreRendererSettings; } }

        public static Primitives.Point ConvertPoint(Windows.Foundation.Point point)
        {
            return new Primitives.Point(point.X, point.Y);
        }

        public Windows.UI.Color ConvertColor(Primitives.Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public Primitives.Color ConvertColor(Windows.UI.Color color)
        {
            return new Primitives.Color(color.R, color.G, color.B, color.A);
        }

        public Windows.Foundation.Point ConvertPoint(Primitives.Point point)
        {
            return new Windows.Foundation.Point(point.X, point.Y);
        }

        public override void DrawArc(Primitives.Rectangle rect, double startAngle, double sweepAngle, Primitives.Pen pen, MusicalSymbol owner)
        {
            if (Settings.RenderingMode != ScoreRenderingModes.Panorama) rect = rect.Translate(CurrentScore.DefaultPageSettings);

            if (rect.Width < 0 || rect.Height < 0) return;  //TODO: Sprawdzić czemu tak się dzieje, poprawić
            PathGeometry pathGeom = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Windows.Foundation.Point(rect.X, rect.Y);
            ArcSegment arcSeg = new ArcSegment();
            arcSeg.Point = new Windows.Foundation.Point(rect.X + rect.Width, rect.Y);
            arcSeg.RotationAngle = startAngle;
            arcSeg.Size = new Windows.Foundation.Size(rect.Width, rect.Height);
            arcSeg.SweepDirection = sweepAngle < 180 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            arcSeg.IsLargeArc = sweepAngle > 180;
            pf.Segments.Add(arcSeg);
            pathGeom.Figures.Add(pf);

            Path path = new Path();
            path.Stroke = new SolidColorBrush(ConvertColor(pen.Color));
            path.StrokeThickness = pen.Thickness;
            path.Data = pathGeom;
            path.Visibility = BoolToVisibility(owner.IsVisible);
            Canvas.SetZIndex(path, (int)pen.ZIndex);
            Canvas.Children.Add(path);

            OwnershipDictionary.Add(path, owner);
        }

        public override void DrawBezier(Primitives.Point p1, Primitives.Point p2, Primitives.Point p3, Primitives.Point p4, Primitives.Pen pen, MusicalSymbol owner)
        {
            if (Settings.RenderingMode != ScoreRenderingModes.Panorama)
            {
                p1 = p1.Translate(CurrentScore.DefaultPageSettings);
                p2 = p2.Translate(CurrentScore.DefaultPageSettings);
                p3 = p3.Translate(CurrentScore.DefaultPageSettings);
                p4 = p4.Translate(CurrentScore.DefaultPageSettings);
            }

            PathGeometry pathGeom = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Windows.Foundation.Point(p1.X, p1.Y);
            BezierSegment bezierSegment = new BezierSegment();
            bezierSegment.Point1 = ConvertPoint(p2);
            bezierSegment.Point2 = ConvertPoint(p3);
            bezierSegment.Point3 = ConvertPoint(p4);
            pf.Segments.Add(bezierSegment);
            pathGeom.Figures.Add(pf);

            Path path = new Path();
            path.Stroke = new SolidColorBrush(ConvertColor(pen.Color));
            path.StrokeThickness = pen.Thickness;
            path.Data = pathGeom;
            path.Visibility = BoolToVisibility(owner.IsVisible);
            Canvas.Children.Add(path);

            OwnershipDictionary.Add(path, owner);
        }

        public override void DrawLine(Primitives.Point startPoint, Primitives.Point endPoint, Primitives.Pen pen, MusicalSymbol owner)
        {
            if (Settings.RenderingMode != ScoreRenderingModes.Panorama)
            {
                startPoint = startPoint.Translate(CurrentScore.DefaultPageSettings);
                endPoint = endPoint.Translate(CurrentScore.DefaultPageSettings);
            }

            var line = new Line();
            line.Stroke = new SolidColorBrush(ConvertColor(pen.Color));
            line.UseLayoutRounding = true;
            line.X1 = startPoint.X;
            line.X2 = endPoint.X;
            line.Y1 = startPoint.Y;
            line.Y2 = endPoint.Y;
            Canvas.SetZIndex(line, (int)pen.ZIndex);
            line.StrokeThickness = pen.Thickness;
            line.Visibility = BoolToVisibility(owner.IsVisible);

            Canvas.Children.Add(line);

            OwnershipDictionary.Add(line, owner);
        }

        public override void DrawString(string text, MusicFontStyles fontStyle, Primitives.Point location, Primitives.Color color, MusicalSymbol owner)
        {
            if (Settings.RenderingMode != ScoreRenderingModes.Panorama) location = location.Translate(CurrentScore.DefaultPageSettings);

            location = TranslateTextLocation(location, fontStyle);

            TextBlock textBlock = new TextBlock();
            textBlock.FontSize = Fonts.GetSize(fontStyle);
            textBlock.FontFamily = Fonts.Get(fontStyle);
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(ConvertColor(color));
            textBlock.Visibility = BoolToVisibility(owner.IsVisible);
            Canvas.SetLeft(textBlock, location.X + 3d);
            Canvas.SetTop(textBlock, location.Y);
            Canvas.Children.Add(textBlock);

            OwnershipDictionary.Add(textBlock, owner);
        }

        public override void DrawStringInBounds(string text, MusicFontStyles fontStyle, Primitives.Point location, Primitives.Size size, Primitives.Color color, MusicalSymbol owner)
        {
            if (Settings.RenderingMode != ScoreRenderingModes.Panorama) location = location.Translate(CurrentScore.DefaultPageSettings);

            TextBlock textBlock = new TextBlock();
            textBlock.FontFamily = Fonts.Get(fontStyle);
            textBlock.FontSize = 200;
            textBlock.Text = text;
            textBlock.Margin = new Thickness(0, -25, 0, 0);
            textBlock.Foreground = new SolidColorBrush(ConvertColor(color));
            textBlock.Visibility = BoolToVisibility(owner.IsVisible);

            /*var viewBox = new Viewbox();
			viewBox.Child = textBlock;
			viewBox.Width = size.Width;
			viewBox.Height = size.Height;
			viewBox.Stretch = Stretch.Fill;
			viewBox.RenderTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1.9 };
			Canvas.SetLeft(viewBox, location.X + 3d);
			Canvas.SetTop(viewBox, location.Y);
			Canvas.Children.Add(viewBox);

			OwnershipDictionary.Add(textBlock, owner);*/
        }

        protected override void DrawPlaybackCursor(PlaybackCursorPosition position, Primitives.Point start, Primitives.Point end)
        {
            throw new NotImplementedException();
        }

        protected Primitives.Point TranslateTextLocation(Primitives.Point location, MusicFontStyles fontStyle)
        {
            double locationX = location.X + TypedSettings.MusicalFontShiftX;
            double locationY;
            switch (fontStyle)
            {
                case MusicFontStyles.MusicFont:
                    locationY = location.Y + 8d + TypedSettings.MusicalFontShiftY;
                    break;

                case MusicFontStyles.StaffFont:
                    locationY = location.Y + 10d + TypedSettings.MusicalFontShiftY;
                    break;

                case MusicFontStyles.GraceNoteFont:
                    locationY = location.Y + 10.5d + TypedSettings.MusicalFontShiftY;
                    locationX += 0.7d;
                    break;

                default:
                    locationY = location.Y + TypedSettings.MusicalFontShiftY;
                    break;
            }
            return new Primitives.Point(locationX, locationY);
        }

        private Visibility BoolToVisibility(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}