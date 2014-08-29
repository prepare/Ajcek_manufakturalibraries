﻿using Manufaktura.Controls.Model;
using Manufaktura.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
    class BarlineRenderStrategy : MusicalSymbolRenderStrategy<Barline>
    {
        public override void Render(Barline element, ScoreRendererBase renderer)
        {
            if (renderer.State.lastNoteInMeasureEndXPosition > renderer.State.CursorPositionX)
            {
                renderer.State.CursorPositionX = renderer.State.lastNoteInMeasureEndXPosition;
            }
            double? measureWidth = GetCursorPositionForCurrentBarline(renderer);
            if (!renderer.Settings.IgnoreCustomElementPositions && measureWidth.HasValue) renderer.State.CursorPositionX = measureWidth.Value;

            if (element.RepeatSign == RepeatSignType.None)
            {
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX += 16;
                renderer.State.LastMeasurePositionX = renderer.State.CursorPositionX;
                renderer.DrawLine(new Point(renderer.State.CursorPositionX, renderer.State.LinePositions[4]), new Point(renderer.State.CursorPositionX, renderer.State.LinePositions[0]), element);
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX += 6;
            }
            else if (element.RepeatSign == RepeatSignType.Forward)
            {
                //Przesuń w lewo jeśli przed znakiem repetycji znajduje się zwykła kreska taktowa
                //Move to the left if there is a plain measure bar before the repeat sign
                if (renderer.State.CurrentStaff.Elements.IndexOf(element) > 0)
                {
                    MusicalSymbol s = renderer.State.CurrentStaff.Elements[renderer.State.CurrentStaff.Elements.IndexOf(element) - 1];
                    if (s.Type == MusicalSymbolType.Barline)
                    {
                        if (((Barline)s).RepeatSign == RepeatSignType.None)
                        {
                            if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX -= 16;
                        }
                    }
                }
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX += 2;
                renderer.State.LastMeasurePositionX = renderer.State.CursorPositionX;
                renderer.DrawString(MusicalCharacters.RepeatForward, FontStyles.StaffFont, renderer.State.CursorPositionX,
                    renderer.State.LinePositions[0] - 15.5f, element);
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX += 20;
            }
            else if (element.RepeatSign == RepeatSignType.Backward)
            {
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX -= 2;
                renderer.State.LastMeasurePositionX = renderer.State.CursorPositionX;
                renderer.DrawString(MusicalCharacters.RepeatBackward, FontStyles.StaffFont, renderer.State.CursorPositionX,
                    renderer.State.LinePositions[0] - 15.5f, element);
                if (renderer.Settings.IgnoreCustomElementPositions || !measureWidth.HasValue) renderer.State.CursorPositionX += 6;
            }
            renderer.State.firstNoteInMeasureXPosition = renderer.State.CursorPositionX;

            for (int i = 0; i < 7; i++)
                renderer.State.alterationsWithinOneBar[i] = 0;

            renderer.State.CurrentMeasure++;
        }

        public double? GetCursorPositionForCurrentBarline(ScoreRendererBase renderer)
        {
            Staff staff = renderer.State.CurrentStaff;
            if (staff.MeasureWidths.Count <= renderer.State.CurrentMeasure) return null;
            double? width = staff.MeasureWidths[renderer.State.CurrentMeasure];
            if (!width.HasValue) return null;
            return renderer.State.LastMeasurePositionX + width * renderer.Settings.CustomElementPositionRatio;
        }
    }
}