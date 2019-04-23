using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;

namespace monop {
    public class Plateau : Panel{
        protected int tour,nbtour,des1,des2;
        public PictureBox pic;
        public Dictionary<int,Case> plateauliste;
        public List<Player> playerliste;
        public List<Terrain> terrainliste;
        Panel DES1,DES2;


        public Plateau(int nbj) {
            bool platTaille;
            if(Screen.PrimaryScreen.Bounds.Width > 1600 && Screen.PrimaryScreen.Bounds.Height > 990){
                Size = new Size(880,880);
                platTaille = false;
            }
            else{
                Size = new Size(770,770);
                platTaille = true;
            }
            Location = new Point(10,10);
            BackgroundImage = new Bitmap("img/dollar3.jpg");
            Font = new Font("Feeling Babylon",7);

            tour = 0;
            nbtour = 0;
            
            plateauliste = new Dictionary<int,Case>(40);
            playerliste = new List<Player>(Monopoly.nbjoueur);
            terrainliste = new List<Terrain>(28);

            Color GareColor = new Color();
            GareColor = Color.FromKnownColor(KnownColor.DarkGray);
            Color CompagnieColor = new Color();
            CompagnieColor = Color.FromKnownColor(KnownColor.Transparent);
            
            int[] loyergare = new int[] {25,50,100,200,0,0};

            Size casehorizontal,casevertical,caseGROSSE;
            int largeur,positiongauchebas,positiondroithaut,gap;
            if(platTaille == false){
                largeur = 100;
                gap = 6;
                casehorizontal = new Size(67,largeur);
            }
            else{
                largeur = 80;
                gap = 5;
                casehorizontal = new Size(60,largeur);
            }
            positiondroithaut = 10;
            positiongauchebas = Width - largeur - 10;
            casevertical = new Size(largeur,casehorizontal.Width);
            caseGROSSE = new Size(largeur,largeur);


            // cases du bas
            plateauliste.Add(0,new Case("DEPART",Color.Lime,200,0,new Point(Width - caseGROSSE.Width-10,positiongauchebas),caseGROSSE));
            plateauliste.Add(1,new Terrain("Boulevard de Belleville","marron",Color.Brown,"BoulevardBelleville.png",60,50,30,new int[] {2,10,30,90,160,250},1,new Point(plateauliste[0].Location.X-casehorizontal.Width - gap,positiongauchebas),casehorizontal));
            plateauliste.Add(2,new Caisse("Impots",100,2,new Point(plateauliste[0].Location.X-casehorizontal.Width*2 - gap*2,positiongauchebas),casehorizontal));
            plateauliste.Add(3,new Terrain("Rue Lecourbe","marron",Color.Brown,"RueLecourbe.png",100,50,30,new int[] {4,20,60,180,320,450},3,new Point(plateauliste[0].Location.X-casehorizontal.Width*3 - gap*3,positiongauchebas),casehorizontal));
            plateauliste.Add(4,new Caisse("Impots",200,4,new Point(plateauliste[0].Location.X-casehorizontal.Width*4 - gap*4,positiongauchebas),casehorizontal));
            plateauliste.Add(5,new Terrain("Gare Montparnasse","noir",GareColor,"GareMontParnasse.png",200,0,0,loyergare,5,new Point(plateauliste[0].Location.X-casehorizontal.Width*5 - gap*5,positiongauchebas),casehorizontal));
            plateauliste.Add(6,new Terrain("Rue de Vaugigard","ciel",Color.DeepSkyBlue,"RueVaugirard.png",100,50,50,new int[] {6,30,90,270,400,550},6,new Point(plateauliste[0].Location.X-casehorizontal.Width*6 - gap*6,positiongauchebas),casehorizontal));
            plateauliste.Add(7,new Caisse("?",0,7,new Point(plateauliste[0].Location.X-casehorizontal.Width*7 - gap*7,positiongauchebas),casehorizontal));
            plateauliste.Add(8,new Terrain("Rue de Courcelles","ciel",Color.DeepSkyBlue,"RueCourcelles.png",100,50,50,new int[] {6,30,90,270,400,550},8,new Point(plateauliste[0].Location.X-casehorizontal.Width*8 - gap*8,positiongauchebas),casehorizontal));
            plateauliste.Add(9,new Terrain("Avenue de la Republique","ciel",Color.DeepSkyBlue,"AvenueRepublique.png",120,50,60,new int[] {8,40,100,300,450,600},9,new Point(plateauliste[0].Location.X-casehorizontal.Width*9 - gap*9,positiongauchebas),casehorizontal));
            plateauliste.Add(10,new Prison("LA PRISON",0,10,new Point(10,positiongauchebas),caseGROSSE));

            // cases de gauche  
            plateauliste.Add(11,new Terrain("Boulevard de la Villette","rose",Color.HotPink,"BoulevardVillette.png",140,100,70,new int[] {10,50,150,450,625,750},11,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height - gap),casevertical));
            plateauliste.Add(12,new Terrain("Electricite de Paris","distrib",CompagnieColor,"CompagnieElec.png",150,0,75,new int[] {0,0,0,0,0,0},12,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*2 - gap*2),casevertical));
            plateauliste.Add(13,new Terrain("Avenue de Neuilly","rose",Color.HotPink,"AvenueNeuilly.png",140,100,70,new int[] {10,50,150,450,625,750},13,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*3 - gap*3),casevertical));
            plateauliste.Add(14,new Terrain("Rue de paradis","rose",Color.HotPink,"RueDeParadis.png",160,100,80,new int[] {12,60,180,500,700,900},14,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*4 - gap*4),casevertical));
            plateauliste.Add(15,new Terrain("Gare de Lyon","noir",GareColor,"GareLyon.png",200,0,0,loyergare,15,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*5 - gap*5),casevertical));
            plateauliste.Add(16,new Terrain("Avenue Mozart","orange",Color.Orange,"AvenueMozart.png",180,100,90,new int[] {14,70,200,550,750,950},16,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*6 - gap*6),casevertical));
            plateauliste.Add(17,new Caisse("Impots",100,17,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*7 - gap*7),casevertical));
            plateauliste.Add(18,new Terrain("Boulevard Saint Michel","orange",Color.Orange,"SaintMichel.png",180,100,90,new int[] {14,70,200,550,750,950},18,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*8 - gap*8),casevertical));
            plateauliste.Add(19,new Terrain("Place Pigalle","orange",Color.Orange,"PlacePigalle.png",200,100,100,new int[] {16,80,220,600,800,1000},19,new Point(positiondroithaut,plateauliste[10].Location.Y-casevertical.Height*9 - gap*9),casevertical));
            plateauliste.Add(20,new Case("PARC GRATUIT",Color.IndianRed,0,20,new Point(positiondroithaut,10),caseGROSSE));

            // cases du haut
            plateauliste.Add(21,new Terrain("Avenue Matignon","rouge",Color.Red,"AvenueMatignon.png",220,150,110,new int[] {18,90,250,700,875,1050},21,new Point(plateauliste[20].Location.X + plateauliste[20].Width + gap,positiondroithaut),casehorizontal));
            plateauliste.Add(22,new Caisse("?",0,22,new Point(plateauliste[21].Location.X + casehorizontal.Width + gap,positiondroithaut),casehorizontal));
            plateauliste.Add(23,new Terrain("Boulevard Malheserbe","rouge",Color.Red,"BoulevardMalesherbe.png",220,150,110,new int[] {18,90,250,700,875,1050},23,new Point(plateauliste[21].Location.X + casehorizontal.Width*2 + gap*2,positiondroithaut),casehorizontal));
            plateauliste.Add(24,new Terrain("Avenue Henri-Martin","rouge",Color.Red,"HenriMartin.png",240,150,120,new int[] {20,100,300,750,925,1100},24,new Point(plateauliste[21].Location.X + casehorizontal.Width*3 + gap*3,positiondroithaut),casehorizontal));
            plateauliste.Add(25,new Terrain("Gare du Nord","noir",GareColor,"GareNord.png",200,0,0,loyergare,25,new Point(plateauliste[21].Location.X + casehorizontal.Width*4 + gap*4,positiondroithaut),casehorizontal));
            plateauliste.Add(26,new Terrain("Faubourg Saint-Honore","jaune",Color.Gold,"SaintHonore.png",260,150,130,new int[] {22,110,330,800,975,1150},26,new Point(plateauliste[21].Location.X + casehorizontal.Width*5 + gap*5,positiondroithaut),casehorizontal));
            plateauliste.Add(27,new Terrain("Place de la bourse","jaune",Color.Gold,"LaBourse.png",260,150,130,new int[] {22,110,330,800,975,1150},27,new Point(plateauliste[21].Location.X + casehorizontal.Width*6 + gap*6,positiondroithaut),casehorizontal));
            plateauliste.Add(28,new Terrain("Eaux de Paris","distrib",CompagnieColor,"CompagnieEaux.png",150,0,75,new int[] {0,0,0,0,0,0},28,new Point(plateauliste[21].Location.X + casehorizontal.Width*7 + gap*7,positiondroithaut),casehorizontal));
            plateauliste.Add(29,new Terrain("Rue La Fayette","jaune",Color.Gold,"LaFayette.png",280,150,140,new int[] {24,120,360,850,1025,1200},29,new Point(plateauliste[21].Location.X + casehorizontal.Width*8 + gap*8,positiondroithaut),casehorizontal));
            plateauliste.Add(30,new Prison("GOPRISON",0,30,new Point(plateauliste[21].Location.X + casehorizontal.Width*9 + gap*9,positiondroithaut),caseGROSSE));

            // cases de droite
            plateauliste.Add(31,new Terrain("Avenue de Breteuil","vert",Color.MediumAquamarine,"AvenueBreteuil.png",300,200,150,new int[] {26,130,390,900,1100,1275},31,new Point(positiongauchebas, plateauliste[30].Location.Y + largeur + gap),casevertical));
            plateauliste.Add(32,new Terrain("Avenue Foch","vert",Color.MediumAquamarine,"AvenueFoch.png",300,200,150,new int[] {26,130,390,900,1100,1275},32,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height + gap),casevertical));
            plateauliste.Add(33,new Caisse("Impots",100,33,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*2 + gap*2),casevertical));
            plateauliste.Add(34,new Terrain("Boulevard des Capucines","vert",Color.MediumAquamarine,"BoulevardCapucines.png",320,200,160,new int[] {28,150,450,1000,1200,1400},34,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*3 + gap*3),casevertical));
            plateauliste.Add(35,new Terrain("Gare Saint-Lazare","noir",GareColor,"GareSaintLaz.png",200,0,0,loyergare,35,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*4 + gap*4),casevertical));
            plateauliste.Add(36,new Caisse("?",0,36,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*5 + gap*5),casevertical));
            plateauliste.Add(37,new Terrain("Avenue des Champs-Elysees","bleu",Color.Navy,"AvenueChamps.png",350,200,175,new int[] {35,175,500,1100,1300,1500},37,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*6 + gap*6),casevertical));
            plateauliste.Add(38,new Caisse("Impots",100,38,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*7 + gap*7),casevertical));
            plateauliste.Add(39,new Terrain("Rue de la Paix","bleu",Color.Navy,"RuePaix.png",400,200,200,new int[] {50,200,600,1400,1700,2000},39,new Point(positiongauchebas, plateauliste[31].Location.Y + casevertical.Height*8 + gap*8),casevertical));
            
            for(int t = 0; t < plateauliste.Count;t++){
                if(plateauliste[t].GetType() == plateauliste[1].GetType()){
                    terrainliste.Add((Terrain)plateauliste[t]);
                }
            }

            pic = new PictureBox();
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.Location = new Point(plateauliste[24].Location.X-plateauliste[24].Width/2,plateauliste[17].Location.Y);
            pic.BackColor = Color.Transparent;
            Controls.Add(pic);

            DES1 = new Panel();
            DES2 = new Panel();
            DES1.Size = new Size(100,100);
            DES2.Size = DES1.Size;
            DES1.Location = new Point(plateauliste[23].Location.X,plateauliste[18].Location.Y);
            DES2.Location = new Point(plateauliste[27].Location.X,plateauliste[18].Location.Y);
            DES1.BackgroundImageLayout = ImageLayout.Stretch;
            DES2.BackgroundImageLayout = ImageLayout.Stretch;
            DES1.BackColor = Color.Transparent;
            DES2.BackColor = Color.Transparent;
            Controls.Add(DES1);
            Controls.Add(DES2);
        }

        public int Tour{ get => tour ; set => tour = value; }
        public int Nbtour { get => nbtour; set => nbtour = value; }
        public int Des1 { get => des1; set => des1 = value; }
        public int Des2 { get => des2; set => des2 = value; }

        public void createPlayers(int nbj,string[] nomjoueurs){
            Size PlayersSize = new Size(16,13);
            int debut = 7,gap = 2;
            int x = debut;
            int y = 30;
            for(int i = 0;i < nbj;i++){
                string name = nomjoueurs[i];
                string fichiernom = (i+1) + ".jpg"; 
                playerliste.Add(new Player(name,"img/Pions2/" + fichiernom ,"img/Icones/" + fichiernom,PlayersSize,new Point(x,y)));
                x+= PlayersSize.Width + gap;
                if(i == 2){
                    x = debut;
                    y += PlayersSize.Height + gap;
                }
            }
        }
        public void setPlayers(){
            for(int i = 0;i < playerliste.Count;i++){
                this.plateauliste[0].Controls.Add(playerliste[i]);
            }
        }
        public void setPlat(){
            for(int i = 0;i < plateauliste.Count;i++){
                this.Controls.Add(plateauliste[i]);
            }
        }

        public void updateMoney(){
            for(int i = 0;i < Monopoly.nbjoueur; i++){
                Monopoly.panelplayerliste[i].Controls[1].Text = playerliste[i].Argent.ToString() + " $";
            }
        }
        public void updatePosition(){
            for(int i = 0;i < Monopoly.nbjoueur; i++){
                Player.tpPlayer(playerliste[Tour],playerliste[Tour].Numcase);
            }
        }
        public void updateTour(){
            if(Tour == Monopoly.nbjoueur-1){
                Tour = 0;
                Nbtour++; 
                Monopoly.print(Monopoly.receivebox,"plateau","au tour du joueur " + playerliste[Tour].Name);
            }
            else{
                Tour++; 
                Monopoly.print(Monopoly.receivebox,"plateau","au tour du joueur " + playerliste[Tour].Name);
            }
        }
        public void updateOwnerAff(){
            for(int i = 0; i < terrainliste.Count; i++){
                if(terrainliste[i].Owner != null){
                    terrainliste[i].nomlabel.BackgroundImage = new Bitmap(terrainliste[i].Owner.pionimg);
                    terrainliste[i].nomlabel.BackgroundImageLayout = ImageLayout.Zoom;
                }
                else{
                    terrainliste[i].nomlabel.BackgroundImage = null;
                }
            }
        }
        public void updateMaisonsAff(){
            for(int i = 0; i < terrainliste.Count; i++){
                if(terrainliste[i].nbmaison > 0){
                    terrainliste[i].maisonlabel.Text = terrainliste[i].nbmaison + " M";
                }
                else if(terrainliste[i].nbmaison == 5){
                     terrainliste[i].maisonlabel.Text = " H";
                }
                else {
                    terrainliste[i].maisonlabel.Text = null;
                }
            }
        }
               
        public void updateDes(){
            Random r = new Random();
            for(int i = 0;i< 8;i++){
                if(pic.Image == null){
                    DES1.BringToFront();
                    DES2.BringToFront();
                }
                else{
                    DES1.SendToBack();
                    DES2.SendToBack();
                }
                DES1.BackgroundImage = new Bitmap("img/Des/Dé" + r.Next(1,7) + ".png");
                DES2.BackgroundImage = new Bitmap("img/Des/Dé" + r.Next(1,7) + ".png");
                Thread.Sleep(200);
            }
            DES1.BackgroundImage = new Bitmap("img/Des/Dé" + des1.ToString() + ".png");
            DES2.BackgroundImage = new Bitmap("img/Des/Dé" + des2.ToString() + ".png");
            Thread.Sleep(2000);
        }
    }
}