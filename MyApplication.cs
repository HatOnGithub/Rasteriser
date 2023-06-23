using System.Diagnostics;
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

        public static Camera cam = new Camera();

        public SceneGraph SceneGraph = new SceneGraph();

        public float yawSpeed = 5;
        public float pitchSpeed = 5;
        public float rollSpeed = 5;
        public float fovChangeSpeed = 5;


        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            // load assets
            teapot = new Node(new Mesh("../../../assets/teapot.obj", new Vector3(10, 0, 0), null, null, 0.9f, 0.1f, 100), "../../../assets/wood.jpg");
            floor =  new Node(new Mesh("../../../assets/floor.obj", null, null, null, 0.9f, 0.1f, 100), "../../../assets/wood.jpg");
            prideOfHiigara = new Node(new Mesh("../../../assets/poh.obj", new Vector3(0, 5, 0), null, new Vector3(0.01f, 0.01f, 0.01f), 0.9f, 0.1f, 100), "../../../assets/wood.jpg");

            // initialize stopwatch
            timer.Reset();
            timer.Start();
            // create shaders
            shader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postproc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            // create the render target
            if (useRenderTarget) target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();

            SceneGraph.AddLight(new Vector3(0, 30, 0), Vector3.One * 255, 10);
            SceneGraph.AddChild(floor);
            SceneGraph.AddChild(teapot);
            floor.AddChild(prideOfHiigara);
        }

        // tick for background surface
        public void Tick(FrameEventArgs e, KeyboardState keyboard)
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);

            HandleInput(e, keyboard);
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

            Matrix4 cameraToScreen = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), (float)screen.width/screen.height, .1f, 1000);

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