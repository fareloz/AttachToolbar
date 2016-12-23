using System;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace AttachToolbar
{
    public class SettingsManager
    {
        public SettingsManager(Package settingProvider)
        {
            ShellSettingsManager settingsManager = new ShellSettingsManager(settingProvider);
            _settings = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            LoadSettings();
        }

        public void LoadSettings()
        {
            try
            {
                string attachList = _settings.GetString("AttachToolbar", "ProcessList");
                State.ProcessList = attachList.Split(';').ToList();

                string lastProgram = _settings.GetString("AttachToolbar", "LastProcessName");
                State.ProcessName = State.ProcessList.Contains(lastProgram) ?
                    lastProgram : State.ProcessList.First();

                State.EngineType = _settings.GetString("AttachToolbar", "LastEngineType").GetAttachType();
            }
            catch (Exception)
            {
                CreateDefaultSettings();
                State.Clear();
            }
        }

        public void SaveSettings()
        {
            _settings.SetString("AttachToolbar", "ProcessList", string.Join(";", State.ProcessList));
            _settings.SetString("AttachToolbar", "LastProcessName", State.ProcessName);
            _settings.SetString("AttachToolbar", "LastEngineType", State.EngineType.GetEngineName());
        }

        private void CreateDefaultSettings()
        {
            _settings.DeleteCollection("AttachToolbar");
            _settings.CreateCollection("AttachToolbar");
            _settings.SetString("AttachToolbar", "ProcessList", "");
            _settings.SetString("AttachToolbar", "LastProcessName", "");
            _settings.SetString("AttachToolbar", "LastEngineType", EngineType.Native.GetEngineName());
        }

        private readonly WritableSettingsStore _settings;
    }
}