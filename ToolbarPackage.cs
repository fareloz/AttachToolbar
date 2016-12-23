﻿using System.Windows.Forms;
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
    public sealed class ToolbarPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            _env = GetService(typeof(SDTE)) as DTE2;
            if (_env == null)
                throw new Exception("Failed to get DTE service.");

            _settings = new SettingsManager(this);
            InitializeControls();

            var debugger = GetService(typeof(SVsShellDebugger)) as IVsDebugger;
            if (debugger == null)
                throw new Exception("Failed to get debugger service.");

            if (debugger.AdviseDebugEventCallback(_controller) != VSConstants.S_OK)
                throw new Exception("Failed to set debugger event callback.");
        }

        private void InitializeControls()
        {
            _controller = new ToolbarController(_env);

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
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
            attachButtonCommand.BeforeQueryStatus += BeforeQueryStatusAttach;
            mcs.AddCommand(attachButtonCommand);
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

        private void OnProgramsComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr outValue = eventArgs.OutValue;
                if (outValue != IntPtr.Zero)
                {
                    string[] values = State.ProcessList.ToArray();
                    Marshal.GetNativeVariantForObject(values, outValue);
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
                    Array enumValues = Enum.GetValues(typeof(EngineType));
                    string[] values = Array.ConvertAll(enumValues as EngineType[],
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

        private DTE2 _env;
        private ToolbarController _controller;
        private SettingsManager _settings;
    }
}