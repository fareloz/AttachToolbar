using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace AttachToolbar
{
    public class SettingsManager
    {
        public SettingsManager(Package settingProvider)
        {
            ShellSettingsManager settingsManager = new ShellSettingsManager(settingProvider as IServiceProvider);
            _settings = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (_settings.CollectionExists("AttachToolbar") == false)
            {
                CreateDefaultSettings();
            }

            State.AttachProgramName = _settings.GetString("AttachToolbar", "LastProgramName");
            State.AttachEngineType = _settings.GetString("AttachToolbar", "LastAttachType").GetAttachType();
        }

        public void SaveSettings()
        {
            _settings.SetString("AttachToolbar", "LastProgramName", State.AttachProgramName);
            _settings.SetString("AttachToolbar", "LastAttachType", State.AttachEngineType.GetEngineName());
        }

        private void CreateDefaultSettings()
        {
            _settings.CreateCollection("AttachToolbar");
            _settings.SetString("AttachToolbar", "LastProgramName", "");
            _settings.SetString("AttachToolbar", "LastAttachType", AttachEngineType.Native.GetEngineName());
        }

        private readonly WritableSettingsStore _settings;
    }
}