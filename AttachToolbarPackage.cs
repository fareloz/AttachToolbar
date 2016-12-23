using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace AttachToolbar
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#100", "#102", "1.1", IconResourceID = 400)]

    [ProvideMenuResource("Menus.ctmenu", 1)]

    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]

    [Guid(GuidList.guidAttachToolbarPkgString)]
    public sealed class AttachToolbarPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            _env = GetService(typeof(SDTE)) as DTE2;

            _settings = new SettingsManager(this);

            if (_env != null)
            {
                _controller = new AttachToolbarController(_env);

                OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
                if (mcs != null)
                {
                    // Program names ComboBox
                    // Event on item selection
                    CommandID programsComboCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachProgramsCombo);
                    OleMenuCommand programsComboCommand = new OleMenuCommand(OnProgramsComboItemSelection, programsComboCommandID);
                    programsComboCommand.ParametersDescription = "$"; // accept any argument string
                    programsComboCommand.BeforeQueryStatus += BeforeQueryStatusPrograms;
                    mcs.AddCommand(programsComboCommand);

                    // Engine names ComboBox
                    // Event on item selection
                    CommandID enginesComboCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachEngineCombo);
                    OleMenuCommand enginesComboCommand = new OleMenuCommand(OnEnginesComboItemSelection, enginesComboCommandID);
                    enginesComboCommand.ParametersDescription = "$"; // accept any argument string
                    mcs.AddCommand(enginesComboCommand);
                    // Event on combo list expanding
                    CommandID enginesComboGetListCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachEngineComboGetList);
                    MenuCommand enginesComboGetListCommand = new OleMenuCommand(OnEnginesComboGetList, enginesComboGetListCommandID);
                    mcs.AddCommand(enginesComboGetListCommand);

                    // Attach button
                    CommandID attachButtonCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int) PkgCmdIDList.cmdidAttachButton);
                    OleMenuCommand attachButtonCommand = new OleMenuCommand(OnAttachButtonClickCallback, attachButtonCommandID);
                    attachButtonCommand.BeforeQueryStatus += BeforeQueryStatusAttach;
                    mcs.AddCommand(attachButtonCommand);
                }
            }

            var debugger = GetService(typeof(SVsShellDebugger)) as IVsDebugger;
            if (debugger != null)
            {
                if (debugger.AdviseDebugEventCallback(_controller) != VSConstants.S_OK) ;
            }
        }

        private void OnProgramsComboItemSelection(object sender, EventArgs e)
        {
            
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr outValue = eventArgs.OutValue;

                if (outValue != IntPtr.Zero)
                {
                    // when vOut is non-null, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(State.ProcessName, outValue);
                }
                else if (newChoice != null)
                {
                    // new value was selected or typed in
                    State.ProcessName = newChoice;
                    _settings.SaveSettings();
                }
            }
        }

        private void OnEnginesComboItemSelection(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr outValue = eventArgs.OutValue;

                if (outValue != IntPtr.Zero)
                {
                    // when outValue is non-null, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(State.EngineType.GetEngineName(), outValue);
                }
                else if (newChoice != null)
                {
                    State.EngineType = newChoice.GetAttachType();
                    _settings.SaveSettings();
                }
            }
        }

        private void OnEnginesComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr outValue = eventArgs.OutValue;
                if (outValue != IntPtr.Zero)
                {
                    Array enumValues = Enum.GetValues(typeof(AttachEngineType));
                    string[] values = Array.ConvertAll(enumValues as AttachEngineType[],
                        (attachtype) => attachtype.GetEngineName());

                    Marshal.GetNativeVariantForObject(values, outValue);
                }
            }
        }

        private void OnAttachButtonClickCallback(object sender, EventArgs e)
        {
            _controller.AttachTo(State.ProcessName, State.EngineType);
        }

        private void BeforeQueryStatusAttach(object sender, EventArgs e)
        { 
            OleMenuCommand control = sender as OleMenuCommand;
            if(control != null)
            {
                control.Enabled = !State.IsAttached;
            }
        }

        private void BeforeQueryStatusPrograms(object sender, EventArgs e)
        {
            IVsUIShell uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;
            if (uiShell != null)
            {
                try
                {
                    uiShell.SetMRUComboTextW(new[] { GuidList.guidAttachToolbarCmdSet },
                        (int)PkgCmdIDList.cmdidAttachProgramsCombo, State.ProcessName, 0);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                }
            }
            
            OleMenuCommand control = sender as OleMenuCommand;
            if(control != null)
            {
                control.BeforeQueryStatus -= BeforeQueryStatusPrograms;
            }
        }

        private DTE2 _env;
        private AttachToolbarController _controller;
        private SettingsManager _settings;
    }
}