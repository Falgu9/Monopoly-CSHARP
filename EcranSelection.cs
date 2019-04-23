using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;
using reseau;

namespace monop {
    public class EcranSelection : Form {
        
        FlowLayoutPanel panelmenu,panelreseau;
        Size buttonsize,textboxsize;
        Point panelocation;
        Monopoly monop;
        serveur servvv;
        client cli;
        Thread cltth = null,srvth = null;
        
        public EcranSelection(){
            Icon = new Icon("img/monop.ico");
            Size = new Size(400,400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Fixed3D;

            panelmenu = new FlowLayoutPanel();
            PictureBox monopimage = new PictureBox();
            
            Controls.Add(panelmenu);
            Controls.Add(monopimage);

            buttonsize = new Size(350,80);

            monopimage.Image = new Bitmap("img/Monopoly.jpg");
            monopimage.Location = new Point(15,10);
            monopimage.Size = new Size(buttonsize.Width,120);
            monopimage.SizeMode = PictureBoxSizeMode.StretchImage;
            
            textboxsize = new Size(buttonsize.Width,20);
            panelocation = new Point(15,monopimage.Height+30);

            panelmenu.Location = panelocation;
            panelmenu.FlowDirection = FlowDirection.LeftToRight;
            panelmenu.Size = new Size(400,280);
            
            Button butreseau = creerButton("Reseau",buttonsize,new Point(0,0));
            butreseau.Click += new EventHandler(event_butreseau);

            panelmenu.Controls.Add(butreseau);
        }

        private void event_butreseau(object sender, EventArgs e){
            panelmenu.Visible = false;

            panelreseau = new FlowLayoutPanel();
            TextBox adressebox = new TextBox();
            TextBox portbox = new TextBox();
            TextBox userbox = new TextBox();
            Label labelport = new Label();
            Label labeladresse = new Label();
            Label labeluser = new Label();

            Controls.Add(panelreseau);
            panelreseau.Controls.Add(labeluser);
            panelreseau.Controls.Add(userbox);
            panelreseau.Controls.Add(labeladresse);
            panelreseau.Controls.Add(adressebox);
            panelreseau.Controls.Add(labelport);
            panelreseau.Controls.Add(portbox);

            panelreseau.Location = new Point(panelocation.X,panelocation.Y-15);
            panelreseau.Size = panelmenu.Size;

            labeluser.Text = "Nom d'Utilisateur";
            labeladresse.Text = "Adresse";
            labelport.Text = "Port";
            userbox.Text = "LWI";
            adressebox.Text = "localhost";
            portbox.Text = "1";

            userbox.Size = textboxsize;
            adressebox.Size = textboxsize;
            portbox.Size = textboxsize;

            Button butconnect = creerButton("CONNECT",buttonsize,new Point(0,0));
            panelreseau.Controls.Add(butconnect);
            AcceptButton = butconnect;
            butconnect.Click += new EventHandler(event_butconnect);

            
        }
        private Button creerButton(string text,Size size,Point location){
            Button but = new Button();
            but.Text = text;
            but.Size = size;
            but.Location = location;
            return but;
        }

        private void event_butconnect(object sender, EventArgs e){
            cli = new client();
            servvv = new serveur();
            panelreseau.Controls[6].Click -= event_butconnect;
             try{
                cli.connect(panelreseau.Controls[3].Text,Int32.Parse(panelreseau.Controls[5].Text));
                Console.WriteLine("client co");
            }
            catch(Exception ex){
                srvth = new Thread(new ThreadStart(srv));
                srvth.Start();
                cltth = new Thread(new ThreadStart(clt));
                cltth.Start();
            }
            if(cltth != null){
                while(cltth.ThreadState != ThreadState.Stopped ){
                    Application.DoEvents();
                }
            }
            
            serveur.send("PSD " + panelreseau.Controls[1].Text,cli.sock);
            while(cli.begin != true){
                Application.DoEvents();
            }
            
            monop = new Monopoly(cli);
            monop.Show();
            monop.Closed += new System.EventHandler(eventClosed);
            Visible = false;
            
        }

        private void eventClosed(object sender, EventArgs e){
            if(monop.formchoix.Created)
                monop.formchoix.Close();

            for(int i= 0;i < serveur.THLISTE.Count;i++){
                try{
                    serveur.THLISTE[i].Abort();
                }
                catch(Exception ex){}
                Console.WriteLine("serveur.THLISTE " + i + " :" +serveur.THLISTE[i].ThreadState);
            }
            monop.musique.Stop();
            monop.plth.Interrupt();
            monop.Dispose();
            
            serveur.quitterserveur(servvv.sock);
            
            Dispose();
        }
         private void srv(){
            object balanceLock = new object();
            lock(balanceLock){
                Console.WriteLine("serveur en cours de co"); 
                servvv.landserveur(Int32.Parse(panelreseau.Controls[5].Text));
            }
        }
        private void clt(){
            object balanceLock = new object();
            lock(balanceLock){
                try{
                    Console.WriteLine("client en cours de co");
                    cli.connect(panelreseau.Controls[3].Text,Int32.Parse(panelreseau.Controls[5].Text));
                }
                catch(Exception es){
                    Console.WriteLine(es);
                }
            }
        }
        static public void Main(){
            Application.EnableVisualStyles();
            Application.Run(new EcranSelection());
        }
    }
}