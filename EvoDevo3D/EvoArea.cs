using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using EvoDevo3D.Support;

namespace EvoDevo3D
{
    // Plug from http://www.100byte.ru/stdntswrks/cshrp/sphTK/sphTK.html
    public class EvoArea : GLControl
    {
        private Simulation simulation;
        public Simulation Simulation
        {
            set
            {
                simulation = value;
            }
        }
        private Bitmap[] cellBitmap;
        private int[] cellTexture;

        private Color[] proteinTint;
        int cellSelectionIndex;
        public bool screenshotAwaiting = false;
        public bool rendering = false;
        private Vector3 cameraPosition = new Vector3(0, 0, 100);
        private Vector3 cameraLooksAt = new Vector3(0, 0, 0);
        private Vector3 upVector = new Vector3(0, 1, 0);
        private float cameraPositionAngleAroundUpVector = 0;
        private float cameraPositionAngleAroundRightVector = 0;
        private bool[] visibility;

        /// <summary>
        /// Creates new Render window instance;
        /// </summary>
        public EvoArea()
        {
            KeyDown += Keyboard_KeyDown;
            BackColor = Color.LightGray;
        }

        public void Resize()
        {
            ((EvoForm)FindForm()).Resize();

            if (ClientSize.Height == 0)
                ClientSize = new System.Drawing.Size(ClientSize.Width, 1);

            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 8,
                Width / (float)Height, 50f, 1000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        public void SetVisibility(int cellType, bool visible)
        {
            visibility[cellType] = visible;
        }

        public void TogglePause()
        {
            simulation.paused = !simulation.paused;
        }

        public void Step()
        {
            simulation.paused = false;
            simulation.newActionAllowed = true;
        }

        public void Screenshot()
        {
            screenshotAwaiting = true;
        }

        public void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            Vector3 move = Vector3.Zero;
            Vector3 zoom = Vector3.Zero;
            Vector3 normalCamera = cameraLooksAt - cameraPosition;
            normalCamera.Normalize();
            Vector3 rightVector = Vector3.Transform(upVector,
                    Matrix3.CreateFromAxisAngle(normalCamera, (float)Math.PI / 2));

            bool turnAroundUpVector = false;
            bool turnAroundRightVector = false;
            bool turnUpVectorItself = false;
            float upVectorTurn = 0;

            bool shiftPressed = e.Shift;
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
            }
            if (e.KeyCode == Keys.W && !shiftPressed)
            {
                cameraPositionAngleAroundRightVector = +0.1f;
                turnAroundRightVector = true;
            }
            if (e.KeyCode == Keys.S && !shiftPressed)
            {
                cameraPositionAngleAroundRightVector = -0.1f;
                turnAroundRightVector = true;
            }
            if (e.KeyCode == Keys.A && !shiftPressed)
            {
                cameraPositionAngleAroundUpVector = +0.1f;
                turnAroundUpVector = true;
            }
            if (e.KeyCode == Keys.D && !shiftPressed)
            {
                cameraPositionAngleAroundUpVector = -0.1f;
                turnAroundUpVector = true;
            }
            if (e.KeyCode == Keys.Q)
            {
                upVectorTurn = -0.1f;
                turnUpVectorItself = true;
            }
            if (e.KeyCode == Keys.E)
            {
                upVectorTurn = +0.1f;
                turnUpVectorItself = true;
            }
            if (e.KeyCode == Keys.Right || (shiftPressed && e.KeyCode == Keys.D))
                move -= rightVector;
            if (e.KeyCode == Keys.Left || (shiftPressed && e.KeyCode == Keys.A))
                move += rightVector;
            if (e.KeyCode == Keys.Up || (shiftPressed && e.KeyCode == Keys.W))
                move -= upVector;
            if (e.KeyCode == Keys.Down || (shiftPressed && e.KeyCode == Keys.S))
                move += upVector;

            Vector3 dst = (cameraPosition - cameraLooksAt) / 100f;
            if (shiftPressed)
            {
                dst *= 10f;
            }
            else if (e.Alt)
            {
                dst *= 0.1f;
            }
            if (e.KeyCode == Keys.R)
            {
                zoom += dst;
            }
            if (e.KeyCode == Keys.F)
            {
                zoom -= dst;
            }
            if (e.KeyCode == Keys.Enter)
            {
                Step();
            }
            if (e.KeyCode == Keys.X && shiftPressed)
            {
                FindForm().Dispose();
                return;
            }
            if (move.Length > 0)
            {                
                cameraPosition += move;
                cameraLooksAt += move;
            }
            if (zoom.Length > 0)
            {
                cameraPosition += zoom;
            }
            if (turnAroundUpVector)
            {
                cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                        Matrix3.CreateFromAxisAngle(upVector, cameraPositionAngleAroundUpVector)) + cameraLooksAt;
            }
            if (turnAroundRightVector)
            {
                cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                        Matrix3.CreateFromAxisAngle(rightVector, cameraPositionAngleAroundRightVector)) + cameraLooksAt;

                upVector = Vector3.Transform(upVector,
                        Matrix3.CreateFromAxisAngle(rightVector, cameraPositionAngleAroundRightVector));
            }
            if (turnUpVectorItself)
            {
                Vector3 normalUnCamera = cameraPosition - cameraLooksAt;
                normalUnCamera.Normalize();
                upVector = Vector3.Transform(upVector,
                        Matrix3.CreateFromAxisAngle(normalUnCamera, upVectorTurn));
            }
            /*if (keyboard.IsKeyUp(Keys.Tab))
            {
                if (simulation.selectionTarget == null)
                {
                    simulation.selectionTarget = simulation.Cells[0];
                    cellSelectionIndex = 0;
                }
                else
                {
                    int cellsCount = simulation.Cells.Count;
                    if (shiftPressed)
                    {
                        cellSelectionIndex = (cellsCount + cellSelectionIndex - 1) % cellsCount;
                    }
                    else
                    {
                        cellSelectionIndex = (cellSelectionIndex + 1) % cellsCount;
                    }
                    simulation.selectionTarget = simulation.Cells[cellSelectionIndex];
                }
            }*/
            if (e.KeyCode == Keys.P)
            {
                Screenshot();
            }
            if (e.KeyCode == Keys.Escape)
            {
                simulation.selectionTarget = null;
            }
        }

        /// <summary>
        /// Sets up objects and graphical meshes
        /// </summary>
        private void InitializeObjects()
        {
            Color[] cellMaterial = new Color[10];
            cellBitmap = new Bitmap[10];
            cellTexture = new int[10];
            cellMaterial[0] = Color.LightGray;
            cellBitmap[0] = CreateTexture(16, 16, (x, y) => cellMaterial[0]);
            cellMaterial[1] = Color.Chartreuse;
            cellBitmap[1] = CreateTexture(16, 16, (x, y) =>
                (y == 7 || y == 8 || y == 9 ? Color.Black : cellMaterial[1]));
            cellMaterial[2] = Color.Chocolate;
            cellBitmap[2] = CreateTexture(16, 16, (x, y) =>
                (x == 7 || y == 7 || x == 8 || y == 8 ? Color.Black : cellMaterial[2]));
            cellMaterial[3] = Color.Fuchsia;
            cellBitmap[3] = CreateTexture(16, 16, (x, y) =>
                (x < 8 == y < 8 ? Color.Black : cellMaterial[3]));
            cellMaterial[4] = Color.CornflowerBlue;
            cellBitmap[4] = CreateTexture(16, 16, (x, y) =>
                ((x + y) % 16 == 0 || (x + y) % 16 == 1 ? Color.Black : cellMaterial[4]));
            cellMaterial[5] = Color.ForestGreen;
            cellBitmap[5] = CreateTexture(16, 16, (x, y) => 
                (x == 7 || x == 8 || x == 9 ? Color.Black : cellMaterial[5]));
            cellMaterial[6] = Color.IndianRed;
            cellBitmap[6] = CreateTexture(16, 16, (x, y) => 
                ((x == 4 || x == 5 || x == 11 || x == 12) && (y > 4 && y < 12)) ||
                ((y == 4 || y == 5 || y == 11 || y == 12) && (x > 4 && x < 12))
                ? Color.Black : cellMaterial[6]);
            cellMaterial[7] = Color.LemonChiffon;
            cellBitmap[7] = CreateTexture(16, 16, (x, y) =>
                (Math.Abs(x - 8) + Math.Abs (y - 8)) < 4 ? Color.Black : cellMaterial[7]);
            cellMaterial[8] = Color.BurlyWood;
            cellBitmap[8] = CreateTexture(16, 16, (x, y) => 
                (x % 3 == 0) && (y % 3 == 0) ? Color.Black : cellMaterial[8]);
            cellMaterial[9] = Color.Gainsboro;
            cellBitmap[9] = CreateTexture(16, 16, (x, y) =>
                x == (Math.Abs(8 - y) / 2) || (x + 1) == (Math.Abs(8 - y) / 2) ||
                (x - 8) == (Math.Abs(8 - y) / 2) || (x - 9) == (Math.Abs(8 - y) / 2) 
                ? Color.Black : cellMaterial[9]);
            proteinTint = new Color[10];
            proteinTint[0] = Color.Blue;
            proteinTint[1] = Color.Green;
            proteinTint[2] = Color.Firebrick;
            proteinTint[3] = Color.Bisque;
            proteinTint[4] = Color.BurlyWood;
            proteinTint[5] = Color.Chartreuse;
            proteinTint[6] = Color.Coral;
            proteinTint[7] = Color.CornflowerBlue;
            proteinTint[8] = Color.Crimson;
            proteinTint[9] = Color.DarkGoldenrod;
            visibility = Enumerable.Repeat(true, 10).ToArray();
        }

        /*protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            simulation.newActionAllowed = true;

            int stdin;
            HandleKeyboard(Keyboard.GetState(), 
                buffer.TryTake(out stdin, 0) ? stdin : 0);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Environment.Exit(0);
        }*/

        private void DoDraw(bool forScreenshot)
        {
            Resize();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.LightGray);
            GL.Color3(Color.LightGray);
            GL.Enable(EnableCap.DepthTest);
            int nx, ny;
            // Число сегментов в моддели сферы по X и Y
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            // Сооздаем текстуру
            GL.Enable(EnableCap.Texture2D);
            //MakeTexture();
            // Сооздаем материал и источник света
            // Выводим сферу
            PlaceCamera();

            SetUpLight();
            int visibleCells = DrawCells();
            GL.Disable(EnableCap.Texture2D);
            this.SwapBuffers();
            //GraphicsDevice.Clear(Color.LightGray);


            if (!forScreenshot)
            {
                //DrawConcentrations();
                FindForm().Text = "Age: " + simulation.Cells[0].age + " Cells: " + visibleCells;
            }
        }
 

        protected override void OnPaint(PaintEventArgs ev)
        {
            try
            {
                if (simulation.paused) {
                    cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                            Matrix3.CreateFromAxisAngle(upVector, -0.05f)) + cameraLooksAt;
                    ((EvoForm)FindForm()).Paused();
                }

                if (screenshotAwaiting) {
                    screenshotAwaiting = false;
                    Bitmap screenshot = GrabScreenshot();
                    screenshot.Save(Path.Combine(Cell.Program.Directory.FullName,
                        String.Format("{0}_{1}_{2:000}.png",
                            new Regex("\\.[a-zA-Z0-9]+").Replace(Cell.Program.Name, ""),
                            DateTime.Now.ToString("yy-MM-dd_HH.mm"),
                            simulation.Cells[0].age)));
                }
                else
                {
                    DoDraw(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPS. Rendering exception occured and was brutally ignored" + e.Message);
            }
            finally
            {
                if (!simulation.paused)
                {
                    simulation.newActionAllowed = true;
                }
            }
        }

        private void PlaceCamera()
        {
            //effect.EnableDefaultLighting();

            Matrix4 modelview = Matrix4.LookAt(cameraPosition, cameraLooksAt, upVector);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
        }

        private void SetUpLight()
        {
            // Enable Light 0 and set its parameters.
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { -0.53f, -0.57f, -0.63f});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1.0f, 0.96f, 0.81f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1.0f, 0.96f, 0.81f, 1.0f });

            GL.Light(LightName.Light1, LightParameter.Position, new float[] { 0.72f, 0.34f, 0.6f});
            GL.Light(LightName.Light1, LightParameter.Diffuse, new float[] { 0.96f, 0.76f, 0.41f, 1.0f });
            GL.Light(LightName.Light1, LightParameter.Specular, new float[] { 0.0f, 0.0f, 0.0f, 0.0f });

            GL.Light(LightName.Light2, LightParameter.Position, new float[] { 0.45f, -0.77f, 0.45f});
            GL.Light(LightName.Light2, LightParameter.Diffuse, new float[] { 0.32f, 0.36f, 0.4f, 1.0f });
            GL.Light(LightName.Light2, LightParameter.Specular, new float[] { 0.32f, 0.36f, 0.4f, 1.0f });

            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.05f, 0.01f, 0.18f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Light2);
        }

        /*private void DrawConcentrations()
        {
            Vector cameraAt = new Vector(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);

            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            foreach (Source source in simulation.Sources.Copy().OrderBy(
                source => -(source.position - cameraAt).Length))
            {
                Matrix location = Matrix.CreateTranslation((float)source.position.x,
                                    (float)source.position.y, (float)source.position.z);

                effect.World = location;
                effect.DiffuseColor = proteinTint[source.secretID % 10].ToVector3() * (0.05f * (float) source.strength);
                effect.LightingEnabled = false;
                foreach (SpherePrimitive concentrationSphere in concentrationSpheres) 
                {
                    concentrationSphere.Draw(effect);
                }
            }

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
        }*/

        private int DrawCells()
        {
            int visibleCells = 0;
            Vector cameraAt = new Vector(cameraPosition.X, cameraPosition.Y, cameraPosition.Z);
            foreach (Cell currenttarget in simulation.Cells.Copy().OrderBy(
                cell => -(cell.position - cameraAt).Length))
            {
                if (currenttarget.cellType >= 0
                        && currenttarget.cellType < visibility.Length
                        && !visibility[currenttarget.cellType])
                {
                   continue;
                }

                visibleCells++;

                int currentTexture;
                if (currenttarget.cellType > 0 && currenttarget.cellType < 10)
                {
                    currentTexture = cellTexture[currenttarget.cellType];
                }
                else
                {
                    currentTexture = cellTexture[0];
                }

                GL.BindTexture(TextureTarget.Texture2D, currentTexture);

                GL.PushMatrix();
                GL.Translate(new Vector3(
                    (float)currenttarget.position.x,
                    (float)currenttarget.position.y,
                    (float)currenttarget.position.z));
                Sphere(currenttarget.radius, 4.0);
                GL.PopMatrix();
            }
            return visibleCells;
        }

        public static Bitmap CreateTexture(int width, int height, Func<int,int,Color> paint)
        {
            Bitmap texture = new Bitmap(width, height);

            for(int pixel = 0; pixel<width * height; pixel++)
            {
                texture.SetPixel(pixel / width, pixel % width,
                    paint(pixel / width, pixel % width));
            }

            return texture;
        }
            
        public void MakeTexture()
        {
            GL.Enable(EnableCap.Texture2D);
            int i = 0;
            foreach (Bitmap bitmap in cellBitmap)
            {
                GL.GenTextures(1, out cellTexture[i]);
                GL.BindTexture(TextureTarget.Texture2D, cellTexture[i]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);
                i++;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            InitializeObjects();

            double crds = 12;

            Resize();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-crds, crds, -crds, crds, -crds, crds);

            GL.Rotate(15, new Vector3d(0, 1, 0));
            GL.Rotate(-55, new Vector3d(1, 0, 0));

            MakeTexture();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            simulation.Dispose();
        }

        private void Sphere(double r, double repeats)
        {
            int nx = 32, ny = 32;
            int ix, iy;
            double x, y, z, sy, cy, sy1, cy1, sx, cx, piy, pix, ay, ay1, ax, tx, ty, ty1, dnx, dny, diy;
            dnx = 1.0 / (double)nx;
            dny = 1.0 / (double)ny;
            GL.Begin(PrimitiveType.QuadStrip);
            piy = Math.PI * dny;
            pix = Math.PI * dnx;
            for (iy = 0; iy < ny; iy++)
            {
                diy = (double)iy;
                ay = diy * piy;
                sy = Math.Sin(ay);
                cy = Math.Cos(ay);
                ty = diy * dny;
                ay1 = ay + piy;
                sy1 = Math.Sin(ay1);
                cy1 = Math.Cos(ay1);
                ty1 = ty + dny;
                for (ix = 0; ix <= nx; ix++)
                {
                    ax = 2.0 * ix * pix;
                    sx = Math.Sin(ax);
                    cx = Math.Cos(ax);
                    x = r * sy * cx;
                    y = r * sy * sx;
                    z = -r * cy;
                    tx = (double)ix * dnx;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tx * repeats, ty * repeats);
                    GL.Vertex3(x, y, z);
                    x = r * sy1 * cx;
                    y = r * sy1 * sx;
                    z = -r * cy1;
                    GL.Normal3(x, y, z);
                    GL.TexCoord2(tx * repeats, ty1 * repeats);
                    GL.Vertex3(x, y, z);
                }
            }
            GL.End();
        }
    }
}
