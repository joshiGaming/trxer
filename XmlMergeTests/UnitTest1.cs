using MergeXML;
using System.Xml.Linq;

namespace XmlMergeTests
{
    [TestClass]
    public class UnitTest1
    {
        // Assert.AreEqual(3, result);
        [TestMethod]
        public void TestSortDate()
        {
            XDocument xml1 = new XDocument(new XElement("TestRun", new XElement("Times", new XAttribute("creation", "2023-10-06T07:25:50.1575903+02:00"))));
            XDocument xml2 = new XDocument(new XElement("TestRun", new XElement("Times", new XAttribute("creation", "2023-10-06T07:30:50.1575903+02:00"))));
            XDocument controlldoc = new XDocument(new XElement("TestRun", new XElement("Times", new XAttribute("creation", "2023-10-06T07:30:50.1575903+02:00"))));

            XmlMerger.SortDate(xml1, xml2, "creation", SortDateType.High);

            Assert.AreEqual(controlldoc.ToString(), xml1.ToString());
            
        }


       [TestMethod]
        public void TestCombineAttributes()
        {
            XDocument xml1 = new XDocument(
                 new XElement("TestRun", new XElement("ResultSummary", new XElement("Counters", new XAttribute("passed", "12"), new XAttribute("total", "0")))));
            XDocument xml2 = new XDocument(
                 new XElement("TestRun", new XElement("ResultSummary", new XElement("Counters", new XAttribute("passed", "8"), new XAttribute("total", "43")))));

            XDocument controllDoc = new XDocument(
                new XElement("TestRun", new XElement("ResultSummary", new XElement("Counters", new XAttribute("passed", "20"), new XAttribute("total", "43")))));


            XmlMerger.CombineAttributes(xml1, xml2);
            Assert.AreEqual(controllDoc.ToString(), xml1.ToString());
        }


        [TestMethod]
        public void TestReplacingResults()
        {

            XDocument xml1 = new XDocument(
                 new XElement("TestRun", new XElement("Results", new XElement("UnitTestResult", "TEST of 1"))));
            XDocument xml2 = new XDocument(
               new XElement("TestRun", new XElement("Results", new XElement("UnitTestResult", "TEST of 2"))));

            XDocument controllDoc = new XDocument(new XElement("TestRun", new XElement("Results", new XElement("UnitTestResult", "TEST of 1"), new XElement("UnitTestResult", "TEST of 2"))));

            XmlMerger.ReplaceFor(xml1, xml2, "Results");
            Assert.AreEqual(controllDoc.ToString(),xml1.ToString());

        }


        [TestMethod]
        public void TestReplacingTestDefinitions()
        {
            XDocument xml1 = new XDocument(
                 new XElement("TestRun", new XElement("TestDefinitions", new XElement("UnitTest", "Execution 1"))));
            XDocument xml2 = new XDocument(
               new XElement("TestRun", new XElement("TestDefinitions", new XElement("UnitTest", "Execution 2"))));

            XDocument controllDoc = new XDocument(new XElement("TestRun", new XElement("TestDefinitions", new XElement("UnitTest", "Execution 1"), new XElement("UnitTest", "Execution 2"))));

            XmlMerger.ReplaceFor(xml1, xml2, "TestDefinitions");
            Assert.AreEqual( controllDoc.ToString(), xml1.ToString());
        }


        [TestMethod]
        public void TestReplacingTestEntries()
        {
            XDocument xml1 = new XDocument(
                 new XElement("TestRun", new XElement("TestEntries", new XElement("TestEntry", new XAttribute("testId", "86a79095-43b0-2fb6-53e8-f6e883d90cde")))));
            XDocument xml2 = new XDocument(
               new XElement("TestRun", new XElement("TestEntries", new XElement("TestEntry", new XAttribute("testId", "3e6fb122-e08e-5544-168b-cc66ffd246ac")))));

            XDocument controllDoc = new XDocument(new XElement("TestRun", new XElement("TestEntries",
                new XElement("TestEntry", new XAttribute("testId", "86a79095-43b0-2fb6-53e8-f6e883d90cde")),
                new XElement("TestEntry", new XAttribute("testId", "3e6fb122-e08e-5544-168b-cc66ffd246ac")))));

            XmlMerger.ReplaceFor(xml1, xml2, "TestEntries");
            Assert.AreEqual(controllDoc.ToString(), xml1.ToString());


        }
    }
}