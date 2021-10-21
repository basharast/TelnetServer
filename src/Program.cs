using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TelnetServer
{
    class Program
    {
        public static readonly ConsoleColor DefaultColor = Console.ForegroundColor;
        public static void ShellProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Data);

            // Do Something
        }

        public static void ShellProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(e.Data);

            // Do Something
        }
        // ReSharper disable once UnusedParameter.Local
        static string fullcommand = "";
        static string outputFull = "";
        static Screens.Welcome welcomeScreen = new Screens.Welcome();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Telnet server by Doron Guttman (Started)");
                Console.WriteLine("Enhanced by Bashar Astifan");
                Console.WriteLine("GitHub (Original): https://github.com/doronguttman/csharp-telnet-server");
                Console.WriteLine("GitHub (Enhanced): https://github.com/basharast/TelnetServer");
                Comm.Session sessionGlobal = null;
                var server = new Comm.Server(23);
                server.NewSession += async (sender, session) =>
                {
                    sessionGlobal = session;
                    await session.Send(welcomeScreen);
                    session.Disconnected += (o, e) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Disconnected");
                        Console.ForegroundColor = DefaultColor;
                    };
                    fullcommand = "";
                    outputFull = welcomeScreen.GetScreen();
                    session.MessageReceived += async (o, message) =>
                    {
                        await session.Send(new Telnet.ClearScreen());
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(message);
                        Console.ForegroundColor = DefaultColor;
                        fullcommand += message;
                        outputFull += message;
                        await session.Send(outputFull);
                        if (message.Equals("\r\n") || message.EndsWith("\r\n"))
                        {
                            if (fullcommand.StartsWith("exit"))
                            {
                                session.Close();
                            }
                            else
                            if (fullcommand.StartsWith("cls"))
                            {
                                await session.Send(new Telnet.ClearScreen());
                                Console.Clear();
                                fullcommand = "";
                                outputFull = welcomeScreen.GetScreen();
                                await session.Send(outputFull);
                                Console.WriteLine("Telnet server by Doron Guttman (Started)");
                                Console.WriteLine("Enhanced by Bashar Astifan");
                            }
                            else
                            {
                                try
                                {
                                    Process cmd = new Process();
                                    cmd.StartInfo.FileName = "cmd.exe";
                                    cmd.StartInfo.RedirectStandardInput = true;
                                    cmd.StartInfo.RedirectStandardOutput = true;
                                    cmd.StartInfo.CreateNoWindow = true;
                                    cmd.StartInfo.UseShellExecute = false;
                                    cmd.Start();

                                    cmd.StandardInput.WriteLine(fullcommand);
                                    fullcommand = "";
                                    cmd.StandardInput.Flush();
                                    cmd.StandardInput.Close();
                                    cmd.OutputDataReceived += ShellProcess_OutputDataReceived;
                                    cmd.ErrorDataReceived += ShellProcess_ErrorDataReceived;
                                    cmd.WaitForExit();
                                    string output = cmd.StandardOutput.ReadToEnd();
                                    outputFull += $"\r\n{output}";
                                    await session.Send(outputFull);
                                    Console.WriteLine(output);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    };
                };
                server.Start();

                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();
                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();
                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();
                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();
                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();
                Console.ReadLine();
                resetConsole(sessionGlobal).Wait();

                server.Stop();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static async Task resetConsole(Comm.Session session)
        {
            if (session != null)
            {
                await session.Send(new Telnet.ClearScreen());
            }
            Console.Clear();
            fullcommand = "";

            outputFull = welcomeScreen.GetScreen();
            if (session != null)
            {
                await session.Send(outputFull);
            }
            Console.WriteLine("Telnet server by Doron Guttman (Started)");
            Console.WriteLine("Enhanced by Bashar Astifan");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Don't run commands here, connect to 127.0.0.1 using telnet");
        }
        private static void ShellProcess_Exited(object sender, EventArgs e)
        {

        }
    }
}
