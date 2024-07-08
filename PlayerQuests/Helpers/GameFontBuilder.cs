using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;

namespace PlayerQuests.Helpers;

public class GameFontBuilder
{
    private static readonly string Ranges = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a\u008b\u008c\u008d\u008e\u008f\u0090\u0091\u0092\u0093\u0094\u0095\u0096\u0097\u0098\u0099\u009a\u009b\u009c\u009d\u009e\u009f\u00a0¡\u00a2\u00a3\u00a4\u00a5\u00a6§\u00a8\u00a9ª«\u00ac\u00ad\u00ae\u00af\u00b0\u00b1\u00b2\u00b3\u00b4µ¶·\u00b8\u00b9º»\u00bc\u00bd\u00be¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ\u00d7ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö\u00f7øùúûüýþ";

    static GameFontBuilder()
    {
        IEnumerable<int> ranges =
        [
            .. Enumerable.Range(0x3000, 0x30FF), // CJK Symbols and Punctuations, Hiragana, Katakana
            .. Enumerable.Range(0x31F0, 0x31FF), // Katakana Phonetic Extension
            .. Enumerable.Range(0xFF00, 0xFFeF), // Half Width characters
            0xFFFD                               // Invalid
        ];
        var japaneseRanges = ranges.Select(c => (char)c);
        Ranges += string.Concat(japaneseRanges);

        var resourcePath = typeof(GameFontBuilder).Assembly.GetManifestResourceNames().FirstOrDefault(name => name.Contains("regular_use+personal_name_utf8"));
        if (resourcePath != null)
        {
            using var stream = typeof(GameFontBuilder).Assembly.GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                Ranges += reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Usage of the IFontHandle (stored as FontHandle here):
    /// <code>
    /// IDisposable? fontDisposer = null;
    /// if (FontHandle.Available)
    /// {
    ///     fontDisposer = FontHandle.Push();
    /// }
    /// // ImGui text goes here
    /// fontDisposer?.Dispose();
    /// </code>
    /// </summary>
    /// <param name="font"></param>
    /// <returns></returns>
    public static IFontHandle GetFont(GameFontFamilyAndSize font)
    {
        return Plugin.PluginInterface.UiBuilder.FontAtlas.NewDelegateFontHandle(x =>
        {
            x.OnPreBuild(tk =>
            {
                var gameFontStyle = new GameFontStyle(font);
                tk.AddGameGlyphs(gameFontStyle, Ranges.ToGlyphRange(), default);
            });
        });
    }
}
