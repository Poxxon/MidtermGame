using System;
using System.IO;
using StbImageSharp;
using GL = OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Graphics.OpenGL4;

namespace MidtermGame.Engine;

public class Texture : IDisposable
{
    public int Handle { get; }
    private readonly TextureUnit _unit;

    public Texture(string path, TextureUnit unit)
    {
        _unit = unit;
        Handle = GL.GenTexture();
        Bind();

        if (!File.Exists(path))
            throw new FileNotFoundException("Texture not found", path);

        using var stream = File.OpenRead(path);
        var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
            img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    }

    public void Bind()
    {
        GL.ActiveTexture(_unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose() => GL.DeleteTexture(Handle);
}