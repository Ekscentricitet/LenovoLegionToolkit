﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LenovoLegionToolkit.Lib.Settings
{
    public class SettingsManager
    {
        private class SettingsStore
        {
            public Dictionary<PowerModeState, string> PowerPlans { get; set; } = new();
        }

        private static SettingsManager _instance;
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                return _instance;
            }
        }

        private JsonSerializerOptions _jsonSerializerOptions;
        private SettingsStore _settingsStore;
        private string _settingsStorePath;

        public Dictionary<PowerModeState, string> PowerPlans { 
            get => _settingsStore.PowerPlans;
            set => _settingsStore.PowerPlans = value;
        }

        private SettingsManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderPath = Path.Combine(appData, "LenovoLegionToolkit");
            var settingsStorePath = Path.Combine(folderPath, "settings.json");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            if (!File.Exists(settingsStorePath))
                File.Create(settingsStorePath);

            _jsonSerializerOptions = new() { WriteIndented = true };
            _settingsStorePath = settingsStorePath;

            Deserialize();
        }

        public void Synchronize() => Serialize();

        private void Deserialize()
        {
            var settingsSerialized = File.ReadAllText(_settingsStorePath);
            try
            {
                _settingsStore = JsonSerializer.Deserialize<SettingsStore>(settingsSerialized, _jsonSerializerOptions);
            }
            catch (JsonException)
            {
                _settingsStore = new();
                Serialize();
            }
        }

        private void Serialize()
        {
            var settingsSerialized = JsonSerializer.Serialize(_settingsStore, _jsonSerializerOptions);
            File.WriteAllText(_settingsStorePath, settingsSerialized);
        }
    }
}