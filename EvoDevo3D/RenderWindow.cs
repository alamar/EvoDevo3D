using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using D3D = Microsoft.DirectX.Direct3D;
using System.Collections;
using Microsoft.DirectX.Direct3D;


using System.Threading;


namespace EvoDevo4
{
    public class RenderWindow : Form
    {
        private Matrix cameraProjection = Matrix.Identity;
        private D3D.Font TextRenderer;
        private Matrix cameraView = Matrix.Identity;
        private Vector mouseRay = new Vector();
        private Vector mousePos = new Vector();
        private System.ComponentModel.IContainer components;
        private Microsoft.DirectX.Direct3D.Device device;
        private ArrayList celllist = new ArrayList();
        private Mesh cellmesh;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private Material[] cellMaterial;
        private Material selectedCellMaterial;
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
        private CustomVertex.PositionNormalColored[] vertices;
        private short[] indices;
        private IndexBuffer ib;
        private VertexBuffer vb;
        private Thread ColorReMapper;
        private float turnAxis1=0;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbPlay;
        private ToolStripButton tsbPause;
        private ToolStripButton tsbStep;
        private ToolStripButton tsbSnapshot;
        private ToolStripButton tsbVideo;
        private ToolStripButton tsbClear;
        private System.Windows.Forms.Timer tmFPSChecker;
        private System.Windows.Forms.Timer tmWorldHeartbeat;
        private Vector3 cameraPosition = new Vector3(0, 0, 200);
        private Vector3 cameraLooksAt = new Vector3(0, 0, 0);
        private Vector3 upVector = new Vector3(0, 0.5f, 0);
        private float cameraPositionAngleAroundUpVector = 0;
        private float cameraPositionAngleAroundRightVector = 0;
        private CheckBox chb0Visible;
        private CheckBox chb1Visible;
        private CheckBox chb2Visible;
        private CheckBox chb3Visible;
        private CheckBox chb4Visible;
        private CheckBox chb5Visible;
        private CheckBox chb6Visible;
        private CheckBox chb7Visible;
        private CheckBox chb8Visible;
        private CheckBox chb9Visible;
        private int cellSelectionIndex;
        private ToolStripLabel lblProcess;
        private ToolStripLabel lblCells;


        private Thread heartbeatThread;

        /// <summary>
        /// Creates new Render window instance;
        /// </summary>
        public RenderWindow()
        {

            InitializeComponent();

            GeneticCode gc = new GeneticCode();
            gc.Show();
            

            tmFPSChecker.Start();
            heartbeatThread = new Thread(World.Instance.ActionsManager);
            heartbeatThread.Start();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            if (!InitializeDevice()) throw new Exception("DirectX initialization failed");
            SetUpCamera();
            InitializeObjects();
            VertexDeclaration();
            IndicesDeclaration();
            InitializeTextOutput();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 50;
            timer.Enabled = true;
            gc.BringToFront();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Vector4 move = new Vector4(0, 0, 0, 0);
            Vector4 zoom = new Vector4(0, 0, 0, 0);
            Vector4 right = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
            bool turnAroundUpVector = false;
            bool turnAroundRightVector = false;
            bool turnUpVectorItself = false;
            float upVectorTurn = 0;
            right.Transform(Matrix.RotationAxis((cameraLooksAt - cameraPosition), (float)Math.PI / 2));
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
                temp.Transform(Matrix.RotationAxis(upVector, cameraPositionAngleAroundUpVector));
       
                cameraPosition.X = cameraLooksAt.X + temp.X;
                cameraPosition.Y = cameraLooksAt.Y + temp.Y;
                cameraPosition.Z = cameraLooksAt.Z + temp.Z;
                return;
            }
            if (turnAroundRightVector)
            {
                Vector4 temp = new Vector4(cameraPosition.X - cameraLooksAt.X, cameraPosition.Y - cameraLooksAt.Y, cameraPosition.Z - cameraLooksAt.Z, 0);
                temp.Transform(Matrix.RotationAxis(new Vector3(right.X, right.Y, right.Z), cameraPositionAngleAroundRightVector));
                cameraPosition.X = cameraLooksAt.X + temp.X;
                cameraPosition.Y = cameraLooksAt.Y + temp.Y;
                cameraPosition.Z = cameraLooksAt.Z + temp.Z;
                temp = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
                temp.Transform(Matrix.RotationAxis(new Vector3(right.X, right.Y, right.Z), cameraPositionAngleAroundRightVector));
                upVector.X = temp.X;
                upVector.Y = temp.Y;
                upVector.Z = temp.Z;
                return;
            }
            if (turnUpVectorItself)
            {
                Vector3 tempAxis = new Vector3(cameraPosition.X - cameraLooksAt.X, cameraPosition.Y - cameraLooksAt.Y, cameraPosition.Z - cameraLooksAt.Z);
                Vector4 temp = new Vector4(upVector.X, upVector.Y, upVector.Z, 0);
                temp.Transform(Matrix.RotationAxis(tempAxis, upVectorTurn));
                upVector.X = temp.X;
                upVector.Y = temp.Y;
                upVector.Z = temp.Z;
                return;
            }
            if (e.KeyCode == Keys.Tab)
            {
                if (World.Instance.selectionTarget == null)
                {
                    World.Instance.selectionTarget = World.Instance.Cells[0];
                    cellSelectionIndex=0;
                }
                else
                {
                    cellSelectionIndex++;
                    if (World.Instance.Cells.Count<=cellSelectionIndex)
                    {
                        cellSelectionIndex=0;
                    }
                    World.Instance.selectionTarget = World.Instance.Cells[cellSelectionIndex];
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                World.Instance.selectionTarget = null;
            }
        }

        private void Screenshot()
        {
            Surface renderTarget = device.GetRenderTarget(0);
            DateTime n = DateTime.Now;
            if (!rendering)
            {
                screenshotFile = n.ToString("yyyy-MM-dd_HH-mm-ss") + ".bmp";
            }
            else
            {
                screenshotFile = "render_at_" + n.ToString("yyyy-MM-dd") + "_frame_" + frameNo++.ToString() + ".bmp";
            }
            SurfaceLoader.Save(screenshotFile, ImageFileFormat.Bmp, renderTarget);
            screenshotAwaiting = false;
        }

        private void InitializeTextOutput()
        {
            System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericMonospace,8);
            TextRenderer = new Microsoft.DirectX.Direct3D.Font(device, font);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            World.Instance.ReTarget(mouseRay, mousePos);
            base.OnMouseClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Vector3 near = new Vector3(e.X, e.Y, 0.3f);
            Vector3 far = new Vector3(e.X, e.Y, 1f);
            Viewport vp = new Viewport();
            vp.MaxZ = 1;
            vp.MinZ = 0;
            vp.X = 0;
            vp.Y = 0;
            vp.Height = this.ClientSize.Height;
            vp.Width = this.ClientSize.Width;

            Matrix transformation = Matrix.Translation((float)visualShift.x, (float)visualShift.y, 0);
            
            near.Unproject(vp, cameraProjection,cameraView, transformation);
            far.Unproject(vp, cameraProjection, cameraView, transformation);
            mouseRay = new Vector(far.X - near.X, far.Y - near.Y, far.Z - near.Z);
            mousePos = new Vector(near.X, near.Y, near.Z);


            if (World.Instance.Cells.Count < 500)
            {
                World.Instance.ReTarget(mouseRay, mousePos);
            }
            
            base.OnMouseMove(e);
        }

  


        
      

        private void ResetColorMap()
        {
            if (World.Instance.ConcentrationsChanged)
            {
                for (int x = 0; x < WIDTH; x++)
                {

                    for (int y = 0; y < HEIGHT; y++)
                    {
                        vertices[x + y * WIDTH].Color = World.Instance.GetColor(new Vector(x - WIDTH / 2, y - HEIGHT / 2, 0)).ToArgb();
                    }
                }
                vb.SetData(vertices, 0, LockFlags.None);
                World.Instance.ConcentrationsChanged = false;
            }
        }
        private void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), WIDTH * HEIGHT, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertices = new CustomVertex.PositionNormalColored[WIDTH * HEIGHT];
            for (int x = 0; x < WIDTH; x++)
            {

                for (int y = 0; y < HEIGHT; y++)
                {
                    vertices[x + y * WIDTH].Position = new Vector3(x-WIDTH/2, (y-HEIGHT/2), -1);
                    //vertices[x + y * WIDTH].Color = World.Instance.GetColor(new Vector(x-WIDTH/2,y-HEIGHT/2)).ToArgb();
                    vertices[x + y * WIDTH].Color = Color.White.ToArgb();
                    vertices[x + y * WIDTH].Normal = new Vector3(0, 0, 1);
                }
            }
            vb.SetData(vertices, 0, LockFlags.None);
        }

        private void IndicesDeclaration()
        {
            ib = new IndexBuffer(typeof(short), (WIDTH - 1) * (HEIGHT - 1) * 6, device, Usage.WriteOnly, Pool.Default);
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
            ib.SetData(indices, 0, LockFlags.None);
        }

        

        public int frames;
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

           

            deviceBlock = false;
        }

        /// <summary>
        /// Sets up objects and graphical meshes
        /// </summary>
        private void InitializeObjects()
        {
            cellMaterial = new Material[10];
            cellMaterial[0] = new Material();
            cellMaterial[0].Diffuse = Color.LightGray;
            cellMaterial[0].Ambient = Color.LightGray;
            cellMaterial[1] = new Material();
            cellMaterial[1].Diffuse = Color.Chocolate;
            cellMaterial[1].Ambient = Color.Chocolate;
            cellMaterial[2] = new Material();
            cellMaterial[2].Diffuse = Color.Chartreuse;
            cellMaterial[2].Ambient = Color.Chartreuse;
            cellMaterial[3] = new Material();
            cellMaterial[3].Diffuse = Color.Fuchsia;
            cellMaterial[3].Ambient = Color.Fuchsia;
            cellMaterial[4] = new Material();
            cellMaterial[4].Diffuse = Color.CornflowerBlue;
            cellMaterial[4].Ambient = Color.CornflowerBlue;
            cellMaterial[5] = new Material();
            cellMaterial[5].Diffuse = Color.ForestGreen;
            cellMaterial[5].Ambient = Color.ForestGreen;
            cellMaterial[6] = new Material();
            cellMaterial[6].Diffuse = Color.IndianRed;
            cellMaterial[6].Ambient = Color.IndianRed;
            cellMaterial[7] = new Material();
            cellMaterial[7].Diffuse = Color.LemonChiffon;
            cellMaterial[7].Ambient = Color.LemonChiffon;
            cellMaterial[8] = new Material();
            cellMaterial[8].Diffuse = Color.BurlyWood;
            cellMaterial[8].Ambient = Color.BurlyWood;
            cellMaterial[9] = new Material();
            cellMaterial[9].Diffuse = Color.Gainsboro;
            cellMaterial[9].Ambient = Color.Gainsboro;

            selectedCellMaterial = new Material();
            selectedCellMaterial.Diffuse = Color.Gray;
            selectedCellMaterial.Ambient = Color.Gray;
            cellmesh  = Mesh.Sphere(device, 1f, 20, 20);
        }

        /// <summary>
        /// Sets up connection to a device
        /// </summary>
        /// <returns>True if device was created successfully and false othewise</returns>
        public bool InitializeDevice()
        {
            deviceBlock = true;
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            presentParams.AutoDepthStencilFormat = DepthFormat.D24S8;
            presentParams.EnableAutoDepthStencil = true;
            
            device = new D3D.Device(0, D3D.DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
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
            
            device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
            device.DeviceReset += new EventHandler(device_DeviceReset);
            device.Disposing += new EventHandler(device_Disposing);
            
            
            deviceBlock = false;
            return (true);
        }

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

        protected override void  OnSizeChanged(EventArgs e)
        {
            ReInitializeDevice();
            SetUpCamera();
            this.Invalidate();
            deviceBlock = false;
        }


       
       

        private void SetUpCamera()
        {
            if (device == null) return;
            cameraProjection = Matrix.PerspectiveFovLH((float)Math.PI / 8, (float)this.Width / (float)this.Height, 0.3f, 400f);
            cameraView = Matrix.LookAtLH(new Vector3(0f, 0f, 190f), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            device.Transform.Projection = cameraProjection;
            device.Transform.View = cameraView;            
        }

        private bool nowPainting = false;
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
            base.OnPaint(e);
        }
        

        private void Draw()
        {
            frames++;
            if (deviceBlock)
            {
            }
            else
            {
                try
                {
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.White, 1.0f, 0);
                    PlaceCamera();
                    device.BeginScene();


                    device.Transform.World = Matrix.Identity;


                    device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                    device.SetStreamSource(0, vb, 0);
                    device.Indices = ib;

                    //device.Transform.World = Matrix.Translation(-HEIGHT / 2, -WIDTH / 2, 0);

                   
                    
                    //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, WIDTH * HEIGHT, 0, indices.Length / 3);
                    //device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, indices.Length, indices.Length / 3, indices, true, vertices);
                    
                    DrawCells();
                    DrawConcentrations();
                    DrawTargetData();
                    //DrawConcentrationsData();
                    
                    device.EndScene();
                    if (screenshotAwaiting||rendering)
                        Screenshot();

                    device.Present();
                    this.Invalidate();
                }
                catch (Exception e)
                {
                    deviceBlock = true;
                    try
                    {
                        device.EndScene();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    ReInitializeDevice();
                    SetUpCamera();
                    this.Invalidate();
                    deviceBlock = false;
                    Console.WriteLine("OOPS. Rendering exception occured and was brutally ignored" + e.Message);
                }

                Application.DoEvents();
            }
            nowPainting = false;
        }

        private void PlaceCamera()
        {
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = cameraLooksAt - cameraPosition;
            device.Lights[0].Enabled = true;
            device.Lights[0].Update();            

            cameraProjection = Matrix.PerspectiveFovLH((float)Math.PI / 8, (float)this.Width / (float)this.Height, 50f, 1000f);
            cameraView = Matrix.LookAtLH(cameraPosition, cameraLooksAt, upVector);
            device.Transform.Projection = cameraProjection;
            device.Transform.View = cameraView;            
        }
        private CustomVertex.PositionNormalColored[] GetHexagonAroundSource(Source source)
        {
            CustomVertex.PositionNormalColored[] cvertices = new CustomVertex.PositionNormalColored[14];
            int colorCenter = Color.FromArgb((int)(150 * source.strength), source.color).ToArgb();
            double colorPower = source.strength * (Math.Pow(SignallingProtein.Array[source.secretID].pentration, 30));
            int colorEdge = Color.FromArgb((int)(150 * colorPower), source.color).ToArgb();


            cvertices[0].Position = new Vector3(0, 0, 0);
            cvertices[0].Color = colorCenter;

            Vector4 baseV = new Vector4(upVector.X * 60, upVector.Y * 60, upVector.Z * 60, 0);
            for (int i = 1; i < 14; i++)
            {
                baseV.Transform(Matrix.RotationAxis(cameraLooksAt - cameraPosition, (float)Math.PI / 6));
                cvertices[i].Position = new Vector3(baseV.X, baseV.Y, baseV.Z);
                cvertices[i].Color = colorEdge;
            }


            for (int i = 0; i < 14; i++)
            {
                cvertices[i].Normal =  cameraPosition - cameraLooksAt;
                cvertices[i].Position = new Vector3(cvertices[i].Position.X + (float)source.position.x, cvertices[i].Position.Y + (float)source.position.y, cvertices[i].Position.Z + (float)source.position.z);
            }
            return cvertices;
        }

        private void DrawConcentrations()
        {
            device.Transform.World = Matrix.Identity;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            List<Source> sortedSources = new List<Source>();
            foreach (Source source in World.Instance.Sources)
            {
                sortedSources.Add(source);
            }
            sortedSources.Sort(WhoSCloser);

            foreach (Source source in sortedSources)
            {
                CustomVertex.PositionNormalColored[] cvertices = GetHexagonAroundSource(source);                
                device.DrawUserPrimitives(PrimitiveType.TriangleFan, 12, cvertices);
            }
            device.RenderState.AlphaBlendEnable = false;
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
        private void DrawConcentrationsData()
        {
            /*string outStr = "(";
            for (int i=0;i<SignallingProtein.Array.Count;i++)
            {
                if (i > 0)
                    outStr += ", ";
                outStr += World.Instance.GetConcentration(mousePosition, i).ToString("f");
            }
            outStr += ")";*/
            string outStr = mouseRay.x + " " + mouseRay.y + " " + mouseRay.z;
            TextRenderer.DrawText(null, outStr, new Point(10, this.ClientSize.Height - 40), Color.DarkGreen);
        }

        private void DrawTargetData()
        {            
            if (World.Instance.selectionTarget != null)
            {
                Cell cell = World.Instance.selectionTarget;
                
                TextRenderer.DrawText(null, cell.ToString(), new Point(20, 28), Color.DarkBlue);
            }
        }
        private void DrawCells()
        {
            device.SetTexture(0, null);
            foreach (Cell currenttarget in World.Instance.Cells.GetRange(0,World.Instance.Cells.Count))
            {
                switch (currenttarget.cellType)
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

                }
                device.Transform.World = Matrix.Scaling((float)currenttarget.radius, (float)currenttarget.radius, (float)currenttarget.radius) * Matrix.Translation((float)currenttarget.position.x, (float)currenttarget.position.y, (float)currenttarget.position.z); 
                if (currenttarget.color > 0 && currenttarget.color < 10)
                {
                    device.Material = cellMaterial[currenttarget.color];
                }
                else
                {
                    device.Material = cellMaterial[0];
                }
               
                cellmesh.DrawSubset(0);

            }
            if (World.Instance.selectionTarget != null)
            {
                Cell currenttarget = World.Instance.selectionTarget;
                //device.Transform.World = Matrix.Scaling((float)currenttarget.radius, (float)currenttarget.radius, (float)currenttarget.radius) * Matrix.Translation((float)currenttarget.position.x + (float)visualShift.x, (float)currenttarget.position.y + (float)visualShift.y, 0);
                device.Transform.World = Matrix.Scaling((float)currenttarget.radius, (float)currenttarget.radius, (float)currenttarget.radius) * Matrix.Translation((float)currenttarget.position.x, (float)currenttarget.position.y, (float)currenttarget.position.z);
                
                device.Material = selectedCellMaterial;
                cellmesh.DrawSubset(0);
            }
        }
        static Random random = new Random();
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSnapshot = new System.Windows.Forms.ToolStripButton();
            this.tsbVideo = new System.Windows.Forms.ToolStripButton();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStep = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tmFPSChecker = new System.Windows.Forms.Timer(this.components);
            this.tmWorldHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.chb0Visible = new System.Windows.Forms.CheckBox();
            this.chb1Visible = new System.Windows.Forms.CheckBox();
            this.chb2Visible = new System.Windows.Forms.CheckBox();
            this.chb3Visible = new System.Windows.Forms.CheckBox();
            this.chb4Visible = new System.Windows.Forms.CheckBox();
            this.chb5Visible = new System.Windows.Forms.CheckBox();
            this.chb6Visible = new System.Windows.Forms.CheckBox();
            this.chb7Visible = new System.Windows.Forms.CheckBox();
            this.chb8Visible = new System.Windows.Forms.CheckBox();
            this.chb9Visible = new System.Windows.Forms.CheckBox();
            this.lblProcess = new System.Windows.Forms.ToolStripLabel();
            this.lblCells = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSnapshot,
            this.tsbVideo,
            this.tsbPlay,
            this.tsbPause,
            this.tsbStep,
            this.tsbClear,
            this.lblProcess,
            this.lblCells});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(792, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbSnapshot
            // 
            this.tsbSnapshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSnapshot.Image = global::EvoDevo4.Properties.Resources.snapshot;
            this.tsbSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSnapshot.Name = "tsbSnapshot";
            this.tsbSnapshot.Size = new System.Drawing.Size(23, 22);
            this.tsbSnapshot.Text = "Snapshot";
            this.tsbSnapshot.Click += new System.EventHandler(this.tsbSnapshot_Click);
            // 
            // tsbVideo
            // 
            this.tsbVideo.CheckOnClick = true;
            this.tsbVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVideo.Image = global::EvoDevo4.Properties.Resources.video;
            this.tsbVideo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbVideo.Name = "tsbVideo";
            this.tsbVideo.Size = new System.Drawing.Size(23, 22);
            this.tsbVideo.Text = "tsbVideo";
            this.tsbVideo.Click += new System.EventHandler(this.tsbVideo_Click);
            // 
            // tsbPlay
            // 
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Image = global::EvoDevo4.Properties.Resources.control_play;
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.Size = new System.Drawing.Size(23, 22);
            this.tsbPlay.Text = "Play";
            this.tsbPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.tsbPlay.Click += new System.EventHandler(this.tsbPlay_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPause.Enabled = false;
            this.tsbPause.Image = global::EvoDevo4.Properties.Resources.pause;
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(23, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // tsbStep
            // 
            this.tsbStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStep.Image = global::EvoDevo4.Properties.Resources.step;
            this.tsbStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStep.Name = "tsbStep";
            this.tsbStep.Size = new System.Drawing.Size(23, 22);
            this.tsbStep.Text = "Step Froward";
            this.tsbStep.Click += new System.EventHandler(this.tsbStep_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClear.Image = global::EvoDevo4.Properties.Resources.clear;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(23, 22);
            this.tsbClear.Text = "Clear";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // tmFPSChecker
            // 
            this.tmFPSChecker.Enabled = true;
            this.tmFPSChecker.Interval = 40;
            this.tmFPSChecker.Tick += new System.EventHandler(this.tmFPSChecker_Tick);
            // 
            // tmWorldHeartbeat
            // 
            this.tmWorldHeartbeat.Enabled = true;
            this.tmWorldHeartbeat.Interval = 50;
            this.tmWorldHeartbeat.Tick += new System.EventHandler(this.tmWorldHeartbeat_Tick);
            // 
            // chb0Visible
            // 
            this.chb0Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb0Visible.AutoSize = true;
            this.chb0Visible.Checked = true;
            this.chb0Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb0Visible.Location = new System.Drawing.Point(0, 28);
            this.chb0Visible.Name = "chb0Visible";
            this.chb0Visible.Size = new System.Drawing.Size(23, 23);
            this.chb0Visible.TabIndex = 1;
            this.chb0Visible.TabStop = false;
            this.chb0Visible.Text = "0";
            this.chb0Visible.UseVisualStyleBackColor = true;
            this.chb0Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb1Visible
            // 
            this.chb1Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb1Visible.AutoSize = true;
            this.chb1Visible.Checked = true;
            this.chb1Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb1Visible.Location = new System.Drawing.Point(0, 50);
            this.chb1Visible.Name = "chb1Visible";
            this.chb1Visible.Size = new System.Drawing.Size(23, 23);
            this.chb1Visible.TabIndex = 2;
            this.chb1Visible.TabStop = false;
            this.chb1Visible.Text = "1";
            this.chb1Visible.UseVisualStyleBackColor = true;
            this.chb1Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb2Visible
            // 
            this.chb2Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb2Visible.AutoSize = true;
            this.chb2Visible.Checked = true;
            this.chb2Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb2Visible.Location = new System.Drawing.Point(0, 72);
            this.chb2Visible.Name = "chb2Visible";
            this.chb2Visible.Size = new System.Drawing.Size(23, 23);
            this.chb2Visible.TabIndex = 3;
            this.chb2Visible.TabStop = false;
            this.chb2Visible.Text = "2";
            this.chb2Visible.UseVisualStyleBackColor = true;
            this.chb2Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb3Visible
            // 
            this.chb3Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb3Visible.AutoSize = true;
            this.chb3Visible.Checked = true;
            this.chb3Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb3Visible.Location = new System.Drawing.Point(0, 94);
            this.chb3Visible.Name = "chb3Visible";
            this.chb3Visible.Size = new System.Drawing.Size(23, 23);
            this.chb3Visible.TabIndex = 4;
            this.chb3Visible.TabStop = false;
            this.chb3Visible.Text = "3";
            this.chb3Visible.UseVisualStyleBackColor = true;
            this.chb3Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb4Visible
            // 
            this.chb4Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb4Visible.AutoSize = true;
            this.chb4Visible.Checked = true;
            this.chb4Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb4Visible.Location = new System.Drawing.Point(0, 116);
            this.chb4Visible.Name = "chb4Visible";
            this.chb4Visible.Size = new System.Drawing.Size(23, 23);
            this.chb4Visible.TabIndex = 5;
            this.chb4Visible.TabStop = false;
            this.chb4Visible.Text = "4";
            this.chb4Visible.UseVisualStyleBackColor = true;
            this.chb4Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb5Visible
            // 
            this.chb5Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb5Visible.AutoSize = true;
            this.chb5Visible.Checked = true;
            this.chb5Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb5Visible.Location = new System.Drawing.Point(0, 138);
            this.chb5Visible.Name = "chb5Visible";
            this.chb5Visible.Size = new System.Drawing.Size(23, 23);
            this.chb5Visible.TabIndex = 6;
            this.chb5Visible.TabStop = false;
            this.chb5Visible.Text = "5";
            this.chb5Visible.UseVisualStyleBackColor = true;
            this.chb5Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb6Visible
            // 
            this.chb6Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb6Visible.AutoSize = true;
            this.chb6Visible.Checked = true;
            this.chb6Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb6Visible.Location = new System.Drawing.Point(0, 160);
            this.chb6Visible.Name = "chb6Visible";
            this.chb6Visible.Size = new System.Drawing.Size(23, 23);
            this.chb6Visible.TabIndex = 7;
            this.chb6Visible.TabStop = false;
            this.chb6Visible.Text = "6";
            this.chb6Visible.UseVisualStyleBackColor = true;
            this.chb6Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb7Visible
            // 
            this.chb7Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb7Visible.AutoSize = true;
            this.chb7Visible.Checked = true;
            this.chb7Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb7Visible.Location = new System.Drawing.Point(0, 182);
            this.chb7Visible.Name = "chb7Visible";
            this.chb7Visible.Size = new System.Drawing.Size(23, 23);
            this.chb7Visible.TabIndex = 8;
            this.chb7Visible.TabStop = false;
            this.chb7Visible.Text = "7";
            this.chb7Visible.UseVisualStyleBackColor = true;
            this.chb7Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb8Visible
            // 
            this.chb8Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb8Visible.AutoSize = true;
            this.chb8Visible.Checked = true;
            this.chb8Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb8Visible.Location = new System.Drawing.Point(0, 204);
            this.chb8Visible.Name = "chb8Visible";
            this.chb8Visible.Size = new System.Drawing.Size(23, 23);
            this.chb8Visible.TabIndex = 9;
            this.chb8Visible.TabStop = false;
            this.chb8Visible.Text = "8";
            this.chb8Visible.UseVisualStyleBackColor = true;
            this.chb8Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb9Visible
            // 
            this.chb9Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb9Visible.AutoSize = true;
            this.chb9Visible.Checked = true;
            this.chb9Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb9Visible.Location = new System.Drawing.Point(0, 226);
            this.chb9Visible.Name = "chb9Visible";
            this.chb9Visible.Size = new System.Drawing.Size(23, 23);
            this.chb9Visible.TabIndex = 10;
            this.chb9Visible.TabStop = false;
            this.chb9Visible.Text = "9";
            this.chb9Visible.UseVisualStyleBackColor = true;
            this.chb9Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // lblProcess
            // 
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(86, 22);
            this.lblProcess.Text = "Process: NONE";
            // 
            // lblCells
            // 
            this.lblCells.Name = "lblCells";
            this.lblCells.Size = new System.Drawing.Size(44, 22);
            this.lblCells.Text = "Cells: 1";
            // 
            // RenderWindow
            // 
            this.ClientSize = new System.Drawing.Size(792, 742);
            this.Controls.Add(this.chb9Visible);
            this.Controls.Add(this.chb8Visible);
            this.Controls.Add(this.chb7Visible);
            this.Controls.Add(this.chb6Visible);
            this.Controls.Add(this.chb5Visible);
            this.Controls.Add(this.chb4Visible);
            this.Controls.Add(this.chb3Visible);
            this.Controls.Add(this.chb2Visible);
            this.Controls.Add(this.chb1Visible);
            this.Controls.Add(this.chb0Visible);
            this.Controls.Add(this.toolStrip1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenderWindow";
            this.Text = "EvoDevo 4";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderWindow_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void RenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Vector3 near = new Vector3(e.X, e.Y, 1f);
                Viewport vp = new Viewport();
                vp.MaxZ = 1;
                vp.MinZ = 0;
                vp.X = 0;
                vp.Y = 0;
                vp.Height = this.ClientSize.Height;
                vp.Width = this.ClientSize.Width;

                Matrix transformation = Matrix.Translation((float)visualShift.x, (float)visualShift.y, 0);
                near.Unproject(vp, cameraProjection, cameraView, transformation);
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
        }

        private void RenderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void tsbPlay_Click(object sender, EventArgs e)
        {
            World.Instance.paused = !World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
            tsbPlay.Enabled = World.Instance.paused;
        }

        private void tsbPause_Click(object sender, EventArgs e)
        {
            World.Instance.paused = !World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
            tsbPlay.Enabled = World.Instance.paused;
        }

        private void tsbStep_Click(object sender, EventArgs e)
        {
            World.Instance.AwaitingQueue.Enqueue('s');
            World.Instance.paused = false;
            World.Instance.newActionAllowed = true;
            tsbPause.Enabled = false;
            tsbPlay.Enabled = true;
        }

        private void tsbSnapshot_Click(object sender, EventArgs e)
        {
            screenshotAwaiting = true;
        }

        private void tsbVideo_Click(object sender, EventArgs e)
        {
            rendering = !rendering;
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset the world to initial state. Are you sure?", "EvoDevo IV", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                World.Instance.Reset();
            }
        }

        private void tmWorldHeartbeat_Tick(object sender, EventArgs e)
        {
            
            World.Instance.newActionAllowed = true;
        }

        private void chb9Visible_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void tmFPSChecker_Tick(object sender, EventArgs e)
        {
            lblProcess.Text = "Process: " + World.Instance.state;
            lblCells.Text = "Cells: " + World.Instance.Cells.Count;
            tsbPlay.Enabled = World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
        }

        private void RenderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }


    }
}
