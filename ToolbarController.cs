using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace AttachToolbar
{
    public class ToolbarController
    {
        public ToolbarController(DTE2 dte)
        {
            _env = dte;
            _dbg = _env.Debugger as Debugger2;
        }

        public void AttachTo(string processName, EngineType attachEngineType) 
        {
            if(processName == String.Empty)
                return;

            bool found = false;
            foreach (Process2 process in _dbg.LocalProcesses)
            {
                string fileName = Path.GetFileName(process.Name);
                if (fileName != null && fileName.Equals(processName, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    Transport transport = _dbg.Transports.Item("default");
                    Engine[] engines = { transport.Engines.Item(attachEngineType.GetEngineName()) };
                    
                    try {
                        process.Attach2(engines);
                    }
                    catch {
                        found = false;
                    }
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show(
                    $"Failed to attach to {processName}.",
                    "Attach Toolbar", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private readonly DTE2 _env;
        private readonly Debugger2 _dbg;
    }
}
