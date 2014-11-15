using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestHtmlParser
{
    public class HotelAvailablity 
    {
        public String HotelName { get; set; }
        public String Price { get; set; }
    }

    public class TestLoading
    {
        public void Test1()
        {
            var html = @"<h3>
            <div id='lib_presta'>
                Chambre standard 1 pers du <span class=''>03/03/2014</span>  au <span class=''>05/03/2014 </span>
            </div>
            <div id='prix_presta'>
                127.76 &euro;
            </div>
        </h3><h3>
            <div id='lib_presta'>
                Chambre standard 2 pers du <span class=''>03/03/2014</span>  au <span class=''>05/03/2014 </span>
            </div>
            <div id='prix_presta'>
                227.76 &euro;
            </div>
        </h3>";

            CQ dom = html;

            var libs = dom["#lib_presta"];
            var prixs = dom["#prix_presta"];

            var list = libs.Zip(prixs, (k, v) => new { k, v })
              .Select(h => new HotelAvailablity { HotelName = h.k.InnerText.Trim(), Price = h.v.InnerText.Trim() });
        }

        public void Test2()
        {
            CQ fragment = CQ.CreateFragment("<p>some text</p>");
            CQ html = CQ.CreateFromFile(@"index.html");
            CQ modified_html = html.Select("#test").Append(fragment);
            modified_html.Save(@"index_modified.html");
        }

        public void Test3()
        {
            XDocument xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            XElement wordsElement = new XElement("words");
            xDocument.Add(wordsElement);
            XElement root = xDocument.Root;

            CQ html = CQ.CreateFromFile(@"vocabulaire_russe.htm");

            // <div class="summary">Le vocabulaire<br></div>
            // <table>
            var div = html.Select("div.summary");
            var table = div.Next("table");
            var rows = table.Find("tr");

            foreach (var row in rows)
            {
                CQ tdcells = row.Cq().Find("td");
                if (tdcells.Length == 2)
                {
                    string russian = tdcells[0].Cq().Text().Trim();
                    string french = tdcells[1].Cq().Text().Trim();

                    string output = String.Format("{0} {1}", russian, french);
                    Console.WriteLine(output);

                    XElement wordElement = new XElement("word", new XAttribute("russian", russian), new XAttribute("french", french));
                    root.Add(wordElement);
                }
                else
                {
                    string output = String.Format("{0}",
                        tdcells[0].Cq().Text());
                }
            }

            xDocument.Save("vocabulaire_russe.xml");
        }


    }
}
