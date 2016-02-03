using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Win32;

namespace Amazon.Kinesis.ClientLibrary.Bootstrap
{
    /// <summary>
    /// The Bootstrap program helps the user download and launch the KCL multi-lang daemon (which is in java).
    /// </summary>
    public static class MainClass
    {
        private static readonly OperatingSystemCategory CURRENT_OS = Environment.OSVersion.ToString().Contains("Unix")
            ? OperatingSystemCategory.UNIX
            : OperatingSystemCategory.WINDOWS;

        private static readonly List<MavenPackage> MAVEN_PACKAGES = new List<MavenPackage>()
        {
            new MavenPackage("com.amazonaws", "amazon-kinesis-client", "1.2.1"),
            new MavenPackage("com.fasterxml.jackson.core", "jackson-core", "2.1.1"),
            new MavenPackage("org.apache.httpcomponents", "httpclient", "4.2"),
            new MavenPackage("org.apache.httpcomponents", "httpcore", "4.2"),
            new MavenPackage("com.fasterxml.jackson.core", "jackson-annotations", "2.1.1"),
            new MavenPackage("commons-codec", "commons-codec", "1.3"),
            new MavenPackage("joda-time", "joda-time", "2.4"),
            new MavenPackage("com.amazonaws", "aws-java-sdk", "1.8.11"),
            new MavenPackage("com.amazonaws", "aws-java-sdk-core", "1.8.11"),
            new MavenPackage("com.fasterxml.jackson.core", "jackson-databind", "2.1.1"),
            new MavenPackage("commons-logging", "commons-logging", "1.1.1"),
        };

        /// <summary>
        /// Downloads all the required jars from Maven and returns a classpath string that includes all those jars.
        /// </summary>
        /// <returns>Classpath string that includes all the jars downloaded.</returns>
        /// <param name="jarFolder">Folder into which to save the jars.</param>
        private static string FetchJars(string jarFolder)
        {
            if (jarFolder == null)
            {
                jarFolder = "jars";
            }
            if (!Path.IsPathRooted(jarFolder))
            {
                jarFolder = Path.Combine(Directory.GetCurrentDirectory(), jarFolder);
            }

            Console.Error.WriteLine("Fetching required jars...");

            foreach (MavenPackage mp in MAVEN_PACKAGES)
            {
                mp.Fetch(jarFolder);
            }
            Console.Error.WriteLine("Done.");

            List<string> files = Directory.GetFiles(jarFolder).Where(f => f.EndsWith(".jar")).ToList();
            files.Add(Directory.GetCurrentDirectory());
            return string.Join(Path.PathSeparator.ToString(), files);
        }

        private static string FindJava(string java)
        {
            // See if "java" is already in path and working.
            if (java == null)
            {
                java = "java";
            }
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = java,
                    Arguments = "-version",
                    UseShellExecute = false
                }
            };
            try
            {
                proc.Start();
                proc.WaitForExit();
                return java;
            }
            catch
            {
            }

            // Failing that, look in the registry.
            foreach (var view in new [] { RegistryView.Registry64, RegistryView.Registry32 })
            { 
                var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                var javaRootKey = localKey.OpenSubKey(@"SOFTWARE\JavaSoft\Java Runtime Environment");
                foreach (var jreKeyName in javaRootKey.GetSubKeyNames())
                {
                    var jreKey = javaRootKey.OpenSubKey(jreKeyName);
                    var javaHome = jreKey.GetValue("JavaHome") as string;
                    var javaExe = Path.Combine(javaHome, "bin", "java.exe");
                    if (File.Exists(javaExe))
                    {
                        return javaExe;
                    }
                }
            }
                
            return null;
        }

        public static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                string javaClassPath = FetchJars(options.JarFolder);
                string java = FindJava(options.JavaLocation);

                if (java == null)
                {
                    Console.Error.WriteLine("java could not be found. You may need to install it, or manually specifiy the path to it.");
                    Environment.Exit(2);
                }

                List<string> cmd = new List<string>()
                {
                    java,
                    "-cp",
                    javaClassPath,
                    "com.amazonaws.services.kinesis.multilang.MultiLangDaemon",
                    options.PropertiesFile
                };
                if (options.ShouldExecute)
                {
                    // Start the KCL.
                    Process proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = cmd[0],
                            Arguments = string.Join(" ", cmd.Skip(1)),
                            UseShellExecute = false
                        }
                    };
                    proc.Start();
                    proc.WaitForExit();
                }
                else
                {
                    // Print out a command that can be used to start the KCL.
                    string c = string.Join(" ", cmd.Select(f => "\"" + f + "\""));
                    if (CURRENT_OS == OperatingSystemCategory.WINDOWS)
                    {
                        c = "& " + c;
                    }
                    Console.WriteLine(c);
                }
            }
        }
    }
}