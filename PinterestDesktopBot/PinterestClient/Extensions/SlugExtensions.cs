using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinterestDesktopBot.PinterestClient.Extensions
{
    internal static class SlugExtensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }
        
        public static string ToSlug(this string title, bool remapToAscii = true, int maxlength = 200)
        {
            if (title == null)
            {
                return string.Empty;
            }

            var length = title.Length;
            var prevdash = false;
            var stringBuilder = new StringBuilder(length);

            for (var i = 0; i < length; ++i)
            {
                var c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    stringBuilder.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lower-case
                    stringBuilder.Append((char) (c | 32));
                    prevdash = false;
                }
                else if ((c == ' ') || (c == ',') || (c == '.') || (c == '/') ||
                         (c == '\\') || (c == '-') || (c == '_') || (c == '='))
                {
                    if (!prevdash && (stringBuilder.Length > 0))
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    var previousLength = stringBuilder.Length;

                    if (remapToAscii)
                    {
                        stringBuilder.Append(RemapInternationalCharToAscii(c));
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }

                    if (previousLength != stringBuilder.Length)
                    {
                        prevdash = false;
                    }
                }

                if (i == maxlength)
                {
                    break;
                }
            }

            return prevdash
                ? stringBuilder.ToString().Substring(0, stringBuilder.Length - 1)
                : stringBuilder.ToString();
        }

        private static string RemapInternationalCharToAscii(char character)
        {
            var s = character.ToString().ToLowerInvariant();
            
            if ("àåáâäãåąā".Contains(s))
            {
                return "a";
            }

            if ("èéêëę".Contains(s))
            {
                return "e";
            }

            if ("ìíîïıİ".Contains(s))
            {
                return "i";
            }

            if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }

            if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }

            if ("çćčĉ".Contains(s))
            {
                return "c";
            }

            if ("żźž".Contains(s))
            {
                return "z";
            }

            if ("śşšŝ".Contains(s))
            {
                return "s";
            }

            if ("ñń".Contains(s))
            {
                return "n";
            }

            if ("ýÿ".Contains(s))
            {
                return "y";
            }

            if ("ğĝ".Contains(s))
            {
                return "g";
            }

            if (character == 'ř')
            {
                return "r";
            }

            if (character == 'ł')
            {
                return "l";
            }

            if (character == 'đ')
            {
                return "d";
            }

            if (character == 'ß')
            {
                return "ss";
            }

            if (character == 'Þ')
            {
                return "th";
            }

            if (character == 'ĥ')
            {
                return "h";
            }

            if (character == 'ĵ')
            {
                return "j";
            }

            if (character == 'ə')
            {
                return "e";
            }

            return string.Empty;
        }
    }
}