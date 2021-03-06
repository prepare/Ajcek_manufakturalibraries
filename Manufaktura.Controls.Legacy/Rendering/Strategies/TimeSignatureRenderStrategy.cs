﻿using Manufaktura.Controls.Model;
using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
    /// <summary>
    /// Strategy for rendering a TimeSignature.
    /// </summary>
    public class TimeSignatureRenderStrategy : MusicalSymbolRenderStrategy<TimeSignature>
    {
        private readonly IScoreService scoreService;
        public TimeSignatureRenderStrategy(IScoreService scoreService)
        {
            this.scoreService = scoreService;
        }
        public override void Render(TimeSignature element, ScoreRendererBase renderer)
        {
            double timeSignaturePositionY = (scoreService.CurrentLinePositions[0] - 11);
            if (element.SignatureType == TimeSignatureType.Common)
                renderer.DrawString(renderer.Settings.CurrentFont.CommonTime, MusicFontStyles.MusicFont, 
                scoreService.CursorPositionX, timeSignaturePositionY, element);
            else if (element.SignatureType == TimeSignatureType.Cut)
                renderer.DrawString(renderer.Settings.CurrentFont.CutTime, MusicFontStyles.MusicFont, 
                scoreService.CursorPositionX, timeSignaturePositionY, element);
            else
            {
                renderer.DrawString(Convert.ToString(element.NumberOfBeats),
                    MusicFontStyles.TimeSignatureFont, scoreService.CursorPositionX, timeSignaturePositionY + 9, element);
                renderer.DrawString(Convert.ToString(element.TypeOfBeats),
                    MusicFontStyles.TimeSignatureFont, scoreService.CursorPositionX, timeSignaturePositionY + 21, element);
            }
            scoreService.CursorPositionX += 20;
        }
    }
}
