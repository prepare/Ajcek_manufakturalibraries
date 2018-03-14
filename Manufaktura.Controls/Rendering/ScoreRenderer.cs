﻿using Manufaktura.Controls.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Manufaktura.Controls.Rendering
{
	/// <summary>
	/// Base class for rendering scores on specific canvas.
	/// </summary>
	/// <typeparam name="TCanvas">Canvas object</typeparam>
	public abstract class ScoreRenderer<TCanvas> : ScoreRendererBase
	{
		/// <summary>
		/// Initializes a new ScoreRendere with specific canvas object.
		/// </summary>
		/// <param name="canvas"></param>
		public ScoreRenderer(TCanvas canvas)
		{
			Canvas = canvas;
		}

        public ScoreRenderer(TCanvas canvas, ScoreRendererSettings settings) : base(settings)
        {
            Canvas = canvas;
        }

        public TCanvas Canvas { get; internal set; }

		public sealed override void Render(Measure measure)
		{
            scoreService.ReturnToFirstSystem();
			scoreService.MoveTo(measure, Settings);
			if (Settings.RenderingMode == ScoreRenderingModes.Panorama && scoreService.CurrentSystemNo != 1)
			{
				scoreService.ReturnToFirstSystem(); //W trybie panoramy system zawsze jest pierwszy
				scoreService.CurrentClef.TextBlockLocation = new Primitives.Point(scoreService.CursorPositionX, scoreService.CurrentLinePositions[4] - 24.4f - (scoreService.CurrentClef.Line - 1) * Settings.LineSpacing);
			}
			measurementService.LastMeasurePositionX = scoreService.CursorPositionX;
			measurementService.LastNoteEndXPosition = scoreService.CursorPositionX;
			measurementService.LastNotePositionX = scoreService.CursorPositionX;
			measurementService.LastNoteInMeasureEndXPosition = scoreService.CursorPositionX;
			alterationService.Reset();
			foreach (MusicalSymbol symbol in measure.Elements.Where(e => !(e is Barline) && !(e is PrintSuggestion)))
			{
				try
				{
					var noteOrRest = symbol as NoteOrRest;
					if (noteOrRest != null) scoreService.CurrentVoice = noteOrRest.Voice;
					var renderStrategy = GetProperRenderStrategy(symbol);
					var clefRenderStrategy = renderStrategy as ClefRenderStrategy;
					if (clefRenderStrategy != null) clefRenderStrategy.WasSystemChanged = measure.Staff.Elements.OfType<Clef>().Any();
					if (renderStrategy != null) renderStrategy.Render(symbol, this);
				}
				catch (Exception ex)
				{
					Exceptions.Add(ex);
				}
			}
			foreach (var finishingTouch in FinishingTouches)
			{
				try
				{
					finishingTouch.PerformOnMeasure(measure, this);
				}
				catch (Exception ex)
				{
					Exceptions.Add(ex);
				}
			}
		}

        /// <summary>
        /// Additional actions before the Score is rendered
        /// </summary>
        /// <param name="score">Score to render</param>
		protected virtual void BeforeRenderScore(Score score) { }


		/// <summary>
		/// Renders score on canvas.
		/// </summary>
		/// <param name="score">Score</param>
		public override sealed void Render(Score score)
		{
			CurrentScore = score;
			BeforeRenderScore(score);

			scoreService.BeginNewScore(score);
			foreach (Staff staff in score.Staves)
			{
				try
				{
					RenderStaff(staff);
				}
				catch (Exception ex)
				{
					Exceptions.Add(ex);
				}
			}

			//Set height of current system in Panorama mode. This is used for determining the size of the control:
			if (Settings.RenderingMode == ScoreRenderingModes.Panorama && scoreService.CurrentSystem != null && scoreService.CurrentSystem.Height == 0)
			{
				scoreService.CurrentSystem.Height = (scoreService.CurrentStaffHeight + Settings.LineSpacing) * scoreService.CurrentScore.Staves.Count;
			}

			foreach (var finishingTouch in FinishingTouches)
			{
				try
				{
					finishingTouch.PerformOnScore(score, this);
				}
				catch (Exception ex)
				{
					Exceptions.Add(ex);
				}
			}
		}

		public async Task RenderAsync(Score score)
		{
			await Task.Factory.StartNew(() => Render(score));
		}

		protected bool EnsureProperPage(MusicalSymbol owner)
		{
			return Settings.RenderingMode != ScoreRenderingModes.SinglePage || owner == null || !owner.PageNumber.HasValue || owner.PageNumber == Settings.CurrentPage;
		}
	}
}