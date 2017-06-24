using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)
using Xamasoft.JsonClassGenerator;
using System.IO;
using Newtonsoft.Json;

namespace JsonClassGen
{
    [Verb("batch", HelpText="Use a config file for batch generation.")]
    class BatchOptions
    {
        [Option('f', "file", Required = true, HelpText = "The config file.")]
        public string ConfigFile { get; set; }

        //[HelpOption]
        //public string GetUsage()
        //{
        //    return HelpText.AutoBuild(this).ToString();
        //}
    }

    [Verb("generate", HelpText="Generate from a single JSON file")]
    class SingleOptions
    {
        [Option('f', "filename", Required = true, SetName = "single", HelpText = "Input JSON file to read.")]
        public string InputJsonFile { get; set; }

        [Option('t', "targetfolder", Required = false, SetName = "single", HelpText = "Target output folder.")]
        public string TargetFolder { get; set; }

        [Option('n', "namespace", Required = true, SetName = "single", HelpText = "Target Namespace.")]
        public string Namespace { get; set; }

        [Option('p', "useproperties", Default = true, SetName = "single", Required = false, HelpText = "Use Properties.")]
        public bool UseProperties { get; set; }

        [Option('i', "internal", Default = false, SetName = "single", Required = false, HelpText = "Internal visibility only.")]
        public bool InternalVisibility { get; set; }
        [Option('x', "explicit", Default = false, SetName = "single", Required = false, HelpText = "Explicit deserialization.")]
        public bool ExplicitDeserialization { get; set; }
        [Option('h', "nohelp", Default = false, SetName = "single", Required = false, HelpText = "No helper class.")]
        public bool NoHelperClass { get; set; }
        [Option('m', "mainclass", Required = false, HelpText = "Main class.")]
        public string MainClass { get; set; }
        [Option("pascal", Default = false, SetName = "single", Required = false, HelpText = "Pascal Case.")]
        public bool UsePascalCase { get; set; }
        [Option("nested", Default = false, SetName = "single", Required = false, HelpText = "Nested classes.")]
        public bool UseNestedClasses { get; set; }
        [Option('o', "obfuscation", Default = false, SetName = "single", Required = false, HelpText = "Apply obfuscation attributes.")]
        public bool ApplyObfuscationAttributes { get; set; }
        [Option('s', "singlefile", Default = false, SetName = "single", Required = false, HelpText = "Single file.")]
        public bool SingleFile { get; set; }
        //public ICodeWriter CodeWriter { get; set; }
        //public TextWriter OutputStream { get; set; }
        [Option("nullable", Default = false, SetName = "single", Required = false, HelpText = "Always use nullable values.")]
        public bool AlwaysUseNullableValues { get; set; }
        [Option("examples", Default = false, SetName = "single", Required = false, HelpText = "Examples in documentation.")]
        public bool ExamplesInDocumentation { get; set; }

        //private PluralizationService pluralizationService = PluralizationService.CreateService(new CultureInfo("en-us"));

        //private bool used = false;

        //[Option('i', "input", Required = true, HelpText = "Input file to read.")]
        //public string InputFile { get; set; }
        
        [Option('v', "verbose", HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

       
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            var options = new SingleOptions();
            CommandLine.Parser.Default.ParseArguments<BatchOptions, SingleOptions>(args)
            .WithParsed<SingleOptions>(o =>
            {
                GenerateSingle(o);
            })
            .WithParsed<BatchOptions>(batchOption =>
            {
                GenerateBatch(batchOption);
            });
        }

        private static void GenerateBatch(BatchOptions o)
        {
            if (File.Exists(o.ConfigFile))
            {
                var configJson = File.OpenText(o.ConfigFile).ReadToEnd();
                var batch = JsonConvert.DeserializeObject<JsonClassGenConfig>(configJson);
                
                foreach (var config in batch.Classes)
                {
                    var fi = new FileInfo(o.ConfigFile);
                    config.Filename = fi.Directory.FullName + Path.DirectorySeparatorChar + config.Filename;
                    if (File.Exists(config.Filename))
                    {
                        var json = File.OpenText(config.Filename).ReadToEnd();
                        var info = new FileInfo(config.Filename);
                        var targetFolder = info.Directory.FullName;

                        JsonClassGenerator gen = new JsonClassGenerator()
                        {
                            Namespace = config.Namespace,
                            Example = json,
                            InternalVisibility = batch.Options.Internalvisibility,
                            SingleFile = batch.Options.Singlefile,
                            UsePascalCase = batch.Options.Pascal,
                            UseProperties = batch.Options.Useproperties,
                            TargetFolder = targetFolder,
                            MainClass = config.Mainclass,
                            UseNestedClasses = batch.Options.Nested
                        };
                        Console.WriteLine("Generating: " + config.Namespace + "." + config.Mainclass);
                        gen.GenerateClasses();
                    }
                }
            }
        }

        public static void GenerateSingle(SingleOptions options)
        {
            var json = "";
            if(File.Exists(options.InputJsonFile))
            {
                json = File.OpenText(options.InputJsonFile).ReadToEnd();
            }
            else
            {
                Console.WriteLine(String.Format("Cannot find input file '{0}'", options.InputJsonFile));
            }
            var info = new FileInfo(options.InputJsonFile);

            var targetFolder = info.Directory.FullName;
            if(options.TargetFolder != null)
            {
                targetFolder = options.TargetFolder;
            }

            JsonClassGenerator gen = new JsonClassGenerator()
            {
                Namespace = options.Namespace,
                Example = json,
                InternalVisibility = options.InternalVisibility,
                SingleFile = options.SingleFile,
                UsePascalCase = options.UsePascalCase,
                NoHelperClass = true,
                UseProperties = options.UseProperties,
                TargetFolder = targetFolder,
                MainClass = options.MainClass,
                UseNestedClasses = options.UseNestedClasses
            };
            Console.WriteLine("Generating: " + options.Namespace + "." + options.MainClass);
            gen.GenerateClasses();
        }
    }
}
