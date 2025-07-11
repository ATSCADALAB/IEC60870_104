using System;
using ATDriver_Server;
using IEC60870Driver;

class OptimalWriteUsage
{
    static void Main()
    {
        var driver = new ATDriver();

        try
        {
            driver.DeviceID = "192.168.1.51|2404|1|0|2|2|3";

            Console.WriteLine("Optimal Write Performance Test");
            Console.WriteLine("============================");

            while (true)
            {
                Console.Write("\nEnter value (0 or 1, 'q' to quit): ");
                string input = Console.ReadLine();

                if (input?.ToLower() == "q") break;

                if (input == "0" || input == "1")
                {
                    Console.WriteLine($"Writing {input} to IOA 24577...");

                    var startTime = DateTime.Now;

                    var sendPack = new SendPack()
                    {
                        ChannelAddress = driver.ChannelAddress,
                        DeviceID = driver.DeviceID,
                        TagAddress = "24577",
                        TagType = "Bool",  // ✅ XÁC ĐỊNH RÕ - NHANH NHẤT
                        Value = input
                    };

                    string result = driver.Write(sendPack);

                    var duration = DateTime.Now - startTime;

                    if (result == "Good")
                    {
                        Console.WriteLine($"✅ SUCCESS in {duration.TotalMilliseconds:F0}ms");
                    }
                    else
                    {
                        Console.WriteLine($"❌ FAILED in {duration.TotalMilliseconds:F0}ms");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            driver.Dispose();
        }
    }
}