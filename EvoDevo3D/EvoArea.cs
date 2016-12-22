using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EvoDevo4.Primitives;

namespace EvoDevo4
{
    public class EvoArea : Game 
    {
        private Simulation simulation;
        private Matrix cameraProjection = Matrix.Identity;
        private SpriteFont textRenderer;
        private Matrix cameraView = Matrix.Identity;
        private Vector mouseRay = new Vector();
        private Vector mousePos = new Vector();
        private ArrayList celllist = new ArrayList();
        private ModelMesh cellmesh;
        private SpherePrimitive sphere;
        private SpherePrimitive[] concentrationSpheres;
        private Color[] cellMaterial;
        int cellSelectionIndex;
        private Color selectedCellMaterial;
        private bool deviceBlock = true;
        private int WIDTH = 256;
        private int HEIGHT = 256;
        public bool screenshotAwaiting = false;
        public bool rendering = false;
        private int frameNo = 0;
        private string screenshotFile = @"screenshot.bmp";
        private Vector startDragPosition = new Vector();
        private bool isDragging= false;
        private Vector visualShift = new Vector();
        private float turnAxis1=0;
        private Vector3 cameraPosition = new Vector3(0, 0, 200);
        private Vector3 cameraLooksAt = new Vector3(0, 0, 0);
        private Vector3 upVector = new Vector3(0, 1, 0);
        private float cameraPositionAngleAroundUpVector = 0;
        private float cameraPositionAngleAroundRightVector = 0;
        private GraphicsDeviceManager graphics;
        private BasicEffect effect;
        private bool forceRedraw = false;

        /// <summary>
        /// Creates new Render window instance;
        /// </summary>
        public EvoArea(Simulation simulation) 
        {
            this.simulation = simulation;
            graphics = new GraphicsDeviceManager (this);

            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = false;   
           
            //if (!InitializeDevice()) throw new Exception("DirectX initialization failed");
            /*SetUpCamera();
            VertexDeclaration();
            IndicesDeclaration();
            InitializeTextOutput();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 50;
            timer.Enabled = true;
            */
            //gc.BringToFront();
        }

        protected override void LoadContent()
        {
            InitializeObjects();
            deviceBlock = false;
        }

        private void HandleKeyboard(KeyboardState keyboard)
        {
            if (keyboard.GetPressedKeys().Length == 0)
            {
                return;
            }
            this.forceRedraw = true;

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
            if (keyboard.IsKeyDown(Keys.Space))
                screenshotAwaiting = true;
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
                return;
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
/*
        private void Screenshot()
        {
            RenderTargetBinding renderTarget = device.GetRenderTargets()[0];
            DateTime n = DateTime.Now;
         


            if (!rendering)
            {
                screenshotFile = n.ToString("yyyy-MM-dd_HH-mm-ss") + ".bmp";
            }
            else
            {
                screenshotFile = "render_at_" + n.ToString("yyyy-MM-dd") + "_frame_" + frameNo++.ToString() + ".bmp";
            }
//            SurfaceLoader.Save(screenshotFile, ImageFileFormat.Bmp, renderTarget);
            screenshotAwaiting = false;
        }*/
        /*
        private void InitializeTextOutput()
        {
            //textRenderer = new SD.Font(SD.FontFamily.GenericMonospace, 8);
        }
            System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericMonospace,8);
            TextRenderer = new Microsoft.DirectX.Direct3D.Font(device, font);
        }*/
    /*
        private void HandleMouse(MouseState mouse)
        {
            Matrix transformation = Matrix.CreateTranslation((float)visualShift.x, (float)visualShift.y, 0);
            
            Vector3 near = graphics.GraphicsDevice.Viewport.Unproject(new Vector3(mouse.X, mouse.Y, 0.3f),
                    cameraProjection, cameraView, transformation);
            Vector3 far = graphics.GraphicsDevice.Viewport.Unproject(new Vector3(mouse.X, mouse.Y, 1f),
                    cameraProjection, cameraView, transformation);
            mouseRay = new Vector(far.X - near.X, far.Y - near.Y, far.Z - near.Z);
            mousePos = new Vector(near.X, near.Y, near.Z);

            if (simulation.Cells.Count < 500 || mouse.LeftButton == ButtonState.Pressed)
            {
                simulation.ReTarget(mouseRay, mousePos);
            }
        }*/

/*        public int frames;
        public DateTime initTime = DateTime.Now;
        private void ReInitializeDevice()
        {
            deviceBlock = true;
            if (device == null) return;
            device.RenderState.Lighting = true;

            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0, 0, -1);
            device.Lights[0].Update();
            device.Lights[0].Enabled = true;


            device.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
            device.SamplerState[0].MagFilter = TextureFilter.Anisotropic;

            device.SamplerState[0].AddressU = TextureAddress.Mirror;
            device.SamplerState[0].AddressV = TextureAddress.Mirror;
* /
           

            deviceBlock = false;
        }*/

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

            selectedCellMaterial = Color.Gray;
        }

        /// <summary>
        /// Sets up connection to a device
        /// </summary>
        /// <returns>True if device was created successfully and false othewise</returns>
        /*public bool InitializeDevice()
        {
            deviceBlock = true;
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.AutoDepthStencilFormat = DepthFormat.D24S8;
            presentParams.EnableAutoDepthStencil = true;
            
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            if (device == null) return (false);
            device.RenderState.Lighting = true;
            device.RenderState.CullMode = Cull.None;
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0, 0, -1);
            device.Lights[0].Update();
            device.Lights[0].Enabled = true;

            device.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
            device.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
            
            device.SamplerState[0].AddressU = TextureAddress.Mirror;
            device.SamplerState[0].AddressV = TextureAddress.Mirror;
            
            //device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
            device.DeviceReset += new EventHandler(device_DeviceReset);
            device.Disposing += new EventHandler(device_Disposing);
            
            
            deviceBlock = false;
            return (true);
        }*/

        void device_Disposing(object sender, EventArgs e)
        {
        }

        void device_DeviceReset(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }



        void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            deviceBlock = true;
        }

        /*protected override void  OnSizeChanged(EventArgs e)
        {
            ReInitializeDevice();
            SetUpCamera();
            this.Invalidate();
            deviceBlock = false;
        }*/

        /*private void SetUpCamera()
        {
            cameraProjection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 8, 1f/ *(float)this.Width / (float)this.Height* /, 0.3f, 400f);
            cameraView = Matrix.CreateLookAt(new Vector3(0f, 0f, 190f), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }*/

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
            HandleKeyboard(Keyboard.GetState());
            //HandleMouse(Mouse.GetState());
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            simulation.paused = true;
            Exit();
        }
 
        protected override void Draw(GameTime gameTime)
        {
            if (deviceBlock || (!forceRedraw && simulation.state != Simulation.State.None))
            {
                return;
            }
            forceRedraw = false;
            //SetUpCamera();
            GraphicsDevice.Clear(Color.LightGray);//Color.GhostWhite);

            //frames++;
            try
            {
                //device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
//                using (GraphicsDevice device = graphics.GraphicsDevice) {
//                    device.Clear(Color.White);
                if (simulation.paused) {
                    cameraPosition = Vector3.Transform(cameraPosition - cameraLooksAt,
                            Matrix.CreateFromAxisAngle(upVector, -0.01f)) + cameraLooksAt;
                }
                    PlaceCamera();
                    //device.BeginScene();


                    //device.Transform.World = Matrix.Identity;


//                    device.SetVertexBuffer(vb);
//                    device.Indices = ib;

                    //device.Transform.World = Matrix.Translation(-HEIGHT / 2, -WIDTH / 2, 0);

                   
                    
                    //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, WIDTH * HEIGHT, 0, indices.Length / 3);
                    //device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, indices.Length, indices.Length / 3, indices, true, vertices);
                    
                    DrawCells();
                    DrawConcentrations();
                    //DrawTargetData();
                    //DrawConcentrationsData();
                    
                    //device.EndScene();
                    //if (screenshotAwaiting||rendering)
                    //    Screenshot();

//                    device.Present();
//                }
                //this.Invalidate();
            }
            catch (Exception e)
            {
                deviceBlock = true;
                try
                {
                    //device.EndScene();
                }
                catch (Exception)
                {
                    //ignored
                }
                //ReInitializeDevice();
                //SetUpCamera();
                //this.Invalidate();
                deviceBlock = false;
                Console.WriteLine("OOPS. Rendering exception occured and was brutally ignored" + e.Message);
            }
        }

        private void PlaceCamera()
        {
            /*effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            effect.DirectionalLight0.Direction = cameraLooksAt - cameraPosition;
            effect.DirectionalLight0.Enabled = true;*/
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
            foreach (Source source in simulation.Sources)
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
                effect.DiffuseColor = source.color.ToVector3() * (0.05f * (float) source.strength);
                effect.LightingEnabled = false;
                foreach (SpherePrimitive concentrationSphere in concentrationSpheres) 
                {
                    concentrationSphere.Draw(effect);
                }
            }

            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            /*device.Transform.World = Matrix.Identity;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            device.VertexFormat = VertexPositionColor.Format;
            device.RenderState.AlphaBlendEnable = false;*/
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
        /*private void DrawConcentrationsData()
        {
            / *string outStr = "(";
            for (int i=0;i<SignallingProtein.Array.Count;i++)
            {
                if (i > 0)
                    outStr += ", ";
                outStr += simulation.GetConcentration(mousePosition, i).ToString("f");
            }
            outStr += ")";* /
            string outStr = mouseRay.x + " " + mouseRay.y + " " + mouseRay.z;
            //TextRenderer.DrawText(null, outStr, new Point(10, this.ClientSize.Height - 40), Color.DarkGreen);
        }*/

        /*private void DrawTargetData()
        {            
            if (simulation.selectionTarget != null)
            {
                Cell cell = simulation.selectionTarget;
                
                //TextRenderer.DrawText(null, cell.ToString(), new Point(20, 28), Color.DarkBlue);
            }
        }*/
        private void DrawCells()
        {
            //device.SetTexture(0, null);
            foreach (Cell currenttarget in simulation.Cells)
            {
                /*switch (currenttarget.cellType)
                {
                    case 0:
                        if (!chb0Visible.Checked) continue;
                        break;
                    case 1:
                        if (!chb1Visible.Checked) continue;
                        break;
                    case 2:
                        if (!chb2Visible.Checked) continue;
                        break;
                    case 3:
                        if (!chb3Visible.Checked) continue;
                        break;
                    case 4:
                        if (!chb4Visible.Checked) continue;
                        break;
                    case 5:
                        if (!chb5Visible.Checked) continue;
                        break;
                    case 6:
                        if (!chb6Visible.Checked) continue;
                        break;
                    case 7:
                        if (!chb7Visible.Checked) continue;
                        break;
                    case 8:
                        if (!chb8Visible.Checked) continue;
                        break;
                    case 9:
                        if (!chb9Visible.Checked) continue;
                        break;

                }*/
                /*device.Transform.World = */
                Matrix location = /*Matrix.CreateScale((float)currenttarget.radius,
                            (float)currenttarget.radius, (float)currenttarget.radius)
                        */ Matrix.CreateTranslation((float)currenttarget.position.x,
                            (float)currenttarget.position.y, (float)currenttarget.position.z); 
                Color currentMaterial;
                if (currenttarget.color > 0 && currenttarget.color < 10)
                {
                    currentMaterial = cellMaterial[currenttarget.color];
                }
                else
                {
                    currentMaterial = cellMaterial[0];
                }

                effect.World = location;
                effect.DiffuseColor = currentMaterial.ToVector3();
                sphere.Draw(effect);
                //cellmesh.DrawSubset(0);


            }
            /*if (simulation.selectionTarget != null)
            {
                Cell currenttarget = simulation.selectionTarget;
                //device.Transform.World = Matrix.Scaling((float)currenttarget.radius, (float)currenttarget.radius, (float)currenttarget.radius) * Matrix.Translation((float)currenttarget.position.x + (float)visualShift.x, (float)currenttarget.position.y + (float)visualShift.y, 0);
                device.Transform.World = Matrix.Scaling((float)currenttarget.radius, (float)currenttarget.radius, (float)currenttarget.radius) * Matrix.Translation((float)currenttarget.position.x, (float)currenttarget.position.y, (float)currenttarget.position.z);
                
                device.Material = selectedCellMaterial;
                cellmesh.DrawSubset(0);
            }*/
        }
   
        static Random random = new Random();


    /*private void RenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Vector3 near = new Vector3(e.X, e.Y, 1f);
                Viewport vp = new Viewport();
                vp.MaxDepth = 1;
                vp.MinDepth = 0;
                vp.X = 0;
                vp.Y = 0;
                vp.Height = this.ClientSize.Height;
                vp.Width = this.ClientSize.Width;

                Matrix transformation = Matrix.CreateTranslation((float)visualShift.x, (float)visualShift.y, 0);
                vp.Unproject(near, cameraProjection, cameraView, transformation);
                near.X *= 0.95f;
                near.Y *= 0.95f;

                startDragPosition.x = near.X - visualShift.x;
                startDragPosition.y = near.Y - visualShift.y;
                isDragging = true;
            }
            else
            {
                Console.WriteLine(e.Button);
            }
        }*/
    }
}
