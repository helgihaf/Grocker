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

        public string DetachPositional()
        {
            string result = null;
            if (positionals.Count > 0)
            {
                result = positionals[0];
                positionals.RemoveAt(0);
            }
            return result;
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


        public bool DetachNamedBool(string name)
        {
            bool result = false;
            string value;
            if (named.TryGetValue(name, out value))
            {
                named.Remove(name);
                result = true;
                if (value != null)
                {
                    throw new ArgumentException(string.Format("Option {0} is a flag and cannot have a value", name));
                }
            }
            return result;
        }


        private void DoParse(string[] args)
        {
            // Named parameters start with a '/' or '-', example: "/d"
            // First N args that are not named parameters are Positionals
            // After the first named parameter is found, every arg that is not named is an arguments to the previous named parameter

            string lastNamedParameter = null;
            foreach (var arg in args)
            {
                if (IsNamedParameter(arg))
                {
                    if (lastNamedParameter != null)
                    {
                        named.Add(lastNamedParameter, null);
                    }
                    lastNamedParameter = StripNameMarker(arg);
                }
                else
                {
                    // Either a positional or an argument to the previous named
                    if (lastNamedParameter == null)
                    {
                        if (named.Count == 0)
                        {
                            positionals.Add(arg);
                        }
                        else
                        {
                            throw new ArgumentException("Invalid argument syntax");
                        }
                    }
                    else
                    {
                        // Argument to the previous
                        named.Add(lastNamedParameter, arg);
                        lastNamedParameter = null;
                    }
                }
            }

            if (lastNamedParameter != null)
            {
                named.Add(lastNamedParameter, null);
            }
        }

        private static string StripNameMarker(string arg)
        {
            return arg.Substring(1);
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
            return arg.Length >= 2 && (arg[0] == '/' || arg[0] == '-') && (char.IsLetterOrDigit(arg[1]) || char.IsPunctuation(arg[1]));
        }

    }
}
