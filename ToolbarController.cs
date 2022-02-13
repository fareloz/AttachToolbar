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

        public void Attach(string processName, EngineType engineType, AttachType attachType) 
        {
            if(string.IsNullOrEmpty(processName))
                return;

            Transport transport = _dbg.Transports.Item("default");
            Engine[] engines = { transport.Engines.Item(engineType.GetEngineName()) };
            bool found = (attachType == AttachType.First) ?
                AttachFirst(processName, engines) :
                AttachAll(processName, engines);

            if (!found)
            {
                MessageBox.Show( $"No processes with name {processName} are found.",
                    "Attach Toolbar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
            }
        }

        private bool AttachFirst(string processName, Engine[] engines)
        {
            return GetProcesses(processName).Any(p => AttachProcess(p, engines));
        }

        private bool AttachAll(string processName, Engine[] engines)
        {
            return GetProcesses(processName).Where(p => AttachProcess(p, engines)).Count() > 0;
        }

        private bool AttachProcess(Process2 process, Engine[] engines)
        {
            try
            {
                process.Attach2(engines);
                _ = OutputWIndow.MessageAsync($"Attach Toolbar: Attached to {process.Name}[{process.ProcessID}].{Environment.NewLine}");
                return true;
            }
            catch (COMException)
            {
                _ = OutputWIndow.MessageAsync($"Attach Toolbar: Failed to attach to {process.Name}[{process.ProcessID}].{Environment.NewLine}");
                return false;
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
