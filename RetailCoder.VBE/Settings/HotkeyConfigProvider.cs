﻿using Rubberduck.SettingsProvider;

namespace Rubberduck.Settings
{
    public class HotkeyConfigProvider : IConfigProvider<HotkeySettings>
    {
        private readonly IPersistanceService<HotkeySettings> _persister;

        public HotkeyConfigProvider(IPersistanceService<HotkeySettings> persister)
        //public HotkeyConfigProvider(IPersistanceService<HotkeySettings> persister, IEnumerable<CommandBase> commands)
        //public HotkeyConfigProvider(IPersistanceService<HotkeySettings> persister, HotkeySettings hotkeySettings)
        {
            _persister = persister;
        }

        public HotkeySettings Create()
        {
            var prototype = new HotkeySettings();
            return _persister.Load(prototype) ?? prototype;
        }

        public HotkeySettings CreateDefaults()
        {
            return new HotkeySettings();
        }

        public void Save(HotkeySettings settings)
        {
            _persister.Save(settings);
        }
    }
}
