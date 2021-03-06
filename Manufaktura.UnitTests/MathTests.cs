﻿using Manufaktura.Controls.Model;
using Manufaktura.Core;
using Manufaktura.Music.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manufaktura.UnitTests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void ProportionTest()
        {
            Assert.AreEqual((Proportion.Sesquialtera + new Proportion(1, 2)).DecimalValue, 2);
            Assert.IsTrue((Proportion.Sesquialtera + new Proportion(1, 2)) == 2);
            Assert.IsTrue(Proportion.Sesquialtera == new Proportion(6, 4));
            Assert.IsTrue(Proportion.Sesquialtera == new Proportion(6, 4).Normalize());
            Assert.IsTrue(new Proportion(8, 12).Normalize().Denominator == 3);
        }

        [TestMethod]
        public void CentsTest()
        {
            Assert.AreEqual(Proportion.Dupla.Cents, 1200);
            Assert.AreEqual(UsefulMath.CentsToLinear(1200), Proportion.Dupla.DoubleValue);

            var second1 = new Proportion(9, 8);
            var second2 = Proportion.GetApproximatedProportionFromCents(second1.Cents);
            Assert.AreEqual((int)second1.Cents, (int)second2.Cents);

            var pureFifth = Proportion.Sesquialtera.Cents;
        }

        [TestMethod]
        public void DurationTest()
        {
            Assert.IsTrue(RhythmicDuration.Half + RhythmicDuration.Half == RhythmicDuration.Whole.ToProportion());
            Assert.IsTrue(RhythmicDuration.Quarter + RhythmicDuration.Eighth == RhythmicDuration.Eighth + RhythmicDuration.Quarter);
            Assert.IsTrue(RhythmicDuration.Half.ToTimeSpan(Tempo.Allegro) + RhythmicDuration.Half.ToTimeSpan(Tempo.Allegro) == RhythmicDuration.Whole.ToTimeSpan(Tempo.Allegro));
            Assert.IsTrue(RhythmicDuration.Half.ToFractionOf(RhythmicDuration.Whole) == 0.5);
            var halfNoteTimeSpan = RhythmicDuration.Half.ToTimeSpan(Tempo.Allegro);
            var wholeNoteTimeSpan = RhythmicDuration.Whole.ToTimeSpan(Tempo.Adagio);
            Assert.IsFalse (halfNoteTimeSpan + halfNoteTimeSpan == wholeNoteTimeSpan);
        }

        [TestMethod]
        public void ResetableLazyTest()
        {
            var random = new Random();
            var resetableLazy = new ResetableLazy<int>(() => random.Next(int.MaxValue));
            var val1 = resetableLazy.Value;
            var val2 = resetableLazy.Value;
            Assert.AreEqual(val1, val2);
            resetableLazy.Reset();
            var val3 = resetableLazy.Value;
            Assert.AreNotEqual(val1, val3);
            Assert.AreNotEqual(val2, val3);

            int val4 = int.MinValue;
            var task = Task.Factory.StartNew(() => { val4 = resetableLazy.Value; });
            resetableLazy.Reset();
            task.Wait();
            Assert.AreNotEqual(val3, val4);
        }

    }
}
