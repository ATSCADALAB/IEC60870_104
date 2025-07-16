using System;
using System.Threading.Tasks;
using IEC60870Driver;

class SimpleWriteTest
{
    static void Main()
    {
        var driver = new ATDriver();
        try
        {
            driver.DeviceID = "192.168.1.27|2404|1|0|2|2|3";

            Console.WriteLine("=== IEC60870 Write Test ===");

            // ✅ ĐỌC trước
            driver.TagAddress = "16385";
            driver.TagType = "Int";
            for(int i=0;i<10000;i++)
            {
                var readResult = driver.Read();
                Console.Write(readResult.Value+$"{i}");
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi: " + ex.Message);
        }
        finally
        {
            driver.Dispose();
        }

        Console.ReadKey();
    }
}
