using System;
using System.IO;
using OpenTK.Mathematics;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Graphics.OpenGL4;

namespace MidtermGame.Engine;

public class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexSource   = File.ReadAllText(vertexPath);
        var fragmentSource = File.ReadAllText(fragmentPath);

        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vertexSource);
        GL.CompileShader(vs);
        GL.GetShader(vs, ShaderParameter.CompileStatus, out int vStatus);
        if (vStatus == 0) throw new Exception($"Vertex compile error: {GL.GetShaderInfoLog(vs)}");

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fragmentSource);
        GL.CompileShader(fs);
        GL.GetShader(fs, ShaderParameter.CompileStatus, out int fStatus);
        if (fStatus == 0) throw new Exception($"Fragment compile error: {GL.GetShaderInfoLog(fs)}");

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vs);
        GL.AttachShader(Handle, fs);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        if (linkStatus == 0) throw new Exception($"Program link error: {GL.GetProgramInfoLog(Handle)}");

        GL.DetachShader(Handle, vs);
        GL.DetachShader(Handle, fs);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetInt(string name, int value)    => GL.Uniform1(GetLocation(name), value);
    public void SetFloat(string name, float value)=> GL.Uniform1(GetLocation(name), value);
    public void SetVector3(string name, Vector3 v)=> GL.Uniform3(GetLocation(name), v);
    public void SetBool(string name, bool v)      => GL.Uniform1(GetLocation(name), v ? 1 : 0);
    public void SetMatrix4(string name, Matrix4 m)=> GL.UniformMatrix4(GetLocation(name), false, ref m);

    private int GetLocation(string name)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        if (loc == -1) throw new Exception($"Uniform '{name}' not found.");
        return loc;
    }

    public void Dispose() => GL.DeleteProgram(Handle);
}