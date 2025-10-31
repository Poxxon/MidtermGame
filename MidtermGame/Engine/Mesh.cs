using System;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Graphics.OpenGL4;

namespace MidtermGame.Engine;

public class Mesh : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;
    private readonly int _indexCount;

    public Mesh(float[] vertices, uint[] indices)
    {
        _indexCount = indices.Length;

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        int stride = 8 * sizeof(float);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
    }

    // --------- Helpers ---------

    public static Mesh CreatePlane(int w, int h)
    {
        float halfW = w * 0.5f;
        float halfH = h * 0.5f;

        float[] v =
        {
            // pos                // normal    // uv
            -halfW, 0f, -halfH,   0f,1f,0f,    0f, 0f,
             halfW, 0f, -halfH,   0f,1f,0f,    1f, 0f,
             halfW, 0f,  halfH,   0f,1f,0f,    1f, 1f,
            -halfW, 0f,  halfH,   0f,1f,0f,    0f, 1f
        };
        uint[] idx = { 0,1,2, 0,2,3 };
        return new Mesh(v, idx);
    }

    public static Mesh CreateCube(float s)
    {
        float hs = s * 0.5f;

        float[] v = new float[]
        {
            // Front
            -hs, -hs,  hs,  0f,0f,1f,  0f,0f,
             hs, -hs,  hs,  0f,0f,1f,  1f,0f,
             hs,  hs,  hs,  0f,0f,1f,  1f,1f,
            -hs,  hs,  hs,  0f,0f,1f,  0f,1f,
            // Back
             hs, -hs, -hs,  0f,0f,-1f, 0f,0f,
            -hs, -hs, -hs,  0f,0f,-1f, 1f,0f,
            -hs,  hs, -hs,  0f,0f,-1f, 1f,1f,
             hs,  hs, -hs,  0f,0f,-1f, 0f,1f,
            // Left
            -hs, -hs, -hs, -1f,0f,0f,  0f,0f,
            -hs, -hs,  hs, -1f,0f,0f,  1f,0f,
            -hs,  hs,  hs, -1f,0f,0f,  1f,1f,
            -hs,  hs, -hs, -1f,0f,0f,  0f,1f,
            // Right
             hs, -hs,  hs,  1f,0f,0f,  0f,0f,
             hs, -hs, -hs,  1f,0f,0f,  1f,0f,
             hs,  hs, -hs,  1f,0f,0f,  1f,1f,
             hs,  hs,  hs,  1f,0f,0f,  0f,1f,
            // Top
            -hs,  hs,  hs,  0f,1f,0f,  0f,0f,
             hs,  hs,  hs,  0f,1f,0f,  1f,0f,
             hs,  hs, -hs,  0f,1f,0f,  1f,1f,
            -hs,  hs, -hs,  0f,1f,0f,  0f,1f,
            // Bottom
            -hs, -hs, -hs,  0f,-1f,0f, 0f,0f,
             hs, -hs, -hs,  0f,-1f,0f, 1f,0f,
             hs, -hs,  hs,  0f,-1f,0f, 1f,1f,
            -hs, -hs,  hs,  0f,-1f,0f, 0f,1f
        };

        uint[] idx = new uint[36];
        uint baseV = 0;
        for (int face = 0; face < 6; face++)
        {
            idx[face * 6 + 0] = baseV + 0;
            idx[face * 6 + 1] = baseV + 1;
            idx[face * 6 + 2] = baseV + 2;
            idx[face * 6 + 3] = baseV + 0;
            idx[face * 6 + 4] = baseV + 2;
            idx[face * 6 + 5] = baseV + 3;
            baseV += 4;
        }

        return new Mesh(v, idx);
    }

    public static Mesh CreatePyramid(float baseSize, float height)
    {
        float hs = baseSize * 0.5f;

        float[] v = {
            // Base
            -hs,0f,-hs,  0f,-1f,0f,  0f,0f,
             hs,0f,-hs,  0f,-1f,0f,  1f,0f,
             hs,0f, hs,  0f,-1f,0f,  1f,1f,
            -hs,0f, hs,  0f,-1f,0f,  0f,1f,

            // Front side
            -hs,0f, hs,   0.0f,0.707f,0.707f, 0f,0f,
             hs,0f, hs,   0.0f,0.707f,0.707f, 1f,0f,
             0f, height,0f, 0.0f,0.707f,0.707f, 0.5f,1f,

            // Right side
             hs,0f, hs,   0.707f,0.707f,0.0f, 0f,0f,
             hs,0f,-hs,   0.707f,0.707f,0.0f, 1f,0f,
             0f, height,0f, 0.707f,0.707f,0.0f, 0.5f,1f,

            // Back side
             hs,0f,-hs,   0.0f,0.707f,-0.707f, 0f,0f,
            -hs,0f,-hs,   0.0f,0.707f,-0.707f, 1f,0f,
             0f, height,0f, 0.0f,0.707f,-0.707f, 0.5f,1f,

            // Left side
            -hs,0f,-hs,  -0.707f,0.707f,0.0f,  0f,0f,
            -hs,0f, hs,  -0.707f,0.707f,0.0f,  1f,0f,
             0f, height,0f, -0.707f,0.707f,0.0f, 0.5f,1f,
        };

        uint[] idx = {
            0,1,2, 0,2,3,
            4,5,6,
            7,8,9,
            10,11,12,
            13,14,15
        };

        return new Mesh(v, idx);
    }
}