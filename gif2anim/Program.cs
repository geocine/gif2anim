using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Gif2Anim;

namespace Gif2Anim
{

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path to the animated GIF to be converted")]
        public string InputFile { get; set; }

        [Option('o', DefaultValue = "res", HelpText = "The output folder, default is 'res'.")]
        public string OutputFolder { get; set; }

        [Option('d', DefaultValue = "mdpi", HelpText = "Android density indicator (e.g., mdpi, hdpi, xhdpi, xxhdpi)")]
        public string Density { get; set; }

        [Option('s', "oneshot", DefaultValue = false, HelpText = "Adds the android:oneshot=\"true\" attribute to the AnimationDrawable")]
        public bool OneShot { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("gif2anim", "1.0"),
                Copyright = new CopyrightInfo("Aivan Monceller", 2013),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Convert animated GIFs to AnimationDrawables");
            help.AddPreOptionsLine("Usage: gif2anim -i file.gif");
            help.AddOptions(this);
            return help;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {

                ImageManager imageManager = ImageManager.ProcessImage(options);

                XMLManager xmlManager = new XMLManager();
                xmlManager.Option = options;
                xmlManager.ImageManager = imageManager;
                xmlManager.GenerateXML();

            }
        }
    }
}
