namespace SingleThreadedOverlayWithCoroutines
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ClickableTransparentOverlay;
    using Coroutine;
    using ImGuiNET;
    using ImGuiv2;
    using LordsAPI.Memory;
    using static ImGuiv2.WinAPI;

    /// <summary>
    /// Render Loop and Logic Loop are synchronized.
    /// </summary>
    internal class SampleOverlay : Overlay
    {
        private int data;
        private string data2;
        private bool isRunning = true;
        private Event myevent = new Event();
        private ActiveCoroutine myRoutine1;
        private ActiveCoroutine myRoutine2;

        public SampleOverlay()
        {
            myRoutine1 = CoroutineHandler.Start(TickServiceAsync(), name: "MyRoutine-1");
            myRoutine2 = CoroutineHandler.Start(EventServiceAsync(), name: "MyRoutine-2");
        }

        private IEnumerator<Wait> TickServiceAsync()
        {
            int counter = 0;
            while (true)
            {
                counter++;
                yield return new Wait(3);
                this.data = counter;
            }
        }

        private IEnumerator<Wait> EventServiceAsync()
        {
            int counter = 0;
            data2 = "Initializing Event Routine";
            while (true)
            {
                yield return new Wait(myevent);
                data2 = $"Event Raised x {++counter}";
            }
        }
        bool first = false;
        protected override Task Render()
        {
            // Render
            CoroutineHandler.Tick(ImGui.GetIO().DeltaTime);
            if (data % 5 == 1)
            {
                CoroutineHandler.RaiseEvent(myevent);
            }
            Process[] game = Process.GetProcessesByName("Lords Mobile");
            if (game.Length == 1)
            {
                ImGui.Begin("Neki_play Engine for Lords Mobile", ref isRunning, ImGuiWindowFlags.AlwaysAutoResize);
                Rect r = new Rect();
                if (WinAPI.GetWindowRect((game[0].MainWindowHandle), ref r))
                {
                    if (!first)
                    {
                        ImGui.SetWindowPos(new System.Numerics.Vector2(r.left, r.top));
                        ImGui.SetWindowSize(new System.Numerics.Vector2(r.bottom, r.right));
                        first = true;
                    }
                }
                VAMemory memory = new VAMemory(game[0]);
                var energyadress = Utils.ProcessUtils.PointRead(Utils.ProcessUtils.getModuleAdress("GameAssembly.dll", Process.GetProcessesByName("Lords Mobile").FirstOrDefault()), new[] { 0x02657640, 0x88, 0x0, 0x40, 0x20, 0x490 });
                ImGui.Text("Energy: " + memory.ReadInt32(energyadress));
                ImGui.NewLine();
                ImGui.Text("By: Neki_play and SkillShop Techonology");
                ImGui.End();
                if (!isRunning)
                {
                    Close();
                }
            }
            else
            {
                first = false;
            }

            return Task.CompletedTask;
        }
    }
    public class Utils
    {
        public class ProcessUtils
        {
            public static IntPtr PointRead(IntPtr baseAddres, int[] offsets)
            {
                VAMemory vam = new VAMemory(Process.GetProcessesByName("Lords Mobile").FirstOrDefault());
                for (int i = 0; i < offsets.Count() - 1; i++)
                {
                    baseAddres = (IntPtr)vam.ReadInt64(IntPtr.Add(baseAddres, offsets[i]));
                }
                return baseAddres + offsets[offsets.Count() - 1];
            }
            public static IntPtr getModuleAdress(string modulname, Process proc)
            {
                IntPtr result = IntPtr.Zero;
                for (int i = 0; i < proc.Modules.Count; i++)
                {
                    if (proc.Modules[i].ModuleName == modulname)
                    {
                        result = proc.Modules[i].BaseAddress;
                        break;
                    }
                }
                return result;
            }
        }
    }
}