using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace monop{
    public class Case : Panel {
        public int numcase,prix;

        protected bool available;
        public Color color1;
        public Label nomlabel;
        protected Player owner;
        public string couleur;

        // CONSTRUCTEUR
        public Case (string nom,Color col,int price,int num,Point position,Size taille) : base() {
            nomlabel = new Label();
            
            available = true;
            Name = nom;
            prix = price;
            numcase = num;
            color1 = col;
            owner = null;
            Location = position;
            Size = taille;

            nomlabel.Text = Name;
            nomlabel.Location = new Point(0,1);
            nomlabel.Size = new Size(taille.Width,30);
            nomlabel.TextAlign = ContentAlignment.MiddleCenter;
            nomlabel.BackColor = Color.Transparent;
            Controls.Add(nomlabel);

            Paint += new PaintEventHandler(paintEvent);
        }

        public bool Available{ get => available;  set => available = value; }
        public int Prix{ get => prix;  set => prix = value; }
        public Player Owner{ get => owner;  set => owner = value; }

        public void paintEvent(object sender,PaintEventArgs pe){
            Control cont = (Control)sender;
            LinearGradientBrush degrade =  new LinearGradientBrush(new Point(0, 10),new Point(100, 10),Color.Beige,this.color1);
            Graphics graphics = pe.Graphics;
            graphics.FillRectangle(degrade,new Rectangle(0,0,cont.Width,cont.Height));
            degrade.Dispose();
            graphics.Dispose();
        }
    }

    public class Terrain : Case {
        public int nbmaison,prixmaison,valhypothecaire;
        public int[] loyer;
        public Image cartepropriete;
        public Label maisonlabel;

        public Terrain(string nom,string couleur,Color col,string CartePropriete,int prix,int prixmaison,int valhypothecaire,int[] loyer,int numcase, Point position,Size taille) : base(nom,col,prix,numcase,position,taille) {
            
            this.couleur = couleur;
            this.nbmaison = 0;
            this.prixmaison = prixmaison;
            this.valhypothecaire = prix/2;
            this.loyer = loyer;
            cartepropriete = new Bitmap("img/CartesProprietes/" + CartePropriete);
            
            MouseHover += new EventHandler(MouseHoverCardsEvent);
            MouseLeave += new EventHandler(MouseLeaveEvent);

            nomlabel.MouseHover += new EventHandler(MouseHoverCardsEvent);
            nomlabel.MouseLeave += new EventHandler(MouseLeaveEvent);

            maisonlabel = new Label();
            maisonlabel.Text = null;
            maisonlabel.Size = new Size(20,20);
            if(numcase > 0 && numcase < 10 || numcase > 20 && numcase < 30)
                maisonlabel.Location = new Point(39,Height-20);
            else
                maisonlabel.Location = new Point(58,Height-20);

            
            maisonlabel.TextAlign = ContentAlignment.MiddleCenter;
            maisonlabel.BackColor = Color.Transparent;
            Controls.Add(maisonlabel);
            
        }

        public int Nbmaison{ get => nbmaison ; set => nbmaison = value; }

        // METHODES
        public int getloyer(){
            Plateau plat = (Plateau)Parent;
            int div = 0;
            if(couleur == "noir"){
                for(int i = 0; i < owner.boughtcases.Count; i++){
                    if(owner.boughtcases[i].couleur == "noir"){
                        div++;
                    }
                }
                return this.loyer[div-1];
            }
            else if(couleur == "distrib"){
                for(int i = 0; i < owner.boughtcases.Count; i++){
                    if(owner.boughtcases[i].couleur == "distrib"){
                        div++;
                    }
                }
                if(div == 1){
                    Random r = new Random();
                    int l = 4*r.Next(1,13);
                    return l;
                }
                else{
                    Random r = new Random();
                    int l = 10*r.Next(1,13);
                    return l;
                }
            }
            else{
                if(nbmaison == 0){
                    for(int i = 0; i < owner.boughtcases.Count; i++){
                        if(owner.boughtcases[i].couleur == couleur){
                            div++;
                        }
                    }
                    int nbterrain = 0;
                    for(int i = 0; i< plat.plateauliste.Count; i++){
                        if(couleur == plat.plateauliste[i].couleur){
                            nbterrain++;
                        }
                    }
                    if(div == nbterrain){
                        return loyer[0]*2;
                    }
                    else{
                        return loyer[0];
                    }
                }
                else{
                    return loyer[nbmaison];
                }
            }
        }
        public void MouseHoverCardsEvent(object sender,EventArgs e){
            Plateau plat = (Plateau)Parent;
            plat.pic.BringToFront();
            plat.pic.Image = cartepropriete;
        }
        public void MouseLeaveEvent(object sender,EventArgs e){
            Plateau plat = (Plateau)Parent;
            plat.pic.SendToBack();
            plat.pic.Image = null;
        }
    }
    
    public class Prison : Case{
        public Prison(string nom,int loyer,int numcase, Point position,Size taille) : base(nom,Color.Indigo,loyer,numcase,position,taille) {
        }
    }
    public class Caisse : Case {
        public Caisse(string nom,int loyer,int numcase, Point position,Size taille) : base(nom,Color.MintCream,loyer,numcase,position,taille) {
        }
    }
}