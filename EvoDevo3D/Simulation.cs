using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using EvoDevo3D.Support;

namespace EvoDevo3D
{
    public class Simulation : IDisposable
    {
        public enum Mode {
            Run,
            FastForward,
            Pause,
            // Ephemeral:
            Step
        }
        private const int NUM_PASSIVE_CYCLES = 5;
        private Thread heartbeatThread;
        private int step;
        public int Step
        {
            get
            {
                return step;
            }
        }

        public const double ALMOST_ZERO = 0.000001;
        public Cell selectionTarget;
        public volatile Mode mode = Mode.Run;
        public volatile bool newActionAllowed = false;

        public List<Cell> Cells = new List<Cell>();
        public List<Source> Sources = new List<Source>();
        public double[] proteinPenetrations = new double[] {
            0.9, 0.9, 0.9, 0.8, 0.8, 0.8, 0.5, 0.5, 0.5, 1,
            0.9, 0.9, 0.9, 0.8, 0.8, 0.8, 0.5, 0.5, 0.5, 1 };

        public void FixOrganizm()
        {
            if (Cells.Count > 1)
            {
                foreach (Cell cell in Cells)
                {
                    if (cell != Cells[0])
                        cell.ConnectTo(Cells[0]);
                }
            }
        }

        /*public Color GetColor(Vector location)
        {
            byte r = 255;
            byte g = 255;
            byte b = 255;

            foreach (Source sc in Sources)
            {
                if (sc != null) //it can be null if a source is removed while in cycle
                {
                    double tempconc = sc.strength * (Math.Pow(SignallingProtein.Array[sc.secretID].pentration, Vector.Distance(sc.position, location)));
                    r = (byte)Math.Round(r * (1 - tempconc) + tempconc * SignallingProtein.Array[sc.secretID].color.R);
                    g = (byte)Math.Round(g * (1 - tempconc) + tempconc * SignallingProtein.Array[sc.secretID].color.G);
                    b = (byte)Math.Round(b * (1 - tempconc) + tempconc * SignallingProtein.Array[sc.secretID].color.B);
                }
            }

            Color retval = new Color();
            if (r > 255)
                r = 255;

            if (g > 255)
                g = 255;

            if (b > 255)
                b = 255;

            retval = new Color(r, g, b);
            return (retval);
        }*/

        /// <summary>
        /// Returns the list of cells touching or hidden within the given cell
        /// </summary>
        /// <param name="cell">Center cell</param>
        /// <returns>The list of adjacent cells</returns>
        public List<Cell> GetMyAdjacentNeighbours(Cell cell)
        {
            List<Cell> retval = new List<Cell>();
            foreach (Cell cellmate in cell.surroundingCells)
            {
                double sqDist = Vector.SqDistance(cell.position, cellmate.position);
                double sqCritDist = (cell.radius + cellmate.radius);
                sqCritDist *= sqCritDist;
                if (sqDist < sqCritDist)
                {
                    retval.Add(cellmate);
                }
            }
            return retval;
            /*
            List<Cell> retval = new List<Cell>();
            int cellRaidus = (int)cell.radius;
            if (cellRaidus == 0) cellRaidus = 1;
            for (int x = cell.MyPos.x - cellRaidus; x <= cell.MyPos.x + cellRaidus; x++)
            {
                for (int y = cell.MyPos.y - cellRaidus; y <= cell.MyPos.y + cellRaidus; y++)
                {
                    for (int z = cell.MyPos.z - cellRaidus; z <= cell.MyPos.z + cellRaidus; z++)
                    {

                        MapPos mp = new MapPos();
                        mp.x = x;
                        mp.y = y;
                        mp.z = z;
                        if (cellMap.ContainsKey(mp))
                        {
                            foreach (Cell testCell in cellMap[mp])
                            {
                                if (testCell != null)
                                {
                                    if ((testCell != cell) && (Vector.SqDistance(testCell.position, cell.position) <= (testCell.radius + cell.radius) * (testCell.radius + cell.radius)))
                                    {
                                        retval.Add(testCell);
                                    }
                                }
                            }
                        }
                    }
                }
            } 
            return (retval);
            */
        }

        public Simulation(Type cell)
        {
            Cell newCell = Cell.GenerateRandomCell(cell, this);
            newCell.radius = 1;
            newCell.position.Trivialize();
            Cells.Add(newCell);

            heartbeatThread = new Thread(ActionsManager);
            heartbeatThread.IsBackground = true;
            heartbeatThread.Start();
        }
        
        
        public void Register(Source source)
        {
            Sources.Add(source);
        }

        public void UnRegister(int ProteinId, Cell cell)
        {
            Source tgt=null;
            foreach (Source src in Sources)
            {
                if (src.OriginatesFrom(cell)&&src.secretID == ProteinId)
                {
                    tgt = src;
                    break;
                }
            }
            if (tgt != null)
            {
                Sources.Remove(tgt);
            }
        }

        public void RegisterNewCell(Cell cell)
        {
            Cells.Add(cell);
            cell.neighbours = GetMyAdjacentNeighbours(cell);
        }

        public void UnregisterCell(Cell cell)
        {
            lock (Cells)
            {
                for (int i = 0; i < proteinPenetrations.Length; i++)
                {
                    if (cell.secrettingNow[i])
                    {
                        UnRegister(i, cell);
                    }
                }
                Cells.Remove(cell);
            }
        }

        public void Heartbeat()
        {
            List<Cell> TempCells = new List<Cell>();
            List<Cell> TempActiveCells = new List<Cell>();
            foreach (Cell cell in Cells)
            {
                if (!cell.holdingPosition)
                {
                    if (!cell.IsMoving)
                    {
                        TempCells.Add(cell);
                    }
                    else
                    {
                        TempActiveCells.Add(cell);
                    }
                }
                
            }
            foreach (Cell cell in TempActiveCells.OrderBy(a => Cell.random.Next()))
            {
                double stepDist = ((cell.desiredDistance < cell.movingSpeed) ? cell.desiredDistance : cell.movingSpeed);
                MoveMeAndMySwarm(cell, cell.activeMovingDirection * stepDist);
                
                cell.desiredDistance -= stepDist;
                if (cell.desiredDistance <= 0)
                {
                    cell.desiredDistance = 0;
                    cell.activeMovingDirection.Trivialize();
                    cell.IsMoving = false;
                }
            }
            
            bool shouldLog = TempCells.Count > 1000;
            DateTime startMoment = DateTime.Now;
            StringBuilder log = new StringBuilder();
            int stepCounter = 0;
            int trueCounter = 0;
            foreach (Cell cell in TempCells)
            {
                int cellRadius = (int)cell.radius;
                if (cellRadius == 0) cellRadius = 1;
                //List<Cell> neighbours = new List<Cell>(Cells);
                foreach (Cell neighbour in cell.neighbours)
                {
                    stepCounter++;
                    if (cell.connectedCells.Count == 0 || !cell.connectedCells.Contains(neighbour))
                    {
                        double criticalDistance = (neighbour.radius + cell.radius) * (neighbour.resilience + cell.resilience) * 0.5;
                        if (Vector.SqDistance(neighbour.position, cell.position) < criticalDistance * criticalDistance)
                        {
                            trueCounter++;
                            double movingLength = 0.0;
                            double distLimit = (cell.radius + neighbour.radius) * cell.resilience;
                            double dist = (cell.position - neighbour.position).Length;
                            if (dist < distLimit && dist > Simulation.ALMOST_ZERO)
                            {
                                movingLength = 0.1 * cell.radius * (1 - dist / distLimit);
                                Vector temp = (cell.position - neighbour.position) * (movingLength / dist);
                                cell.passiveMovingDirection += temp;
                            }

                        }
                    }
                }

                foreach (Cell linked in cell.linkedCells)
                {
                    double criticalDistance = (linked.radius + cell.radius) * 1.5;
                    if (Vector.SqDistance(linked.position, cell.position) > criticalDistance * criticalDistance)
                    {
                        double movingLength = 0.0;
                        double distLimit = (cell.radius + linked.radius);
                        double dist = (cell.position - linked.position).Length;
                        if (dist > distLimit && dist > Simulation.ALMOST_ZERO)
                        {
                            movingLength = 0.1 * cell.radius * (dist / distLimit);
                            Vector temp = (linked.position - cell.position) * (movingLength / dist);
                            cell.passiveMovingDirection += temp;
                        }
                    }
                }
            }
            
            foreach (Cell cell in TempCells)
            {
                MoveMeAndMySwarm(cell, cell.passiveMovingDirection);
                /*DeMapCell(cell);
                cell.position += cell.passiveMovingDirection;
                foreach (Cell connectedCell in cell.connectedCells)
                {
                    DeMapCell(connectedCell);
                    connectedCell.position += cell.passiveMovingDirection;
                    MapCell(connectedCell);
                }
                MapCell(cell);*/
                cell.passiveMovingDirection.Trivialize();
            }

            foreach (Cell cell in Cells)
            {
                cell.neighbours = GetMyAdjacentNeighbours(cell);
            }
        }

       
        private void MoveMeAndMySwarm(Cell cell, Vector where)
        {
            if (where.Length > cell.radius * 2) return;
            cell.position += where;

            if (cell.connectedCells.Count > 0)
            {
                foreach (Cell swarmer in cell.connectedCells)
                {
                    swarmer.position += where;
                }
            }
        }

        public void ActionsManager()
        {
            while (true)
            {
                while (!newActionAllowed)
                {
                    Thread.Sleep(40);
                }

                newActionAllowed = false;
                GeneticTick();
                for (int i = 0; i < NUM_PASSIVE_CYCLES; i++)
                {
                    Heartbeat();
                }
            }
        }

        public List<Cell> GetSurroungingCells(Cell cell)
        {
            List<Cell> retval = new List<Cell>();
            foreach (Cell cellmate in Cells)
            {
                double sqDist = Vector.SqDistance(cell.position, cellmate.position);
                if (sqDist > ALMOST_ZERO && sqDist < (cell.radius * cell.radius * 36))
                {
                    retval.Add(cellmate);
                }
            }
            return retval.OrderBy(other => (cell.position - other.position).Length).ToList();
        }

        public void GeneticTick()
        {
            foreach (Cell cell in Cells.Copy())
            {
                cell.LiveOn();                
            }
            foreach (Cell cell in Cells)
            {
                cell.surroundingCells = GetSurroungingCells(cell);

                for (int secretID = 0; secretID < proteinPenetrations.Length; secretID++)
                {
                    double conc = 0;
                    Vector gradient = new Vector();
                    foreach (Source sc in Sources)
                    {
                        if (sc.secretID == secretID)
                        {
                            double curConc = sc.strength *
                                             Math.Pow(proteinPenetrations[secretID],
                                                 Vector.Distance(sc.position, cell.position));
                            gradient += (sc.position - cell.position).Normalize() * curConc;
                            conc += curConc;
                        }
                    }
                    cell.sensorReaction[secretID] = conc * cell.sensitivity[secretID];
                    cell.gradient[secretID] = gradient.Normalize();
                }
            }
            step++;
        }

        internal void ReTarget(Vector mouseRay, Vector mousePos)
        {
            selectionTarget = null;
            double selectionSqDist = 0 ;
            foreach (Cell cell in Cells.ToArray())
            {
                Vector v1 = mouseRay;
                Vector v2 = mousePos - cell.position;
                Vector v3 = new Vector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
                double sqDist = v3.modSq / mouseRay.modSq;
                if (sqDist <= cell.radius * cell.radius)
                {
                    if (selectionTarget == null || Vector.SqDistance(cell.position, mousePos) < selectionSqDist)
                    {
                        selectionTarget = cell;
                        selectionSqDist = Vector.SqDistance(cell.position, mousePos);
                    }
                }
            }
        }

        public void Dispose()
        {
            heartbeatThread.Abort();
        }
    }
}
