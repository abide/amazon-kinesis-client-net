using CommandLine;
using CommandLine.Text;

namespace Amazon.Kinesis.ClientLibrary.Bootstrap
{
    /// <summary>
    /// Command line options.
    /// </summary>
    internal class Options
    {
        [Option('j', "java", Required = false, HelpText = "Path to java, used to start the KCL multi-lang daemon. Attempts to auto-detect if not specified.")]
        public string JavaLocation { get; set; }

        [Option('p', "properties", Required = true, HelpText = "Path to properties file used to configure the KCL.")]
        public string PropertiesFile { get; set; }

        [Option("jar-folder", Required = false, HelpText = "Folder to place required jars in. Defaults to ./jars")]
        public string JarFolder { get; set; }

        [Option('e', "execute", HelpText = "Actually launch the KCL. If not specified, prints the command used to launch the KCL.")]
        public bool ShouldExecute { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddOptions(this);
            return help;
        }
    }
}