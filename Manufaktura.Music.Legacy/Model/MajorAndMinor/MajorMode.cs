﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Music.Model.MajorAndMinor
{
    /// <summary>
    /// Represents a major mode.
    /// </summary>
    public class MajorMode : Mode
    {
        public override IEnumerable<int> Intervals
        {
            get { return new[] { 2, 2, 1, 2, 2, 2, 1 }; }
        }
    }
}
