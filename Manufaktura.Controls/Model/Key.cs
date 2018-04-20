﻿using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Music.Model;
using Manufaktura.Music.Model.MajorAndMinor;

namespace Manufaktura.Controls.Model
{
    /// <summary>
    /// Represents a key signature.
    /// </summary>
    public class Key : MusicalSymbol, IRenderedAsTextBlock
    {
        private int fifths;

        /// <summary>
        /// Initializes a new instance of key
        /// </summary>
        /// <param name="numberOfFifths"></param>
        public Key(int numberOfFifths)
        {
            fifths = numberOfFifths;
        }

        /// <summary>
        /// Number on the Circle of Fifths
        /// </summary>
        public int Fifths { get { return fifths; } }

        public Primitives.Point TextBlockLocation { get; internal set; }

        /// <summary>
        /// Creates a new instance of Key from given midi pitch.
        /// </summary>
        /// <param name="midiPitch">Midi pitch</param>
        /// <param name="flags">Flags</param>
        /// <returns>Key</returns>
        public static Key FromMidiPitch(int midiPitch, MajorAndMinorScaleFlags flags)
        {
            return new Key(CircleOfFifths.CalculateFifths(midiPitch, flags));
        }

        /// <summary>
        /// Creates a new instance of Key from given Scale
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Key FromScale(MajorOrMinorScale scale)
        {
            return new Key(scale.Fifths);
        }

        /// <summary>
        /// Creates a new instance of Key from given step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Key FromTonic(Step step, MajorAndMinorScaleFlags flags)
        {
            return new Key(CircleOfFifths.CalculateFifths(step, flags));
        }

        public char GetCharacter(IMusicFont font)
        {
            if (fifths > 0)
                return font.Sharp;
            else if (fifths < 0)
                return font.Flat;
            else
                return '\0';
        }

        /// <summary>
        /// Returns the alteration of specific step.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public int StepToAlter(string step)
        {
            return CircleOfFifths.GetAlterOfStepFromNumberOfFifths(step, Fifths);
        }

        /// <summary>
        /// Returns a string representation of this symbol for debugging purposes
        /// </summary>
        /// <returns>String representation of this symbol for debugging purposes</returns>
        public override string ToString()
        {
            return string.Format("{0} with {1} generator intervals.", base.ToString(), Fifths);
        }
    }
}