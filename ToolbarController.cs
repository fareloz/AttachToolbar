using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE80;

namespace AttachToolbar
{
    public class ToolbarController
    {
        public enum AttachType
        {
            First,
            ToAll
        }

        public ToolbarController(Debugger2 dbg)
        {
            _dbg = dbg;
        }

        public void Attach(string processName, EngineType attachEngineType, AttachType attachType) 
        {
            if(string.IsNullOrEmpty(processName))
                return;

            bool found = false;
            Transport transport = _dbg.Transports.Item("default");
            Engine[] engines = { transport.Engines.Item(attachEngineType.GetEngineName()) };
            foreach (Process2 process in GetProcesses(processName))
            {
                try
                {
                    process.Attach2(engines);
                    found = true;
                    _ = OutputWIndow.MessageAsync($"Attach Toolbar: Attached to {processName}[{process.ProcessID}].{Environment.NewLine}");
                    if (attachType == AttachType.First)
                        break;
                }
                catch (COMException)
                {
                    _ = OutputWIndow.MessageAsync($"Attach Toolbar: Failed to attach to {processName}[{process.ProcessID}].{Environment.NewLine}");
                }
            }

            if (!found)
            {
                MessageBox.Show( $"No processes with name {processName} are found.",
                    "Attach Toolbar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
        }

        private List<Process2> GetProcesses(string processName)
        {
            var processes =
                from p in _dbg.LocalProcesses.Cast<Process2>().ToList()
                where Path.GetFileName(p.Name)?.Equals(processName, StringComparison.InvariantCultureIgnoreCase) == true
                select p;
            return processes.ToList();
        }

        private readonly Debugger2 _dbg;
    }
}
