using Microsoft.VisualStudio.TestPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;
using custom;

namespace HtmlTransformer
{
   static  class HtmlTransformer
    {
        /// <summary>
        /// Embedded Resource name
        /// </summary>
        private const string XSLT_FILE = "Trxer.xslt";


        /// <summary>
        /// Main entry of TrxerConsole
        /// </summary>
        /// <param name="args">First cell shoud be TRX path</param>
       public static void Init(string fileTo, string filename)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);
            Console.WriteLine("Trx File\n{0}", filename);
            Transform(fileTo, xmldoc, PrepareXsl());
        }
        public static void Init(string fileTo, XmlDocument xmldoc)
        {
                
            Transform(fileTo, xmldoc, PrepareXsl());
        }

        /// <summary>
        /// Transforms trx int html document using xslt
        /// </summary>
        /// <param name="fileName">Trx file path</param>
        /// <param name="xsl">Xsl document</param>
        private static void Transform(string fileTo, XmlDocument xmldoc, XmlDocument xsl)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
            };
            XsltArgumentList xslArg = new XsltArgumentList();
        
            XslCompiledTransform x = new XslCompiledTransform(true);

            TrxerFunctions obj = new TrxerFunctions();

           

            x.Load(xsl, new XsltSettings(true, true), null);
            Console.WriteLine("Transforming...");
            xslArg.AddExtensionObject("urn:price-conv", obj);
            settings.Reset();
            using (StreamWriter writer = File.CreateText(Path.Join( fileTo, @"output.html")))
            {
                x.Transform(xmldoc, xslArg, writer);
            }
        }

        /// <summary>
        /// Loads xslt form embedded resource
        /// </summary>
        /// <returns>Xsl document</returns>
        private static XmlDocument PrepareXsl()
        {
            XmlDocument xslDoc = new XmlDocument();
            Console.WriteLine("Loading xslt template...");
            xslDoc.Load(ResourceReader.StreamFromResource(XSLT_FILE));
            MergeCss(xslDoc);
            MergeJavaScript(xslDoc);
            return xslDoc;
        }

        /// <summary>
        /// Merges all javascript linked to page into Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeJavaScript(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading javascript...");
            XmlNode scriptEl = xslDoc.GetElementsByTagName("script")[0]!;
            XmlAttribute scriptSrc = scriptEl.Attributes["src"]!;
            string script = ResourceReader.LoadTextFromResource(scriptSrc.Value);
            scriptEl.Attributes.Remove(scriptSrc);
            scriptEl.InnerText = script;
        
        }

        /// <summary>
        /// Merges all css linked to page ito Trxer html report itself
        /// </summary>
        /// <param name="xslDoc">Xsl document</param>
        private static void MergeCss(XmlDocument xslDoc)
        {
            Console.WriteLine("Loading css...");
            XmlNode headNode = xslDoc.GetElementsByTagName("head")[0]!;
            XmlNodeList linkNodes = xslDoc.GetElementsByTagName("link");
            List<XmlNode> toChangeList = linkNodes.Cast<XmlNode>().ToList();

            foreach (XmlNode xmlElement in toChangeList)
            {
                XmlElement styleEl = xslDoc.CreateElement("style");
                styleEl.InnerText = ResourceReader.LoadTextFromResource(xmlElement.Attributes["href"]!.Value);
                headNode.ReplaceChild(styleEl, xmlElement);
            }
        }
    }
}
