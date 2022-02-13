using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: NeutralResourcesLanguage("en")]

namespace AttachToolbar
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading=true)]
    [InstalledProductRegistration("#100", "#102", "1.1", IconResourceID = 400)]

    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionsPage), "Attach Toolbar", "General Settings", 
        PkgCmdIDList.cmdidOptionsCategory, PkgCmdIDList.cmdidOptionsPage, true)]

    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]

    [Guid(GuidList.guidAttachToolbarPkgString)]
    public sealed class ToolbarPackage : AsyncPackage
    {
        protected async override Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            _env = await GetServiceAsync(typeof(SDTE)) as DTE2;
            if (_env == null)
                throw new Exception("Failed to get DTE service.");

            _controller = new ToolbarController(_env.Debugger as Debugger2);
            State.Settings = new SettingsManager(this);
            await InitializeControlsAsync();
        }

        private async Task InitializeControlsAsync()
        {
            OleMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
                throw new Exception("Failed to get Menu command service.");

            // Program names ComboBox
            // Event on item selection
            CommandID programsComboCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachProgramsCombo);
            OleMenuCommand programsComboCommand = new OleMenuCommand(OnProgramsComboItemSelection, programsComboCommandID);
            programsComboCommand.ParametersDescription = "$"; // accept any argument string
            //programsComboCommand.BeforeQueryStatus += BeforeQueryStatusPrograms;
            mcs.AddCommand(programsComboCommand);
            // Event on combo list expanding
            CommandID programsComboGetListCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachProgramsComboGetList);
            MenuCommand programsComboGetListCommand = new OleMenuCommand(OnProgramsComboGetList, programsComboGetListCommandID);
            mcs.AddCommand(programsComboGetListCommand);

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
            CommandID attachButtonCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachButton);
            OleMenuCommand attachButtonCommand = new OleMenuCommand(OnAttachButtonClickCallback, attachButtonCommandID);
            mcs.AddCommand(attachButtonCommand);
            // Attach to all button
            CommandID attachToAllButtonCommandID = new CommandID(GuidList.guidAttachToolbarCmdSet, (int)PkgCmdIDList.cmdidAttachToAllButton);
            OleMenuCommand attachToAllButtonCommand = new OleMenuCommand(OnAttachToAllButtonClickCallback, attachToAllButtonCommandID);
            mcs.AddCommand(attachToAllButtonCommand);
        }

        private void OnProgramsComboItemSelection(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs == null)
                return;

            IntPtr outValue = eventArgs.OutValue;
            if (outValue != IntPtr.Zero)
            {
                // when vOut is non-null, the IDE is requesting the current value for the combo
                Marshal.GetNativeVariantForObject(State.ProcessName, outValue);
                return;
            }

            string newChoice = eventArgs.InValue as string;
            if (newChoice != null)
            {
                // new value was selected or typed in
                State.ProcessName = newChoice;
                State.Settings.SaveSettings();
            }
        }

        private void OnProgramsComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs == null)
                return;

            IntPtr outValue = eventArgs.OutValue;
            if (outValue != IntPtr.Zero)
            {
                string[] values = State.ProcessList.ToArray();
                Marshal.GetNativeVariantForObject(values, outValue);
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
                    State.Settings.SaveSettings();
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
                    Array enumValues = Enum.GetValues(typeof(EngineType));
                    string[] values = Array.ConvertAll(enumValues as EngineType[],
                        (attachtype) => attachtype.GetEngineName());

                    Marshal.GetNativeVariantForObject(values, outValue);
                }
            }
        }

        private void OnAttachButtonClickCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _controller.Attach(State.ProcessName, State.EngineType, ToolbarController.AttachType.First);
        }

        private void OnAttachToAllButtonClickCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _controller.Attach(State.ProcessName, State.EngineType, ToolbarController.AttachType.ToAll);
        }

        private DTE2 _env;
        private ToolbarController _controller;
    }
}
