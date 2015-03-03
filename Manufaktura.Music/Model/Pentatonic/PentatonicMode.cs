﻿using Manufaktura.Music.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Music.Model
{
    public class PentatonicMode : Mode
    {
        public override IEnumerable<int> Intervals
        {
            get { return new[] { 2, 2, 1, 2, 2 }; }
        }

        public override IEnumerable<MusicalRule> Rules
        {
            get { return new MusicalRule[] { }; }
        }
    }
}
