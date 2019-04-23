using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Threading;
using monop;
using reseau;

public class Monopoly : Form {
    public Plateau plat;
    public SoundPlayer musique = new SoundPlayer();
    public static Dictionary<int,Panel> panelplayerliste;
    public static TextBox receivebox;
    public static string[] nomjoueurs;
    public static int nbjoueur;
    public static client clic;
    public BlockingCollection<string> blk;
    public bool continuer;
    public Thread plth;
    public Form formchoix;
    
    public Monopoly(client cli) {
        Text = "MONOPOLY";
        Icon = new Icon("img/monop.ico");
        Size = new Size(Screen.PrimaryScreen.Bounds.Width,Screen.PrimaryScreen.Bounds.Height);
        MinimumSize = new Size(350,400);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Font = new Font("Feeling Babylon",10);
        BackColor = Color.Beige;

        clic = cli;
        continuer = true;
        blk = new BlockingCollection<string>();
        clic.blk = blk;
        formchoix = new Form();
        
        // INIT PLAT
        plat = new Plateau(nbjoueur);
        plat.setPlat();
        Controls.Add(plat);

        // INIT PLAYERS
        nomjoueurs = new string[6];
        getinf(cli.inf);
        plat.createPlayers(nbjoueur,nomjoueurs);
        plat.setPlayers();
        
        // INIT PLAYERS ICONES
        panelplayerliste = new Dictionary<int, Panel>();
        createPanelPlayers(nbjoueur);

        // HISTORIQUE
        receivebox = createReceiveBox();
        Controls.Add(receivebox);

       musique.SoundLocation = "snd/ost.wav";
       musique.PlayLooping();

        plth = new Thread(new ThreadStart(PltThread));
        plth.Start();

        if(plat.Nbtour == 0 && plat.Tour == 0 && clic.nbordre == plat.Tour){
            Monopoly.print(Monopoly.receivebox,"plateau","c'est au tour du joueur " + plat.playerliste[plat.Tour].Name);
        }
    }
       
    public static void snd(){  
        SoundPlayer simpleSound = new SoundPlayer("snd/caisse.wav");  
        simpleSound.Play();
    }

    public void Game(){
        if(clic.nbordre == plat.Tour){
            Player joueur = (Player)plat.playerliste[plat.Tour];
            Case caseact = (Case)joueur.Parent;
            if(joueur.Numcase - plat.Des1 - plat.Des2 <= 0 && joueur.Numcase != 0 && plat.Nbtour != 0){
                joueur.Argent += 200;
                print(receivebox,joueur.Name,"vous avez gagne 200 $ car vous etes passe sur la case depart");
            }
            if(caseact.GetType() == plat.plateauliste[1].GetType()){
                Terrain caseterrain = (Terrain)caseact;
                if(caseterrain.Available){
                    formchoix = createdialog("Achat");
                    formchoix.ShowDialog();
                    if(formchoix.DialogResult == DialogResult.Yes){
                        if(joueur.Argent - caseterrain.Prix > 0){
                            joueur.buy(caseterrain.numcase);
                            print(receivebox,joueur.Name,"vous avez achete " + caseterrain.Name + " pour " + caseterrain.Prix + " $");
                        }
                        else{
                            print(receivebox,joueur.Name,"vous n'avez pas assez d'argent pour acheter " +  caseterrain.Name);
                        }
                        //Monopoly.snd();
                    }
                    else{
                        print(receivebox,joueur.Name,"Pas d'achat");
                    }
                }
                else if(!caseterrain.Available && caseterrain.Owner != joueur){
                    int l = caseterrain.getloyer();
                    if(joueur.Argent - l > 0){
                        joueur.Argent -= l;
                        caseterrain.Owner.Argent += l;
                        Monopoly.print(receivebox,joueur.Name,"vous etes passe sur " + caseterrain.Name + " pour " + l + " $ et l'avez offert a : " + caseterrain.Owner.Name);
                    }
                    else{
                        serveur.send("FIN " + joueur.Name,Monopoly.clic.sock);
                    }
                }
                else if (!caseterrain.Available && caseterrain.Owner == joueur){
                    string objet;
                    if(caseterrain.couleur != "distrib" && caseterrain.couleur != "noir"){
                        objet = "Achat de maison";
                    }
                    else{
                        objet = "vente";
                    }
                        formchoix = createdialog(objet);
                        formchoix.ShowDialog();
                    if(formchoix.DialogResult == DialogResult.Yes){
                        int paye,nbma = 0;
                        while (nbma == 0){
                            try{
                                nbma = Int32.Parse(Console.ReadLine());
                            }
                            catch(Exception e){
                                Console.WriteLine(e);
                            }
                        }
                        paye = caseterrain.prixmaison*nbma;
                        if(joueur.Argent - paye > 0 && caseterrain.Nbmaison + nbma < 6){
                            joueur.buyhouse(caseact.numcase,nbma,paye);
                            Monopoly.print(receivebox,joueur.Name,"vous etes passe sur " + caseterrain.Name + " et avez achete " + nbma + " maisons pour " + paye + " $");
                        }
                        else{
                            print(Monopoly.receivebox,joueur.Name,"vous n'avez pas assez d'argent ou il y'a trop de maisons pour acheter " + nbma + " maisons sur " + caseterrain.Name);
                        }
                    }
                    else if(formchoix.DialogResult == DialogResult.Cancel){
                        joueur.sell(caseterrain.numcase);
                        print(receivebox,joueur.Name,"vous avez vendu " + caseterrain.Name + " pour " + caseterrain.valhypothecaire + " $");
                    }
                    else if(formchoix.DialogResult == DialogResult.No){
                        print(receivebox,joueur.Name,"Pas d'action");
                    }
                }
            }
            else if(caseact.GetType() == plat.plateauliste[2].GetType()){
                if(caseact.numcase == 22 || caseact.numcase == 7 || caseact.numcase == 36){
                    Random r = new Random();
                    int has = r.Next(-200,200);
                    joueur.Argent += has;
                    print(receivebox,joueur.Name,"CHANCE = " + has);
                }
                else{
                    joueur.Argent -= caseact.prix;
                    print(receivebox,joueur.Name,"vous etes passe sur " + caseact.Name + " pour " + caseact.Prix + " $");
                }
            }  
            else if(caseact.GetType() == plat.plateauliste[10].GetType()){
                print(receivebox,joueur.Name,caseact.Name + " vous deplace de 20 cases");
                if(caseact.numcase == 10){
                   Player.tpPlayer(joueur,30); 
                }
                else if(caseact.numcase == 30){
                    Player.tpPlayer(joueur,10); 
                }
            }
            else{
                print(receivebox,joueur.Name,caseact.Name.ToString());
            }
            if(joueur.Argent > 0){
                plat.updateTour();
                Thread.Sleep(1000);
                monopsendplt();
            }
            else{
                serveur.send("FIN " + joueur.Name,Monopoly.clic.sock);
            }         
        }
    }
    
    public void createPanelPlayers(int nbjoueur){
        int depart = plat.Width+55;
        int x = depart,y = 10;
        int largeur = 200;
        int gap = 0;
        for(int i = 0;i < nbjoueur;i++){
            
            Panel panel = plat.playerliste[i].remplirPanel(new Point(x,y));
            panelplayerliste.Add(i,panel);
            Controls.Add(panelplayerliste[i]);
            x += largeur + gap;
            if(i == 2){
                y += largeur + gap;
                x = depart;
            }
        }
    }
    
    public void monopsendplt(){
        string data = null;
        int e = 0;
        for(int i = 0;i < 72;i++){
            
            if(i < 6){
                if(i > nbjoueur-1){
                    data += "0/";
                }
                else{
                    data += plat.playerliste[i].Numcase.ToString();
                    data += '/';
                }
                
            }
            else if(5 < i && i < 12){
                if(i > nbjoueur+5){
                    data += "0/";
                }
                else{
                    data += plat.playerliste[i-6].Argent.ToString();
                    data += '/';
                }
                
            }
            else if(11 < i  && i< 68){
                if(i%2 == 0){
                    if(plat.terrainliste[e].Owner == null){
                        data += "0/";
                    }
                    else{
                        data += (plat.playerliste.IndexOf(plat.terrainliste[e].Owner)+1).ToString(); 
                        data += '/';
                    }
                }
                else{
                    data += plat.terrainliste[e].Nbmaison.ToString();
                    data += '/';
                    e++;
                }
                
            }
            else if(i == 68){
                data += plat.Des1.ToString();
                data += '/';
            }
            else if (i == 69){
                data += plat.Des2.ToString();
                data += '/';
            }
            else if (i == 70){
                data += plat.Tour.ToString();
                data += '/';
            }
            else if (i == 71){
                data += plat.Nbtour.ToString();
            }
        }
        serveur.send("PLT " + data,clic.sock);
    }
    private void getplt(string data){
        string[] datas;
        int e = 0;
        datas = data.Split(new Char[] {'/'} );
        for(int i = 0; i < 72; i ++){
            int num = Int32.Parse(datas[i]);
            if(i < 6){
                plat.playerliste[i].Numcase = num;
                if(i == nbjoueur-1){
                    i = 5;
                }
            }
            
            else if(5 < i && i < 12){
                plat.playerliste[i-6].Argent = num;
                if(i == nbjoueur+5){
                    i = 11;
                }
            }
            else if(11 < i  && i< 68){
                if(i%2 == 0){
                    if(num > 0){
                        plat.terrainliste[e].Owner = plat.playerliste[num-1];
                        plat.terrainliste[e].Available = false;
                        if(!plat.playerliste[num-1].boughtcases.Contains(plat.terrainliste[e]))
                            plat.playerliste[num-1].boughtcases.Add(plat.terrainliste[e]);               
                    }
                    else{
                        plat.terrainliste[e].Owner = null;
                        plat.terrainliste[e].Available = true;
                        for(int v = 0;v < nbjoueur;v++){
                            if(plat.playerliste[v].boughtcases.Contains(plat.terrainliste[e])){
                                plat.playerliste[v].boughtcases.Remove(plat.terrainliste[e]);
                            }
                        }
                    }
                }
                else{
                    plat.terrainliste[e].Nbmaison = num;
                    e++;
                }
            }
            else if(i == 68){
                plat.Des1 = num;
            }
            else if (i == 69){
                plat.Des2 = num;
            }
            else if (i == 70)
                plat.Tour = num;
            else if (i == 71)
                plat.Nbtour = num;
        }
    }
    private void PltThread(){
        while(continuer){
           plat.updateMoney();
            string take = blk.Take();
            if(take.StartsWith("data")){
                take = take.Substring(9);
                getplt(take);
                plat.updateDes();
                plat.updateMoney();
                plat.updatePosition();
                plat.updateOwnerAff();
                plat.updateMaisonsAff();
                Game();
            }
            else if(take.StartsWith("hist")){
                take = take.Substring(9);
                receivebox.Text += take;
            }
            else if(take.StartsWith("fini")){
                take = take.Substring(9);
                receivebox.Text += DateTime.Now.ToLongTimeString() + " - [ " + take + " ] ----> La partie est finie car je suis en failite";
                Thread.Sleep(5000);
                Close();
                continuer = false;
                
                
            }
        }
    }
    
    private void getinf(string data){
        string[] datas;
        data = data.Substring(4);
        datas = data.Split(new Char[] {'/'} );
        for(int i = 0; i< datas.Length;i++){
            if(i == 0)
                nbjoueur = Int32.Parse(datas[i]);
            else
                nomjoueurs[i-1] = datas[i];
        }
    }

    // AFFICHAGE
    private Form createdialog(string action){
        Terrain ter = (Terrain)plat.playerliste[plat.Tour].Parent;
        Size textboxsize = new Size(350,80);

        Form form = new Form();
        form.Text = plat.playerliste[plat.Tour].Name;
        form.Icon = new Icon("img/monop.ico");
        form.BackColor = Color.Beige;
        form.Size = new Size(400,400);
        form.Opacity = .95;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.FormBorderStyle = FormBorderStyle.Fixed3D;

        FlowLayoutPanel panel = new FlowLayoutPanel();
        panel.Size = form.Size;
        panel.Location = new Point(15,30);
        panel.BackColor = Color.Transparent;
        form.Controls.Add(panel);

        Label lb = new Label();
        lb.Text = "Vous etes sur " + ter.Name;
        lb.Size = new Size(textboxsize.Width,50);
        panel.Controls.Add(lb);
        if(action == "Achat")
            lb.Text += " et le prix d'achat est " + ter.Prix + " $";

        if(action != "vente"){
            Button button = new Button();
            button.Text = action;
            button.Size = textboxsize;
            button.DialogResult = DialogResult.Yes;
            panel.Controls.Add(button);
        }

        if(action == "Achat de maison" && ter.Nbmaison < 5 || action == "vente"){
            if(action == "Achat de maison")
                lb.Text += ". Vous possedez ce terrain et pouvez acheter des maisons (vous en avez " + ter.Nbmaison + " sur ce terrain) ou le vendre pour " + ter.valhypothecaire + " $" + " (si vous achetez des maisons veuillez entrer le nombre dans la console (entre 1 et 5))";
            

            Button button2 = new Button();
            button2.Text = "Vente";
            button2.Size = textboxsize;
            button2.DialogResult = DialogResult.Cancel;
            panel.Controls.Add(button2);
        }

        Button fintour = new Button();
        fintour.Text = "Fin du Tour";
        fintour.Size = textboxsize;
        fintour.DialogResult = DialogResult.No;
        panel.Controls.Add(fintour);
        return form;
    }

       public TextBox createReceiveBox(){
        int depart = plat.Width+55;
        TextBox text = new TextBox();
        text.Location = new Point(depart,415);
        text.Size = new Size(600,365);
        text.Multiline = true;
        text.BorderStyle = BorderStyle.None;
        text.ReadOnly = true;
        text.BackColor = Color.DarkSeaGreen;
        text.Cursor = Cursors.Arrow;
        
        text.TextChanged += new EventHandler(ScrollEvent);
        text.GotFocus += new EventHandler(GotFocusEvent);

        return text;
    }

    public static void print(Control e, string user, string text){
       string s = null;
        s += DateTime.Now.ToLongTimeString() + " - [ " + user + " ] " + " ----> " + text + "\r\n";
        e.Text += s;
        serveur.send("HIS " + s,clic.sock);
    }

    // EVENTS
    private void ScrollEvent(object sender,EventArgs e){
        TextBox text = (TextBox)sender;
        text.SelectionStart = text.Text.Length;
        text.ScrollToCaret();
    }
    
    [DllImport("user32.dll")]
    static extern bool HideCaret(IntPtr hWnd);
    private void GotFocusEvent(object sender,EventArgs e){
        TextBox text = (TextBox)sender;
        HideCaret(text.Handle);
    }
}