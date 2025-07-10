using System;
using System.Threading;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using IEC60870.Object;
using IEC60870.SAP;

namespace IEC104Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("IEC 60870-5-104 Server");
            Console.WriteLine("======================");

            try
            {
                var server = new ServerSAP("127.0.0.1", 2404);

                server.NewASdu += ProcessReceivedAsdu;

                server.StartListen(10);
                Console.WriteLine("Server started on 127.0.0.1:2404");
                Console.WriteLine("Waiting for client connections...");
                Console.WriteLine("Press 'q' to quit");

                // Simulation thread - gửi data định kỳ
                var simulationThread = new Thread(() => SimulateData(server));
                simulationThread.IsBackground = true;
                simulationThread.Start();

                // Wait for quit command
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                } while (key.KeyChar != 'q');

                Console.WriteLine("\nShutting down server...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server error: {e.Message}");
            }
        }

        static void ProcessReceivedAsdu(ASdu asdu)
        {
            Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Received: {asdu.GetTypeIdentification()}");
            Console.WriteLine($"Common Address: {asdu.GetCommonAddress()}");
            Console.WriteLine($"Cause: {asdu.GetCauseOfTransmission()}");

            // Xử lý các loại command
            switch (asdu.GetTypeIdentification())
            {
                case TypeId.C_IC_NA_1: // General Interrogation
                    Console.WriteLine("Processing General Interrogation...");
                    break;

                case TypeId.C_SC_NA_1: // Single Command
                    Console.WriteLine("Processing Single Command...");
                    var ios = asdu.GetInformationObjects();
                    if (ios != null && ios.Length > 0)
                    {
                        Console.WriteLine($"IOA: {ios[0].GetInformationObjectAddress()}");
                    }
                    break;

                case TypeId.C_TS_NA_1: // Test Command
                    Console.WriteLine("Processing Test Command...");
                    break;

                default:
                    Console.WriteLine($"Unhandled ASDU type: {asdu.GetTypeIdentification()}");
                    break;
            }
        }

        static void SimulateData(ServerSAP server)
        {
            var random = new Random();
            int counter = 0;

            while (true)
            {
                try
                {
                    Thread.Sleep(5000); // Gửi data mỗi 5 giây

                    // Simulate single point data
                    var singlePoint = new IeSinglePointWithQuality(
                        random.Next(2) == 1, // random on/off
                        false, false, false, false);

                    var ios = new InformationObject[]
                    {
                        new InformationObject(counter % 100 + 1,
                            new[] { new InformationElement[] { singlePoint } })
                    };

                    var asdu = new ASdu(TypeId.M_SP_NA_1, false,
                        CauseOfTransmission.SPONTANEOUS, false, false, 0, 1, ios);

                    server.SendASdu(asdu);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sent spontaneous data - IOA:{counter % 100 + 1}");

                    counter++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Simulation error: {e.Message}");
                }
            }
        }
    }
}