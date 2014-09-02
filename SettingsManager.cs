using System;
using AttachManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace Attachmanager
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
            if (_settings.CollectionExists("AttachManager") == false)
            {
                CreateDefaultSettings();
            }

            State.AttachProgramName = _settings.GetString("AttachManager", "LastProgramName");
            State.AttachEngineType = _settings.GetString("AttachManager", "LastAttachType").GetAttachType();
        }

        public void SaveSettings()
        {
            _settings.SetString("AttachManager", "LastProgramName", State.AttachProgramName);
            _settings.SetString("AttachManager", "LastAttachType", State.AttachEngineType.GetEngineName());
        }

        private void CreateDefaultSettings()
        {
            _settings.CreateCollection("AttachManager");
            _settings.SetString("AttachManager", "LastProgramName", "");
            _settings.SetString("AttachManager", "LastAttachType", AttachEngineType.Native.GetEngineName());
        }

        private readonly WritableSettingsStore _settings;
    }
}