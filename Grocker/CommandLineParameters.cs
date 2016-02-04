using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocker
{
    class CommandLineParameters
    {
        private readonly List<string> positionals = new List<string>();
        private readonly Dictionary<string, string> named = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public static CommandLineParameters Parse(string[] args)
        {
            var clp = new CommandLineParameters();
            clp.DoParse(args);
            return clp;
        }

        private CommandLineParameters()
        {
        }

        public IReadOnlyList<string> Positionals
        {
            get
            {
                return positionals;
            }
        }


        public IReadOnlyDictionary<string, string> Named
        {
            get
            {
                return named;
            }
        }

        public string DetachNamed(string name)
        {
            string value;
            if (named.TryGetValue(name, out value))
            {
                named.Remove(name);
            }
            else
            {
                value = null;
            }
            return value;
        }

        private void DoParse(string[] args)
        {
            foreach (var arg in args)
            {
                if (IsNamedParameter(arg))
                {
                    string name;
                    string value;
                    if (SplitNameValue(arg, out name, out value))
                    {
                        named.Add(name, value);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid argument format: " + arg);
                    }
                }
                else
                {
                    positionals.Add(arg);
                }
            }
        }

        private static bool SplitNameValue(string arg, out string name, out string value)
        {
            name = null;
            value = null;
            if (arg.Length <= 1)
            {
                return false;
            }

            int nameEndMarkerIndex = arg.IndexOf(':', 1);
            if (nameEndMarkerIndex == -1)
            {
                name = arg.Substring(1);
                return true;
            }

            name = arg.Substring(1, nameEndMarkerIndex - 1);
            if (nameEndMarkerIndex + 1 < arg.Length)
            {
                value = arg.Substring(nameEndMarkerIndex + 1);
            }
            return true;
        }

        private static bool IsNamedParameter(string arg)
        {
            return arg.StartsWith("/") || arg.StartsWith("-");
        }

    }
}
