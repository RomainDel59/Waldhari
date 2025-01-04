// This class is under MIT License,
// copyrighted to https://github.com/lucasvinbr,
// and comes from https://github.com/lucasvinbr/GTA5GangMod.
// I adapt it to my use.

using System;
using System.IO;

namespace Waldhari.Common.Files
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        // Level setup (default = INFO)
        public static int Level = 4;
        
        public static string GetFilePath()
        {
            return PersistenceHandler.GetDirectory() + PersistenceHandler.ModName + ".log";
        }

        /// <summary>
        /// logs the message to a file... 
        /// but only if the log level defined in the mod options is greater or equal to the message's level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level">The smaller the level, the more relevant, with 1 being very important failure messages and 5 being max debug spam</param>
        private static void Log(object message, int level)
        {
            if (level > Level) return;
            
            File.AppendAllText(GetFilePath(), $"{DateTime.Now} {GetType(level)} {message}{Environment.NewLine}");
        }

        private static string GetType(int level)
        {
            switch (level)
            {
                case 1: return "EXCEPTION";
                case 2: return "ERROR";
                case 3: return "WARNING";
                case 4: return "INFO";
                case 5: return "DEBUG";
                default: return "UNKNOWN";
            }
        }
        
        /// <summary>
        /// Overwrites the log file's content with a "cleared log" message
        /// </summary>
        public static void Clear()
        {
            File.WriteAllText(GetFilePath(),"");
            Warning("Cleared log! (This happens when the mod is initialized)");
        }

        public static void Exception(string message)
        {
            Log(message, 1);
        }

        public static void Error(string message)
        {
            Log(message, 2);
        }

        public static void Warning(string message)
        {
            Log(message, 3);
        }

        public static void Info(string message)
        {
            Log(message, 4);
        }

        public static void Debug(string message)
        {
            Log(message, 5);
        }
    }
}