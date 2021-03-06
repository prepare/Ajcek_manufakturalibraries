﻿using Manufaktura.Controls.Model.SMuFL;
using System.Collections.Generic;

namespace Manufaktura.Controls.Model.Fonts
{
    public abstract class FontProfile
    {
        public Dictionary<MusicFontStyles, double> FontSizes { get; } = new Dictionary<MusicFontStyles, double>
        {
            { MusicFontStyles.MusicFont, 25 },
            { MusicFontStyles.GraceNoteFont, 16 },
            { MusicFontStyles.StaffFont, 30.5 },
            { MusicFontStyles.LyricsFont, 11},
            { MusicFontStyles.DirectionFont, 11},
            { MusicFontStyles.TimeSignatureFont, 14.5}
        };

        public abstract IMusicFont MusicFont { get; }
        public abstract ISMuFLFontMetadata SMuFLMetadata { get; }
        public bool IsSMuFLFont => MusicFont.IsSMuFLFont && SMuFLMetadata != null;

        public FontProfile SetFontSize(MusicFontStyles style, double size)
        {
            FontSizes[style] = size;
            return this;
        }
    }
}