using System.Text.RegularExpressions;
using KeePass.Plugins;
using KeePass.Util;
using KeePass.Util.Spr;

namespace ClipboardPlaceholder
{
    public sealed class ClipboardPlaceholderExt : Plugin
    {
        private const string mPlaceholder = "{Clipboard}";

        private IPluginHost m_host = null;

        public override string UpdateUrl
        {
            get { return "https://raw.githubusercontent.com/Angelelz/ClipboardPlaceholder/master/keepass.version"; }
        }

        public override bool Initialize(IPluginHost host)
        {
            m_host = host;
            AutoType.FilterCompilePre += OnAutoTypeFilterCompilePre;
            SprEngine.FilterPlaceholderHints.Add(mPlaceholder);
            return true;
        }

        public override void Terminate()
        {
            SprEngine.FilterPlaceholderHints.Remove(mPlaceholder);
            AutoType.FilterCompilePre -= OnAutoTypeFilterCompilePre;
        }

        private void OnAutoTypeFilterCompilePre(object sender, AutoTypeEventArgs autoTypeEventArgs)
        {
            Regex replacer = new Regex(Regex.Escape(mPlaceholder), RegexOptions.IgnoreCase);

            autoTypeEventArgs.Sequence = replacer.Replace(autoTypeEventArgs.Sequence, match =>
            {
                var clip = ClipboardUtil.GetText();
                return BracketReplace(clip);
            });
        }

        private static string BracketReplace(string str)
        {
            string nstr = "";
            for(int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '{' || str[i] == '}' || str[i] == '\u0009' || str[i] == '\u2386')
                {
                    switch (str[i])
                    {
                        case '{':
                            nstr = nstr + "{{}";
                            break;
                        case '}':
                            nstr = nstr + "{}}";
                            break;
                        case '\u0009':
                            nstr = nstr + '\u0009';
                            break;
                        case '\u2386':
                            nstr = nstr + '\u2386';
                            break;
                    }
                }
                else nstr = nstr + str[i];

            }
            return nstr.Replace("[", "{[}").Replace("]", "{]}")
                       .Replace("(", "{(}").Replace(")", "{)}")
                       .Replace("~", "{~}").Replace("^", "{^}")
                       .Replace("%", "{%}").Replace("+", "{+}");
        }

    }
}
