using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.UnitTests
{
    static class TestLineCreator
    {
        private class CharEntry : IComparable<CharEntry>
        {
            public CharEntry(char c, int sumIndex)
            {
                Char = c;
                SumIndex = sumIndex;
            }

            public char Char;
            public int SumIndex;

            public int CompareTo(CharEntry other)
            {
                if (other != null)
                {
                    if (SumIndex < other.SumIndex)
                        return -1;
                    if (SumIndex > other.SumIndex)
                        return 1;
                    return 0;
                }
                return -1;
            }
        }

        private static readonly CharEntry[] charEntries = new[]
            {
                new CharEntry('\t', 0),
                new CharEntry(' ', 2854),
                new CharEntry('!', 21318157),
                new CharEntry('"', 21416262),
                new CharEntry('#', 21416712),
                new CharEntry('$', 21417871),
                new CharEntry('%', 21418485),
                new CharEntry('&', 21426948),
                new CharEntry('\'', 21817904),
                new CharEntry('(', 21818188),
                new CharEntry(')', 22036010),
                new CharEntry('+', 22253832),
                new CharEntry(',', 22271942),
                new CharEntry('-', 24212040),
                new CharEntry('.', 29577295),
                new CharEntry('/', 63057040),
                new CharEntry('0', 63636149),
                new CharEntry('1', 154593010),
                new CharEntry('2', 208900394),
                new CharEntry('3', 240935720),
                new CharEntry('4', 252740352),
                new CharEntry('5', 265259415),
                new CharEntry('6', 290285066),
                new CharEntry('7', 308284208),
                new CharEntry('8', 317816252),
                new CharEntry('9', 331175864),
                new CharEntry(':', 342728731),
                new CharEntry(';', 382238634),
                new CharEntry('<', 382240548),
                new CharEntry('=', 382240833),
                new CharEntry('>', 389491652),
                new CharEntry('?', 396158096),
                new CharEntry('A', 396158104),
                new CharEntry('B', 400451880),
                new CharEntry('C', 409888861),
                new CharEntry('D', 429011633),
                new CharEntry('E', 441851600),
                new CharEntry('F', 459218786),
                new CharEntry('G', 462969141),
                new CharEntry('H', 463571195),
                new CharEntry('I', 465678565),
                new CharEntry('J', 491079144),
                new CharEntry('K', 491153065),
                new CharEntry('L', 491508592),
                new CharEntry('M', 510283023),
                new CharEntry('N', 512087933),
                new CharEntry('O', 518775710),
                new CharEntry('P', 521312557),
                new CharEntry('Q', 523145149),
                new CharEntry('R', 523145229),
                new CharEntry('S', 549122477),
                new CharEntry('T', 581779437),
                new CharEntry('U', 598752175),
                new CharEntry('V', 598937901),
                new CharEntry('W', 601622365),
                new CharEntry('X', 601632663),
                new CharEntry('Y', 602209885),
                new CharEntry('Z', 608421135),
                new CharEntry('[', 608559080),
                new CharEntry('\\', 608576704),
                new CharEntry(']', 678847821),
                new CharEntry('_', 678865445),
                new CharEntry('`', 680958286),
                new CharEntry('a', 680958416),
                new CharEntry('b', 716036112),
                new CharEntry('c', 717209509),
                new CharEntry('d', 743149074),
                new CharEntry('e', 751706803),
                new CharEntry('f', 860474777),
                new CharEntry('g', 865304924),
                new CharEntry('h', 869119078),
                new CharEntry('i', 870452602),
                new CharEntry('j', 907191668),
                new CharEntry('k', 907599184),
                new CharEntry('l', 923286914),
                new CharEntry('m', 942870727),
                new CharEntry('n', 954082428),
                new CharEntry('o', 983628630),
                new CharEntry('p', 1021558509),
                new CharEntry('q', 1041085548),
                new CharEntry('r', 1046088683),
                new CharEntry('s', 1111793299),
                new CharEntry('t', 1167012310),
                new CharEntry('u', 1200325596),
                new CharEntry('v', 1225343117),
                new CharEntry('w', 1241192707),
                new CharEntry('x', 1243449359),
                new CharEntry('y', 1246531710),
                new CharEntry('z', 1250356648),
                new CharEntry('{', 1250840155),
                new CharEntry('|', 1250840587),
                new CharEntry('}', 1290491295),
                new CharEntry('~', 1290491727),
            };
        private const int CharEntryLength = 1292042883;

        public static string Create(Random random, int lineLength)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < lineLength; i++)
            {
                sb.Append(GetChar(random));
            }

            return sb.ToString().Trim();
        }

        private static char GetChar(Random random)
        {
            int index = random.Next(CharEntryLength);
            return GetCharAt(index);
        }

        private static char GetCharAt(int index)
        {
            int foundIndex = Array.BinarySearch<CharEntry>(charEntries, new CharEntry(' ', index));
            if (foundIndex < 0)
            {
                foundIndex = ~foundIndex;
                return charEntries[foundIndex - 1].Char;
            }
            return charEntries[foundIndex].Char;
        }
    }
}
