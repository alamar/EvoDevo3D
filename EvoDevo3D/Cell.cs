using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace EvoDevo4
{
    public abstract class Cell
    {
        public Simulation simulation;
        public List<Cell> neighbours = new List<Cell>();
        public List<Cell> surroundingCells = new List<Cell>();
        public int ID=counter++;
        public static int counter=0;
        public Vector position;
        public Vector passiveMovingDirection;
        public double radius;
        public int age=0;
        public int numDivisions = 0;
        public double resilience; //упругость если что :)
        public bool IsMoving;
        public Vector activeMovingDirection;
        public double desiredDistance;
        private double MOVINGSPEED;
        public int serviceTag;
        public double movingSpeed
        {
            get
            {
                return (MOVINGSPEED);
            }
            set
            {
                if (value > Simulation.ALMOST_ZERO && value < this.radius)
                    MOVINGSPEED = value;
            }
        }
        public Vector polarization;
        public int cellType;
        public bool[] secrettingNow = new bool[SignallingProtein.Array.Count];
        public double[] secretLevel = new double[SignallingProtein.Array.Count];
        public bool holdingPosition = false;
        public double[] sensitivity = new double[SignallingProtein.Array.Count];
        public Vector[] gradient
        {
            get
            {
                Vector[] retval = new Vector[SignallingProtein.Array.Count];
                for (int i = 0; i < SignallingProtein.Array.Count; i++)
                {
                    retval[i] = simulation.GetGradient(position, i);
                }
                return retval;
            }
        }
        public double[] sensorReaction
        {
            get
            {
                double[] retval = new double[SignallingProtein.Array.Count];
                for (int i = 0; i < SignallingProtein.Array.Count; i++)
                {
                    retval[i] = simulation.GetConcentration(position, i) * sensitivity[i];
                }
                return retval;
            }
        }
        public int neighbourCount
        {
            get
            {
                return (simulation.GetMyAdjacentNeighbours(this).Count);
            }
        }
        public List<Cell> connectedCells = new List<Cell>();
        public List<Cell> linkedCells = new List<Cell>();
        public List<Cell> offspring = new List<Cell>();
        public Cell parent;
        public Cell lastOffspring;

        public static Random random = new Random();
        public void FixOrganizm() {simulation.FixOrganizm();}
        public static double rnd
        {
            get
            {
                return random.NextDouble();
            }
        }

        public static string GeneticCode = "";
        public static CompilerResults CompiledGeneticStrategy;
        public const String geneCodeTemplatePiece1 = @"
                                using System;
                                using System.Collections.Generic;
                                using EvoDevo4;
                                class CellStrategy : Cell
                                {
                                    public CellStrategy(Simulation simulation, Vector position, double radius, double resilience)
                                        : base(simulation, position, radius, resilience)
                                    {
                                    }";
        public static string geneCodeTemplatePiece2="";
        public const String geneCodeTemplatePiece3 = @" 
                                    public override void CellLiveOn()
                                    {
                                        ";
        public static String geneCodeTemplateEnd = @"
                                        
                                    }
                                }";
        public static Dictionary<String, String> MemberMethods = new Dictionary<string, string>();
        public static Dictionary<String, String> MemberProperties = new Dictionary<string, string>();
        public int color = 0; 

        public override string ToString()
        {
            string format = @"Cell: position              ({0:f}, {1:f}, {10:f})
      radius                {8:f}
      resilience            {9:f}
      number of divisions   {2}
      cell type             {3}
      access to environment {4:f}
      neighbours count      {7}
      is holding position   {5}
      is moving             {6}";
            string sensStr = "      sensitivity           (";
            string sensReact = "      sensor reaction       (";
            string secretNow = "      secreting now         (";
            string secrAm = "      secret amount         (";
            for (int i = 0; i < SignallingProtein.Array.Count; i++)
            {
                if (i!=0)
                {
                    sensStr += ", ";
                    sensReact += ", ";
                    secretNow += ", ";
                    secrAm += ", ";
                }
                sensStr += this.sensitivity[i].ToString("f");
                sensReact += this.sensorReaction[i].ToString("f");
                secretNow += (this.secrettingNow[i]) ? "1" : "0";
                secrAm += this.secretLevel[i].ToString("f");
            }
            
            sensStr += ")";
            sensReact += ")";
            secretNow += ")";
            secrAm += ")";
            format += "\n" + sensStr;
            format += "\n" + sensReact;
            format += "\n" + secretNow;
            format += "\n" + secrAm;
            return string.Format(format, position.x, position.y, numDivisions, cellType, EnvironmentalAccess, holdingPosition, IsMoving, neighbourCount, radius, resilience, position.z);
        }

        static Cell()
        {
            Type CType = Type.GetType("EvoDevo4.Cell");
            MemberInfo[] methods = CType.GetMembers();
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name.Contains("counter") || methods[i].Name.Contains("Equals") || methods[i].Name.Contains("Member") || methods[i].Name.Contains("random") || methods[i].Name.Contains("CodeTemplate") || methods[i].Name.Contains("desiredDistance") || methods[i].Name.Contains("MovingDirection") || methods[i].Name.Contains("Get") || methods[i].Name.Contains("position") || methods[i].Name.Contains("MyPos") || methods[i].Name.Contains("ID") || methods[i].Name.Contains("String") || methods[i].Name.Contains("ompile") || methods[i].Name.Equals("GeneticCode") || methods[i].Name.Contains("GenerateRandomCell") || methods[i].Name.Contains("iveOn"))
                {                    
                    continue;
                }
                else
                {
                    string invocation="";
                    if (methods[i].MemberType == MemberTypes.Property)
                    {
                        invocation = methods[i].Name;
                        MemberProperties.Add(invocation, invocation);
                    }
                    
                    if (methods[i].MemberType == MemberTypes.Field)
                    {
                        invocation = methods[i].Name;
                        MemberProperties.Add(invocation, invocation);
                    }
                    if (methods[i].MemberType == MemberTypes.Method)
                    {
                        if (methods[i].Name.Contains("set_") || methods[i].Name.Contains("get_"))
                        {
                            continue;
                        }
                        MethodInfo mi = (MethodInfo)methods[i];
                        invocation = mi.Name + "(";
                        ParameterInfo[] parametres = mi.GetParameters();
                        for (int ii = 0; ii < parametres.Length; ii++)
                        {
                            
                            invocation += ((ii == 0) ? "" : ",");
                        }
                        invocation += ");";
                        if (MemberMethods.ContainsKey(mi.Name))
                        {
                            if (MemberMethods[mi.Name].Length < invocation.Length)
                            {
                                MemberMethods[mi.Name] = invocation;
                            }
                        }
                        else
                        {
                            MemberMethods.Add(mi.Name, invocation);
                        }
                    }
                }
            }
        }

        public abstract void CellLiveOn();

        public static Type Recompile()
        {
            CodeDomProvider provider = new CSharpCodeProvider();
            string script = geneCodeTemplatePiece1 + geneCodeTemplatePiece2 + geneCodeTemplatePiece3 + GeneticCode + geneCodeTemplateEnd;
            CompiledGeneticStrategy = CompileScript(script, "", provider);
            if (CompiledGeneticStrategy.Errors.HasErrors)
            {
                foreach (CompilerError err in CompiledGeneticStrategy.Errors)
                    System.Windows.Forms.MessageBox.Show(err.ErrorText);
                return null;
            }
            return CompiledGeneticStrategy.CompiledAssembly.GetType("CellStrategy");
        }

        public static CompilerResults CompileScript(string Source, string Reference, CodeDomProvider Provider)
        {
            //ICodeCompiler compiler = Provider.CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            CompilerResults results;
            // Configure parameters
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;
            if (Reference != null && Reference.Length != 0)
                parms.ReferencedAssemblies.Add(Reference);
            parms.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            // Compile
            results = Provider.CompileAssemblyFromSource(parms, Source);
            return results;
        }


        public Cell(Simulation simulation)
        {
            this.simulation = simulation;
            position = new Vector();
            radius = 1;
            resilience = 0.8;
            passiveMovingDirection = new Vector();
            activeMovingDirection = new Vector();
            desiredDistance = 0;
            IsMoving = false;
            movingSpeed = 0.2;
            cellType = 0;
            polarization = Vector.CreateRandom();
            for (int i = 0; i < SignallingProtein.Array.Count; i++)
            {
                secrettingNow[i] = false;
                secretLevel[i] = 0.5;
                sensitivity[i] = 1;
            }
            parent = null;
            offspring = new List<Cell>();
            lastOffspring = null;
        }

        public Cell(Simulation simulation, Vector position, double radius, double resilience)
        {
            this.simulation = simulation;
            this.position = new Vector(position.x, position.y, position.z);
            this.radius = radius;
            this.resilience = resilience;
            this.passiveMovingDirection = new Vector();
            activeMovingDirection = new Vector();
            desiredDistance = 0;
            IsMoving = false;
            movingSpeed = 0.2;
            cellType = 0;
            polarization = Vector.CreateRandom();
            for (int i = 0; i < SignallingProtein.Array.Count; i++)
            {
                secrettingNow[i] = false;
                secretLevel[i] = 0.5;
                sensitivity[i] = 0.5;
            }
            parent = null;
            offspring = new List<Cell>();
            lastOffspring = null;
        }

        /// <summary>
        /// Copys all but position onto a new cell
        /// </summary>
        /// <param name="ancestor">Parent cell to inherit from</param>
        private void InheritFrom(Cell ancestor)
        {
            this.color = ancestor.color;
            this.polarization = new Vector(ancestor.polarization.x, ancestor.polarization.y, ancestor.polarization.z);
            this.radius = ancestor.radius;
            this.resilience = ancestor.resilience;
            this.cellType = ancestor.cellType;
            this.age = ancestor.age;
            this.numDivisions = ancestor.numDivisions;
            for (int i = 0; i < SignallingProtein.Array.Count; i++)
            {
                secrettingNow[i] = false;
                secretLevel[i] = ancestor.secretLevel[i];
                sensitivity[i] = ancestor.sensitivity[i];
            }
            parent = ancestor;
            ancestor.offspring.Add(this);
            ancestor.lastOffspring = this;
        }

        public MapPos MyPos
        {
            get
            {
                MapPos mp = new MapPos();
                mp.x = (int)Math.Truncate(position.x);
                mp.y = (int)Math.Truncate(position.y);
                mp.z = (int)Math.Truncate(position.z);
                return (mp);
            }
        }

        public static Cell GenerateRandomCell(Type cell, Simulation simulation)
        {
            Vector startingVector = new Vector(random.NextDouble() * 20 - 10,
                                        random.NextDouble() * 20 - 10,
                                        random.NextDouble() * 20 - 10);
            return (Cell) Activator.CreateInstance(cell, simulation, startingVector,
                            (0.875 + random.NextDouble() / 4), 0.8);
        }

        public double EnvironmentalAccess
        {
            get
            {
                List<Cell> touchingCells = simulation.GetMyAdjacentNeighbours(this);
                
                int fi1Sections = 4;
                int fi2Sections = 4;
                int numFree = 0;
                bool intersection = false;
                Vector turnVector;
                Vector curTest = new Vector();
                for (double fi1 = 0; fi1 < Math.PI * 2; fi1+=Math.PI*2.0/fi1Sections)
                    for (double fi2 = 0; fi2 < Math.PI * 2; fi2 += Math.PI * 2.0 / fi2Sections)
                    {
                        turnVector = new Vector(Math.Cos(fi1) * Math.Cos(fi2), Math.Sin(fi1) * Math.Cos(fi2), Math.Sin(fi2));
                        curTest = this.position + turnVector;
                        intersection = false;
                        foreach (Cell cell in touchingCells)
                        {
                            if (curTest.DistanceFrom(cell.position) < cell.radius)
                            {
                                intersection = true;
                                break;
                            }
                        }
                        if (!intersection)
                        {
                            numFree++;
                        }

                    }

                return ((double)numFree / (double)(fi1Sections * fi2Sections));
            }
        }

        public Vector FromTheCrowd
        {
            get
            {
                Vector retval = new Vector();
                retval = this.position * surroundingCells.Count;
                foreach (Cell cell in surroundingCells)
                {
                    retval -= cell.position;
                }
                return retval;
            }
        }


        public void LiveOn()
        {
            CellLiveOn();
            age++;
        }

        #region      --- Genetic Actions ---

        /// <summary>
        /// Creates an offspring cell. Divison plane is positioned randomly;
        /// </summary>
        /// <returns></returns>
        public Cell SpawnWherever()
        {
            Cell newCell = (Cell) Activator.CreateInstance(GetType(), simulation,
                    this.position + (Vector.CreateRandom() * this.radius / 5), this.radius, this.resilience);
            numDivisions++;
            newCell.InheritFrom(this);
            simulation.RegisterNewCell(newCell);
            return newCell;
        }


        /// <summary>
        /// Creates an offspring cell at specified location.
        /// </summary>
        /// <returns></returns>
        public Cell SpawnAt(double x, double y, double z)
        {
            Cell newCell = (Cell) Activator.CreateInstance(GetType(), simulation,
                    new Vector(x, y, z), this.radius, this.resilience);
            numDivisions++;
            newCell.InheritFrom(this);
            simulation.RegisterNewCell(newCell);
            return newCell;
        }

        public void MoveFromTheCrowd()
        {
            MoveFromTheCrowd(false);
        }

        public void MoveFromTheCrowd(bool force)
        {
            MoveFromTheCrowd(force, this.radius);
        }

        public void MoveFromTheCrowd(bool force, double distance)
        {
            if (!IsMoving || force)
            {
                activeMovingDirection = FromTheCrowd.Normalize();
                desiredDistance = distance;
                IsMoving = true;
            }
        }

        public void MoveToTheCrowd()
        {
            MoveToTheCrowd(false);
        }

        public void MoveToTheCrowd(bool force)
        {
            MoveToTheCrowd(force, this.radius);
        }

        public void MoveToTheCrowd(bool force, double distance)
        {
            if (!IsMoving || force)
            {
                activeMovingDirection = FromTheCrowd.Normalize();
                activeMovingDirection.Invert();
                desiredDistance = distance;
                IsMoving = true;
            }
        }

        public void Die()
        {
            try
            {
                BreakFree();
                simulation.UnregisterCell(this);
                this.parent.offspring.Remove(this);
            }
            catch (Exception)
            {
            }
        }

        public void Spill(int proteinID)
        {
            if (!this.secrettingNow[proteinID])
            {
                this.secrettingNow[proteinID] = true;
                simulation.Register(new Source(proteinID, this.secretLevel[proteinID], this));                
            }
        }

        public void DeSpill(int proteinID)
        {
            if (this.secrettingNow[proteinID])
            {
                this.secrettingNow[proteinID] = false;
                simulation.UnRegister(proteinID, this);
            }
        }

        public void ConnectTo(Cell another)
        {
            
            if (another == null) return;
            if (!connectedCells.Contains(another))
            {
                connectedCells.Add(another);
            }
            foreach (Cell swarmer in another.connectedCells.Copy())
            {
                if (!connectedCells.Contains(swarmer))
                    connectedCells.Add(swarmer);

                if (!swarmer.connectedCells.Contains(this))
                    swarmer.connectedCells.Add(this);
            }

            if (!another.connectedCells.Contains(this))
                another.connectedCells.Add(this);
            

        }

        public void DisconnectFrom(Cell another)
        {
            if (connectedCells.Contains(another))
            {
                another.connectedCells.Remove(this);
                connectedCells.Remove(another);
            }
        }

        public void LinkTo(Cell another)
        {
            if (another == null || another == this) return;
            if (!linkedCells.Contains(another))
            {
                linkedCells.Add(another);
            }

            if (!another.linkedCells.Contains(this))
            {
                another.linkedCells.Add(this);
            }
        }

        public void UnlinkFrom(Cell another)
        {
            if (linkedCells.Contains(another))
            {
                another.linkedCells.Remove(this);
                linkedCells.Remove(another);
            }
        }

        public void BreakFree()
        {
            lock (simulation.Cells)
            {
                foreach (Cell cell in connectedCells.Copy())
                {
                    DisconnectFrom(cell);
                }

                // XXX BreakApart()?
                foreach (Cell cell in linkedCells.Copy())
                {
                    UnlinkFrom(cell);
                }
            }
        }

        /// <summary>
        /// Moves the current cell
        /// </summary>
        /// <returns>Movement direction</returns>
        public Vector Move()
        {
            if (!IsMoving)  // If I'm not moving already
            {
                activeMovingDirection = polarization.Clone();
                IsMoving = true;
                desiredDistance = this.radius;
                return activeMovingDirection;
            }
            return null;
        }

        /// <summary>
        /// Moves the current cell
        /// </summary>
        /// <param name="Force">True if the cell should be forced in new direction even if it's already moving;</param>
        /// <param name="Polarized">True if cell shoul move in the polarization direction;</param>
        /// <param name="distance">Desired distance;</param>
        /// <returns>Movement direction</returns>
        public Vector Move(bool Force, bool Polarized, double distance)
        {
            if ((!IsMoving) || (Force))  // If I'm not moving already or am forced to
            {
                if (Polarized)
                {
                    if (polarization.Length > Simulation.ALMOST_ZERO)
                    {
                        activeMovingDirection = polarization.Clone();
                    }
                    else
                    {
                        activeMovingDirection = Vector.CreateRandom();
                    }
                }
                else
                {
                    activeMovingDirection = Vector.CreateRandom();
                }
                IsMoving = true;
                desiredDistance = distance;
                return activeMovingDirection;
            }
            return null;
        }

        /// <summary>
        /// Moves the current cell according to protein distribution
        /// </summary>
        /// <param name="proteinID">Number of protein to account</param>
        /// <param name="Force">True if the cell should be forced in new direction even if it's already moving;</param>
        /// <param name="AlongGradient">True if cell should along the gradient, false if otherwize</param>
        /// <param name="distance">Desired distance;</param>
        /// <returns>Movement direction</returns>
        public Vector MoveGradient(int proteinID, bool Force, bool AlongGradient, double distance)
        {
            if ((!IsMoving) || (Force))  // If I'm not moving already or am forced to
            {
                Vector gradient = simulation.GetGradient(this.position, proteinID);
                if (AlongGradient)
                {
                    if (gradient.Length > Simulation.ALMOST_ZERO)
                    {
                        activeMovingDirection = gradient;
                    }
                    else
                    {
                        activeMovingDirection = Vector.CreateRandom();
                    }
                }
                else
                {
                    if (gradient.Length > Simulation.ALMOST_ZERO)
                    {
                        activeMovingDirection = gradient;
                        activeMovingDirection.Invert();
                    }
                    else
                    {
                        activeMovingDirection = Vector.CreateRandom();
                    }
                }
                IsMoving = true;
                desiredDistance = distance;
                return activeMovingDirection;
            }
            return null;
        }

        #endregion
    }

}
