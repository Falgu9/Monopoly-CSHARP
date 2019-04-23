using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;

namespace reseau{
    // CLIENT
    public class client {
        public Socket sock;
        public bool begin;
        public string inf;
        public BlockingCollection<string> blk;
        public int nbordre;
        public Thread th;
         
        public client(){
            begin = false;
            sock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            inf = null;
        }
        public void connect(string adresse,int port){
            sock.Connect(adresse,port);
            if(sock.Connected){
                
                th = new Thread(new ThreadStart(ReceiveThread));
                th.Start();
            }
        }
        public void ReceiveThread(){
            while(sock.Connected){
                if(sock.Poll(-1,SelectMode.SelectRead)){
                    string msg = serveur.receive(sock);
                    if(!String.IsNullOrEmpty(msg)){
                    //Console.WriteLine("client recu  = " + msg);
                    }
                    
                    if(msg.StartsWith("NB")){
                        nbordre = Int32.Parse(msg.Substring(3));
                    }
                    else if(msg.StartsWith("PLT")){
                        blk.Add("data " + msg);
                    }
                    else if(msg.StartsWith("INF")){
                        inf = msg;
                        begin = true;
                    }
                    else if(msg.StartsWith("HIS")){
                        blk.Add("hist " + msg);
                    }
                    else if(msg.StartsWith("FIN")){
                        blk.Add("fini " + msg);
                    }
                }
            }
        }

    }
}