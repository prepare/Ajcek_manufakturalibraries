﻿using Manufaktura.Music.Model;
using System.Linq;

namespace Manufaktura.Controls.Model.Rules
{
    /// <summary>
    /// Rule that applies proper stem direction for notes on the staff. Every note on the third line and above has downward stem direction.
    /// This rule is applied when rhythmic duration of inserted note is not RhythmicDuration.Whole and SubjectToNoteStemRule on Note is set to true.
    /// </summary>
    public class NoteStemRule : StaffRule<Note>
    {
        public override void Apply(Staff staff, Note newElement)
        {
            var clef = staff.Elements.OfType<Clef>().LastOrDefault(c => staff.Elements.IndexOf(c) < staff.Elements.IndexOf(newElement));
            if (clef == null) return;

            var line = newElement.GetLineInSpecificClef(clef);
            newElement.StemDirection = line >= 3 ? VerticalDirection.Down : VerticalDirection.Up;
        }

        public override bool Condition(Staff staff, Note newElement)
        {
            return newElement.Duration != RhythmicDuration.Whole && newElement.SubjectToNoteStemRule;
        }
    }
}