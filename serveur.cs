using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;


namespace reseau {
    public class serveur{
        public Socket sock;
        public static List<client> CLILISTE;
        public static List<Thread> THLISTE;
        int nbconnexion;
        string pseudo;
        
        public serveur(){
                CLILISTE = new List<client>();
                THLISTE = new List<Thread>();
                sock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                nbconnexion = 2;
                pseudo = null;
            }

        public static void send(string r,Socket acc){
            byte[] send = Encoding.ASCII.GetBytes(r);
            acc.Send(send,send.Length,0);
        }
        public void sendtoall(string objet,string msg){
            foreach(client cli in CLILISTE){
                serveur.send(objet + msg,cli.sock);
            }
        }

        public static string receive(Socket acc){
            byte[] receiv = new byte[acc.Available];
            acc.Receive(receiv,acc.Available,0);
            string read = Encoding.ASCII.GetString(receiv);
            return read;
        }

        public void landserveur(int port){
            Thread th;    
            sock.Bind(new IPEndPoint(IPAddress.Any,port));
            sock.Listen(8);
            while(CLILISTE.Count != nbconnexion){
                client client = new client();
                client.sock = sock.Accept();
                CLILISTE.Add(client);
                th = new Thread(() => ReceiveThread(client));
                th.Start();
                THLISTE.Add(th);
            }
        }
        
        public void ReceiveThread(client client){
            try{
                while(client.sock.Connected){
                    if(client.sock.Poll(-1,SelectMode.SelectRead)){
                        string msg = serveur.receive(client.sock);
                        if(!String.IsNullOrEmpty(msg)){
                            //Console.WriteLine("serveur recu = " + msg);
                        }
                        msg.Trim();
                        if(msg.StartsWith("PSD")){
                            msg = msg.Substring(4);
                            pseudo += '/';
                            pseudo += msg;
                            
                            send("NB " + (CLILISTE.Count-1),client.sock);
                            Thread.Sleep(1000);
                            if(CLILISTE.Count == nbconnexion){
                                sendtoall("INF ",nbconnexion.ToString() + pseudo);
                                Thread.Sleep(2000);
                                Random r = new Random();
                                int des1 = r.Next(1,7);
                                int des2 = r.Next(1,7);
                                sendtoall("PLT ",(des1 + des2).ToString()+ "/0/0/0/0/0/1500/1500/1500/1500/1500/1500/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/0/"+ des1 + "/" + des2 +"/0/0");
                                Thread.Sleep(2000);
                            }
                        }
                        else if(msg.StartsWith("PLT")){
                            int[] plt;
                            Random r = new Random();
                            plt = getpltToint(msg);
                            plt[68] = r.Next(1,7);
                            plt[69] = r.Next(1,7);
                            plt[plt[70]] += plt[68]+plt[69];
                            if(plt[plt[70]] > 39)
                                plt[plt[70]] -= 40;
                            sendtoall("PLT ", getpltTostring(plt));
                        }
                        else if (msg.StartsWith("HIS")){
                            foreach(client cli in CLILISTE){
                                if(cli != client){
                                    serveur.send(msg,cli.sock);
                                }
                            }
                        }
                        else if(msg.StartsWith("FIN")){
                            sendtoall(null,msg);
                        }
                    }
                }
            }
            catch(Exception rec){
                Console.WriteLine(rec);
            }
        }

        public int[] getpltToint(string data){
            int[] plt = new int[72];
            string[] datas = null;
            data = data.Substring(4);
            datas = data.Split(new Char[] {'/'} );
            for(int i = 0; i < 72;i++){
                plt[i] = Int32.Parse(datas[i]);
            }
            return plt;
        }
        public string getpltTostring(int[] plt){
            string datas = null;
            for(int i = 0;i < 72;i++){
                datas += plt[i].ToString();
                if(i < 71){
                    datas += '/';
                }
            }
            return datas;
        }

         public static void quitterserveur(Socket acc){
            acc.Shutdown(SocketShutdown.Both);
            acc.Close();
        }
    }
}