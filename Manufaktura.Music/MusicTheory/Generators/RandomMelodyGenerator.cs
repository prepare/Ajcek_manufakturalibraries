﻿using Manufaktura.Music.Model;
using Manufaktura.Music.MusicTheory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Music.MusicTheory.Generators
{
    public class RandomMelodyGenerator
    {
        public Scale Scale { get; set; }

        public IEnumerable<Pitch> Generate()
        {

            foreach (var interval in Scale.Mode.Intervals)
            {

            }

            return null;
        }
    }
}
