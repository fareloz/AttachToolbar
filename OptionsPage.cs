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
                OptionsGeneralWindow page = new OptionsGeneralWindow(this);
                page.Initialize();
                return page;
            }
        }
    }
}
