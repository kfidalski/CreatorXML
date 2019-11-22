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
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;

namespace ConsoleApp20
{
    public class Magazine
    {
       public   string ID { get; set; }
        public  string SNAME { get; set; }
    }


    public sealed class MapClass : ClassMap<Data>
    {
        public MapClass()
        {
            Map(m => m._invoiceNumber).Index(0);
            Map(m => m._productNumber).Index(1);
            Map(m => m._productName).Index(2);
            Map(m => m._productSupplierNumber).Index(3);
            Map(m => m._productGroup).Index(4);
            Map(m => m._customerNumber).Index(5);
            Map(m => m._customerName).Index(6);
            Map(m => m._customerCity).Index(7);
            Map(m => m._customerPostCode).Index(8);
            Map(m => m._customerStreet).Index(9);
            Map(m => m._customerNo).Index(10);
            Map(m => m._customerTaxNumber).Index(11);
            Map(m => m._orderQty).Index(12);
            Map(m => m._orderJm).Index(13);
            Map(m => m._orderTotalValue).Index(14);
            Map(m => m._invoiceDate).Index(15);
            Map(m => m._invoiceMagazine).Index(16);
            Map(m => m._documentType).Index(17);
            Map(m => m._productExDate).Index(18);
            Map(m => m._productPrDate).Index(19);
        }
    }

    class Program : DisplayData
    {
        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("* INTERFEJS BayWa Agro Polska *");
            Console.ForegroundColor = ConsoleColor.White;

            List<Magazine> mag = new List<Magazine>();

            mag.Add(new Magazine() { ID = "P001", SNAME = "BAYWA_GRODZ" });
            mag.Add(new Magazine() { ID = "P002", SNAME = "BAYWA_PAS" });
            mag.Add(new Magazine() { ID = "P003", SNAME = "BAYWA_PRUS" });
            mag.Add(new Magazine() { ID = "P004", SNAME = "BAYWA_BOLE" });
            mag.Add(new Magazine() { ID = "P005", SNAME = "BAYWA_KETR" });
            mag.Add(new Magazine() { ID = "P006", SNAME = "BAYWA_LUB" });
            Console.WriteLine("|---------------------------------------------------------|");
            foreach (var item in mag)
            {
                
                Console.Write($"|*ID ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(item.ID);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" |* SNAME : ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(item.SNAME);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("|---------------------------------------------------------|");

            string path = string.Format(@"C:\basf\dane\");

            string fileCsvName = "basf.csv";
            string xmlName = "";
            string XmlPath = path;
            bool hasheader = true;
            bool displayFileAfterCreation = false;

            var fullPath = path + fileCsvName;

           


        Console.Title = "Interface data exchange 2019";
            //Thread.Sleep(1000);
  
            //Thread.Sleep(500);
            Console.WriteLine("odbiorca : B.A.S.F");
          //  Thread.Sleep(500);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("|---------------------------------------------------------|");
            Console.WriteLine($"|CURRENT DIRECTORY : {fullPath} | HEADER : {hasheader}|");
            Console.Write($"|OPEN REPORTE AFTER CREATION *STATE IS SET TO : ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(displayFileAfterCreation.ToString().ToUpper());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("|---------------------------------------------------------|");
            try
            {
                using (var reader = new StreamReader(path + fileCsvName))
                {
                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration { HasHeaderRecord = hasheader }))
                    {
                        csv.Configuration.RegisterClassMap<MapClass>();
                        var records = csv.GetRecords<Data>().ToList();


                        int counter = records.Count();

                        Console.Write($"Program wykrył"); Console.ForegroundColor = ConsoleColor.Red; Console.Write($" {counter - 1}"); Console.ForegroundColor = ConsoleColor.White; Console.Write(" rekordów do przetworzenia");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(); Console.WriteLine();
                        Console.Write($"KONTYNUOWAĆ ? Y\\N #: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        char input = (char)Console.Read();


                        switch ((char)input)
                        {
                            case 'Y':
                                {
                                    DisplayDataFromCSV(fileCsvName, path);
                                    Console.WriteLine("------------------Zakończono-------------------");

                                    foreach (var magazine in mag)
                                    {
                                        CreateXMlDocument(fileCsvName, path, XmlPath, xmlName, magazine.ID, displayFileAfterCreation);
                                    }

                                    //  CreateXMlDocument(fileCsvName, path, XmlPath, xmlName, "P001",displayFileAfterCreation);

                                    break;
                                }

                            default:
                                {
                                    Console.WriteLine("INVALID INPUT");

                                    for(int i = 3; i >= 1; i--)
                                    {
                                        Thread.Sleep(1000);
                                        Console.WriteLine($"RESTART : {i}s");
                                    }
                                    Thread.Sleep(1000);
                                    var fileName = Assembly.GetExecutingAssembly().Location;
                                    System.Diagnostics.Process.Start(fileName);
                                    Environment.Exit(0);
                                    break;

                                }
                  

                            case 'N':
                                {
                                    Console.WriteLine("nie");
                                    break;
                                }

                                
                        }
                    }
                }
            }catch(System.IO.FileNotFoundException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            

            Console.ReadKey();

        }



       

    }
}