CC = mcs
LIBS = -r:System.Windows.Forms.dll -r:System.Drawing.dll
RESEAU_DIR = img
FILES = Monopoly.cs Case.cs Player.cs Plateau.cs EcranSelection.cs client.cs serveur.cs


Monopoly.exe : $(FILES)	
		$(CC) $(LIBS) $(FILES)

clean:
	rm -v -f Monopoly.exe