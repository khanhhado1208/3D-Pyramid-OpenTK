using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;

public class Shader
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompileError(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompileError(fragmentShader);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);
        CheckProgramLinkError(Handle);

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            Console.WriteLine($"Warning: Uniform '{name}' not found in shader!");
        }
        GL.UniformMatrix4(location, false, ref matrix);
    }

    private void CheckShaderCompileError(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Shader compile error: {infoLog}");
        }
    }

    private void CheckProgramLinkError(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            Console.WriteLine($"Shader Program Link Error: {infoLog}");
        }
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}
