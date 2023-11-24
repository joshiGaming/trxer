
using System.IO;
using System.Xml;
using System.Xml.Linq;
using HtmlTransformer;
using XmlConverter;
namespace MergeXML
{
   public enum SortDateType
    {
        High,
        Low,
    }

   public static class XmlMerger
    {
       public static XDocument MergingTwo(XDocument xml1, XDocument xml2)
        {
            //Replacing Catergories
            ReplaceFor(xml1, xml2, "Results");         
            ReplaceFor(xml1, xml2, "TestDefinitions");      
            ReplaceFor(xml1, xml2, "TestEntries");  
            //Sorting Dates
            SortDate(xml1, xml2, "start", SortDateType.Low);
            SortDate(xml1, xml2, "creation", SortDateType.Low);
            SortDate(xml1, xml2, "queuing", SortDateType.High);
            SortDate(xml1, xml2, "finish", SortDateType.High);
            CombineAttributes(xml1, xml2);

            return xml1;

        }
      public static void CombineAttributes(XDocument xml1, XDocument xml2)
        {
            XNamespace nameSpace = xml1.Root!.GetDefaultNamespace().NamespaceName;
            foreach (XAttribute attribute in xml1.Descendants(nameSpace + "Counters").First().Attributes())
            {
                XAttribute xml2Attribute = xml2.Descendants(nameSpace + "Counters").First().Attribute(attribute.Name)!;
                int value1 = Int32.Parse(attribute.Value);
                int value2 = Int32.Parse(xml2Attribute.Value);
                attribute.SetValue((value1 + value2).ToString());
                
            }
        }
       public static void SortDate(XDocument xml1, XDocument xml2, string attributeXname, SortDateType sortDateType)
        {
            XNamespace nameSpace = xml1.Root!.GetDefaultNamespace().NamespaceName;
            XElement element = xml1.Descendants(nameSpace + "Times").First();
            XElement element2 = xml2.Descendants(nameSpace + "Times").First();

            int result = DateTime.Compare(DateTime.Parse(element.Attribute(attributeXname)!.Value), DateTime.Parse(element2.Attribute(attributeXname)!.Value));
            if(sortDateType == SortDateType.High)
            {
                if (result < 0)
                {
                    element.SetAttributeValue(attributeXname, element2.Attribute(attributeXname)!.Value);
                }
            }else
            {
                if (result > 0)
                {
                    element.SetAttributeValue(attributeXname, element2.Attribute(attributeXname)!.Value);
                }
            }
        }
      public static void ReplaceFor(XDocument xml1, XDocument xml2, XName name)
        {
            foreach (var element in xml1.Root!.Descendants())
            {
                if (element.Name.LocalName != name) continue;

                    XElement resultsfromsec = xml2.Descendants().First((XElement element2) => element2.Name.LocalName == name);
                    element.Add(resultsfromsec.Elements());
                    return;             
            }
        }

        static XDocument Merge(string pathfrom)
        {
            List<string> files = new List<string>();
            XDocument xml1;
            XDocument xml2;
       
            Console.WriteLine("Validating...");

            //Validating
            foreach (var trxFilePath in Directory.GetFiles(pathfrom, "*.trx", SearchOption.TopDirectoryOnly))
            {          
                    try
                    {
                      
                        XDocument.Load(trxFilePath);
                        files.Add(trxFilePath);
                        
                    }
                    catch 
                    {
                        Console.WriteLine(trxFilePath + " | is not Valid");

                    }             
              
            }

            if(files.Count < 1 ) {
                Console.Error.WriteLine("Not Enough valid files Found at " + Path.GetFullPath( pathfrom));
                System.Environment.Exit(-1);
              
            }

            xml1 = XDocument.Load(files[0]);

            //Mergen
            if(files.Count > 2){
               for (var i = 1; files.Count > i; i++)
               {
                    var file = files[i];

                 Console.WriteLine("Merge: " + file);

                    xml2 = XDocument.Load(file);

                    xml1 = MergingTwo(xml1, xml2);

                }
            }
        
          
            XNamespace nameSpace = xml1.Root!.GetDefaultNamespace().NamespaceName;
            XElement counter = xml1.Descendants(nameSpace + "Counters").First();
            Console.WriteLine("===========================================================================");
            Console.WriteLine("RESULT: "
             + counter.Attribute("total")! .Value + " Total, "
             + counter.Attribute("failed")! .Value + " Failed, "
             + counter.Attribute("passed")! .Value + " Passed, "
             + counter.Attribute("executed")!.Value + " Executed, ");
             Console.WriteLine("===========================================================================");
           
            return xml1;

        }
        public static void GetTestResults(string pathfrom) {
            XDocument xml1 = XDocument.Load(pathfrom);
            XNamespace nameSpace = xml1.Root!.GetDefaultNamespace().NamespaceName;
            XElement results = xml1.Root.Element(nameSpace + "Results")!;
            List<string> classnames = new List<string>();

            XElement testdefinitions = xml1.Root!.Element(nameSpace + "TestDefinitions")!;

          foreach (var unitTest in testdefinitions.Elements())
            {

               string classname = unitTest.Element(nameSpace + "TestMethod")!.Attribute("className")!.Value;
                if (!classnames.Contains(classname)) {
                    classnames.Add(classname);
     
                    Console.WriteLine("\n ===" + classname+"=== \n");
   
                    foreach ( XElement innerUnitTest in testdefinitions.Elements() ){

                       if( classname == innerUnitTest.Element(nameSpace + "TestMethod")!.Attribute("className")!.Value)
                        {              
                         
                           
                            GetInResults(results, innerUnitTest, nameSpace);
                            Console.ResetColor();
                           
                                        
                        Console.Write("\n");
                            continue;
                            
                        }

                    }

                }
            }
            XElement counter = xml1.Descendants(nameSpace + "Counters").First();
            Console.WriteLine("===========================================================================");
            Console.WriteLine("RESULT: "
             + counter.Attribute("total")!.Value + " Total, "
             + counter.Attribute("failed")!.Value + " Failed, "
             + counter.Attribute("passed")!.Value + " Passed, "
             + counter.Attribute("executed")!.Value + " Executed, ");
            Console.WriteLine("===========================================================================");


        }

        public static void GetInResults(XElement results, XElement innerUnitTest, XNamespace nameSpace )
        {
            foreach(var unitTestResult in results.Elements() ) {
               if( unitTestResult.Attribute("testId")!.Value == innerUnitTest.Attribute("id")!.Value)
                {
                    string outcome = unitTestResult.Attribute("outcome")!.Value;
                    Console.ForegroundColor = outcome == "Passed" ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.Write(String.Format(" [{0}]", outcome));
                    Console.ResetColor();
                    Console.Write( innerUnitTest.Attribute("name")!.Value + " ");

                    if (outcome == "Failed")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("\n");

                        Console.Write("  || " + unitTestResult.Element(nameSpace + "Output")!.Element(nameSpace + "ErrorInfo")!.Element(nameSpace + "Message")!.Value) ;
                        Console.Write("\n");
                        Console.Write("  >>  " + unitTestResult.Element(nameSpace + "Output")!.Element(nameSpace + "ErrorInfo")!.Element(nameSpace + "StackTrace")!.Value);

                    }
                    break;
                }
            
            }
          
        }

        static int Main(string[] args)
        {
            DateTime timestamp = DateTime.Now;
            if (args.Length <2) {
                PrintHelp();
                return 0;
            }

            
            string command =  args[0] ;
            string pathto = args[1];
            string pathfrom = args.ElementAtOrDefault(2);

           if(command == "print")
            {
                if (!File.Exists(pathto))
                {
                    Console.Error.WriteLine("File (2) does not exist...");
                    return -1;
                }

                GetTestResults(pathto);
                
            }


           if(command == "merge")
            {
                if (!Directory.Exists(pathto))
                {
                    Console.Error.WriteLine("PathTo (1) does not exist...");
                    return -1;
                }
                if (!Directory.Exists(pathfrom))
                {
                    Console.Error.WriteLine("PathFrom (2) does not exist...");
                    return -1;
                }

              XDocument xdoc =  Merge(pathfrom);

                foreach (string arg in args)
                {
                    if (arg == "-transform")
                    {
                      


                        XmlDocument xmldoc = DocumentExtensions.ToXmlDocument(xdoc);
                        HtmlTransformer.HtmlTransformer.Init(pathto, xmldoc);
                    
                    }
                }

                Console.WriteLine("Writing Xml ==========================> " + pathto);
                xdoc.Save(Path.Join(pathto, @"output.trx"));

            }

            Console.WriteLine("DONE: finished in: " + DateTime.Now.Subtract(timestamp).TotalSeconds + " Seconds!");


            return 0;
       
        }

      public static void PrintHelp() {

            Console.WriteLine("XmlMerge.exe merge <INPUT_DIRECTORY> <OUTPUT_DIRECTORY> [-transform]");
            Console.WriteLine("XmlMerge.exe print <FILE>");
        }

    }
}


