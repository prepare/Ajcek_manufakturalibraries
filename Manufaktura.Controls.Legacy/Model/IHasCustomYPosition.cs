﻿namespace Manufaktura.Controls.Model
{
    /// <summary>
    /// Indicates that element can be positioned vertically.
    /// </summary>
    public interface IHasCustomYPosition
    {
        double? DefaultYPosition { get; set; }
    }
}