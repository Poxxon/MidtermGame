using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace MidtermGame;

internal static class Program
{
    private static void Main()
    {
        var nativeSettings = new NativeWindowSettings
        {
            Title = "Mini 3D Explorer",
            ClientSize = new Vector2i(1280, 720),
            Flags = ContextFlags.ForwardCompatible
        };

        using var window = new MiniGame(GameWindowSettings.Default, nativeSettings);
        window.Run();
    }
}