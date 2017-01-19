﻿using Manufaktura.Controls.Model;
using Manufaktura.Music.Model;
using System;

namespace Manufaktura.Controls.Primitives
{
    /// <summary>
    /// Represents a 2D point.
    /// </summary>
    public struct Point
    {
        public Point(double x, double y) : this()
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public static double BeamAngle(Point beamStart, Point beamEnd)
        {
            return UsefulMath.BeamAngle(beamStart.X, beamStart.Y, beamEnd.X, beamEnd.Y);
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        public Point Translate(ScorePage page)
        {
            return new Point(this.X + (page.MarginLeft ?? 0), this.Y + (page.MarginTop ?? 0));
        }

        public Point Translate(double dx, double dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point TranslateByAngleOld(double angle, double length)
        {
            var dy = Math.Sin(angle) * length;
            var dx = Math.Cos(angle) * length;
            return new Point(X + dx, Y + dy);
        }

        public Point TranslateByAngle(double angle, double length)
        {
            var dy = Math.Cos(angle) * length;
            var dx = Math.Sin(angle) * length;
            return new Point(X + dx, Y + dy);
        }

        public Point TranslateHorizontallyAndMaintainAngle(double angle, double dx)
        {
            var dy = Math.Tan(angle) * dx;
            return new Point(X + dx, Y + dy);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
}