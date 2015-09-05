﻿using Manufaktura.Controls.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.Controls.Formatting
{
    public class DefaultScoreFormatter : IScoreFormatter
    {
        public Score Format(Score score)
        {
            SetMeasureWidths(score);
            SetElementPositions(score);
            return score;
        }

        private IEnumerable<IEnumerable<Measure>> GetSystems(Score score)
        {
            var systems = new List<List<Measure>>();
            var measures = new List<Measure>();
            var currentSystem = new StaffSystem();
            foreach (var element in score.FirstStaff.Elements)
            {
                if (element is Barline) measures.Add(new Measure(score.FirstStaff, currentSystem));
                var suggestion = element as PrintSuggestion;
                if (suggestion != null && (suggestion.IsSystemBreak || suggestion.IsPageBreak))
                {
                    systems.Add(measures);
                    measures = new List<Measure>();
                    currentSystem = new StaffSystem();
                }
            }
            return systems;
        }

        private void SetElementPositions(Score score)
        {
            foreach (var staff in score.Staves)
            {
                foreach (var measure in staff.Measures)
                {
                    var marginLeft = staff.Measures.IndexOf(measure) == 0 ? 85 : 14;
                    double x = marginLeft;
                    foreach (var element in measure.Elements)
                    {
                        if (!measure.Width.HasValue) throw new Exception("Measure does not have width. Run SetMeasureWidths first.");
                        var chordElement = element as ICanBeElementOfChord;
                        if (chordElement != null && chordElement.IsChordElement) continue;
                        var durationElement = element as IHasDuration;
                        if (durationElement == null) continue;
                        var defaultXElement = element as IHasCustomXPosition;
                        if (defaultXElement == null) continue;
                        defaultXElement.DefaultXPosition = x;
                        var time = staff.Peek<TimeSignature>(element, Model.PeekStrategies.PeekType.PreviousElement);
                        var duration = durationElement.BaseDuration.ToDouble() * time.WholeNoteCapacity;    //TODO: Dots!
                        x += (measure.Width.Value - marginLeft) * duration;
                    }
                }
            }
        }

        private void SetMeasureWidths(Score score)
        {
            int index = 0;
            foreach (var system in GetSystems(score))
            {
                var averageMeasureWidth = score.DefaultPageSettings.Width / system.Count();
                foreach (var measure in system)
                {
                    foreach (var staff in score.Staves)
                    {
                        Measure staffMeasure;
                        if (staff.Measures.Count > index)
                            staffMeasure = staff.Measures[index];
                        else
                        {
                            staffMeasure = new Measure(staff, null);
                            staff.Measures.Add(staffMeasure);
                        }
                        staffMeasure.Width = averageMeasureWidth;
                    }
                    index++;
                }
            }
        }
    }
}