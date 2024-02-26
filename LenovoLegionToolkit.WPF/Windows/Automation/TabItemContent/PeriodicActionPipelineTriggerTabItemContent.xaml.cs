﻿using System;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent
{
    public partial class PeriodicAutomationPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<IPeriodicAutomationPipelineTrigger>
    {
        private readonly IPeriodicAutomationPipelineTrigger _trigger;
        private readonly TimeSpan _period;

        public PeriodicAutomationPipelineTriggerTabItemContent(IPeriodicAutomationPipelineTrigger trigger)
        {
            _trigger = trigger;
            _period = trigger.Period;
            InitializeComponent();
        }

        public void MinutesTabItem_Initialized(object? sender, EventArgs e)
        {
            _periodPickerMinutes.Value = _period.TotalMinutes;
        }

        public IPeriodicAutomationPipelineTrigger GetTrigger()
        {
            var periodMinutes = (int)_periodPickerMinutes.Value!;

            return _trigger.DeepCopy(TimeSpan.FromMinutes(periodMinutes));
        }
    }
}
