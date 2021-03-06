﻿using System;
using System.Globalization;
using System.Linq;

namespace Manufaktura.Music.Model
{
	public static class UsefulMath
	{
		public static double BeamAngle(double beamStartX, double beamStartY, double beamEndX, double beamEndY)
		{
			if (beamEndX - beamStartX == 0) return 0;
			return Math.Atan((beamEndY - beamStartY) / (beamEndX - beamStartX));
		}

		public static double CentsToLinear(double cents)
		{
			return Math.Pow(2, cents / 1200);
		}

		public static double GradToRadians(double angle)
		{
			return angle * (Math.PI / 180d);
		}

		public static double Mean(params double[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));
			return (double)values.Sum(t => t) / values.Count();
		}

		public static double Median(params double[] values)
		{
			if (values == null) throw new ArgumentNullException(nameof(values));

			int valuesCount = values.Count();
			if (valuesCount == 0) return default(double);
			if (valuesCount == 1) return values.First();
			var sortedValues = values.OrderBy(v => v).ToArray();
			int midIndex = valuesCount / 2;
			if (valuesCount % 2 == 0)
			{
				return Mean(sortedValues[midIndex - 1], sortedValues[midIndex]);
			}
			else
			{
				return sortedValues[midIndex];
			}
		}

		public static string NumberToOrdinal(int number)
		{
			if (number == 1) return "1st";
			if (number == 2) return "2nd";
			if (number == 3) return "3rd";
			if (number > 3) return number + "th";
			else return number.ToString();
		}

		/// <summary>
		/// Returns stem end position for middle note in a group of notes under one beam.
		/// </summary>
		/// <param name="firstNote"></param>
		/// <param name="lastNote"></param>
		/// <param name="searchedNote"></param>
		/// <returns></returns>
		public static double StemEnd(double firstNoteX, double firstNoteY, double middleNoteX, double lastNoteX, double lastNoteY)
		{
			var firstToMiddleX = middleNoteX - firstNoteX;
			var lastToFirstX = lastNoteX - firstNoteX;
			var lastToFirstY = lastNoteY - firstNoteY;
			if (lastToFirstX == 0) return 0;
			return (firstToMiddleX * lastToFirstY) / lastToFirstX;
		}

		public static double? TryParse(string s)
		{
			double result;
			if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) return result;
			return null;
		}

		public static DateTime? TryParseDateTime(string s)
		{
			DateTime result;
			if (DateTime.TryParse(s, out result)) return result;
			return null;
		}

		public static int? TryParseInt(string s)
		{
			int result;
			if (int.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result)) return result;
			return null;
		}
	}
}