using Sap.Data.Hana;
using System;
using Dapper;

namespace SapHanaNet5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            try
            {
                testKoneksiBiasa();
                testKoneksiDapper();
                testKoneksiSapB1ByDIAPI(); 
            }
            catch (Exception ex)
            {

            }
        }

        static void testKoneksiBiasa()
        {
            //TEST KONEKSI BIASA
            using (var conn = new HanaConnection("Server=xxx:30015;UserID=SYSTEM;Password=xxx;Current Schema=SUHUT_NET5;"))
            {
                conn.Open();
                var query = @"SELECT ""Id"", ""Description"" FROM ""Tm_Master01"" ";
                Console.WriteLine("Query result:");
                using (var cmd = new HanaCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Query result:");
                    // Print column names
                    var sbCol = new System.Text.StringBuilder();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        sbCol.Append(reader.GetName(i).PadRight(20));
                    }
                    Console.WriteLine(sbCol.ToString());
                    // Print rows
                    while (reader.Read())
                    {
                        var sbRow = new System.Text.StringBuilder();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            sbRow.Append(reader[i].ToString().PadRight(20));
                        }
                        Console.WriteLine(sbRow.ToString());
                    }
                    conn.Close();
                }
            }
        }

        static void testKoneksiDapper()
        {
            //TEST KONEKSI DENGAN DAPPER
            using (var conn = new HanaConnection("Server=xxx:30015;UserID=SYSTEM;Password=xxx;Current Schema=SUHUT_NET5;"))
            {
                var query = @"INSERT INTO ""Tm_Master01"" (""Description"") VALUES (:Description)  ";
                conn.ExecuteAsync(query, new { Description = "test123" });
                conn.Close();
            }
        }
    
        static void testKoneksiSapB1ByDIAPI()
        {
            int errCode = 0;
            string errMsg = "";

            dynamic oCompany = new SAPbobsCOM.Company();
            oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;
            oCompany.Server = "xxx";
            oCompany.DbUserName = "sa";
            oCompany.DbPassword = "xxx";
            oCompany.CompanyDB = "xxx";
            oCompany.UserName = "manager";
            oCompany.Password = "xxx";
            oCompany.LicenseServer = "xxx";
            oCompany.UseTrusted = false;
            oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
             

            try
            {
                if (oCompany.Connect() == 0)
                {
                    Console.WriteLine("koneksi SapB1 Sukses");

                    SAPbobsCOM.Items newItem = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                     
                    newItem.ItemCode = Guid.NewGuid().ToString();
                    newItem.ItemName = newItem.ItemCode;



                    int res = newItem.Add();
                    if (res == 0)
                    {
                        Console.WriteLine("add item SAPB1 sukses"); 
                    }
                    else
                    {
                        int errorCode = 0;
                        string errorDesc = "";
                        oCompany.GetLastError(out errorCode, out errorDesc);
                        throw new Exception("ErrorCode:" + errorCode.ToString() + "|" + "ErrorDesc:" + errorDesc);

                    }
                }
                else
                {
                    oCompany.GetLastError(out errCode, out errMsg);
                    throw new Exception("ErrorCode:" + errCode.ToString() + "|" + "ErrorDesc:" + errMsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
