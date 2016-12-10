﻿using Manufaktura.Controls.Model;
using Manufaktura.Controls.Model.Exceptions;
using Manufaktura.Music.Model;
using System.Xml.Linq;

namespace Manufaktura.Controls.Parser.MusicXml.Strategies
{
    public class NoteWritingStrategy : MusicXmlWritingStrategy<Note>
    {
        public override string ElementName => "note";

        public override bool IsAttributeElement => false;

        internal static string GetDurationName(NoteOrRest symbol)
        {
            switch (symbol.BaseDuration.Denominator)
            {
                case 1:
                    return "whole";

                case 2:
                    return "half";

                case 4:
                    return "quarter";

                case 8:
                    return "eighth";

                case 16:
                    return "16th";

                case 32:
                    return "32nd";

                case 64:
                    return "64th";

                case 128:
                    return "128th";

                case 256:
                    return "256th";

                case 512:
                    return "512th";

                default:
                    throw new ScoreException(symbol, $"Invalid rhythmic duration denominator {symbol.BaseDuration.Denominator}.");
            }
        }

        protected override void WriteElementInner(Note symbol, XElement element, int quarterNoteDuration)
        {
            var pitchNode = new XElement("pitch");
            element.Add(pitchNode);

            pitchNode.Add(new XElement("step", symbol.Pitch.StepName));
            pitchNode.Add(new XElement("octave", symbol.Pitch.Octave));
            if (symbol.Alter != 0) pitchNode.Add(new XElement("alter", symbol.Pitch.Alter));

            element.Add(new XElement("duration", (symbol.Duration.ToDouble() * quarterNoteDuration) / RhythmicDuration.Quarter.ToDouble()));

            if (symbol.Voice != 0) element.Add(new XElement("voice", symbol.Voice));
            element.Add(new XElement("type", GetDurationName(symbol)));

            for (var dot = 0; dot < symbol.NumberOfDots; dot++) element.Add(new XElement("dot"));

            var stemElement = new XElement("stem", symbol.StemDirection == VerticalDirection.Up ? "up" : "down");
            if (symbol.StemDefaultY != 0) stemElement.Add(new XAttribute("default-y", symbol.StemDefaultY));
            element.Add(stemElement);
            int i = 1;
            foreach (var beam in symbol.BeamList)
            {
                var beamElement = new XElement("beam", GetBeamType(symbol, beam));
                beamElement.Add(new XAttribute("number", i));
                element.Add(beamElement);
                i++;
            }

            foreach (var lyric in symbol.Lyrics)
            {
                var lyricElement = new XElement("lyric");
                element.Add(lyricElement);
                if (lyric.DefaultYPosition.HasValue) lyricElement.Add(new XAttribute("default-y", lyric.DefaultYPosition.Value));
                if (lyric.Number != 0) lyricElement.Add(new XAttribute("number", lyric.Number));
                foreach (var syllable in lyric.Syllables)
                {
                    lyricElement.Add(new XElement("syllabic", lyric.Type.ToString().ToLowerInvariant()));
                    lyricElement.Add(new XElement("text", lyric.Text));
                }
            }
        }

        private static string GetBeamType(Note symbol, NoteBeamType beamType)
        {
            switch (beamType)
            {
                case NoteBeamType.BackwardHook:
                    return "backward hook";

                case NoteBeamType.ForwardHook:
                    return "forward hook";

                case NoteBeamType.Continue:
                    return "continue";

                case NoteBeamType.End:
                    return "end";

                case NoteBeamType.Single:
                    return "single";

                case NoteBeamType.Start:
                    return "begin";

                default:
                    throw new ScoreException(symbol, $"Unsupported beam type {beamType}.");
            }
        }
    }
}