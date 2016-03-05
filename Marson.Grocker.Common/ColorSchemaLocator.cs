using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public static class ColorSchemaLocator
    {
        public const string DefaultFileName = "ColorSchemas.Grocker.xml";
        public const string DefaultAppSettingsName = "colorSchemasFilePath";

        public static string LocateSchemaFile()
        {
            return LocateSchemaFile(DefaultFileName);
        }

        public static string LocateSchemaFile(string fileName)
        {
            return LocateSchemaFile(fileName, DefaultAppSettingsName);
        }

        public static string LocateSchemaFile(string fileName, string appSettingsName)
        { 
            // 1. Try current path
            if (File.Exists(fileName))
            {
                return Path.Combine(Environment.CurrentDirectory, fileName);
            }

            // 2. Try app.config
            string filePath = System.Configuration.ConfigurationManager.AppSettings[appSettingsName];
            if (filePath != null)
            {
                // Note: We don't check for existance. This is an explicit choice by the user.
                return filePath;
            }

            // 3. Location of current assembly
            var codeBaseDirectory = GetCodeBaseDirectory();
            filePath = Path.Combine(codeBaseDirectory, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            // Give up
            return null;
        }

        private static string GetCodeBaseDirectory()
        {
            var codeBaseUri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            return Path.GetDirectoryName(codeBaseUri.LocalPath);
        }


    }
}
