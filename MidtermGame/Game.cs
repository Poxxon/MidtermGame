using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using MidtermGame.Engine;
using GL = OpenTK.Graphics.OpenGL4.GL; // alias to OpenTK GL
using OpenTK.Graphics.OpenGL4;

namespace MidtermGame;

public class MiniGame : GameWindow
{
    private Shader _shader = null!;
    private Camera _camera = null!;
    private Texture _texture = null!;
    private Mesh _floor = null!;
    private Mesh _cube = null!;
    private Mesh _pyramid = null!;

    private Matrix4 _proj;

    private Vector3 _lightPos = new(2f, 2f, 2f);
    private bool _lightOn = true;

    private double _lastMouseX, _lastMouseY;
    private bool _firstMouse = true;

    // Added AABB struct and collision fields
    private struct AABB
    {
        public Vector3 Min;
        public Vector3 Max;

        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }

    private AABB[] _colliders = null!;
    private float _camRadius = 0.3f;
    private Vector3 _camPrevPos;

    public MiniGame(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.05f, 0.06f, 0.08f, 1f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(TriangleFace.Back);

        _camera = new Camera(new Vector3(0f, 1.2f, 4f), Size.X / (float)Size.Y);
        CursorState = CursorState.Grabbed;

        _shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");
        _texture = new Texture("Assets/texture.png", TextureUnit.Texture0);

        _floor   = Mesh.CreatePlane(10, 10);
        _cube    = Mesh.CreateCube(1f);
        _pyramid = Mesh.CreatePyramid(1f, 1f);

        _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), Size.X / (float)Size.Y, 0.1f, 100f);

        // Initialize AABB colliders for cube, pyramid, and boundary walls
        _colliders = new AABB[5];

        // Cube collider at (-1.5, 0.5, 0) with size 1
        _colliders[0] = new AABB(
            new Vector3(-2.0f, 0f, -0.5f),
            new Vector3(-1.0f, 1f, 0.5f)
        );

        // Pyramid collider at (2, 0.5, -1) with size 1
        _colliders[1] = new AABB(
            new Vector3(1.5f, 0f, -1.5f),
            new Vector3(2.5f, 1f, -0.5f)
        );

        // Boundary walls (optional) - roughly enclosing the floor plane 10x10 centered at origin
        // Left wall
        _colliders[2] = new AABB(
            new Vector3(-5f, 0f, -5f),
            new Vector3(-4.5f, 3f, 5f)
        );
        // Right wall
        _colliders[3] = new AABB(
            new Vector3(4.5f, 0f, -5f),
            new Vector3(5f, 3f, 5f)
        );
        // Back wall
        _colliders[4] = new AABB(
            new Vector3(-5f, 0f, -5f),
            new Vector3(5f, 3f, -4.5f)
        );
        // Note: Front wall is open for camera entry
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        _camera.AspectRatio = Size.X / (float)Size.Y;
        _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), _camera.AspectRatio, 0.1f, 100f);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        var kb = KeyboardState;

        if (kb.IsKeyPressed(Keys.Escape))
        {
            CursorState = CursorState == CursorState.Normal ? CursorState.Grabbed : CursorState.Normal;
            _firstMouse = true;
        }

        float speed = 3.0f;
        if (kb.IsKeyDown(Keys.LeftShift)) speed *= 1.8f;

        // Store previous camera position before movement
        _camPrevPos = _camera.Position;

        if (kb.IsKeyDown(Keys.W)) _camera.Position += _camera.Front * speed * (float)args.Time;
        if (kb.IsKeyDown(Keys.S)) _camera.Position -= _camera.Front * speed * (float)args.Time;
        if (kb.IsKeyDown(Keys.A)) _camera.Position -= _camera.Right * speed * (float)args.Time;
        if (kb.IsKeyDown(Keys.D)) _camera.Position += _camera.Right * speed * (float)args.Time;
        if (kb.IsKeyDown(Keys.Space)) _camera.Position += _camera.Up * speed * (float)args.Time;
        if (kb.IsKeyDown(Keys.LeftControl)) _camera.Position -= _camera.Up * speed * (float)args.Time;

        // Resolve collisions on XZ plane and fix Y height
        ResolveCollisionsXZ(ref _camera.Position);
        _camera.Position = new Vector3(_camera.Position.X, 1.2f, _camera.Position.Z);

        if (kb.IsKeyPressed(Keys.E))
            _lightOn = !_lightOn;

        if (CursorState == CursorState.Grabbed)
        {
            var mouse = MouseState;
            if (_firstMouse)
            {
                _lastMouseX = mouse.X;
                _lastMouseY = mouse.Y;
                _firstMouse = false;
            }

            var deltaX = mouse.X - _lastMouseX;
            var deltaY = mouse.Y - _lastMouseY;
            _lastMouseX = mouse.X;
            _lastMouseY = mouse.Y;

            _camera.Yaw   += (float)deltaX * 0.1f;
            _camera.Pitch -= (float)deltaY * 0.1f;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        var view = _camera.GetViewMatrix();
        _shader.SetMatrix4("uView", view);
        _shader.SetMatrix4("uProj", _proj);
        _shader.SetVector3("uViewPos", _camera.Position);

        _shader.SetBool("uLightOn", _lightOn);
        _shader.SetVector3("uLight.position", _lightPos);
        _shader.SetVector3("uLight.ambient",  new Vector3(0.2f));
        _shader.SetVector3("uLight.diffuse",  new Vector3(0.8f));
        _shader.SetVector3("uLight.specular", new Vector3(1.0f));

        _shader.SetInt("uMaterial.diffuse", 0);
        _shader.SetVector3("uMaterial.specular", new Vector3(0.5f));
        _shader.SetFloat("uMaterial.shininess", 32f);

        _texture.Bind();

        var floorModel = Matrix4.CreateTranslation(0f, 0f, 0f);
        _shader.SetMatrix4("uModel", floorModel);
        _shader.SetVector3("uTint", new Vector3(0.7f, 0.7f, 0.75f));
        _floor.Draw();

        var cubeModel = Matrix4.CreateTranslation(-1.5f, 0.5f, 0f);
        _shader.SetMatrix4("uModel", cubeModel);
        _shader.SetVector3("uTint", new Vector3(1f, 1f, 1f));
        _cube.Draw();

        var angle = (float)OpenTK.Windowing.GraphicsLibraryFramework.GLFW.GetTime() * 0.7f;
        var pyrModel = Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(2f, 0.5f, -1f);
        _shader.SetMatrix4("uModel", pyrModel);
        _shader.SetVector3("uTint", new Vector3(1f, 1f, 1f));
        _pyramid.Draw();

        SwapBuffers();
    }

    // Added collision resolution method
    private void ResolveCollisionsXZ(ref Vector3 position)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            var box = _colliders[i];

            // Expand the collider by camera radius in XZ plane
            float expandedMinX = box.Min.X - _camRadius;
            float expandedMaxX = box.Max.X + _camRadius;
            float expandedMinZ = box.Min.Z - _camRadius;
            float expandedMaxZ = box.Max.Z + _camRadius;

            // Check for overlap in XZ plane between camera position and expanded box
            bool overlapX = position.X >= expandedMinX && position.X <= expandedMaxX;
            bool overlapZ = position.Z >= expandedMinZ && position.Z <= expandedMaxZ;

            if (overlapX && overlapZ)
            {
                // Calculate penetration depths
                float penLeft = expandedMaxX - position.X;
                float penRight = position.X - expandedMinX;
                float penTop = expandedMaxZ - position.Z;
                float penBottom = position.Z - expandedMinZ;

                // Find smallest penetration axis
                float minPen = MathF.Min(MathF.Min(penLeft, penRight), MathF.Min(penTop, penBottom));

                if (minPen == penLeft)
                    position.X = expandedMaxX;
                else if (minPen == penRight)
                    position.X = expandedMinX;
                else if (minPen == penTop)
                    position.Z = expandedMaxZ;
                else if (minPen == penBottom)
                    position.Z = expandedMinZ;
            }
        }
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        _floor.Dispose();
        _cube.Dispose();
        _pyramid.Dispose();
        _texture.Dispose();
        _shader.Dispose();
    }
}