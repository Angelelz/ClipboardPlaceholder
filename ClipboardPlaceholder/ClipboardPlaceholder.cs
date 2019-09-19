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
                return clip;
            });
        }

    }
}
