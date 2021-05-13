using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Niflheim.Installer.Services
{
    public class PreferencesService
    {
        private readonly FileInfo preferenceFile;

        private Dictionary<string, string> defaultPreferences = new Dictionary<string, string>();

        public PreferencesService(FileInfo preferenceFile, Dictionary<string, string> defaultPreferences)
        {
            this.preferenceFile = preferenceFile;
            this.defaultPreferences = defaultPreferences;
        }

        public string GetPreference(string preferenceName)
        {
            if (!preferenceFile.Exists)
            {
                CreateDefaultPreferences();
            }

            var prefs = File.ReadAllText(preferenceFile.FullName);

            return JsonSerializer.Deserialize<Dictionary<string, string>>(prefs)[preferenceName];
        }

        public void SetPreference(string preferenceName, string preferenceValue)
        {
            if (!preferenceFile.Exists)
            {
                CreateDefaultPreferences();
            }

            Dictionary<string, string> prefsObject = ReadPreferences();
            prefsObject[preferenceName] = preferenceValue;
            WritePreferences(prefsObject);
        }

        private Dictionary<string, string> ReadPreferences()
        {
            var prefs = File.ReadAllText(preferenceFile.FullName);
            var prefsObject = JsonSerializer.Deserialize<Dictionary<string, string>>(prefs);
            return prefsObject;
        }

        private void CreateDefaultPreferences()
        {
            WritePreferences(this.defaultPreferences);
        }

        private void WritePreferences(Dictionary<string, string> preferences)
        {
            var prefs = JsonSerializer.Serialize(preferences, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            File.WriteAllTextAsync(this.preferenceFile.FullName, prefs).Wait();
        }
    }
}
