using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace AttachToolbar
{
    public class AttachToolbarController : IDebugEventCallback2
    {
        public AttachToolbarController(DTE2 dte)
        {
            _env = dte;
            _dbg = _env.Debugger as Debugger2;
        }

        public void AttachTo(string processName, AttachEngineType attachEngineType) 
        {
            bool found = false;
            if (State.IsAttached) {
                _dbg.DetachAll();
                State.IsAttached = false;
            }

            foreach (Process2 process in _dbg.LocalProcesses)
            {
                string fileName = Path.GetFileName(process.Name);
                if (fileName.Equals(processName, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Transport transport = _dbg.Transports.Item("default");
                    Engine[] engines = new[] { transport.Engines.Item(attachEngineType.GetEngineName()) };
                    
                    try {
                        process.Attach2(engines);
                    }
                    catch {
                        found = false;
                    }
                    break;
                }
            }

            State.IsAttached = found;
            if (!found)
            {
                MessageBox.Show("Failed to attach to " + processName + ".",
                    "Attach Toolbar", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public int Event(IDebugEngine2 engine, IDebugProcess2 process, IDebugProgram2 program,
            IDebugThread2 thread, IDebugEvent2 debugEvent, ref Guid riidEvent, uint attributes)
        {
            // Ignore a few events right away.
            if (debugEvent is IDebugModuleLoadEvent2 ||
                debugEvent is IDebugThreadCreateEvent2 ||
                debugEvent is IDebugThreadDestroyEvent2)
                return VSConstants.S_OK;

            if (process == null)
                return VSConstants.S_OK;

            if (debugEvent is IDebugProcessCreateEvent2) {
                State.IsAttached = true;
            }
            else if (debugEvent is IDebugProcessDestroyEvent2) {
                State.IsAttached = false;
            }

            return VSConstants.S_OK;
        }

        private readonly DTE2 _env;
        private readonly Debugger2 _dbg;
    }
}
