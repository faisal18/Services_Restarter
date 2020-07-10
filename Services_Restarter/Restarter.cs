using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;

namespace Services_Restarter
{
    class Restarter
    {
        public static void Execute()
        {
            try
            {
                //RestartService("10.162.176.206","Spooler","dh\\FAnsari","asdf@1234","start");
                //ResertApppool("10.162.176.206", "dh\\FAnsari", "asdf@1234");
                PSExec();
                Console.WriteLine("Program complete");
                Console.Read();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        private static void RestartService(string remoteMachine, string serviceName, string userName, string password, string operation)
        {
            try
            {
                Console.WriteLine("Applying operation " + operation + "  on service " + serviceName + " on server " + remoteMachine);

                using (new NetworkConnection($"\\\\{remoteMachine}", new NetworkCredential(userName, password)))
                {
                    #region System Diagnostics
                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "CMD.exe",
                            Arguments = $"/C sc \\\\{remoteMachine} {operation} \"{serviceName}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    proc.Start();

                    while (!proc.HasExited)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    #endregion

                    #region ServiceController
                    ServiceController SCMachine = new ServiceController(serviceName, remoteMachine);
                    SCMachine.Start();
                    SCMachine.WaitForStatus(ServiceControllerStatus.Running);
                    #endregion




                    Console.WriteLine("The status of the process " + serviceName + " deployed on server " + remoteMachine + " is " + SCMachine.Status);
                    Console.WriteLine(serviceName + ":" + SCMachine.Status);



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void PSExec()
        {
            string create = string.Empty;
            string execute = string.Empty;
            string output = string.Empty;
            string command = string.Empty;
            string dt = System.DateTime.Now.ToString("yyyyMMddssHHmmss");
            string remoteMachine = "10.162.176.206";
            string serviceName = "spooler";
            string operation = "start";

            try
            {
                create = @"C:\tmp\PSpath\Create_IPCONFIG_" + dt + ".bat";
                command = @"psservice \\" + remoteMachine + " -u dh\\FAnsari -p asdf@1234 " + operation + " " + serviceName;

                //Create Batch
                using (StreamWriter writer = File.CreateText(create))
                {
                    writer.Write(command);
                }
                //Execute Batch
                Process obj = Process.Start(create);

                //Output CSV
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void ResertApppool(string remoteMachine, string userName, string password)
        {
            try
            {
                using (new NetworkConnection($"\\\\{remoteMachine}", new NetworkCredential(userName, password)))
                {

                    //var process = Process.GetProcessesByName("Spooler", remoteMachine);
                    //process.Count();
                    //Arguments = $"/C \\\\{remoteMachine} \"{argument}\"",


                    string path = @"\C$\Windows\System32\inetsrv\";
                    string argument = "APPCMD start apppool /apppool.name:CCIS";

                    path = remoteMachine + path;
                    var proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "CMD.exe",
                            Arguments = path + argument,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = false,
                            Verb = "runas"
                            //WorkingDirectory = @"\\" + path
                        }
                    };

                    proc.Start();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void RestartIIS()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void RestartServer()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }





}
