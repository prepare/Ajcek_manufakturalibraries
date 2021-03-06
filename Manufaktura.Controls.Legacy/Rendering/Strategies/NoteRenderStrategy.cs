﻿using Manufaktura.Controls.Model;
using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Primitives;
using Manufaktura.Controls.Rendering.Snippets;
using Manufaktura.Controls.Services;
using Manufaktura.Music.Model;
using System;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
	public class NoteRenderStrategy : MusicalSymbolRenderStrategy<Note>
	{
		private readonly IAlterationService alterationService;
		private readonly IBeamingService beamingService;
		private readonly IMeasurementService measurementService;
		private readonly IScoreService scoreService;
		private VerticalPlacement slurStartPlacement = VerticalPlacement.Above;

		public NoteRenderStrategy(IMeasurementService measurementService, IAlterationService alterationService, IScoreService scoreService, IBeamingService beamingService)
		{
			this.measurementService = measurementService;
			this.alterationService = alterationService;
			this.scoreService = scoreService;
			this.beamingService = beamingService;
		}

		public override void Render(Note element, ScoreRendererBase renderer)
		{
			//Jeśli ustalono default-x, to pozycjonuj wg default-x, a nie automatycznie
			if (!renderer.Settings.IgnoreCustomElementPositions && element.DefaultXPosition.HasValue)
			{
				scoreService.CursorPositionX = measurementService.LastMeasurePositionX + element.DefaultXPosition.Value * renderer.Settings.CustomElementPositionRatio;
			}

			if (scoreService.CurrentMeasure.FirstNoteInMeasureXPosition == 0) scoreService.CurrentMeasure.FirstNoteInMeasureXPosition = scoreService.CursorPositionX;

			//If it's second voice, rewind position to the beginning of measure (but only if default-x is not set or is ignored):
			if (element.Voice > scoreService.CurrentVoice && (renderer.Settings.IgnoreCustomElementPositions || !element.DefaultXPosition.HasValue))
			{
				scoreService.CursorPositionX = scoreService.CurrentMeasure.FirstNoteInMeasureXPosition;
				measurementService.LastNoteInMeasureEndXPosition = measurementService.LastNoteEndXPosition;
			}
			scoreService.CurrentVoice = element.Voice;

			if (element.Tuplet == TupletType.Start)
			{
				Tuplet tuplet = new Tuplet();
				measurementService.TupletState = tuplet;
				tuplet.NumberOfNotesUnderTuplet = 0;
				tuplet.TupletPlacement = element.TupletPlacement.HasValue ? element.TupletPlacement.Value :
					(element.StemDirection == VerticalDirection.Down ? VerticalPlacement.Below : VerticalPlacement.Above);
			}
			if (measurementService.TupletState != null && !element.IsUpperMemberOfChord) measurementService.TupletState.NumberOfNotesUnderTuplet++;

			if (element.IsUpperMemberOfChord) scoreService.CursorPositionX = measurementService.LastNotePositionX;

			double noteTextBlockPositionY = CalculateNotePositionY(element, renderer);

			int numberOfSingleAccidentals = Math.Abs(element.Alter) % 2;
			int numberOfDoubleAccidentals = Convert.ToInt32(Math.Floor((double)(Math.Abs(element.Alter) / 2)));

			MakeSpaceForAccidentals(renderer, element,
				numberOfSingleAccidentals, numberOfDoubleAccidentals);          //Move the element a bit to the right if it has accidentals / Przesuń nutę trochę w prawo, jeśli nuta ma znaki przygodne
			DrawNote(renderer, element, noteTextBlockPositionY);                //Draw an element / Rysuj nutę
			DrawLedgerLines(renderer, element, noteTextBlockPositionY);         //Ledger lines / Linie dodane
			DrawStems(renderer, element, noteTextBlockPositionY);               //Stems are vertical lines, beams are horizontal lines / Rysuj ogonki (ogonki to są te w pionie - poziome są belki)
			DrawFlagsAndTupletMarks(renderer, element);                         //Draw beams / Rysuj belki
			DrawTies(renderer, element, noteTextBlockPositionY);                //Draw ties / Rysuj łuki
			DrawSlurs(renderer, element, noteTextBlockPositionY);               //Draw slurs / Rysuj łuki legatowe
			DrawLyrics(renderer, element);                                      //Draw lyrics / Rysuj tekst
			DrawArticulation(renderer, element, noteTextBlockPositionY);        //Draw articulation / Rysuj artykulację:
			DrawTrills(renderer, element, noteTextBlockPositionY);              //Draw trills / Rysuj tryle //TODO: REFAKTOR - Przenieść do DrawOrnaments
			DrawOrnaments(renderer, element, noteTextBlockPositionY);           //Draw ornaments / Rysuj ornamenty
			DrawTremolos(renderer, element, noteTextBlockPositionY);            //Draw tremolos / Rysuj tremola
			DrawFermataSign(renderer, element, noteTextBlockPositionY);         //Draw fermata sign / Rysuj symbol fermaty
			DrawAccidentals(renderer, element, noteTextBlockPositionY,
				numberOfSingleAccidentals, numberOfDoubleAccidentals);          //Draw accidentals / Rysuj znaki przygodne:
			DrawDots(renderer, element, noteTextBlockPositionY);                //Draw dots / Rysuj kropki

			if (renderer.Settings.IgnoreCustomElementPositions || !element.DefaultXPosition.HasValue) //Pozycjonowanie automatyczne tylko, gdy nie określono default-x
			{
				if (element.Duration == RhythmicDuration.Whole) scoreService.CursorPositionX += 50;
				else if (element.Duration == RhythmicDuration.Half) scoreService.CursorPositionX += 30;
				else if (element.Duration == RhythmicDuration.Quarter) scoreService.CursorPositionX += 18;
				else if (element.Duration == RhythmicDuration.Eighth) scoreService.CursorPositionX += 15;
				else scoreService.CursorPositionX += 14;

				//Przesuń trochę w prawo, jeśli nuta ma tekst, żeby litery nie wchodziły na siebie
				//Move a bit right if the element has a lyric to prevent letters from hiding each other
				if (element.Lyrics.Count > 0)
				{
					scoreService.CursorPositionX += element.Lyrics[0].Text.Length * 2;
				}
			}
			measurementService.LastNoteEndXPosition = scoreService.CursorPositionX;
		}

		private double CalculateNotePositionY(Note element, ScoreRendererBase renderer)
		{
			return scoreService.CurrentClef.TextBlockLocation.Y + Pitch.StepDistance(scoreService.CurrentClef.Pitch,
				element.Pitch) * ((double)renderer.Settings.LineSpacing / 2.0f);
		}

		private void DrawAccidentals(ScoreRendererBase renderer, Note element, double notePositionY, int numberOfSingleAccidentals, int numberOfDoubleAccidentals)
		{
			if (element.Alter - scoreService.CurrentKey.StepToAlter(element.Step) - alterationService.Get(element.Step) > 0)
			{
				alterationService.Set(element.Step, element.Alter - scoreService.CurrentKey.StepToAlter(element.Step));
				double accPlacement = scoreService.CursorPositionX - 9 * numberOfSingleAccidentals - 9 * numberOfDoubleAccidentals;
				for (int i = 0; i < numberOfSingleAccidentals; i++)
				{
					renderer.DrawString(renderer.Settings.CurrentFont.Sharp, MusicFontStyles.MusicFont, accPlacement, notePositionY, element);
					accPlacement += 9;
				}
				for (int i = 0; i < numberOfDoubleAccidentals; i++)
				{
					renderer.DrawString(renderer.Settings.CurrentFont.DoubleSharp, MusicFontStyles.MusicFont, accPlacement, notePositionY, element);
					accPlacement += 9;
				}
			}
			else if (element.Alter - scoreService.CurrentKey.StepToAlter(element.Step) - alterationService.Get(element.Step) < 0)
			{
				alterationService.Set(element.Step, element.Alter - scoreService.CurrentKey.StepToAlter(element.Step));
				double accPlacement = scoreService.CursorPositionX - 9 * numberOfSingleAccidentals -
					9 * numberOfDoubleAccidentals;
				for (int i = 0; i < numberOfSingleAccidentals; i++)
				{
					renderer.DrawString(renderer.Settings.CurrentFont.Flat, MusicFontStyles.MusicFont, accPlacement, notePositionY, element);
					accPlacement += 9;
				}
				for (int i = 0; i < numberOfDoubleAccidentals; i++)
				{
					renderer.DrawString(renderer.Settings.CurrentFont.DoubleFlat, MusicFontStyles.MusicFont, accPlacement, notePositionY, element);
					accPlacement += 9;
				}
			}
			if (element.HasNatural == true)
			{
				renderer.DrawString(renderer.Settings.CurrentFont.Natural, MusicFontStyles.MusicFont, scoreService.CursorPositionX - 9, notePositionY, element);
			}
		}

		private void DrawArticulation(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.Articulation != ArticulationType.None)
			{
				double articulationPosition = notePositionY + 10;
				if (element.ArticulationPlacement == VerticalPlacement.Above)
					articulationPosition = notePositionY - 10;
				else if (element.ArticulationPlacement == VerticalPlacement.Below)
					articulationPosition = notePositionY + 10;

				if (element.Articulation == ArticulationType.Staccato)
					renderer.DrawString(renderer.Settings.CurrentFont.Dot, MusicFontStyles.MusicFont, scoreService.CursorPositionX + 6, articulationPosition, element);
				else if (element.Articulation == ArticulationType.Accent)
					renderer.DrawString(">", MusicFontStyles.MiscArticulationFont, scoreService.CursorPositionX + 6, articulationPosition + 16, element);
			}
		}

		private void DrawDots(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.NumberOfDots > 0) scoreService.CursorPositionX += 16;
			for (int i = 0; i < element.NumberOfDots; i++)
			{
				renderer.DrawString(renderer.Settings.CurrentFont.Dot, MusicFontStyles.MusicFont, scoreService.CursorPositionX, notePositionY, element);
				scoreService.CursorPositionX += 6;
			}
		}

		private void DrawFermataSign(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.HasFermataSign)
			{
				double ferPos = notePositionY - renderer.Settings.TextBlockHeight;
				string fermataVersion = renderer.Settings.CurrentFont.FermataUp;

				renderer.DrawString(fermataVersion, MusicFontStyles.MusicFont, scoreService.CursorPositionX, ferPos, element);
			}
		}

		private void DrawFlagsAndTupletMarks(ScoreRendererBase renderer, Note element)
		{
			int beamOffset = 0;
			//Powiększ listę poprzednich pozycji stemów jeśli aktualna liczba belek jest większa
			//Extend the list of previous stem positions if current number of beams is greater than the list size
			if (beamingService.PreviousStemEndPositionsY.Count < element.BeamList.Count)
			{
				int tmpCount = beamingService.PreviousStemEndPositionsY.Count;
				for (int i = 0; i < element.BeamList.Count - tmpCount; i++)
					beamingService.PreviousStemEndPositionsY.Add(new int());
			}
			if (beamingService.PreviousStemPositionsX.Count < element.BeamList.Count)
			{
				int tmpCount = beamingService.PreviousStemPositionsX.Count;
				for (int i = 0; i < element.BeamList.Count - tmpCount; i++)
					beamingService.PreviousStemPositionsX.Add(new int());
			}
			int beamLoop = 0;
			foreach (NoteBeamType beam in element.BeamList)
			{
				var beamSpaceDirection = element.StemDirection == VerticalDirection.Up ? 1 : -1;
				if (beam == NoteBeamType.Start)
				{
					if (beamLoop == 0)
					{
						beamingService.PreviousStemEndPositionsY[beamLoop] = beamingService.CurrentStemEndPositionY;
					}
					beamingService.PreviousStemPositionsX[beamLoop] = beamingService.CurrentStemPositionX;
				}
				else if (beam == NoteBeamType.End)
				{
					//Draw tuplet mark / Rysuj oznaczenie trioli:
					if (element.Tuplet == TupletType.Stop && measurementService.TupletState != null)
					{
						Beams.TupletMark(measurementService, scoreService, renderer, element, beamLoop);
						measurementService.TupletState = null;
					}
				}
				else if ((beam == NoteBeamType.Single) && (!element.IsUpperMemberOfChord))
				{
					Beams.Flag(beamingService, measurementService, scoreService, renderer, element, beamSpaceDirection, beamLoop, beamOffset);
				}

				beamOffset += 4;
				beamLoop++;
			}
		}

		private void DrawLedgerLines(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			double tmpXPos = scoreService.CursorPositionX + 16;
			if (notePositionY + 25.0f > scoreService.CurrentLinePositions[4] + renderer.Settings.LineSpacing / 2.0f)
			{
				for (double i = scoreService.CurrentLinePositions[4]; i < notePositionY + 24f - renderer.Settings.LineSpacing / 2.0f; i += renderer.Settings.LineSpacing)
				{
					renderer.DrawLine(new Point(scoreService.CursorPositionX + 4, i + renderer.Settings.LineSpacing),
						new Point(tmpXPos, i + renderer.Settings.LineSpacing), element);
				}
			}
			if (notePositionY + 25.0f < scoreService.CurrentLinePositions[0] - renderer.Settings.LineSpacing / 2)
			{
				for (double i = scoreService.CurrentLinePositions[0]; i > notePositionY + 26.0f + renderer.Settings.LineSpacing / 2.0f; i -= renderer.Settings.LineSpacing)
				{
					renderer.DrawLine(new Point(scoreService.CursorPositionX + 4, i - renderer.Settings.LineSpacing),
						new Point(tmpXPos, i - renderer.Settings.LineSpacing), element);
				}
			}
		}

		private void DrawLyrics(ScoreRendererBase renderer, Note element)
		{
			double versePositionY = scoreService.CurrentLinePositions[4] + 10;    //Default value if default-y is not set
			foreach (Lyrics lyrics in element.Lyrics)
			{
				var textPosition = lyrics.DefaultYPosition.HasValue ?
					scoreService.CurrentLinePositions[0] - renderer.TenthsToPixels(lyrics.DefaultYPosition.Value) :
					versePositionY;

				StringBuilder sBuilder = new StringBuilder();
				sBuilder.Append(lyrics.Text);

				//TODO: Dodać do kalkulacji wyliczoną szerokość stringa w poprzednim lyricu i odkomentować :)
				//A, i jeszcze wtedy wywalić warunek na middleDistance.
				//double middleDistanceBetweenTwoLyrics = (scoreService.CursorPositionX - renderer.State.LastNoteEndXPosition) / 2.0d;
				// double hyphenXPosition = scoreService.CursorPositionX - middleDistanceBetweenTwoLyrics;
				//if ((lyrics.Type == SyllableType.Middle || lyrics.Type == SyllableType.End) && middleDistanceBetweenTwoLyrics > 20)
				//{
				//    renderer.DrawString("-", FontStyles.LyricsFont, hyphenXPosition, textPositionY, element);
				//}
				//else
				if (lyrics.Type == SyllableType.Begin || lyrics.Type == SyllableType.Middle) sBuilder.Append("-");

				renderer.DrawString(sBuilder.ToString(), MusicFontStyles.LyricsFont, scoreService.CursorPositionX, textPosition, lyrics);

				if (!lyrics.DefaultYPosition.HasValue) versePositionY += 12; //Move down if default-y is not set
			}
		}

		private void DrawNote(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.IsGraceNote || element.IsCueNote)
				renderer.DrawString(element.MusicalCharacter, MusicFontStyles.GraceNoteFont, scoreService.CursorPositionX + 1, notePositionY + 7, element);
			else
				renderer.DrawString(element.MusicalCharacter, MusicFontStyles.MusicFont, scoreService.CursorPositionX, notePositionY, element);

			measurementService.LastNotePositionX = scoreService.CursorPositionX;
			element.TextBlockLocation = new Point(scoreService.CursorPositionX, notePositionY);
		}

		private void DrawOrnaments(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			foreach (Ornament ornament in element.Ornaments)
			{
				double yPositionShift = ornament.DefaultYPosition.HasValue ? renderer.TenthsToPixels(ornament.DefaultYPosition.Value) * -1 : (ornament.Placement == VerticalPlacement.Above ? -20 : 20);
				Mordent mordent = ornament as Mordent;
				if (mordent != null)
				{
					renderer.DrawString(renderer.Settings.CurrentFont.MordentShort, MusicFontStyles.GraceNoteFont, scoreService.CursorPositionX - 2, notePositionY + yPositionShift, element);
					renderer.DrawString(renderer.Settings.CurrentFont.Mordent, MusicFontStyles.GraceNoteFont, scoreService.CursorPositionX + 3.5, notePositionY + yPositionShift, element);
				}
			}
		}

		private void DrawSlurs(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.Slur == null) return;
			VerticalPlacement slurPlacement;
			if (element.Slur.Placement.HasValue) slurPlacement = element.Slur.Placement.Value;
			else slurPlacement = element.StemDirection == VerticalDirection.Up ? VerticalPlacement.Below : VerticalPlacement.Above;

			if (element.Slur.Type == NoteSlurType.Start)
			{
				slurStartPlacement = slurPlacement;
				if (slurPlacement == VerticalPlacement.Above)
					measurementService.SlurStartPoint = new Point(scoreService.CursorPositionX, element.StemDirection == VerticalDirection.Down ? notePositionY : notePositionY + element.StemDefaultY);
				else
					measurementService.SlurStartPoint = new Point(scoreService.CursorPositionX, notePositionY);
			}
			else if (element.Slur.Type == NoteSlurType.Stop)
			{
				if (slurStartPlacement == VerticalPlacement.Above)
				{
					renderer.DrawBezier(measurementService.SlurStartPoint.X + 10, measurementService.SlurStartPoint.Y + 18,
						measurementService.SlurStartPoint.X + 12, measurementService.SlurStartPoint.Y + 9,
						scoreService.CursorPositionX + 8, (element.StemDirection == VerticalDirection.Up ? element.StemDefaultY + scoreService.Systems.Take(scoreService.CurrentSystemNo - 1).Sum(s => s.Height) : notePositionY + 9),
						scoreService.CursorPositionX + 10, (element.StemDirection == VerticalDirection.Up ? element.StemDefaultY + scoreService.Systems.Take(scoreService.CurrentSystemNo - 1).Sum(s => s.Height) + 9 : notePositionY + 18), element);
				}
				else if (slurStartPlacement == VerticalPlacement.Below)
				{
					renderer.DrawBezier(measurementService.SlurStartPoint.X + 10, measurementService.SlurStartPoint.Y + 30,
						measurementService.SlurStartPoint.X + 12, measurementService.SlurStartPoint.Y + 44,
						scoreService.CursorPositionX + 8, notePositionY + 44,
						scoreService.CursorPositionX + 10, notePositionY + 30, element);
				}
			}
		}

		private void DrawStems(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.Duration == RhythmicDuration.Whole) return;

			double tmpStemPosY;
			tmpStemPosY = scoreService.CurrentStaffTop + renderer.TenthsToPixels(element.StemDefaultY);

			if (element.StemDirection == VerticalDirection.Down)
			{
				//Ogonki elementów akordów nie były dobrze wyświetlane, jeśli stosowałem
				//default-y. Dlatego dla akordów zostawiam domyślne rysowanie ogonków.
				//Stems of chord elements were displayed wrong when I used default-y
				//so I left default stem drawing routine for chords.
				if (element.IsUpperMemberOfChord)
					beamingService.CurrentStemEndPositionY = notePositionY + 18;
				else if (renderer.Settings.IgnoreCustomElementPositions || !element.HasCustomStemEndPosition)
					beamingService.CurrentStemEndPositionY = notePositionY + 18;
				else
					beamingService.CurrentStemEndPositionY = tmpStemPosY - 4;
				beamingService.CurrentStemPositionX = scoreService.CursorPositionX + 7 + (element.IsGraceNote || element.IsCueNote ? -0.5 : 0);
			}
			else
			{
				//Ogonki elementów akordów nie były dobrze wyświetlane, jeśli stosowałem
				//default-y. Dlatego dla akordów zostawiam domyślne rysowanie ogonków.
				//Stems of chord elements were displayed wrong when I used default-y
				//so I left default stem drawing routine for chords.
				if (element.IsUpperMemberOfChord)
					beamingService.CurrentStemEndPositionY = notePositionY - 25 < beamingService.CurrentStemEndPositionY ? beamingService.CurrentStemEndPositionY : notePositionY - 25;
				else if (renderer.Settings.IgnoreCustomElementPositions || !element.HasCustomStemEndPosition)
				{
					//var notesUnderBeam = beamingService.GetAllNotesUnderOneBeam(element);
					//var maxDistance = notesUnderBeam == null || !notesUnderBeam.Any() ? 0 : GetMaxVertDistanceBetweenNotes(renderer, notesUnderBeam.ToArray());
					//maxDistance *= 2;
					beamingService.CurrentStemEndPositionY = notePositionY - 25;
				}
				else
					beamingService.CurrentStemEndPositionY = tmpStemPosY - 6;
				beamingService.CurrentStemPositionX = scoreService.CursorPositionX + 13 + (element.IsGraceNote || element.IsCueNote ? -2 : 0);
			}

			var uglyModifier = element.StemDirection == VerticalDirection.Down ? 3 : 7;
			if (element.BeamList.Count > 0)
				if ((element.BeamList[0] != NoteBeamType.Continue) || element.HasCustomStemEndPosition)
					renderer.DrawLine(new Point(beamingService.CurrentStemPositionX, notePositionY - uglyModifier + 30),
						new Point(beamingService.CurrentStemPositionX, beamingService.CurrentStemEndPositionY + 28), element);
			element.StemEndLocation = new Point(beamingService.CurrentStemPositionX, beamingService.CurrentStemEndPositionY);
		}

		private void DrawTies(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.TieType == NoteTieType.Start)
			{
				measurementService.TieStartPoint = new Point(scoreService.CursorPositionX, notePositionY);
			}
			else if (element.TieType != NoteTieType.None) //Stop or StopAndStartAnother / Stop lub StopAndStartAnother
			{
				double arcWidth = scoreService.CursorPositionX - measurementService.TieStartPoint.X - 12;
				double arcHeight = arcWidth * 0.7d;
				if (element.StemDirection == VerticalDirection.Down)
				{
					renderer.DrawArc(new Rectangle(measurementService.TieStartPoint.X + 16, measurementService.TieStartPoint.Y + 22,
						arcWidth, arcHeight), 180, 180, new Pen(renderer.CoalesceColor(element), 1.5), element);
				}
				else if (element.StemDirection == VerticalDirection.Up)
				{
					renderer.DrawArc(new Rectangle(measurementService.TieStartPoint.X + 16, measurementService.TieStartPoint.Y + 22,
						arcWidth, arcHeight), 0, 180, new Pen(renderer.CoalesceColor(element), 1.5), element);
				}
				if (element.TieType == NoteTieType.StopAndStartAnother)
				{
					measurementService.TieStartPoint = new Point(scoreService.CursorPositionX + 2, notePositionY);
				}
			}
		}

		private void DrawTremolos(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			double currentTremoloPos = notePositionY + 18;
			for (int j = 0; j < element.TremoloLevel; j++)
			{
				if (element.StemDirection == VerticalDirection.Up)
				{
					currentTremoloPos -= 4;
					renderer.DrawLine(scoreService.CursorPositionX + 9, currentTremoloPos + 1, scoreService.CursorPositionX + 16, currentTremoloPos - 1, element);
					renderer.DrawLine(scoreService.CursorPositionX + 9, currentTremoloPos + 2, scoreService.CursorPositionX + 16, currentTremoloPos, element);
				}
				else
				{
					currentTremoloPos += 4;
					renderer.DrawLine(scoreService.CursorPositionX + 3, currentTremoloPos + 11 + 1, scoreService.CursorPositionX + 11, currentTremoloPos + 11 - 1, element);
					renderer.DrawLine(scoreService.CursorPositionX + 3, currentTremoloPos + 11 + 2, scoreService.CursorPositionX + 11, currentTremoloPos + 11, element);
				}
			}
		}

		private void DrawTrills(ScoreRendererBase renderer, Note element, double notePositionY)
		{
			if (element.TrillMark != NoteTrillMark.None)
			{
				double trillPos = notePositionY - 1;
				if (element.TrillMark == NoteTrillMark.Above)
				{
					trillPos = notePositionY - 1;
					if (trillPos > scoreService.CurrentLinePositions[0] - renderer.Settings.TextBlockHeight)
					{
						trillPos = scoreService.CurrentLinePositions[0] - renderer.Settings.TextBlockHeight - 1.0f;
					}
				}
				else if (element.TrillMark == NoteTrillMark.Below)
				{
					trillPos = notePositionY + 10;
				}
				renderer.DrawString(renderer.Settings.CurrentFont.Trill, MusicFontStyles.MusicFont, scoreService.CursorPositionX + 6, trillPos, element);
			}
		}

		private double GetMaxVertDistanceBetweenNotes(ScoreRendererBase renderer, params Note[] notes)
		{
			var distances = notes.Select(n => CalculateNotePositionY(n, renderer));
			return distances.Max() - distances.Min();
		}

		private void MakeSpaceForAccidentals(ScoreRendererBase renderer, Note element, int numberOfSingleAccidentals, int numberOfDoubleAccidentals)
		{
			if (renderer.Settings.IgnoreCustomElementPositions || !element.DefaultXPosition.HasValue)
			{
				if (element.Alter - scoreService.CurrentKey.StepToAlter(element.Step) != 0)
				{
					if (numberOfSingleAccidentals > 0) scoreService.CursorPositionX += 9;
					if (numberOfDoubleAccidentals > 0)
						scoreService.CursorPositionX += (numberOfDoubleAccidentals) * 9;
				}
				if (element.HasNatural == true) scoreService.CursorPositionX += 9;
			}
		}
	}
}