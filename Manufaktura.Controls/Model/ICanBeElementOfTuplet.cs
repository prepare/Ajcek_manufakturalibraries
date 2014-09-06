﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Model
{
    interface ICanBeElementOfTuplet
    {
        TupletType Tuplet { get; set; }
        VerticalPlacement? TupletPlacement { get; set; }
    }
}