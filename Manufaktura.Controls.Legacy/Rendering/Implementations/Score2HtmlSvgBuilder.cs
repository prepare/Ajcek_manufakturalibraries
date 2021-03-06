﻿using Manufaktura.Controls.Model;
using Manufaktura.Music.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Manufaktura.Controls.Rendering.Implementations
{
    public class Score2HtmlSvgBuilder : Score2HtmlBuilder<HtmlSvgScoreRenderer, XElement>
    {
        public Score2HtmlSvgBuilder(IEnumerable<Score> scores, string canvasPrefix, HtmlScoreRendererSettings settings)
        {
            if (string.IsNullOrWhiteSpace(canvasPrefix)) throw new ArgumentNullException("canvasPrefix");
            Scores = scores;
            CanvasPrefix = canvasPrefix;
            Settings = settings;
        }

        public Score2HtmlSvgBuilder(Score score, string canvasName, HtmlScoreRendererSettings settings)
            : this(new List<Score> { score }, canvasName, settings)
        {
        }

        public override void BuildFontInformation(XElement canvas)
        {
            var element = new XElement("style", new XAttribute("type", "text/css"));
            element.Value = GetFontFaceDeclaration();
            canvas.Add(element);
        }

        public override void BuildScoreElementWrapper(XElement canvas, XElement scoreCanvas, Score score, string scoreElementName)
        {
            var wrapper = new XElement("svg",
                new XAttribute("width", Settings.PageWidth.ToStringInvariant()),
                new XAttribute("height", Settings.Height.ToStringInvariant()));
            canvas.Add(wrapper);
            foreach (var element in scoreCanvas.Elements())
            {
                wrapper.Add(element);
            }
        }

        public override string GetHtmlStringFromCanvas(XElement canvas)
        {
            var sb = new StringBuilder();
            foreach (var element in canvas.Elements())
            {
                sb.Append(element.ToString());
            }
            return sb.ToString();
        }

        public override XElement CreateCanvas()
        {
            return new XElement("root");
        }
    }
}
