using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gif2Anim
{
    public class XMLManager
    {

        public Options Option { get; set; }
        public ImageManager ImageManager { get; set; }

        public void GenerateXML()
        {

            String oneShot = Option.OneShot ? "true" : "false";

            StringBuilder xmlBuilder = new StringBuilder();
            xmlBuilder.Append("<animation-list xmlns:android=\"http://schemas.android.com/apk/res/android\" android:oneshot=\"" + oneShot + "\">");
            foreach (ImageParts imageParts in ImageManager.ImageParts)
            {
                xmlBuilder.Append("<item android:drawable=\"@drawable/" + imageParts.Drawable + "\" android:duration=\"" + imageParts.Duration.ToString() + "\"/>");
            }
            xmlBuilder.Append("</animation-list>");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlBuilder.ToString());

            String drawablePath = Path.Combine(Option.OutputFolder, "drawable");

            if (!Directory.Exists(drawablePath))
            {
                Directory.CreateDirectory(drawablePath);
            }

            // Save the document to a file and auto-indent the output.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(Path.Combine(drawablePath, ImageManager.OutputFileName + ".xml"), settings))
            {
                doc.Save(xmlWriter);
            }
        }
    }
}
