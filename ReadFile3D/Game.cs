using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ReadFile3D.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Game : GameWindow
{
    private List<Vertex> vertices = new List<Vertex>();
    private List<int> indices = new List<int>(); // Save list of indices
    private int vao, vbo, ebo;
    private Shader shader = null!;
    private float rotationAngle = 0.0f;

    public Game() : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = new OpenTK.Mathematics.Vector2i(800, 600),
        Title = "3D Pyramid",
        Flags = ContextFlags.ForwardCompatible
    })
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");

        if (shader == null)
        {
            Console.WriteLine("Shader failed to load!");
        }
        else
        {
            Console.WriteLine("Shader loaded successfully!");
        }

        GL.Enable(EnableCap.DepthTest); // Turn Z-buffer on 
        GL.Disable(EnableCap.CullFace); // Turn off cull face
        GL.CullFace(TriangleFace.Back); // Only show front face
        GL.FrontFace(FrontFaceDirection.Ccw); // Front face is counter clockwise

        // Read file obj
        try
        {
            string[] lines = System.IO.File.ReadAllLines("pyramid.obj");
            foreach (var line in lines)
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0 || parts[0].Trim().StartsWith("#")) continue;

                if (parts[0] == "v")
                {
                    if (float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                        float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                        float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                    {
                        vertices.Add(new Vertex(x, y, z));
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing vertex: {line}");
                    }
                }
                else if (parts[0] == "f")
                {
                    foreach (var index in parts.Skip(1))
                    {
                        indices.Add(int.Parse(index) - 1); // Convert to 0-based index
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading obj file: {e.Message}");
        }

        // Create VAO, VBO, EBO
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float) * 3, vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray(), BufferUsageHint.StaticDraw);

        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

        // Setup vertex attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        Console.WriteLine($"VAO: {vao}, VBO: {vbo}, EBO: {ebo}");
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use(); // Use shader

        // Create Model Matrix (rotate pyramid slightly for better view)
        Matrix4 model = Matrix4.Identity;
        model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationAngle)); // Rotate pyramid
        shader.SetMatrix4("model", model); // Send model matrix to shader

        // Create Projection Matrix
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45.0f), // Viewing angle
            Size.X / (float)Size.Y,  // Aspect ratio
            0.1f, 100.0f // Near and far clipping planes
        );
        shader.SetMatrix4("projection", projection); // Send Projection to shader

        // Create View Matrix
        Matrix4 modelView = Matrix4.LookAt(
            new Vector3(3.0f, 3.0f, 5.0f), // Camera position
            new Vector3(0, 0, 0),  // Looking at the center
            Vector3.UnitY // Up direction
        );
        shader.SetMatrix4("view", modelView); // Send View to shader

        // Draw pyramid
        GL.BindVertexArray(vao);
        GL.EnableVertexAttribArray(0); // Turn on Vertex Attribute 
        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.DisableVertexAttribArray(0); // Turn off Vertex Attribute

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        // Rotate pyramid to give a dynamic effect
        rotationAngle += 0.2f;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
    }
}
