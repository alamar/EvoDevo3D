using System;
using Microsoft.Xna.Framework;

namespace EvoDevo4
{
    public class Source
    {
        private Cell CELL;
        public Vector position
        {
            get
            {
                return (CELL.position);
            }
        }
        public double strength;
        public int secretID;
        public Color color
        {
            get
            {
                if (secretID >= 0 && secretID < SignallingProtein.Array.Count)
                {
                    return SignallingProtein.Array[secretID].color;
                }
                throw new Exception("Located a source of unknown protein!");
                //return Color.Black;
            }
        }
        public bool OriginatesFrom(Cell cell)
        {
            return (CELL == cell);
        }

        /// <summary>
        /// Initiates the new instance of Source
        /// </summary>
        /// <param name="secretID">ID of a protein</param>
        /// <param name="strength">Secret amount</param>
        /// <param name="position">Position of a source</param>
        public Source(int secretID, double strength, Cell cell)
        {
            CELL = cell;
            this.secretID = secretID;
            this.strength = strength;
        }
    }
}
