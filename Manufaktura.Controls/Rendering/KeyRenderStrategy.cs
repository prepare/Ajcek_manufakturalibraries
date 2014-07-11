﻿using Manufaktura.Controls.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
    class KeyRenderStrategy : MusicalSymbolRenderStrategy<Key>
    {
        public override void Render(Key element, ScoreRendererBase renderer)
        {
            renderer.State.currentKey = element;
            double flatOrSharpPositionY = 0;
            bool jumpFourth = false;
            int jumpDirection = 1;
            int octaveShiftSharp = 0; //In G clef sharps (not flats) should be written an octave higher / W kluczu g krzyżyki (bemole nie) powinny być zapisywane o oktawę wyżej
            if (renderer.State.CurrentClef.TypeOfClef == ClefType.GClef) octaveShiftSharp = 1;
            int octaveShiftFlat = 0;
            if (renderer.State.CurrentClef.TypeOfClef == ClefType.FClef) octaveShiftFlat = -1;
            if (renderer.State.currentKey.Fifths > 0)
            {
                flatOrSharpPositionY = renderer.State.currentClefPositionY + MusicalSymbol.StepDifference(renderer.State.CurrentClef,
                (new Note("F", 0, renderer.State.CurrentClef.Octave + octaveShiftSharp, MusicalSymbolDuration.Whole, NoteStemDirection.Up,
                    NoteTieType.None, null)))
                * (renderer.State.lineSpacing / 2);
                jumpFourth = true;
                jumpDirection = 1;

            }
            else if (renderer.State.currentKey.Fifths < 0)
            {
                flatOrSharpPositionY = renderer.State.currentClefPositionY + MusicalSymbol.StepDifference(renderer.State.CurrentClef,
                (new Note("B", 0, renderer.State.CurrentClef.Octave + octaveShiftFlat, MusicalSymbolDuration.Whole, NoteStemDirection.Up,
                    NoteTieType.None, null)))
                * (renderer.State.lineSpacing / 2);
                jumpFourth = true;
                jumpDirection = -1;
            }
            for (int i = 0; i < Math.Abs(renderer.State.currentKey.Fifths); i++)
            {

                renderer.DrawString(element.MusicalCharacter, FontStyles.MusicFont, renderer.State.currentXPosition, flatOrSharpPositionY);
                if (jumpFourth) flatOrSharpPositionY += 3 * 3 * jumpDirection;
                else flatOrSharpPositionY += 3 * 4 * jumpDirection;
                jumpFourth = !jumpFourth;
                jumpDirection *= -1;
                renderer.State.currentXPosition += 8;
            }
            renderer.State.currentXPosition += 10;
        }
    }
}
