using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            string ip;
            string id;
            string pw;
            ushort port;

            Console.Write("IP : ");
            ip = Console.ReadLine();

            Console.Write("ID : ");
            id = Console.ReadLine();

            Console.Write("PW : ");
            pw = Console.ReadLine();

            Console.Write("port : ");
            string p = Console.ReadLine();
            port = Convert.ToUInt16(p);

            g2main.app_initialize(G2LANGUAGE.ID.ENGLISH);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form_watch(ip, id, pw, port));

            g2main.app_finalize();

        }
    }
}
