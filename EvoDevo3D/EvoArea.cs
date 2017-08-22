using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EvoDevo4.Primitives;

namespace EvoDevo4
{
    public class EvoArea : Game 
    {
        // XXX remove?
        private Simulation simulation;
        public Simulation Simulation
        {
            set
            {
                simulation = value;
            }
        }
        private Matrix cameraProjection = Matrix.Identity;
        private Matrix cameraView = Matrix.Identity;
        private SpherePrimitive sphere;
        private SpherePrimitive[] concentrationSpheres;
        private Color[] cellMaterial;
        private Color[] proteinTint;
        int cellSelectionIndex;
        public bool screenshotAwaiting = false;
        public bool rendering = false;
        private Vector3 cameraPosition = new Vector3(0, 0, 200);
        private Vector3 cameraLooksAt = new Vector3(0, 0, 0);
        private Vector3 upVector = new Vector3(0, 1, 0);
        private float cameraPositionAngleAroundUpVector = 0;
        private float cameraPositionAngleAroundRightVector = 0;
        private GraphicsDeviceManager graphics;
        private BasicEffect effect;
        private bool[] visibility;

        private readonly BlockingCollection<int> buffer = new BlockingCollection<int>(1);
        private readonly Thread readThread;

        /// <summary>
        /// Creates new Render window instance;
        /// </summary>
        public EvoArea()
        {
            graphics = new GraphicsDeviceManager (this);
            readThread = new Thread(() => {
                if (Console.IsInputRedirected) {
                    int i;
                    do {
                        i = Console.Read();
                        buffer.Add(i);
                    } while (i != -1);
                } else {
                    while (true) { 
                        var consoleKeyInfo = Console.ReadKey(true);
                        if (consoleKeyInfo.KeyChar == 0) continue;  // ignore dead keys
                        buffer.Add(consoleKeyInfo.KeyChar);
                    }
                }
            });
            readThread.Start();

            graphics.IsFullScreen = false;
            IsFixedTimeStep = false;
        }

        protected override void LoadContent()
        {
            InitializeObjects();
        }

        private void HandleKeyboard(KeyboardState keyboard, int stdin)
        {
            if (keyboard.GetPressedKeys().Length == 0 && stdin == 0)
            {
                return;
            }

            if (stdin >= 'a' && stdin < 'k')
            {
                visibility[stdin - 'a'] = false;
            }
            if (stdin >= 'A' && stdin < 'K')
            {
                visibility[stdin - 'A'] = true;
            }

            Vector3 move = Vector3.Zero;
            Vector3 zoom = Vector3.Zero;
            Vector3 normalCamera = cameraLooksAt - cameraPosition;
            normalCamera.Normalize();
            Vector3 rightVector = Vector3.Transform(upVector,
                    Matrix.CreateFromAxisAngle(normalCamera, (float)Math.PI / 2));

            bool turnAroundUpVector = false;
            bool turnAroundRightVector = false;
            bool turnUpVectorItself = false;
            float upVectorTurn = 0;

            bool shiftPressed = keyboard.IsKeyDown(Keys.RightShift) || keyboard.IsKeyDown(Keys.LeftShift);
            if (keyboard.IsKeyDown(Keys.Space) || stdin == ' ')
            {
                // XXX Track single press!
                simulation.paused = !simulation.paused;
            }
            if (keyboard.IsKeyDown(Keys.W) && !shiftPressed)
            {
                cameraPositionAngleAroundRightVector = +0.1f;
                turnAroundRightVector = true;
            }
            if (keyboard.IsKeyDown(Keys.S) && !shiftPressed)
            {
                cameraPositionAngleAroundRightVector = -0.1f;
                turnAroundRightVector = true;
            }
            if (keyboard.IsKeyDown(Keys.A) && !shiftPressed)
            {
                cameraPositionAngleAroundUpVector = +0.1f;
                turnAroundUpVector = true;
            }
            if (keyboard.IsKeyDown(Keys.D) && !shiftPressed)
            {
                cameraPositionAngleAroundUpVector = -0.1f;
                turnAroundUpVector = true;
            }
            if (keyboard.IsKeyDown(Keys.Q))
            {
                upVectorTurn = -0.1f;
                turnUpVectorItself = true;
            }
            if (keyboard.IsKeyDown(Keys.E))
            {
                upVectorTurn = +0.1f;
                turnUpVectorItself = true;
            }
            if (keyboard.IsKeyDown(Keys.Right) || (shiftPressed && keyboard.IsKeyDown(Keys.D)))
                move -= rightVector;
            if (keyboard.IsKeyDown(Keys.Left) || (shiftPressed && keyboard.IsKeyDown(Keys.A)))
                move += rightVector;
            if (keyboard.IsKeyDown(Keys.Up) || (shiftPressed && keyboard.IsKeyDown(Keys.W)))
                move -= upVector;
            if (keyboard.IsKeyDown(Keys.Down) || (shiftPressed && keyboard.IsKeyDown(Keys.S)))
                move += upVector;

            Vector3 dst = (cameraPosition - cameraLooksAt) / 100f;
            if (shiftPressed)
            {
                dst *= 10f;
            }
            else if (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt))
            {
                dst *= 0.1f;
            }
            if (keyboard.IsKeyDown(Keys.R))
            {
                zoom += dst;
            }
            if (keyboard.IsKeyDown(Keys.F))
            {
                zoom -= dst;
            }
            if (keyboard.IsKeyDown(Keys.Enter) || stdin == 's')
            {
                simulation.AwaitingQueue.Enqueue('s');
                simulation.paused = false;
                simulation.newActionAllowed = true;
            }
            if ((keyboard.IsKeyDown(Keys.X) && shiftPressed)
                || stdin == 'X')
            {
                Exit();
            }
            if (move.Length() > 0)
            {                
                cameraPosition += move;
                cameraLooksAt += move;
            }
            if (zoom.Length() > 0)
            {
                cameraPosition += zoom;
            }
            if (turnAroundUpVector)
            {
                cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                        Matrix.CreateFromAxisAngle(upVector, cameraPositionAngleAroundUpVector)) + cameraLooksAt;
            }
            if (turnAroundRightVector)
            {
                cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                        Matrix.CreateFromAxisAngle(rightVector, cameraPositionAngleAroundRightVector)) + cameraLooksAt;

                upVector = Vector3.Transform(upVector,
                        Matrix.CreateFromAxisAngle(rightVector, cameraPositionAngleAroundRightVector));
            }
            if (turnUpVectorItself)
            {
                Vector3 normalUnCamera = cameraPosition - cameraLooksAt;
                normalUnCamera.Normalize();
                upVector = Vector3.Transform(upVector,
                        Matrix.CreateFromAxisAngle(normalUnCamera, upVectorTurn));
            }
            if (keyboard.IsKeyUp(Keys.Tab))
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
            }
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                simulation.selectionTarget = null;
            }
        }

        /// <summary>
        /// Sets up objects and graphical meshes
        /// </summary>
        private void InitializeObjects()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
            sphere = new SpherePrimitive(graphics.GraphicsDevice, 2, 16);
            concentrationSpheres = new SpherePrimitive[3];
            concentrationSpheres[0] = new SpherePrimitive(graphics.GraphicsDevice, 3, 16);
            concentrationSpheres[1] = new SpherePrimitive(graphics.GraphicsDevice, 7, 16);
            concentrationSpheres[2] = new SpherePrimitive(graphics.GraphicsDevice, 15, 16);
            cellMaterial = new Color[10];
            cellMaterial[0] = Color.LightGray;
            cellMaterial[1] = Color.Chartreuse;
            cellMaterial[2] = Color.Chocolate;
            cellMaterial[3] = Color.Fuchsia;
            cellMaterial[4] = Color.CornflowerBlue;
            cellMaterial[5] = Color.ForestGreen;
            cellMaterial[6] = Color.IndianRed;
            cellMaterial[7] = Color.LemonChiffon;
            cellMaterial[8] = Color.BurlyWood;
            cellMaterial[9] = Color.Gainsboro;
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

        protected override void Update(GameTime gameTime)
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
        }
 
        protected override void Draw(GameTime gameTime)
        {
            /*if (deviceBlock || (!forceRedraw && simulation.state != Simulation.State.None))
            {
                return;
            }
            forceRedraw = false;*/
            GraphicsDevice.Clear(Color.LightGray);

            try
            {
                if (simulation.paused) {
                    cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                            Matrix.CreateFromAxisAngle(upVector, -0.01f)) + cameraLooksAt;
                }
                    PlaceCamera();
                    DrawCells();
                    DrawConcentrations();
                    this.Window.Title = "Age: " + simulation.Cells[0].age + " Cells: " + simulation.Cells.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPS. Rendering exception occured and was brutally ignored" + e.Message);
            }
        }

        private void PlaceCamera()
        {
            effect.EnableDefaultLighting();

            cameraProjection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 8,
                graphics.GraphicsDevice.Viewport.AspectRatio, 50f, 1000f);
            cameraView = Matrix.CreateLookAt(cameraPosition, cameraLooksAt, upVector);
            effect.Projection = cameraProjection;
            effect.View = cameraView;
        }

        private void DrawConcentrations()
        {
            List<Source> sortedSources = new List<Source>();
            foreach (Source source in simulation.Sources.Copy())
            {
                sortedSources.Add(source);
            }
            sortedSources.Sort(WhoSCloser);

            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            foreach (Source source in sortedSources)
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
        }

        private int WhoSCloser(Source a, Source b)
        {
            Vector diff = a.position - b.position;
            Vector3 dir = cameraPosition - cameraLooksAt;
            dir.Normalize();
            double cos = diff.x * dir.X + diff.y * dir.Y + diff.z * dir.Z;
            // I know that above formulae lacks lower denominator but it's always > 0 so I don't give a damn

            if (cos > 0)
                return 1;
            if (cos < 0)
                return -1;
            return 0;
        }

        private void DrawCells()
        {
            //bool[] visibility = session.Controls.visibility();
            foreach (Cell currenttarget in simulation.Cells.Copy())
            {
                if (currenttarget.cellType >= 0
                        && currenttarget.cellType < visibility.Length
                        && !visibility[currenttarget.cellType])
                {
                   continue;
                }

                Matrix location = Matrix.CreateScale((float)currenttarget.radius,
                            (float)currenttarget.radius, (float)currenttarget.radius)
                        * Matrix.CreateTranslation((float)currenttarget.position.x,
                            (float)currenttarget.position.y, (float)currenttarget.position.z); 
                Color currentMaterial;
                if (currenttarget.cellType > 0 && currenttarget.cellType < 10)
                {
                    currentMaterial = cellMaterial[currenttarget.cellType];
                }
                else
                {
                    currentMaterial = cellMaterial[0];
                }

                effect.World = location;
                effect.DiffuseColor = currentMaterial.ToVector3();
                sphere.Draw(effect);
            }
        }
    }
}
