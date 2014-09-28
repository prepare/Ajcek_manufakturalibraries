﻿using Manufaktura.Controls.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering.Implementations
{
    public class HtmlCanvasScoreRenderer : ScoreRenderer<StringBuilder>
    {
        public Dictionary<FontStyles, HtmlFontInfo> Fonts { get; private set; }

        public HtmlCanvasScoreRenderer(StringBuilder stringBuilder, Dictionary<FontStyles, HtmlFontInfo> fonts)
            : base(stringBuilder)
        {
            Fonts = fonts;

            stringBuilder.AppendLine("<style>");
            foreach (var font in Fonts.Values)
            {
                stringBuilder.AppendLine("@font-face {");
                stringBuilder.AppendLine(string.Format("font-family: '{0}';", font.Name));
                stringBuilder.AppendLine(string.Format("src: local('{0}');", font.Uri)); 
                stringBuilder.AppendLine("}");
            }
            stringBuilder.AppendLine("</style>");
            stringBuilder.AppendLine("<canvas id=\"myCanvas\" width=\"578\" height=\"200\"></canvas>");
            stringBuilder.AppendLine("<script>");
            stringBuilder.AppendLine("var canvas = document.getElementById('myCanvas');");
            stringBuilder.AppendLine("var context = canvas.getContext('2d');");
        }

        public override void DrawString(string text, FontStyles fontStyle, Primitives.Point location, Primitives.Color color, Model.MusicalSymbol owner)
        {
            if (!Fonts.ContainsKey(fontStyle)) return;   //Nie ma takiego fontu zdefiniowanego. Nie rysuj.

            double locationY = location.Y + 25d;
            Canvas.AppendLine(string.Format("context.font = '{0}pt {1}';", Fonts[fontStyle].Size.ToString(CultureInfo.InvariantCulture), Fonts[fontStyle].Name));
            Canvas.AppendLine(string.Format("context.fillText('{0}', {1}, {2});", text, location.X.ToString(CultureInfo.InvariantCulture), locationY.ToString(CultureInfo.InvariantCulture)));
        }

        public override void DrawLine(Primitives.Point startPoint, Primitives.Point endPoint, Primitives.Pen pen, Model.MusicalSymbol owner)
        {
            Canvas.AppendLine("context.beginPath();");
            Canvas.AppendLine(string.Format("context.moveTo({0}, {1});", startPoint.X, startPoint.Y));
            Canvas.AppendLine(string.Format("context.lineTo({0}, {1});", endPoint.X, endPoint.Y));
            Canvas.AppendLine("context.stroke();");
        }

        public override void DrawArc(Primitives.Rectangle rect, double startAngle, double sweepAngle, Primitives.Pen pen, Model.MusicalSymbol owner)
        {
        }

        public override void DrawBezier(Primitives.Point p1, Primitives.Point p2, Primitives.Point p3, Primitives.Point p4, Primitives.Pen pen, Model.MusicalSymbol owner)
        {
        }

        public struct HtmlFontInfo
        {
            public string Name { get; private set; }
            public string Uri { get; private set; }
            public double Size { get; private set; }

            public HtmlFontInfo(string name, string uri, double size) : this()
            {
                Name = name;
                Uri = uri;
                Size = size;
            }

        }
    }
}