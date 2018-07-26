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
using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Model.SMuFL;
using Manufaktura.Controls.Primitives;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Manufaktura.Controls.Rendering
{
    /// <summary>
    /// Initial settings of score renderer.
    /// </summary>
    public class ScoreRendererSettings
    {
        private IMusicFont currentFont;

        /// <summary>
        /// Initializes a new instance of ScoreRendererSettings
        /// </summary>
        public ScoreRendererSettings()
        {
            RenderingMode = ScoreRenderingModes.Panorama;
            IgnoreCustomElementPositions = false;
            CustomElementPositionRatio = 0.7d;
            PageWidth = 200;
            PaddingTop = 20;
            LineSpacing = 6;
            DefaultColor = Color.Black;
            CurrentFont = new PolihymniaFont();
        }

        /// <summary>
        /// Key mapping for current font
        /// </summary>
        public IMusicFont CurrentFont { get => currentFont; set { currentFont = value; IsSMuFLFont = value is SMuFLMusicFont; } }

        /// <summary>
        /// Page to display if renderer is in SinglePage mode.
        /// </summary>
        public int CurrentPage { get; set; }

        public ISMuFLFontMetadata CurrentSMuFLMetadata { get; set; }
        public double CustomElementPositionRatio { get; set; }
        public double DefaultBarlineThickness { get; set; } = 0.7;
        public double DefaultBeamThickness { get; set; } = 2.6;
        /// <summary>
        /// Default color
        /// </summary>
        public Color DefaultColor { get; set; }

        public double DefaultStaffLineThickness { get; set; } = 0.5;

        public double DefaultStemThickness { get; set; } = 1;

        public double DefaultTupletBracketThickness { get; set; } = 1;

        /// <summary>
        /// True, to ignore element positions which are implicitly set in MusicXml file
        /// </summary>
        public bool IgnoreCustomElementPositions { get; set; }

        /// <summary>
        /// If true, all staves will have equal width (like in blank music paper)
        /// </summary>
        public bool IsMusicPaperMode { get; set; }
        public bool IsSMuFLFont { get; private set; }

        /// <summary>
        /// Default line spacing
        /// </summary>
        public int LineSpacing { get; private set; }

        /// <summary>
        /// Default padding top
        /// </summary>
        public int PaddingTop { get; private set; }

        /// <summary>
        /// Default page width
        /// </summary>
        public double PageWidth { get; set; }

        /// <summary>
        /// Rendering moed
        /// </summary>
        public ScoreRenderingModes RenderingMode { get; set; }

        public void LoadSMuFLMetadata(string metadataJson)
        {
            CurrentSMuFLMetadata = JsonConvert.DeserializeObject<SMuFLFontMetadata>(metadataJson);
            CurrentFont = new SMuFLMusicFont();
        }

        public void LoadSMuFLMetadata(ISMuFLFontMetadata metadata)
        {
            CurrentSMuFLMetadata = metadata;
            CurrentFont = new SMuFLMusicFont();
        }

        public void LoadSMuFLMetadata(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                LoadSMuFLMetadata(text);
            }
        }

        public async Task LoadSMuFLMetadataAsync(string metadataJson)
        {
            CurrentSMuFLMetadata = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<SMuFLFontMetadata>(metadataJson));
            CurrentFont = new SMuFLMusicFont();
        }

        public async Task LoadSMuFLMetadataAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var text = await reader.ReadToEndAsync();
                await LoadSMuFLMetadataAsync(text);
            }
        }

        public virtual void SetPolihymniaFont()
        {
            CurrentFont = new PolihymniaFont();
            CurrentSMuFLMetadata = null;
        }
    }
}