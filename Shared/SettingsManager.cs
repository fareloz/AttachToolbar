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
                string processList = _settings.GetString("AttachToolbar", "ProcessList");
                if(!string.IsNullOrEmpty(processList))
                    State.ProcessList = processList.Split(';').ToList();

                State.ProcessIndex = _settings.GetInt32("AttachToolbar", "ProcessIndex");
                State.EngineType = _settings.GetString("AttachToolbar", "EngineType").GetAttachType();
            }
            catch (ArgumentException)
            {
                CreateDefaultSettings();
                State.Clear();
            }
        }

        public void SaveSettings()
        {
            _settings.SetString("AttachToolbar", "ProcessList", string.Join(";", State.ProcessList));
            _settings.SetInt32("AttachToolbar", "ProcessIndex", State.ProcessIndex);
            _settings.SetString("AttachToolbar", "EngineType", State.EngineType.GetEngineName());
        }

        private void CreateDefaultSettings()
        {
            _settings.DeleteCollection("AttachToolbar");
            _settings.CreateCollection("AttachToolbar");
            _settings.SetString("AttachToolbar", "ProcessList", "");
            _settings.SetInt32("AttachToolbar", "ProcessIndex", -1);
            _settings.SetString("AttachToolbar", "EngineType", EngineType.Native.GetEngineName());
        }

        private readonly WritableSettingsStore _settings;
    }
}