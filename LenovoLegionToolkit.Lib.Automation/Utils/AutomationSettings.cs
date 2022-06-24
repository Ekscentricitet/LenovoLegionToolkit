﻿using System.Collections.Generic;
using System.IO;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LenovoLegionToolkit.Lib.Automation.Utils
{
    public class AutomationSettings
    {
        public class AutomationSettingsStore
        {
            public bool IsEnabled { get; set; } = false;

            public List<AutomationPipeline> Pipelines { get; set; } = new()
            {
                new AutomationPipeline
                {
                    Trigger = new ACAdapterConnectedAutomationPipelineTrigger(),
                    Steps = { new PowerModeAutomationStep(PowerModeState.Balance) },
                },
                new AutomationPipeline
                {
                    Trigger = new ACAdapterDisconnectedAutomationPipelineTrigger(),
                    Steps = { new PowerModeAutomationStep(PowerModeState.Quiet) },
                },
                new AutomationPipeline
                {
                    Name = "Deactivate GPU",
                    Steps = { new DeactivateGPUAutomationStep(DeactivateGPUAutomationStepState.KillApps) },
                },
            };
        }

        private readonly AutomationSettingsStore _settingsStore;

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly string _automationStorePath;

        public bool IsEnabled
        {
            get => _settingsStore.IsEnabled;
            set => _settingsStore.IsEnabled = value;
        }

        public List<AutomationPipeline> Pipeliness
        {
            get => _settingsStore.Pipelines;
            set => _settingsStore.Pipelines = value;
        }

        public AutomationSettings()
        {
            _jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                Converters =
                {
                    new StringEnumConverter(),
                }
            };
            _automationStorePath = Path.Combine(Folders.AppData, "automation.json");

            try
            {
                var settingsSerialized = File.ReadAllText(_automationStorePath);
                _settingsStore = JsonConvert.DeserializeObject<AutomationSettingsStore>(settingsSerialized, _jsonSerializerSettings) ?? new();
            }
            catch
            {
                _settingsStore = new();
                Synchronize();
            }
        }

        public void Synchronize()
        {
            var settingsSerialized = JsonConvert.SerializeObject(_settingsStore, _jsonSerializerSettings);
            File.WriteAllText(_automationStorePath, settingsSerialized);
        }
    }
}