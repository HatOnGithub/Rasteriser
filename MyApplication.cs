using System.Diagnostics;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Node? teapot, floor, prideOfHiigara;                    // meshes to draw using OpenGL
        float a = 0;                            // teapot rotation angle
        readonly Stopwatch timer = new();       // timer for measuring frame duration
        Shader? shader;                         // shader to use for rendering
        Shader? postproc;                       // shader to use for post processing
        RenderTarget? target;                   // intermediate render target
        ScreenQuad? quad;                       // screen filling quad for post processing
        readonly bool useRenderTarget = true;   // required for post processing

        public static Camera cam;

        public SceneGraph SceneGraph = new SceneGraph();

        public float yawSpeed = 5;
        public float pitchSpeed = 5;
        public float rollSpeed = 5;
        public float fovChangeSpeed = 5;

        public Light l1 = new(new Vector3(40, 30, 0), Vector3.One * 255, 3, Vector3.Zero, 10);
        public Light l2 = new(new Vector3(40, 30, 0), Vector3.One * 255, 3, Vector3.Zero, 10);
        public Light l3 = new(new Vector3(40, 30, 0), Vector3.One * 255, 3, Vector3.Zero, 10);
        public Light l4 = new(new Vector3(40, 30, 0), Vector3.One * 255, 3, Vector3.Zero, 10);

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
            cam = new Camera(ref screen);
        }
        // initialize
        public void Init()
        {
            // load assets
            teapot = new Node(new Mesh("../../../assets/teapot.obj", new Vector3(15, 10, 0), null, null, 0.9f, 0.6f, 200, Material.DiffuseMirror), "../../../assets/wood.jpg");
            floor =  new Node(new Mesh("../../../assets/floor.obj", null, null, null, 0.9f, 0.1f, 100), "../../../assets/wood.jpg");
            prideOfHiigara = new Node(new Mesh("../../../assets/poh.obj", new Vector3(0, 5, 0), null, new Vector3(0.01f, 0.01f, 0.01f), 1f, 1f, 100, Material.Mirror), Color.Gray);

            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            SceneGraph.AddLight(l1);
            SceneGraph.AddLight(l2);
            SceneGraph.AddLight(l3);
            SceneGraph.AddLight(l4);

            SceneGraph.AddChild(teapot);

            for (int i = 1; i <= 400; i++)
            {
                Node torpedo = new Node(new Mesh("../../../assets/torpedoFrigate.obj", new Vector3(400 * i, 0, 0), null, Vector3.One * 2, 0.9f, default, 100, Material.Glossy), "../../../assets/wood.jpg");
                prideOfHiigara.AddChild(torpedo);
            }

            for (int i = 1; i <= 400; i++)
            {
                Node ion = new Node(new Mesh("../../../assets/ionFrigate.obj", new Vector3(400 * i, 250, 0), null, Vector3.One * 2, 0.9f, default, 100, Material.Glossy), "../../../assets/wood.jpg");
                prideOfHiigara.AddChild(ion);
            }

            for (int i = 1; i <= 220; i++)
            {
                Node interceptor = new Node(new Mesh("../../../assets/interceptor.obj", new Vector3(-500  + (- 300 * (i % 5)), 0, 200 * (i / 5)), null, Vector3.One * 5, default, 0.5f, 100, Material.DiffuseMirror), Color.DarkGray);
                prideOfHiigara.AddChild(interceptor);
            }

            for (int i = 1; i <= 120; i++)
            {
                Node pulsargunship = new Node(new Mesh("../../../assets/pulsarGunship.obj", new Vector3(- 500 + (-500 * (i % 5)), 0, -200 + (-300 * (i / 5))), null, Vector3.One * 4, default, 0.5f, 100, Material.DiffuseMirror), Color.DarkGray);
                prideOfHiigara.AddChild(pulsargunship);
            }

            SceneGraph.AddChild(prideOfHiigara);

            
        }

        // tick for background surface
        public void Tick(FrameEventArgs e, KeyboardState keyboard)
        {
            float dt = (float)e.Time;
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);

            HandleInput(e, keyboard);

            l1.Mesh.Pos *= Matrix3.CreateRotationY(MathHelper.DegreesToRadians(10) * dt);
            l2.Mesh.Pos *= Matrix3.CreateRotationY(MathHelper.DegreesToRadians(20) * dt);
            l3.Mesh.Pos *= Matrix3.CreateRotationY(MathHelper.DegreesToRadians(30) * dt);
            l4.Mesh.Pos *= Matrix3.CreateRotationY(MathHelper.DegreesToRadians(40) * dt);

            prideOfHiigara.Mesh.Rot.Y += MathHelper.DegreesToRadians(20) * dt;
        }

        public void HandleInput(FrameEventArgs e, KeyboardState keyboard)
        {
            Vector3 movement = new();
            float pitch = 0;
            float yaw = 0;
            float roll = 0;
            float fovChange = 0;

            if (keyboard[Keys.W]) movement.Z += 1;
            if (keyboard[Keys.A]) movement.X -= 1;
            if (keyboard[Keys.S]) movement.Z -= 1;
            if (keyboard[Keys.D]) movement.X += 1;
            if (keyboard[Keys.Space]) movement.Y += 1;
            if (keyboard[Keys.LeftShift]) movement.Y -= 1;

            if (keyboard[Keys.Q]) yaw -= 1;
            if (keyboard[Keys.E]) yaw += 1;

            if (keyboard[Keys.R]) pitch -= 1;
            if (keyboard[Keys.F]) pitch += 1;

            if (keyboard[Keys.Z]) roll += 1;
            if (keyboard[Keys.X]) roll -= 1;

            if (keyboard[Keys.T]) fovChange += 1;
            if (keyboard[Keys.G]) fovChange -= 1;

            cam.MoveCamera(movement * 5 * (float)e.Time, new Vector3(pitch, yaw, roll));

        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            Matrix4 worldToCamera = cam.WorldToCamera;

            float vFov = (screen.height * 0.5f) / (screen.width * 0.5f / MathF.Tan(0.5f * MathHelper.DegreesToRadians(cam.FOV)));

            Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView( vFov, (float)screen.width/screen.height, .1f, 1000);

            if (useRenderTarget && target != null && quad != null)
            {
                // enable render target
                target.Bind();

                // render scene to render target
                if (shader != null)
                {
                    SceneGraph.Render(shader, worldToCamera, cameraToScreen, cam.e);
                }

                // render quad
                target.Unbind();
                if (postproc != null)
                    quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                if (shader != null)
                {
                    SceneGraph.Render(shader, worldToCamera, cameraToScreen, cam.e);
                }
            }
        }
    }
}