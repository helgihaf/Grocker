using System;
using System.Reflection;
using System.Text;

namespace Grocker
{
    /// <summary>
    /// Encapsulates the command-line options used by Grocker
    /// </summary>
    public class Options
    {
        public bool HelpWanted { get; set; }
        public string DirectoryPath { get; set; }
        public string Filter { get; set; }
        public bool EnableColorSchema { get; set; }
        public string ColorSchemaName { get; set; }

        /// <summary>
        /// Parses the specified string arguments into a new instance of the Options class.
        /// </summary>
        /// <param name="args">The arguments to parse.</param>
        /// <returns>A new instance of the Options class initialized from the specidifed string arguments</returns>
        /// <exception cref="ArgumentException">There is an error parsing an argument.</exception>
        public static Options Parse(string[] args)
        {
            var parameters = CommandLineParameters.Parse(args);

            var options = CreateOptions(parameters);
            CheckForInvalidParameters(parameters);

            return options;
        }

        private static Options CreateOptions(CommandLineParameters parameters)
        {
            var options = new Options();
            options.HelpWanted = parameters.DetachNamedBool("?");
            if (options.HelpWanted)
            {
                return options;
            }

            options.DirectoryPath = parameters.DetachPositional();
            options.Filter = parameters.DetachNamed("f");
            options.EnableColorSchema = !parameters.DetachNamedBool("nc");
            if (options.EnableColorSchema)
            {
                options.ColorSchemaName = parameters.DetachNamed("c");
            }

            return options;
        }

        private static void CheckForInvalidParameters(CommandLineParameters parameters)
        {
            if (parameters.Named.Count > 0)
            {
                foreach (var name in parameters.Named.Keys)
                {
                    throw new ArgumentException("Invalid option /" + name);
                }
            }

            if (parameters.Positionals.Count > 0)
            {
                throw new ArgumentException(string.Format("Invalid argument \"{0}\"", parameters.Positionals[0]));
            }
        }

        public static string GetHelpText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sb = new StringBuilder();
            sb.AppendLine("Marson Grocker. Version " + assembly.GetName().Version.ToString());
            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAttribute != null)
            {
                sb.AppendLine(copyrightAttribute.Copyright);
            }
            sb.AppendLine();
            sb.AppendLine("Usage: Grocker [directory] [-f filter] [-c [colorSchemaName]]");
            sb.AppendLine();
            sb.AppendLine("  [directory]");
            sb.AppendLine("    The directory to monitor. Default is current directory.");
            sb.AppendLine("  [-f filter]");
            sb.AppendLine("    File filter wildcard. Default is *.log");
            sb.AppendLine("  [-c colorSchemaName]");
            sb.AppendLine("    Color schema. Default is auto detect. Incompatible with -nc");
            sb.AppendLine("  [-nc]");
            sb.AppendLine("    No color schema. Incompatible with -c");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
