﻿using Manufaktura.Controls.Primitives;
using Manufaktura.Controls.Rendering;
using Manufaktura.Model.MVVM;
using Manufaktura.Music.Model;
using System;
using System.Runtime.CompilerServices;

namespace Manufaktura.Controls.Model
{
	public enum ArticulationType { None, Staccato, Accent };

	public enum BarlineStyle { Regular, LightHeavy }

	public enum ClefType { GClef, CClef, FClef };

	public enum DesiredHookDirections { Any, ForwardHook, BackwardHook };

	public enum DirectionPlacementType { Above, Below, Custom };

	public enum HorizontalPlacement { Left, Right };

	public enum NoteBeamType { Single, Start, Continue, End, ForwardHook, BackwardHook };

	public enum NoteSlurType { None, Start, Stop };

	public enum NoteTieType { None, Start, Stop, StopAndStartAnother };

	public enum NoteTrillMark { None, Above, Below };

	public enum RepeatSignType { None, Forward, Backward };

	public enum SyllableType { None, Begin, Middle, End, Single };

	public enum TimeSignatureType { Common, Cut, Numbers };

	public enum TupletType { None, Start, Stop };

	public enum VerticalDirection { Up, Down };

	public enum VerticalPlacement { Above, Below };

	/// <summary>
	/// Base class for all musical symbols
	/// </summary>
	public abstract class MusicalSymbol : ViewModel
	{
		private Color? customColor;
		private bool isVisible;

		protected MusicalSymbol()
		{
			isVisible = true;
			Id = Guid.NewGuid();
		}

		/// <summary>
		/// Returns a new instance of null musical symbol
		/// </summary>
		public static NullMusicalSymbol Null
		{
			get
			{
				return new NullMusicalSymbol();
			}
		}

		public Color? CustomColor { get { return customColor; } set { customColor = value; OnPropertyChanged(() => CustomColor); } }
		public Guid Id { get; private set; }

		/// <summary>
		/// Gets or sets the symbol's visibility. Visibility can be treated differently varying on implementation of rendering.
		/// </summary>
		public bool IsVisible { get { return isVisible; } set { isVisible = value; OnPropertyChanged(() => IsVisible); } }

		public virtual Measure Measure { get; internal set; }
		public ScorePage Page => Measure?.System?.Page;

		public int? PageNumber => Page == null ? null : Staff?.Score?.Pages?.IndexOf(Page) + 1;
		public virtual Staff Staff { get; internal set; }

		internal bool SuppressEvents { get; set; }

		/// <summary>
		/// Converts a VerticalDirection to VerticalPlacement.
		/// </summary>
		/// <param name="direction">VerticalDirection to convert to VerticalPlacement.</param>
		/// <returns>VerticalPlacement converted from VerticalDirection.</returns>
		public static VerticalPlacement DirectionToPlacement(VerticalDirection direction)
		{
			return direction == VerticalDirection.Up ? VerticalPlacement.Above : VerticalPlacement.Below;
		}

		[Obsolete("Use Duration.ToTimeSpan() instead")]
		public static TimeSpan DurationToTime(IHasDuration durationElement, Tempo tempo)
		{
			double singleNoteDuration = 60d / tempo.BeatsPerMinute;
			double ratio = (double)tempo.BeatUnit.Denominator / (double)durationElement.BaseDuration.Denominator;
			if (durationElement.NumberOfDots > 0)
			{
				ratio += ratio * Math.Pow(0.5, durationElement.NumberOfDots);
			}
			return TimeSpan.FromSeconds(singleNoteDuration * ratio);
		}

		public Color CoalesceColor(ScoreRendererBase renderer)
		{
			return CustomColor.HasValue ? CustomColor.Value : renderer.Settings.DefaultColor;
		}

		internal void InvalidateMeasure()
		{
			Staff?.FireMeasureInvalidated(this, Measure);
		}

		protected override void OnPropertyChanged(string propertyName)
		{
			if (SuppressEvents) return;
			base.OnPropertyChanged(propertyName);
			Staff?.FireMeasureInvalidated(this, Measure);
		}
	}
}