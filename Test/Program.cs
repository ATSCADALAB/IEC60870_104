using System;
using System.Threading;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using IEC60870.Object;
using IEC60870.SAP;

namespace TestApp
{
    internal class Program
    {
        private static bool readCommandWorked = false;
        private static bool generalInterrogationWorked = false;

        private static void Main()
        {
            Console.WriteLine("Testing Read Command Support for IOA 16385");
            Console.WriteLine("==========================================");

            try
            {
                var client = new ClientSAP("192.168.1.39", 2404);

                client.SetIoaFieldLength(3);
                client.SetCotFieldLength(2);
                client.SetCommonAddressFieldLength(2);

                client.NewASdu += asdu =>
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Received: {asdu.GetTypeIdentification()}");

                    // Kiểm tra response cho Read Command
                    if (asdu.GetCauseOfTransmission() == CauseOfTransmission.REQUEST &&
                        asdu.GetTypeIdentification() != TypeId.C_RD_NA_1)
                    {
                        var ios = asdu.GetInformationObjects();
                        if (ios != null)
                        {
                            foreach (var io in ios)
                            {
                                if (io.GetInformationObjectAddress() == 16385)
                                {
                                    Console.WriteLine("✓ READ COMMAND WORKED!");
                                    Console.WriteLine($"   Value: {GetValueFromIO(io)}");
                                    readCommandWorked = true;
                                    return;
                                }
                            }
                        }
                    }

                    // Kiểm tra response cho General Interrogation
                    if (asdu.GetCauseOfTransmission() == CauseOfTransmission.INTERROGATED_BY_STATION)
                    {
                        var ios = asdu.GetInformationObjects();
                        if (ios != null)
                        {
                            foreach (var io in ios)
                            {
                                if (io.GetInformationObjectAddress() == 16385)
                                {
                                    Console.WriteLine("✓ GENERAL INTERROGATION WORKED!");
                                    Console.WriteLine($"   Value: {GetValueFromIO(io)}");
                                    generalInterrogationWorked = true;
                                    return;
                                }
                            }
                        }
                    }

                    // Kiểm tra negative response
                    if (asdu.IsNegativeConfirm())
                    {
                        Console.WriteLine("✗ NEGATIVE CONFIRM - Command rejected");
                    }
                };

                client.ConnectionClosed += e =>
                {
                    Console.WriteLine($"Connection closed: {e?.Message ?? "Normal"}");
                };

                Console.WriteLine("Connecting...");
                client.Connect();
                Console.WriteLine("Connected!\n");

                Thread.Sleep(1000);

                // TEST 1: Read Command
                Console.WriteLine("TEST 1: Sending Read Command for IOA 16385...");
                SendReadCommand(client, 1, 16385);

                Thread.Sleep(5000); // Đợi 5 giây

                if (!readCommandWorked)
                {
                    Console.WriteLine("✗ Read Command failed or not supported\n");

                    // TEST 2: General Interrogation
                    Console.WriteLine("TEST 2: Sending General Interrogation...");
                    SendGeneralInterrogation(client, 1);

                    Thread.Sleep(10000); // Đợi 10 giây
                }

                // Kết quả
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("RESULTS:");
                Console.WriteLine($"Read Command Support: {(readCommandWorked ? "YES ✓" : "NO ✗")}");
                Console.WriteLine($"General Interrogation: {(generalInterrogationWorked ? "YES ✓" : "NO ✗")}");

                if (readCommandWorked)
                {
                    Console.WriteLine("\n→ Device supports Read Command! You can read specific IOAs.");
                }
                else if (generalInterrogationWorked)
                {
                    Console.WriteLine("\n→ Device only supports General Interrogation.");
                    Console.WriteLine("  You need to filter results to get specific IOA values.");
                }
                else
                {
                    Console.WriteLine("\n→ Neither method worked. Check connection/configuration.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static string GetValueFromIO(InformationObject io)
        {
            var elements = io.GetInformationElements();
            if (elements != null && elements.Length > 0 && elements[0].Length > 0)
            {
                return elements[0][0].ToString();
            }
            return "Unknown";
        }

        static void SendReadCommand(ClientSAP client, int commonAddress, int informationObjectAddress)
        {
            var ios = new InformationObject[]
            {
                new InformationObject(informationObjectAddress, new InformationElement[][] { })
            };
            var asdu = new ASdu(TypeId.C_RD_NA_1, false,
                CauseOfTransmission.REQUEST, false, false, 0, commonAddress, ios);
            client.SendASdu(asdu);
        }

        static void SendGeneralInterrogation(ClientSAP client, int commonAddress)
        {
            var qualifier = new IeQualifierOfInterrogation(20);
            var ios = new InformationObject[]
            {
                new InformationObject(0, new[] { new InformationElement[] { qualifier } })
            };
            var asdu = new ASdu(TypeId.C_IC_NA_1, false,
                CauseOfTransmission.ACTIVATION, false, false, 0, commonAddress, ios);
            client.SendASdu(asdu);
        }
    }
}