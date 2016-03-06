﻿using EloBuddy.SDK.Menu.Values;

namespace ezEvade.Config.Controls
{
    public class DynamicKeyBind
    {
        public KeyBind KeyBind;
        private readonly ConfigValue _configKey;

        public DynamicKeyBind(ConfigValue key, string displayName, bool defaultValue, KeyBind.BindTypes type, uint defaultKey1 = 27, uint defaultKey2 = 27)
        {
            _configKey = key;
            KeyBind = new KeyBind(displayName, defaultValue, type, defaultKey1, defaultKey2);
            Properties.SetValue(_configKey, KeyBind.CurrentValue);
            KeyBind.OnValueChange += KeyBind_OnValueChange;
        }


        private void KeyBind_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            Properties.SetValue(_configKey, KeyBind.CurrentValue, false);
        }
    }
}