﻿using Manufaktura.Controls.Model;
using Manufaktura.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
    public class ScoreRendererState
    {
        public Score CurrentScore { get; set; }
        public Staff CurrentStaff { get; set; }
        public Clef CurrentClef {get; set;}
        public Key CurrentKey { get; set; }
        public int CurrentMeasure { get; set; }
        public int CurrentVoice { get; set; }

        public double currentClefPositionY = 0;

        public double PageWidth { get; set; }
        public bool IsManualMode { get; set; }
        public bool IsPrintMode { get; set; }
        
        public double CursorPositionX { get; set; }
        public double LastCursorPositionX { get; set; } //for chords / dla akordów
        public double lastNoteEndXPosition { get; set; } //for many voices / dla wielu głosów
        public double firstNoteInMeasureXPosition { get; set; } //for many voices - starting point for all voices / dla wielu głosów - punkt rozpoczęcia wszystkich głosów
        public double lastNoteInMeasureEndXPosition { get; set; } //for many voices - location of the last note in the measure / dla wielu głosów - punkt ostatniej nuty w takcie
        public int paddingTop { get; private set; }
        public int lineSpacing { get; private set; }
        public double currentStemEndPositionY { get; set; }
        public int numberOfNotesUnderTuplet { get; set; }
        public List<double> previousStemEndPositionsY { get; set; }
        public double currentStemPositionX { get; set; }
        public List<double> previousStemPositionsX { get; set; }
        public List<Point> beamStartPositionsY { get; set; }
        public List<Point> beamEndPositionsY { get; set; }
        public Point tieStartPoint { get; set; }
        public Point slurStartPoint { get; set; }
        
        public int[] alterationsWithinOneBar { get; set; }
        public bool firstNoteInIncipit { get; set; }
        
        public int[] LinePositions { get; private set; }

        public ScoreRendererState()
        {
            CurrentClef = new Clef(ClefType.CClef, 2);
            CurrentKey = new Key(0);
            CursorPositionX = 0;
            LastCursorPositionX = 0; //for chords / dla akordów
            lastNoteEndXPosition = 0; //for many voices / dla wielu głosów
            firstNoteInMeasureXPosition = 0; //for many voices - starting point for all voices / dla wielu głosów - punkt rozpoczęcia wszystkich głosów
            lastNoteInMeasureEndXPosition = 0; //for many voices - location of the last note in the measure / dla wielu głosów - punkt ostatniej nuty w takcie
            paddingTop = 20;
            lineSpacing = 6;
            currentStemEndPositionY = 0;
            numberOfNotesUnderTuplet = 0;
            previousStemEndPositionsY = new List<double>();
            currentStemPositionX = 0;
            previousStemPositionsX = new List<double>();
            beamStartPositionsY = new List<Point>();
            beamEndPositionsY = new List<Point>();
            tieStartPoint = new Point();
            slurStartPoint = new Point();
            CurrentVoice = 1;
            LinePositions = new int[5];
            PageWidth = 200;
            
        }
    }
}
