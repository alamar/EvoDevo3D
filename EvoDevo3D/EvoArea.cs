using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private VertexPositionColor[] vertices;
        private short[] indices;
        private IndexBuffer ib;
        private VertexBuffer vb;
        private float turnAxis1=0;
        private Vector3 cameraPosition = new Vector3(0, 0, 200);
        private Vector3 cameraLooksAt = new Vector3(0, 0, 0);
        private Vector3 upVector = new Vector3(0, 0.5f, 0);
        private float cameraPositionAngleAroundUpVector = 0;
        private float cameraPositionAngleAroundRightVector = 0;
        private GraphicsDeviceManager graphics;
        private BasicEffect effect;

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

        /*void timer_Tick(object sender, EventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
        }*/

  /*      protected override void OnKeyDown(KeyEventArgs e)
        {
            Vector4 move = new Vector4(0, 0, 0, 0);
            Vector4 zoom = new Vector4(0, 0, 0, 0);
            Vector4 right = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
            bool turnAroundUpVector = false;
            bool turnAroundRightVector = false;
            bool turnUpVectorItself = false;
            float upVectorTurn = 0;
            Vector4.Transform(right, Matrix.CreateFromAxisAngle((cameraLooksAt - cameraPosition), (float)Math.PI / 2));
            if (e.KeyData == Keys.Space)
                screenshotAwaiting = true;
            if (e.KeyData == Keys.W && e.Modifiers != Keys.Shift)
            {
                cameraPositionAngleAroundRightVector = -0.1f;
                turnAroundRightVector = true;
            }
            if (e.KeyData == Keys.S && e.Modifiers != Keys.Shift)
            {
                cameraPositionAngleAroundRightVector = +0.1f;
                turnAroundRightVector = true;
            }
            if (e.KeyData == Keys.A && e.Modifiers != Keys.Shift)
            {
                cameraPositionAngleAroundUpVector = -0.1f;
                turnAroundUpVector = true;
            }
            if (e.KeyData == Keys.D && e.Modifiers != Keys.Shift)
            {
                cameraPositionAngleAroundUpVector = 0.1f;
                turnAroundUpVector = true;
            }
            if (e.KeyData == Keys.Q)
            {
                upVectorTurn = 0.1f;
                turnUpVectorItself = true;
            }
            if (e.KeyData == Keys.E)
            {
                upVectorTurn = -0.1f;
                turnUpVectorItself = true;
            }
            if (e.KeyData == Keys.Right || (e.KeyCode == Keys.D && e.Modifiers == Keys.Shift))
                move += right;
            if (e.KeyData == Keys.Left || (e.KeyCode == Keys.A && e.Modifiers == Keys.Shift))
                move -= right;
            if (e.KeyData == Keys.Up || (e.KeyCode == Keys.W && e.Modifiers == Keys.Shift))
                move -= new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
            if (e.KeyData == Keys.Down || (e.KeyCode == Keys.S && e.Modifiers == Keys.Shift))
                move += new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
            if (e.KeyCode == Keys.R)
            {
                Vector4 dst = new Vector4((cameraPosition.X - cameraLooksAt.X)/100, (cameraPosition.Y - cameraLooksAt.Y)/100, (cameraPosition.Z - cameraLooksAt.Z)/100, 0);
                if (e.Modifiers == Keys.Shift)
                {
                    zoom -= dst * 10;
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    zoom -= dst * 0.1f;
                }
                else
                {                    
                    zoom -= dst;
                }
            }
            if (e.KeyCode == Keys.F)
            {
                Vector4 dst = new Vector4((cameraPosition.X - cameraLooksAt.X) / 100, (cameraPosition.Y - cameraLooksAt.Y) / 100, (cameraPosition.Z - cameraLooksAt.Z) / 100, 0);
                if (e.Modifiers == Keys.Shift)
                {
                    zoom += dst * 10;
                }
                else if (e.Modifiers == Keys.Alt)
                {
                    zoom += dst *0.1f;
                }
                else
                {
                    zoom += dst;
                }
  
            }
            if (move.Length() > 0)
            {                
                cameraPosition.X += move.X;
                cameraPosition.Y += move.Y;
                cameraPosition.Z += move.Z;
                cameraLooksAt.X += move.X;
                cameraLooksAt.Y += move.Y;
                cameraLooksAt.Z += move.Z;
                return;
            }
            if (zoom.Length() > 0)
            {
                cameraPosition.X += zoom.X;
                cameraPosition.Y += zoom.Y;
                cameraPosition.Z += zoom.Z;
                return;
            }
            if (turnAroundUpVector)
            {
                Vector4 temp = new Vector4(cameraPosition.X - cameraLooksAt.X, cameraPosition.Y - cameraLooksAt.Y, cameraPosition.Z - cameraLooksAt.Z, 0);
                Vector4.Transform(temp, Matrix.CreateFromAxisAngle(upVector, cameraPositionAngleAroundUpVector));
       
                cameraPosition.X = cameraLooksAt.X + temp.X;
                cameraPosition.Y = cameraLooksAt.Y + temp.Y;
                cameraPosition.Z = cameraLooksAt.Z + temp.Z;
                return;
            }
            if (turnAroundRightVector)
            {
                Vector4 temp = new Vector4(cameraPosition.X - cameraLooksAt.X, cameraPosition.Y - cameraLooksAt.Y, cameraPosition.Z - cameraLooksAt.Z, 0);
                Vector4.Transform(temp, Matrix.CreateFromAxisAngle(new Vector3(right.X, right.Y, right.Z), cameraPositionAngleAroundRightVector));
                cameraPosition.X = cameraLooksAt.X + temp.X;
                cameraPosition.Y = cameraLooksAt.Y + temp.Y;
                cameraPosition.Z = cameraLooksAt.Z + temp.Z;
                temp = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
                Vector4.Transform(temp, Matrix.CreateFromAxisAngle(new Vector3(right.X, right.Y, right.Z), cameraPositionAngleAroundRightVector));
                upVector.X = temp.X;
                upVector.Y = temp.Y;
                upVector.Z = temp.Z;
                return;
            }
            if (turnUpVectorItself)
            {
                Vector3 tempAxis = new Vector3(cameraPosition.X - cameraLooksAt.X, cameraPosition.Y - cameraLooksAt.Y, cameraPosition.Z - cameraLooksAt.Z);
                Vector4 temp = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
                Vector4.Transform(temp, Matrix.CreateFromAxisAngle(tempAxis, upVectorTurn));
                upVector.X = temp.X;
                upVector.Y = temp.Y;
                upVector.Z = temp.Z;
                return;
            }
            if (e.KeyCode == Keys.Tab)
            {
                if (simulation.selectionTarget == null)
                {
                    simulation.selectionTarget = simulation.Cells[0];
                    cellSelectionIndex=0;
                }
                else
                {
                    cellSelectionIndex++;
                    if (simulation.Cells.Count<=cellSelectionIndex)
                    {
                        cellSelectionIndex=0;
                    }
                    simulation.selectionTarget = simulation.Cells[cellSelectionIndex];
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                simulation.selectionTarget = null;
            }
        }

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

        /*protected override void OnMouseClick(MouseEventArgs e)
        {
            simulation.ReTarget(mouseRay, mousePos);
            base.OnMouseClick(e);
        }*/
        /*
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Vector3 near = new Vector3(e.X, e.Y, 0.3f);
            Vector3 far = new Vector3(e.X, e.Y, 1f);
            Viewport vp = new Viewport();
            vp.MaxDepth = 1;
            vp.MinDepth = 0;
            vp.X = 0;
            vp.Y = 0;
            vp.Height = this.ClientSize.Height;
            vp.Width = this.ClientSize.Width;

            Matrix transformation = Matrix.CreateTranslation((float)visualShift.x, (float)visualShift.y, 0);
            
            vp.Unproject(near, cameraProjection, cameraView, transformation);
            vp.Unproject(far, cameraProjection, cameraView, transformation);
            mouseRay = new Vector(far.X - near.X, far.Y - near.Y, far.Z - near.Z);
            mousePos = new Vector(near.X, near.Y, near.Z);


            if (simulation.Cells.Count < 500)
            {
                simulation.ReTarget(mouseRay, mousePos);
            }
            
            base.OnMouseMove(e);
        }*/

        private void ResetColorMap()
        {
            if (simulation.ConcentrationsChanged)
            {
                for (int x = 0; x < WIDTH; x++)
                {

                    for (int y = 0; y < HEIGHT; y++)
                    {
                        vertices[x + y * WIDTH].Color = simulation.GetColor(new Vector(x - WIDTH / 2, y - HEIGHT / 2, 0));
                    }
                }
                vb.SetData(vertices);
                simulation.ConcentrationsChanged = false;
            }
        }
 
		private void VertexDeclaration()
        {
            //vb = new VertexBuffer(typeof(VertexPositionColor), WIDTH * HEIGHT, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vb = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), WIDTH * HEIGHT, BufferUsage.WriteOnly);
            vertices = new VertexPositionColor[WIDTH * HEIGHT];
            for (int x = 0; x < WIDTH; x++)
            {

                for (int y = 0; y < HEIGHT; y++)
                {
                    vertices[x + y * WIDTH].Position = new Vector3(x-WIDTH/2, (y-HEIGHT/2), -1);
                    //vertices[x + y * WIDTH].Color = simulation.GetColor(new Vector(x-WIDTH/2,y-HEIGHT/2)).ToArgb();
                    vertices[x + y * WIDTH].Color = Color.White;
                    //vertices[x + y * WIDTH].Normal = new Vector3(0, 0, 1);
                }
            }
            vb.SetData(vertices);
        }

        private void IndicesDeclaration()
        {
            ib = new IndexBuffer(graphics.GraphicsDevice, typeof(short), (WIDTH - 1) * (HEIGHT - 1) * 6, BufferUsage.WriteOnly);
            indices = new short[(WIDTH - 1) * (HEIGHT - 1) * 6];

            for (int x = 0; x < WIDTH - 1; x++)
            {

                for (int y = 0; y < HEIGHT - 1; y++)
                {
                    indices[(x + y * (WIDTH - 1)) * 6] = (short)(x + y * WIDTH);
                    indices[(x + y * (WIDTH - 1)) * 6 + 1] = (short)((x + 1) + y * WIDTH);
                    indices[(x + y * (WIDTH - 1)) * 6 + 2] = (short)((x + 1) + (y + 1) * WIDTH);

                    indices[(x + y * (WIDTH - 1)) * 6 + 3] = (short)(x + (y + 1) * WIDTH);
                    indices[(x + y * (WIDTH - 1)) * 6 + 4] = (short)(x + y * WIDTH);
                    indices[(x + y * (WIDTH - 1)) * 6 + 5] = (short)((x + 1) + (y + 1) * WIDTH);
                }
            }
            ib.SetData(indices);
        }

        

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
            cellMaterial[1] = Color.Chocolate;
            cellMaterial[2] = Color.Chartreuse;
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

        private bool nowPainting = false;
        /*protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
            base.OnPaint(e);
        }*/
        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }
 
        protected override void Draw(GameTime gameTime)
        {
            if (deviceBlock || simulation.state != Simulation.State.None)
            {
                return;
            }
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
                            Matrix.CreateFromAxisAngle(Vector3.Down, 0.01f)) + cameraLooksAt;
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

//            base.Draw(gameTime);
            nowPainting = false;
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
        private VertexPositionColor[] GetHexagonAroundSource(Source source)
        {
            VertexPositionColor[] cvertices = new VertexPositionColor[14];
            Color colorCenter = new Color(source.color, (int)(150 * source.strength));
            double colorPower = source.strength * (Math.Pow(SignallingProtein.Array[source.secretID].pentration, 30));
            Color colorEdge = new Color(source.color, (int)(150 * colorPower));


            cvertices[0].Position = new Vector3(0, 0, 0);
            cvertices[0].Color = colorCenter;

            Vector4 baseV = new Vector4(upVector.X * 60, upVector.Y * 60, upVector.Z * 60, 0);
            for (int i = 1; i < 14; i++)
            {
                Vector4.Transform(baseV, Matrix.CreateFromAxisAngle(cameraLooksAt - cameraPosition, (float)Math.PI / 6));
                cvertices[i].Position = new Vector3(baseV.X, baseV.Y, baseV.Z);
                cvertices[i].Color = colorEdge;
            }


            for (int i = 0; i < 14; i++)
            {
                //cvertices[i].Normal = cameraPosition - cameraLooksAt;
                cvertices[i].Position = new Vector3(cvertices[i].Position.X + (float)source.position.x, cvertices[i].Position.Y + (float)source.position.y, cvertices[i].Position.Z + (float)source.position.z);
            }
            return cvertices;
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
            foreach (Cell currenttarget in simulation.Cells.GetRange(0,simulation.Cells.Count))
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
