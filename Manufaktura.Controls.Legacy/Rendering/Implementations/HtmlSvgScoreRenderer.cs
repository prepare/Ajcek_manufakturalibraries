﻿using Manufaktura.Controls.Audio;
using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Primitives;
using Manufaktura.Music.Extensions;
using System.Xml.Linq;

namespace Manufaktura.Controls.Rendering.Implementations
{
	public class HtmlSvgScoreRenderer : HtmlScoreRenderer<XElement>
	{
		private const string EmptyCharacterWithWidth = "j";

		public HtmlSvgScoreRenderer()
			: base(null)
		{
		}

		public HtmlSvgScoreRenderer(XElement element, string svgCanvasName, HtmlScoreRendererSettings settings)
			: base(element)
		{
			Settings = settings;
		}

		public override void DrawArc(Rectangle rect, double startAngle, double sweepAngle, Pen pen, Model.MusicalSymbol owner)
		{
			var element = new XElement("path",
				new XAttribute("d", string.Format("M{0},{1} A{2},{3} {4} {5},{6} {7},{8}",
					rect.X.ToStringInvariant(),
					rect.Y.ToStringInvariant(),
					rect.Height.ToStringInvariant(),
					rect.Width.ToStringInvariant(),
					(sweepAngle / 2).ToStringInvariant(),
					0,
					1,
					(rect.X + rect.Width).ToStringInvariant(),
					rect.Y.ToStringInvariant())),
				new XAttribute("style", pen.ToCss()));

			Canvas.Add(element);
		}

		public override void DrawBezier(Point p1, Point p2, Point p3, Point p4, Pen pen, Model.MusicalSymbol owner)
		{
			var element = new XElement("path",
				new XAttribute("d", string.Format("M{0},{1} C{2},{3} {4},{5} {6},{7}",
					p1.X.ToStringInvariant(),
					p1.Y.ToStringInvariant(),
					p2.X.ToStringInvariant(),
					p2.Y.ToStringInvariant(),
					p3.X.ToStringInvariant(),
					p3.Y.ToStringInvariant(),
					p4.X.ToStringInvariant(),
					p4.Y.ToStringInvariant())),
				new XAttribute("style", pen.ToCss()));

			Canvas.Add(element);
		}

		public override void DrawLine(Point startPoint, Point endPoint, Pen pen, Model.MusicalSymbol owner)
		{
			var element = new XElement("line",
				new XAttribute("x1", startPoint.X.ToStringInvariant()),
				new XAttribute("y1", startPoint.Y.ToStringInvariant()),
				new XAttribute("x2", endPoint.X.ToStringInvariant()),
				new XAttribute("y2", endPoint.Y.ToStringInvariant()),
				new XAttribute("style", pen.ToCss()));

			Canvas.Add(element);
		}

		public override void DrawString(string text, MusicFontStyles fontStyle, Point location, Color color, Model.MusicalSymbol owner)
		{
			if (!TypedSettings.Fonts.ContainsKey(fontStyle)) return;   //Nie ma takiego fontu zdefiniowanego. Nie rysuj.

			location = TranslateTextLocation(location, fontStyle);

			var element = new XElement("text",
				new XAttribute("x", location.X.ToStringInvariant()),
				new XAttribute("y", location.Y.ToStringInvariant()),
				new XAttribute("style", string.Format("font-color:{0}; font-size:{1}pt; font-family: {2};",
					color.ToCss(),
					TypedSettings.Fonts[fontStyle].Size.ToStringInvariant(),
					TypedSettings.Fonts[fontStyle].Name)));
			element.Value = TypedSettings.Fonts[fontStyle].Name == "Polihymnia" ? string.Format("{0}{1}", text, EmptyCharacterWithWidth) : text;
			Canvas.Add(element);
		}

		public override void DrawStringInBounds(string text, MusicFontStyles fontStyle, Point location, Size size, Color color, Model.MusicalSymbol owner)
		{
		}

		protected override void DrawPlaybackCursor(PlaybackCursorPosition position, Point start, Point end)
		{
		}
	}
}