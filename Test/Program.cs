using System;
using IEC60870Driver;

class SimpleWriteTest
{
    static void Main()
    {
        var driver = new ATDriver();
        try
        {
            driver.DeviceID = "192.168.1.51|2404|1|0|2|2|3";

            Console.WriteLine("=== IEC60870 Write Test ===");

            // ✅ ĐỌC trước
            driver.TagAddress = "16385";
            driver.TagType = "Int";
            var readResult = driver.Read();

            
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