using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace ConsoleApp20
{
    public class DisplayData
    {
      public  static void CreateXMlDocument(string csvName, string csvPath, string xmlPath, string xmlName, string magazine,bool displayFile)
        {


            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                using (var reader = new StreamReader(csvPath + csvName))
                {
                    using (var csv = new CsvReader(reader))
                    {

                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.RegisterClassMap<MapClass>();
                        List<Data> record = csv.GetRecords<Data>().ToList();

                 
                        // selekcja danych tylko dla jednego magazynu
                        var records = from row in record
                                      where row._invoiceMagazine == magazine
                                      select row;

   

                        var trn = from e in records
                                  group e by e._invoiceNumber into g
                                  select new
                                  {
                                      Invoice = g.Key,
                                      TotalQty = g.Sum(x => Convert.ToInt32(x._orderQty.ToString())),
                                      ISDATE = DateTime.ParseExact(((g.First()._invoiceDate).ToString()), "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"),
                                      LOCID = ((g.First()._customerNumber)),
                                      ID_MAG = ((g.First()._invoiceMagazine)),
                                      ORDER_NUM = ((g.First()._invoiceNumber)),
                                      PRDATE = ((g.First()._productExDate)),
                                      EXDATE = ((g.First()._productExDate)),
                                      QTY = ((g.First()._orderQty)),
                                      SKU = ((g.First()._productSupplierNumber)),



                                  };

                       

                        var prd = from p in records
                                  group p by p._productNumber into pr
                                  where pr.Count() >= 1
                                  select new
                                  {
                                      BAYWA_ID = pr.Key,
                                      EAN = ((pr.First()._productNumber)),
                                      NAME = ((pr.First()._productName)),
                                      BASF_ID = ((pr.First()._productSupplierNumber))
                                  };

                        var sto = from m in records
                                  group m by m._invoiceMagazine into mag
                                  where mag.Count() >= 1
                                  orderby mag.Key
                                  select new
                                  {
                                      // kod magazynu
                                      SNAME = mag.Key,

                                  };




                        XDocument basf = new XDocument(new XElement("EC-DIST.XML.SLSRPT",

                                             new XElement("HDR",
                                         new XAttribute("DTIME", "192345"),
                                         new XAttribute("DDATE", "20081024"),
                                         new XAttribute("RECP", "5900000065432"),
                                         new XAttribute("SND", "5900000930123"),
                                         new XAttribute("MFUNC", "F"),
                                         new XAttribute("MVER", "005"),
                                         new XAttribute("MTYPE", "SLSRPT")),
                                      new XElement("HDO",
                                         new XAttribute("REFDOC", "cfg-200810101234"),
                                         new XAttribute("ENCOD", "PL:ąćęłńóśżźĄĆĘŁŃÓŚŻŹ"),
                                         new XAttribute("EDATE", "20080130"),
                                         new XAttribute("SDATE", "20080104"),
                                         new XAttribute("RPTNO", "346/2007")),
                                       new XElement("DST",
                                         new XAttribute("TAX", "5291803886"),
                                         new XAttribute("CNTR", "PL"),
                                         new XAttribute("NO", "42a"),
                                         new XAttribute("STR", "Traugutta"),
                                         new XAttribute("PCODE", "05825"),
                                         new XAttribute("CITY", "Grodzisk Mazowiecki"),
                                         new XAttribute("DESC", "BayWa Agro Polska sp. z.o.o"),
                                         new XAttribute("SNAME", "BayWa Agro Polska"),
                                         new XAttribute("GLN", "1220000000008")),
                                       new XElement("NAD",
                                         new XAttribute("TAX", "5291803886"),
                                         new XAttribute("CNTR", "PL"),
                                         new XAttribute("NO", "42a"),
                                         new XAttribute("STR", "Traugutta"),
                                         new XAttribute("PCODE", "05825"),
                                         new XAttribute("CITY", "Grodzisk Mazowiecki"),
                                         new XAttribute("DESC", "BayWa Agro Polska sp. z.o.o"),
                                         new XAttribute("SNAME", "BayWa Agro Polska")),

                        from w in records
                        select

                     new XElement("LOC",
                new XAttribute("CPTEL2", "-"),
               new XAttribute("CPRSN2", "-"),
               new XAttribute("CPTEL1", "-"),
               new XAttribute("CPRSN1", "-"),
               new XAttribute("REPID", "-"),
               new XAttribute("REGON", "-"),
               new XAttribute("KRS", "-"),
               new XAttribute("LDESC", "-"),
               new XAttribute("LIC", "N"),
               new XAttribute("CTYPE", "P"),
               new XAttribute("TAX", w._customerTaxNumber),
               new XAttribute("LOCID2", "-"),
               new XAttribute("LOCID", w._customerNumber),
               new XAttribute("CNTR", "PL"),
               new XAttribute("NO", w._customerNo),
               new XAttribute("STR", w._customerStreet),
               new XAttribute("PCODE", w._customerPostCode),
               new XAttribute("CITY", w._customerCity),
               new XAttribute("DESC", w._customerName),
               new XAttribute("SNAME", w._customerName)),

                                 from t in trn
                                 select

               new XElement("TRN",
                  new XAttribute("ISDATE", t.ISDATE),
                  new XAttribute("OBIL", "FA"),
                  new XAttribute("TRN", t.Invoice),
                  new XAttribute("DATE", t.ISDATE),
                  new XAttribute("CAT", "TC_PLN"),
                  new XAttribute("LOCID", t.LOCID)),

                                 from p in prd
                                 select

               new XElement("PRD",
                  new XAttribute("MAN", p.NAME),
                  new XAttribute("DESC", p.NAME),
                  new XAttribute("SNAME", p.NAME),
                  new XAttribute("PSUID2", p.BASF_ID),
                  new XAttribute("SKU2", p.BAYWA_ID),
                  new XAttribute("SKU", p.BASF_ID),
                  new XAttribute("EAN", p.EAN)),


                                from mag in sto
                                select
                                new XElement("G1",
                  new XElement("STO",
                  new XAttribute("MAN", "MAGAZYN"),
                  new XAttribute("DESC", mag.SNAME)),

                            from s in trn
                            select

                            new XElement("G2",
                             new XElement("STD",
                               new XAttribute("IPMT", "N"),
                               new XAttribute("ICODE", "O"),
                               new XAttribute("OBIL", "FA"),
                               new XAttribute("INBR", s.Invoice),
                               new XAttribute("ONBR", s.Invoice),
                               new XAttribute("IPDATE", s.ISDATE),
                               new XAttribute("ISDATE", s.ISDATE),
                               new XAttribute("IDATE", s.ISDATE),
                               new XAttribute("DELID", s.LOCID),
                               new XAttribute("BUYID", s.LOCID)),

                             from a in record


                             where (a._customerNumber == s.LOCID)
                             // TUTAJ POTRZEBNA ITERACJA W ATRYBUCIE NBR
                             select
                                new XElement("LIN",
                                    new XAttribute("PRDATE",s.PRDATE),
                                    new XAttribute("EXDATE", s.EXDATE),
                                    new XAttribute("RFNAME",""),
                                    new XAttribute("RBRID",""),
                                    new XAttribute("REPID",s.ID_MAG),
                                    new XAttribute("ONBR",s.Invoice),
                                    new XAttribute("DCNTVAL",""),
                                    new XAttribute("DCNT",""),
                                    new XAttribute("TAXVAL",""),
                                    new XAttribute("TAX",""),
                                    new XAttribute("UNIPRI",""),
                                    new XAttribute("DNUPRI",""),
                                    new XAttribute("NUPRI",""),
                                    new XAttribute("DNVAL","0"),
                                    new XAttribute("NVAL","0"),
                                    new XAttribute("CURR","PLN"),
                                    new XAttribute("QTY",s.QTY),
                                    new XAttribute("UOM",""),
                                    new XAttribute("SKU",s.SKU),
                                    new XAttribute("NBR","1"))   // tutaj iteracja

                                    // new XAttribute("LICZNIK",s.Record)
                                    ))

                   
                                  
                                    
                               // s.LOCIN BUYID !

  ));

                        stopwatch.Start();



                        DateTime time = DateTime.Now;
                        string currentDate =  time.ToString("dd-MM-yyyy");


                        var fileName = currentDate + "_BAYWA_" + magazine+"SLSRPTY"+ ".xml";
                        var fullPath = xmlPath + fileName;

                        basf.Save(fullPath);

                        // np. 2019-11-21_BAYWA_BOLESLAWIECSLSLSRPT21112019.xml
                        Console.ForegroundColor = ConsoleColor.Red;                    
                        Console.WriteLine($"Raport wygenerowany pomyślnie {stopwatch.Elapsed.Milliseconds }ms ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(fullPath);

                        var archiveFolder = "inv\\";

                        var path = xmlPath + archiveFolder;

                        ///// wysyłanie damych.
                        if (!Directory.Exists(path))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(path);

                        }
                      

                        if (File.Exists(fullPath))
                        {
                            //  Process.Start(fullPath);

                            File.Delete(path + fileName);
                            File.Copy(fullPath,path+fileName);
                            File.Delete(fullPath);
                            if (displayFile)
                            {
                                Process.Start(path + fileName);
                                Backup.CREATE_BACKUP(path);
                            }
                        }

                        
                       


                    }

                }

            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nie znaleziono wskazanego pliku {0}", ex.Message);
            }


        }


      public  static void DisplayDataFromCSV(string csvFileName, string csvFilePath)
        {


            var fullName = csvFilePath + csvFileName;
            try
            {

                using (var reader = new StreamReader(fullName))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.HasHeaderRecord = true;
                        csv.Configuration.RegisterClassMap<MapClass>();
                        var records = csv.GetRecords<Data>();


                        Console.WriteLine($"FILE :[{fullName}]|");
                        int counter = 0;

                        foreach (var record in records)
                        {
                            counter++;
                       //     Console.WriteLine($"* {counter} *|{record._invoiceNumber} | {record._invoiceMagazine} | {record._orderQty} | {record._orderJm} | {record._productSupplierNumber}");
                        }


                    }
                }
            }
            catch (System.NotSupportedException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Nieznany błąd {ex.Message}");
            }

        }
    }
}
