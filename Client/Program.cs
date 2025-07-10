using System;
using System.Threading;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.Object;
using IEC60870.SAP;

namespace IEC104Client
{
    class Program
    {
        static ClientSAP client;

        static void Main(string[] args)
        {
            Console.WriteLine("IEC 60870-5-104 Client");
            Console.WriteLine("======================");

            try
            {
                // Tạo client
                client = new ClientSAP("127.0.0.1", 2404);

                client.NewASdu += ProcessReceivedAsdu;
                client.ConnectionClosed += e =>
                {
                    Console.WriteLine($"Connection closed: {e?.Message ?? "Normal"}");
                };

                // Kết nối
                Console.WriteLine("Connecting to server...");
                client.Connect();
                Console.WriteLine("Connected successfully!");

                // Show menu
                ShowMenu();

                // Xử lý input
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey(true);
                    ProcessUserInput(key.KeyChar);
                } while (key.KeyChar != 'q');

                Console.WriteLine("\nDisconnecting...");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client error: {e.Message}");
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nAvailable Commands:");
            Console.WriteLine("1 - General Interrogation");
            Console.WriteLine("2 - Single Command ON");
            Console.WriteLine("3 - Single Command OFF");
            Console.WriteLine("4 - Test Command");
            Console.WriteLine("5 - Clock Synchronization");
            Console.WriteLine("6 - Counter Interrogation");
            Console.WriteLine("h - Show this help");
            Console.WriteLine("q - Quit");
            Console.WriteLine("\nPress a key to send command...");
        }

        static void ProcessUserInput(char key)
        {
            try
            {
                switch (key)
                {
                    case '1':
                        Console.WriteLine("\nSending General Interrogation...");
                        client.Interrogation(1, CauseOfTransmission.ACTIVATION,
                            new IeQualifierOfInterrogation(20));
                        break;

                    case '2':
                        Console.WriteLine("\nSending Single Command ON...");
                        client.SingleCommand(1, 100,
                            new IeSingleCommand(true, 0, false));
                        break;

                    case '3':
                        Console.WriteLine("\nSending Single Command OFF...");
                        client.SingleCommand(1, 100,
                            new IeSingleCommand(false, 0, false));
                        break;

                    case '4':
                        Console.WriteLine("\nSending Test Command...");
                        client.TestCommand(1);
                        break;

                    case '5':
                        Console.WriteLine("\nSending Clock Synchronization...");
                        client.SynchronizeClocks(1, new IeTime56(DateTime.Now.Ticks));
                        break;

                    case '6':
                        Console.WriteLine("\nSending Counter Interrogation...");
                        client.CounterInterrogation(1, CauseOfTransmission.ACTIVATION,
                            new IeQualifierOfCounterInterrogation(5, 0));
                        break;

                    case 'h':
                        ShowMenu();
                        break;

                    case 'q':
                        // Will be handled in main loop
                        break;

                    default:
                        Console.WriteLine($"\nUnknown command: {key}");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending command: {e.Message}");
            }
        }

        static void ProcessReceivedAsdu(ASdu asdu)
        {
            Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Received: {asdu.GetTypeIdentification()}");
            Console.WriteLine($"Common Address: {asdu.GetCommonAddress()}");
            Console.WriteLine($"Cause: {asdu.GetCauseOfTransmission()}");

            var ios = asdu.GetInformationObjects();
            if (ios != null)
            {
                foreach (var io in ios)
                {
                    Console.WriteLine($"IOA: {io.GetInformationObjectAddress()}");

                    var elements = io.GetInformationElements();
                    if (elements != null)
                    {
                        foreach (var elementSet in elements)
                        {
                            foreach (var element in elementSet)
                            {
                                Console.WriteLine($"  - {element}");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Press a key for next command...");
        }
    }
}