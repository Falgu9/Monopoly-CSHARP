using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace monop {
    public class Player : PictureBox {
        int argent,numcase;
        public List<Terrain> boughtcases;
        public string iconeimg,pionimg;

        // CONSTRUCTEUR
        public Player(string nom,string image,string imageicone,Size taille,Point loc) : base() {
            pionimg = image;
            Image = new Bitmap(pionimg);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Location = loc;
            Size = taille;

            argent = 1500;
            Numcase = 0;
            Name = nom;
            iconeimg = imageicone;
            boughtcases = new List<Terrain>();
        }

        // GET SET
        public int Numcase { get => numcase; set => numcase = value; }
        public int Argent { get => argent; set => argent = value; }

        // METHODES
        public static void tpPlayer(Player p,int des){
            Plateau plat = (Plateau)p.Parent.Parent;
            Case cas = (Case)p.Parent;
            int i = cas.numcase;
            p.Numcase = des;
            while(i != des){
                plat.plateauliste[i].Controls.Remove(p);
                if( i == 39)
                    i = 0;
                else
                    i ++;
                plat.plateauliste[i].Controls.Add(p);
                Thread.Sleep(300);
            }    
        }

        public void buy(int des){
            Plateau plat = (Plateau)Parent.Parent;
            Argent -= plat.plateauliste[des].Prix;
            boughtcases.Add((Terrain)plat.plateauliste[des]);
            plat.plateauliste[des].Available = false;
            plat.plateauliste[des].Owner = this;
        }
        public void sell(int des){
            Plateau plat = (Plateau)Parent.Parent;
            Terrain terrain = (Terrain)plat.plateauliste[des];
            Argent += terrain.valhypothecaire;
            boughtcases.Remove(terrain);
            terrain.Available = true;
            terrain.Owner = null;
            terrain.nbmaison = 0;
        }
        
        public void buyhouse(int des,int nbmaison,int prix){
            Plateau plat = (Plateau)Parent.Parent;
            Terrain terrain = (Terrain)plat.plateauliste[des];
            Argent -= prix;
            terrain.Nbmaison += nbmaison;
        }

        public Panel remplirPanel(Point location){
            Plateau plat = (Plateau)Parent.Parent;
            Size labelsize = new Size(60,20);
            Panel pan = new Panel();
            pan.Size = new Size(200,200);
            pan.Location = location;
            pan.BackgroundImage = new Bitmap(iconeimg);
            pan.BackgroundImageLayout = ImageLayout.Stretch;

            Label labeljoueur = new Label();
            labeljoueur.Text = Name;
            if(Monopoly.clic.nbordre == plat.playerliste.IndexOf(this)){
                labeljoueur.Text += " (u)";
            }
            labeljoueur.Location = new Point(2,75);
            labeljoueur.Size = labelsize;
            labeljoueur.BackColor = Color.Beige;
            pan.Controls.Add(labeljoueur);

            Label labelprix = new Label();
            labelprix.Text = argent.ToString()+" $";
            labelprix.Location = new Point(labeljoueur.Location.X,labeljoueur.Location.Y+20); 
            labelprix.Size = labelsize;
            labelprix.BackColor = Color.Gold;
            pan.Controls.Add(labelprix);
            return pan;
        }
    }
}