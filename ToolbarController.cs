using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace AttachToolbar
{
    public class ToolbarController
    {
        public enum AttachType
        {
            First,
            ToAll
        }

        public ToolbarController(DTE2 dte)
        {
            _env = dte;
            _dbg = _env.Debugger as Debugger2;
        }

        public void Attach(string processName, EngineType attachEngineType, AttachType attachType) 
        {
            if(string.IsNullOrEmpty(processName))
                return;

            bool found = false;
            Transport transport = _dbg.Transports.Item("default");
            Engine[] engines = { transport.Engines.Item(attachEngineType.GetEngineName()) };
            var outputWindow = GetOutputWindow();

            foreach (Process2 process in _dbg.LocalProcesses)
            {
                string fileName = Path.GetFileName(process.Name);
                bool? validProcess = fileName?.Equals(processName, StringComparison.InvariantCultureIgnoreCase);
                if (validProcess == null || validProcess == false)
                    continue;

                try
                {
                    process.Attach2(engines);
                    found = true;

                    outputWindow?.OutputString($"Attach Toolbar: Attached to {processName}[{process.ProcessID}].{Environment.NewLine}");
                    if (attachType == AttachType.First)
                        break;
                }
                catch (COMException)
                {
                    outputWindow?.OutputString($"Attach Toolbar: Failed to attach to {processName}[{process.ProcessID}].{Environment.NewLine}");
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

        private static IVsOutputWindowPane GetOutputWindow()
        {
            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            IVsOutputWindowPane debugOutputWindow;
            Guid generalPaneGuid = VSConstants.GUID_OutWindowDebugPane;
            outWindow.GetPane(ref generalPaneGuid, out debugOutputWindow);
            return debugOutputWindow;
        }

        private readonly DTE2 _env;
        private readonly Debugger2 _dbg;
    }
}
