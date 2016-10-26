﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EvoDevo4
{
    public class SignallingProtein
    {
        private static List<SignallingProtein> ARRAY;
        public static List<SignallingProtein> Array
        {
            get
            {
                if (ARRAY == null)
                    ARRAY = new List<SignallingProtein>();
                return (ARRAY);
            }
        }
        static SignallingProtein()
        {
            Array.Add(new SignallingProtein(0.9,Color.Blue));
            Array.Add(new SignallingProtein(0.9, Color.Green));
            Array.Add(new SignallingProtein(0.9, Color.Firebrick));
            Array.Add(new SignallingProtein(0.8, Color.Bisque));
            Array.Add(new SignallingProtein(0.8, Color.BurlyWood));
            Array.Add(new SignallingProtein(0.8, Color.Chartreuse));
            Array.Add(new SignallingProtein(0.5, Color.Coral));
            Array.Add(new SignallingProtein(0.5, Color.CornflowerBlue));
            Array.Add(new SignallingProtein(0.5, Color.Crimson));
            Array.Add(new SignallingProtein(1, Color.DarkGoldenrod));
        }

        public double pentration;
        public Color color;
        public SignallingProtein(double pentration, Color color)
        {
            this.pentration = pentration;
            this.color = color;
        }
    }
}
