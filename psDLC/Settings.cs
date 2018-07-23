using Microsoft.Win32;
using System;

namespace psDLC
{
    class Settings
    {

        public void SaveSetting(string sKey, string sValue)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sValue);
        }


        public void SaveSetting(string sKey, int sValue)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sValue);
        }


        public void SaveSetting(string sKey, bool sValue)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sValue);
        }


        public string GetSetting(string sKey, string sDefault)
        {
            if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, null) != null)
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault).ToString();
            }
            else
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault);
                return sDefault;
            }
        }


        public int GetSetting(string sKey, int sDefault)
        {
            if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, null) != null)
            {
                return Convert.ToInt32(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault));
            }
            else
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault);
                return sDefault;
            }
        }


        public bool GetSetting(string sKey, bool sDefault)
        {
            if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, null) != null)
            {
                return Convert.ToBoolean(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault));
            }
            else
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\psDLC", sKey, sDefault);
                return sDefault;
            }
        }
    }
}
