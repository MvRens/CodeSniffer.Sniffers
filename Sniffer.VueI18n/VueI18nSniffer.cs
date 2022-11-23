using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CodeSniffer.Core.Sniffer;
using Serilog;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sniffer.VueI18n
{
    public class VueI18NIncompatibleComponentException : Exception
    {
        public VueI18NIncompatibleComponentException(string message) : base(message)
        {
        }
    }


    public class VueI18NSniffer : ICsSniffer
    {
        private readonly ILogger logger;
        private readonly VueI18NOptions options;


        public VueI18NSniffer(ILogger logger, VueI18NOptions options)
        {
            this.logger = logger;
            this.options = options;
        }


        public async ValueTask<ICsReport?> Execute(string path, ICsScanContext context, CancellationToken cancellationToken)
        {
            var builder = CsReportBuilder.Create();

            foreach (var vueComponentFile in Directory.EnumerateFiles(path, "*.vue", SearchOption.AllDirectories))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var relativeFilePath = Path.GetRelativePath(path, vueComponentFile);
                if (Path.DirectorySeparatorChar != '\\')
                    relativeFilePath = relativeFilePath.Replace(Path.DirectorySeparatorChar, '\\');

                try
                {
                    var section = await ExtractVueI18NSection(vueComponentFile, cancellationToken);
                    if (section == null)
                        continue;

                    var keys = GetKeysFromYaml(section.Contents);
                    CheckLocales(builder.AddAsset(relativeFilePath, relativeFilePath), keys);
                }
                catch (VueI18NIncompatibleComponentException e)
                {
                    builder
                        .AddAsset(relativeFilePath, relativeFilePath)
                        .SetResult(CsReportResult.Skipped, string.Format(Strings.ResultSkippedIncompatible, e.Message));
                }
                catch (Exception e)
                {
                    builder
                        .AddAsset(relativeFilePath, relativeFilePath)
                        .SetResult(CsReportResult.Error, e.Message);
                }
            }

            return builder.Build();
        }


        private static void CheckLocales(CsReportBuilder.Asset asset, IReadOnlyDictionary<string, IReadOnlyList<string>> locales)
        {
            var allKeys = locales.SelectMany(p => p.Value).Distinct().ToArray();

            foreach (var locale in locales)
            {
                var missingKeys = allKeys.Except(locale.Value).ToArray();
                if (missingKeys.Length == 0)
                    continue;

                asset
                    .SetResultIfHigher(CsReportResult.Warning, Strings.ResultMissingKeys)
                    .AddToOutput(string.Format(Strings.ResultMissingKeysOutput,
                        locale.Key,
                        string.Join(", ", missingKeys)));
            }
        }


        private static readonly Regex VueI18NSectionRegex = new("<i18n(?<args>.*?)>(?<contents>.+?)</i18n>", RegexOptions.Singleline);


        private static async Task<VueI18NSection?> ExtractVueI18NSection(string filename, CancellationToken cancellationToken)
        {
            var fileContents = await File.ReadAllTextAsync(filename, cancellationToken);
            var matches = VueI18NSectionRegex.Matches(fileContents);

            switch (matches.Count)
            {
                case 0:
                    return null;

                case > 1:
                    // TODO add support for multiple i18n sections
                    throw new VueI18NIncompatibleComponentException("Multiple i18n sections are not supported yet by this plugin");
            }

            var match = matches[0];
            if (match.Groups["args"].Value != " lang=\"yaml\"")
                // TODO add support for formats other than YAML
                throw new VueI18NIncompatibleComponentException("Only a single i18n section with 'lang=\"yaml\"' is supported by this plugin");

            return new VueI18NSection(match.Groups["contents"].Value.Trim());
        }


        private static IReadOnlyDictionary<string, IReadOnlyList<string>> GetKeysFromYaml(string contents)
        {
            using var textReader = new StringReader(contents);
            var parser = new Parser(textReader);
            var localesParser = new LocalesParser();

            while (parser.MoveNext() && parser.Current != null)
                parser.Current.Accept(localesParser);

            return localesParser.Locales.ToDictionary(p => p.Key, p => p.Value as IReadOnlyList<string>);
        }


        private class LocalesParser : IParsingEventVisitor
        {
            public Dictionary<string, List<string>> Locales { get; } = new();

            private int currentLevel;
            private List<string>? currentLocaleKeys;
            private readonly Stack<string> currentKey = new();


            public void Visit(AnchorAlias e)
            {
            }

            public void Visit(StreamStart e)
            {
            }

            public void Visit(StreamEnd e)
            {
            }

            public void Visit(DocumentStart e)
            {
            }

            public void Visit(DocumentEnd e)
            {
            }

            public void Visit(Scalar e)
            {
                switch (currentLevel)
                {
                    case 1:
                        if (!e.IsKey)
                            return;

                        currentLocaleKeys = new List<string>();
                        Locales.Add(e.Value, currentLocaleKeys);
                        break;

                    case > 1:
                        if (currentLocaleKeys == null)
                            return;

                        if (e.IsKey)
                        {
                            currentKey.Push(e.Value);
                        }
                        else
                        {
                            currentLocaleKeys.Add(string.Join('.', currentKey.Reverse()));
                            currentKey.Pop();
                        }

                        break;
                }
            }

            public void Visit(SequenceStart e)
            {
            }

            public void Visit(SequenceEnd e)
            {
            }

            public void Visit(MappingStart e)
            {
                currentLevel++;
            }

            public void Visit(MappingEnd e)
            {
                currentLevel--;

                if (currentKey.Count > 0)
                    currentKey.Pop();
            }

            public void Visit(Comment e)
            {
            }
        }


        private class VueI18NSection
        {
            //public string Lang { get; }
            public string Contents { get; }


            public VueI18NSection(/*string lang, */string contents)
            {
                //Lang = lang;
                Contents = contents;
            }
        }
    }
}
