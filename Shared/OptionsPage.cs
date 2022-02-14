using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

namespace AttachToolbar
{
    [Guid(GuidList.guidAttachToolbarOptionPage)]
    public class OptionsPage : DialogPage
    {
        protected override IWin32Window Window
        {
            get
            {
                if(_generalPage == null)
                {
                    _generalPage = new OptionsGeneralWindow();
                    _generalPage.Initialize();
                }
                
                return _generalPage;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_generalPage != null)
                {
                    _generalPage.Dispose();
                    _generalPage = null;
                    State.Settings.SaveSettings();
                }
            }
            base.Dispose(disposing);
        }

        private OptionsGeneralWindow _generalPage;
    }
}
