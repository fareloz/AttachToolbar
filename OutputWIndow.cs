using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;

namespace AttachToolbar
{
    internal static class OutputWIndow
    {
        private static readonly Lazy<IVsOutputWindowPane> lazy =
            new Lazy<IVsOutputWindowPane>(() => GetOutputWindowAsync().Result);
        private static IVsOutputWindowPane window { 
            get { return lazy.Value; } 
        }

        public async static Task MessageAsync(string txt)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            window.OutputStringThreadSafe(txt);
        }

        private async static Task<IVsOutputWindowPane> GetOutputWindowAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Guid generalPaneGuid = VSConstants.GUID_OutWindowDebugPane;
            _ = outWindow.GetPane(ref generalPaneGuid, out IVsOutputWindowPane debugOutputWindow);
            return debugOutputWindow;
        }
    }
}
