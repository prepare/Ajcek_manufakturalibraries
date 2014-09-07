﻿using Manufaktura.Controls.Audio;
using Manufaktura.Controls.Model;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Manufaktura.Controls.Silverlight
{
    public class XnaScorePlayer : ThreadScorePlayer
    {
        private Dictionary<int, string> _stepNames;

        public XnaScorePlayer(Score score) : base(score)
        {
            _stepNames = new Dictionary<int, string> { { 68, "gg" }, { 69, "a" }, { 70, "bb" }, { 71, "b" }, { 72, "c" }, { 73, "cc" }, { 74, "d" }, 
                                                                              { 75, "eb" }, { 76, "e" }, { 77, "f" }, { 78, "ff" }, { 79, "g" } };
            for (int i = 80; i < 93; i++)
            {
                _stepNames.Add(i, _stepNames[i - 12]);
            }
            for (int i = 56; i < 68; i++)
            {
                _stepNames.Add(i, _stepNames[i + 12]);
            }
        }

        public override void PlayElement(MusicalSymbol element, Staff staff)
        {
            if (element is Rest) return;
            Note note = element as Note;
            if (note == null) return;
            if (note.MidiPitch > 92 || note.MidiPitch < 56) return;

            int octaveModifier = 0;
            if (note.MidiPitch > 79) octaveModifier = 1;
            if (note.MidiPitch < 68) octaveModifier = -1;
            
            string soundUri = string.Format("Manufaktura.Controls.Silverlight;component/piano-{0}.wav", _stepNames[note.MidiPitch]);
            var resourceStream = Application.GetResourceStream(new Uri(soundUri, UriKind.RelativeOrAbsolute));
            var effect = SoundEffect.FromStream(resourceStream.Stream);

            var firstNoteInMeasure = staff.Peek<Note>(element, Staff.PeekType.BeginningOfMeasure, Staff.PeekDirection.Backward);
            effect.Play(element == firstNoteInMeasure ? 0.9f : 0.5f, octaveModifier, 0);
        }
    }
}
