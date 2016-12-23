using System.Windows.Forms;

namespace AttachToolbar
{
    public class OptionsGeneralWindow : UserControl
    {
        public OptionsGeneralWindow(OptionsPage page)
        {
            _page = page;
        }

        public void Initialize()
        {
            
        }

        private readonly OptionsPage _page;
    }
}
