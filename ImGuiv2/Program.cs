using SingleThreadedOverlayWithCoroutines;
using System;
using System.Threading.Tasks;

namespace ImGuiv2
{
    class Program
    {
        static async Task Main()
        {
            using var overlay = new SampleOverlay();
            await overlay.Run();
        }
    }
}
