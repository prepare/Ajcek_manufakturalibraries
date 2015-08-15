﻿using Manufaktura.Music.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Model
{
    /// <summary>
    /// Indicates that element has rhythmic duration.
    /// </summary>
    public interface IHasDuration
    {
        /// <summary>
        /// Represents duration without dots
        /// </summary>
        RhythmicDuration BaseDuration { get; }
        int NumberOfDots { get; set; }
        TupletType Tuplet { get; set; }
    }
}
