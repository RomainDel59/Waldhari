// This class is under MIT License,
// copyrighted to https://github.com/lucasvinbr,
// and comes from https://github.com/lucasvinbr/GTA5GangMod.
// I adapt it to my use.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Files
{
    /// <summary>
    /// Static class for handling multiple languages
    /// </summary>
    public static class Localization
    {
        private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");
        
        public static string CurCulture = null;

        internal const string LocaleNotFoundTxt = "-MISSINGLOCALE-";

        /// <summary>
        /// the path to the locales folder, starting from the mod folder
        /// </summary>
        private const string LocalesSubpath = "localization/";

        private static LocalizationFile _currentlyUsedFile;

        private static List<CultureInfo> _availableLanguageCultures;

        private static string LocalesPath => PersistenceHandler.GetDirectory() + LocalesSubpath;

        /// <summary>
        /// sets up a starting locale file and prepares the available locales list.
        /// Should be run before anything else locale-related
        /// </summary>
        public static void Initialize()
        {
            CultureInfo curCulture;
            try
            {
                curCulture = CultureInfo.GetCultureInfo(CurCulture);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex.ToString());
                curCulture = DefaultCulture;
            }
            
            FetchAndStoreAvailableLanguages();

            // check if the local culture exists as one of the lang options;
            // if it does, use it. If not, use a default.
            if (_availableLanguageCultures.Contains(curCulture))
            {
                SetCurrentLangCulture(curCulture);
            }
            else
            {
                SetCurrentLangCulture(DefaultCulture);
            }

            Logger.Debug(_currentlyUsedFile?.DebugDumpLocaleData());
        }

        private static void FetchAndStoreAvailableLanguages()
        {
            _availableLanguageCultures = new List<CultureInfo>();
            Logger.Debug("Check available cultures in " + LocalesPath);
            if (Directory.Exists(LocalesPath))
            {
                foreach (var filePath in Directory.EnumerateFiles(LocalesPath))
                {
                    Logger.Debug("Found file : " + filePath);
                    if (filePath.EndsWith(".xml"))
                    {
                        var fileNameNoExtension = Path.GetFileNameWithoutExtension(filePath);
                        try
                        {
                            var fileCulture = CultureInfo.GetCultureInfo(fileNameNoExtension);
                            _availableLanguageCultures.Add(fileCulture);
                        }
                        catch (Exception ex)
                        {
                            Logger.Exception(ex.ToString());
                        }
                    }
                }
            }
        }

        public static string GetTextByKey(string localeKey, List<string> values = null)
        {
            if (_currentlyUsedFile == null)
            {
                Logger.Warning($"localeKey='{localeKey}' is missing");
                return LocaleNotFoundTxt;
            }
            
            if (string.IsNullOrEmpty(localeKey)) return string.Empty;

            var text = _currentlyUsedFile.GetTextByKey(localeKey);
            Logger.Debug($"text='{text}'");
            
            // Merge values if exist
            if (values != null && values.Count > 0)
            {
                var convertedValues = values.Select(
                    value => int.TryParse(value, out int number) ? 
                        NumberHelper.ConvertToAmount(number) : 
                        value
                    ).ToArray();
                
                text = string.Format(text, convertedValues);
                Logger.Debug($"text with values='{text}'");
            }
            
            return text;
        }

        /// <summary>
        /// returns true on success
        /// </summary>
        /// <param name="targetCulture"></param>
        /// <returns></returns>
        private static void SetCurrentLangCulture(CultureInfo targetCulture)
        {
            var fileSubPath = LocalesSubpath + targetCulture.Name;

            var fileData = PersistenceHandler.LoadFromFile<LocalizationFile>(fileSubPath);

            if (fileData != null)
            {
                _currentlyUsedFile = fileData;
            }
            else
            {
                Logger.Error($"Could not load locale culture file: {targetCulture.Name}.");
            }
        }
    }

    /// <summary>
    /// file containing texts for a language
    /// </summary>
    [Serializable]
    public class LocalizationFile
    {
        public string LanguageCode;
        public List<LocaleKey> Locales;

        public LocalizationFile()
        {
        }

        public string GetTextByKey(string localeKey)
        {
            var entry = Locales.Find(l => l.Key == localeKey);

            // if it's a valid entry that we found, return its value
            if (entry.Key == localeKey)
            {
                return entry.Value;
            }

            Logger.Warning($"Locale file '{LanguageCode}': key '{localeKey}' not found.");
            return Localization.LocaleNotFoundTxt;
        }

        /// <summary>
        /// returns a (possibly very big) string containing all locale keys and values
        /// </summary>
        /// <returns></returns>
        public string DebugDumpLocaleData()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Locale file {LanguageCode}: dump locales start");

            foreach (var kvp in Locales)
            {
                sb.AppendLine($"key: {kvp.Key} - Value: {kvp.Value}");
            }

            sb.AppendLine($"Locale file {LanguageCode}: dump locales end");

            return sb.ToString();
        }
    }

    /// <summary>
    /// a simple package for locale entries, containing a string key and a string value
    /// </summary>
    [Serializable]
    public struct LocaleKey
    {
        public string Key, Value;
    }
}