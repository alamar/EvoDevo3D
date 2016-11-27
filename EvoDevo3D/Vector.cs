using System;
namespace EvoDevo4
{
    public class Vector
    {
        private static Random random = new Random();
        public double x;
        public double y;
        public double z;


        /// <summary>
        /// Sets up a new flat vector having two nontrivial coordinates;
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Sets up ne trivial flat vector;
        /// </summary>
        public Vector()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(b.x + a.x, b.y + a.y, a.z + b.z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.x * b, a.y * b, a.z * b);
        }

        public static Vector operator *(double b, Vector a)
        {
            return new Vector(a.x * b, a.y * b, a.z * b);
        }

        public static Vector operator /(Vector a, double b)
        {
            if (b < Simulation.ALMOST_ZERO) throw new ArgumentException("Division by almost zero.");
            return new Vector(a.x / b, a.y / b, a.z / b);
        }

        /// <summary>
        /// Returns distance from a to b;
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns></returns>
        public static double Distance(Vector a, Vector b)
        {
            return (Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z)));
        }

        public static double SqDistance(Vector a, Vector b)
        {
            return ((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        }
        
        public double DistanceFrom(Vector a)
        {
            return Math.Sqrt((a.x - this.x) * (a.x - this.x) + (a.y - this.y) * (a.y - this.y) + (a.z - this.z) * (a.z - this.z));
            //return (Math.Sqrt(Math.Pow(a.x - this.x, 2) + Math.Pow(a.y - this.y, 2) + Math.Pow(a.z - this.z, 2)));
        }

        /// <summary>
        /// Normalizes the vector setting the length to  1
        /// </summary>
        public Vector Normalize()
        {
            return Normalize(1.0);
            
        }

        /// <summary>
        /// Normalizes the vector setting the length to a desired value;
        /// </summary>
        /// <param name="desiredLength">Desired length</param>
        public Vector Normalize(double desiredLength)
        {
            Vector v = new Vector();
            v.x = x;
            v.y = y;
            v.z = z;
            double len = this.Length;
            if (desiredLength < Simulation.ALMOST_ZERO)
            {
                return new Vector(0.0, 0.0, 0.0);
            }
            if (len > Simulation.ALMOST_ZERO)
            {
                v.x *= 1.0 / len;
                v.y *= 1.0 / len;
                v.z *= 1.0 / len;
                v.x *= desiredLength;
                v.y *= desiredLength;
                v.z *= desiredLength;
                return v;
            }
            else
            {
                return new Vector(0.0, 0.0, 0.0);
            }
        }

        /// <summary>
        /// Turns Vector specific angle around turnAxis
        /// </summary>
        /// <param name="a">Vector that should be turned</param>
        /// <param name="angle">Turn angle measured in radians</param>
        /// <param name="turnAxis">Axis to turn vector around</param>
        /// <returns>Turned vector</returns>
        public static Vector Turn(Vector a, double angle, Vector turnAxis)
        {
            turnAxis.Normalize();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            double x = a.x * (cos + (1 - cos) * turnAxis.x * turnAxis.x + (1 - cos) * turnAxis.x * turnAxis.y - sin * turnAxis.z + (1 - cos) * turnAxis.x * turnAxis.z + sin * turnAxis.y);
            double y = a.y * ((1 - cos) * turnAxis.y * turnAxis.x + sin * turnAxis.z + cos + (1 - cos) * turnAxis.y * turnAxis.y + (1 - cos) * turnAxis.y * turnAxis.z - sin * turnAxis.x);
            double z = a.z * ((1 - cos) * turnAxis.z * turnAxis.x - sin * turnAxis.y + (1 - cos) * turnAxis.z * turnAxis.y + sin * turnAxis.x + cos + (1 - cos) * turnAxis.z * turnAxis.z);
            return (new Vector(x, y, z));
        }

        /// <summary>
        /// Creates vector opposite of given
        /// </summary>
        /// <param name="a">Vector t invert</param>
        /// <returns></returns>
        public static Vector Invert(Vector a)
        {
            return new Vector(-1 * a.x, -1 * a.y, -1 * a.z);
        }

        /// <summary>
        /// Turns Vector specific angle around random axis
        /// </summary>
        /// <param name="a">Vector that should be turned</param>
        /// <param name="angle">Turn angle measured in radians</param>
        /// <returns>Turned vector</returns>
        public static Vector Turn(Vector a, double angle)
        {
            return Vector.Turn(a, angle, Vector.CreateRandom());
        }

        /// <summary>
        /// Turns Vector around random axis;
        /// </summary>
        /// <param name="angle">Turn angle measured in radians</param>
        public void Turn(double angle)
        {
            Vector temp = Turn(this, angle);
            this.x = temp.x;
            this.y = temp.y;
            this.z = temp.z;
        }

        /// <summary>
        /// Turns vector 180 degrees... clockwise:)
        /// </summary>
        public void Invert()
        {
            Vector temp = Vector.Invert(this);
            this.x = temp.x;
            this.y = temp.y;
            this.z = temp.z;
        }

        /// <summary>
        /// Turns Vector around given axis;
        /// </summary>
        /// <param name="angle">Turn angle measured in radians</param>
        public void Turn(double angle, Vector axis)
        {
            Vector temp = Turn(this, angle, axis);
            this.x = temp.x;
            this.y = temp.y;
            this.z = temp.z;
        }

        public double Length
        {
            get
            {
                return (Math.Sqrt(x*x + y*y + z*z));
            }
            set
            {
                if (value < Simulation.ALMOST_ZERO)
                {
                    this.x = 0;
                    this.y = 0;
                    this.z = 0;
                }
                if ((x*x+ y*y+z*z) > Simulation.ALMOST_ZERO)
                {
                    Vector temp = this.Normalize(value);
                    this.x = temp.x;
                    this.y = temp.y;
                    this.z = temp.z;
                }
                else
                {
                    this.x = 0;
                    this.y = 0;
                    this.z = 0;
                }
            }
        }

        /// <summary>
        /// Generates random normalized vector;
        /// </summary>
        /// <returns>Generated vector;</returns>
        public static Vector CreateRandom()
        {
            double fi = random.NextDouble() * Math.PI * 2;
            double fi1 = random.NextDouble() * Math.PI * 2;

            Vector retval = new Vector(Math.Cos(fi) * Math.Cos(fi1), Math.Sin(fi) * Math.Cos(fi1), Math.Sin(fi1));
            return retval;
        }

        /// <summary>
        /// Duplicates thi given Vector
        /// </summary>
        /// <param name="OriginalVector">Vector to copy</param>
        /// <returns>Copy of a vector</returns>
        public static Vector CopyFrom (Vector one)
        {
            return new Vector(one.x, one.y, one.z);
        }

        public Vector Clone()
        {
            return (new Vector(this.x, this.y, this.z));
        }

        public void DuplicateFrom(Vector that)
        {
            this.x = that.x;
            this.y = that.y;
            this.z = that.z;
        }

        /// <summary>
        /// Resets vector coordinates to zero;
        /// </summary>
        /// <returns>Zero-length vector</returns>
        public Vector Trivialize()
        {
            x = 0;
            y = 0;
            z = 0;
            return (this);
        }

        public double mod
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public double modSq
        {
            get
            {
                return (x * x + y * y + z * z);
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }
    }
}
