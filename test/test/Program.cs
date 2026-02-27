// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static Poker.Program;

namespace Poker
{
    class Program
    {
        // -----------------------
        // DECLARATION DES DONNEES
        // -----------------------
        // Importation des DL (librairies de code) permettant de gérer les couleurs en mode console
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        static uint STD_OUTPUT_HANDLE = 0xfffffff5;
        static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        // Pour utiliser la fonction C 'getchar()' : sasie d'un caractère
        [DllImport("msvcrt")]
        static extern int _getch();

        //-------------------
        // TYPES DE DONNEES
        //-------------------

        // Fin du jeu
        public static bool fin = false;

        // Codes COULEUR
        public enum couleur { VERT = 10, ROUGE = 12, JAUNE = 14, BLANC = 15, NOIRE = 0, ROUGESURBLANC = 252, NOIRESURBLANC = 240 };

        // Coordonnées pour l'affichage
        public struct coordonnees
        {
            public int x;
            public int y;
        }

        // Une carte
        //struct est l'equivalant d'une class
        public struct carte
        {
            public char valeur;
            public int famille;
        };

        // Liste des combinaisons possibles
        public enum combinaison { RIEN, PAIRE, DOUBLE_PAIRE, BRELAN, QUINTE, FULL, COULEUR, CARRE, QUINTE_FLUSH };

        // Valeurs des cartes : As, Roi,...
        public static char[] valeurs = { 'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        // Codes ASCII (3 : coeur, 4 : carreau, 5 : trèfle, 6 : pique)
        public static char[] familles = { '\u2665', '\u2666', '\u2663', '\u2660' };

        // Numéros des cartes à échanger
        public static int[] echange = { 0, 0, 0, 0 };

        // Jeu de 5 cartes
        public static carte[] MonJeu = new carte[5];

        //----------
        // FONCTIONS
        //----------

        // Génère aléatoirement une carte : {valeur;famille}
        // Retourne une expression de type "structure carte"
        public static carte tirage()
        {
            // la fonction commence par créer une carte vide puis selectionne une valeur et une famille aléatoirement avec Random
            // enfin on applique valeur et famille à la nouvelle carte
            carte carte_tire;
            Random rnd = new Random();
            int v = rnd.Next(0, 13);
            int f = rnd.Next(0, 4);
            carte_tire.valeur = valeurs[v];
            carte_tire.famille = familles[f];
            return carte_tire;
        }

        // Indique si une carte est déjà présente dans le jeu
        // Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
        // Retourne un entier (booléen)
        public static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
        {
            bool resultat = true; // on part du principe que notre carte est bien unique
            for(int i = 0; i < 5; i++)
            {
                if (i != numero) // on evite de compare la carte avec elle même
                {
                    if (uneCarte.valeur == unJeu[i].valeur && uneCarte.famille == unJeu[i].famille) // on verifie s'il y une carte identique
                    {
                        resultat = false;

                    }
                }
            }
            return resultat;
        }

        // Calcule et retourne la COMBINAISON (paire, double-paire... , quinte-flush)
        // pour un jeu complet de 5 cartes.
        // La valeur retournée est un élement de l'énumération 'combinaison' (=constante)
        public static combinaison cherche_combinaison(ref carte[] unJeu)
        {
            // On défini 2 tableaux vident pour pouvoir compter les élèments similaires

            combinaison result = new combinaison();
            result = combinaison.RIEN;
            int[] similaire = { 0, 0, 0, 0, 0 };
            int[] famille = { 0, 0, 0, 0, 0 };
            char[,] quintes = { { 'X', 'V', 'D', 'R', 'A' }, { '9', 'X', 'V', 'D', 'R' }, { '8', '9', 'X', 'V', 'D' }, { '7', '8', '9', 'X', 'V' } };
            int c = 0;
            int compte = 0;
            int compte2 = 0;
            bool b = false;
            bool p = false;

            // Permet la recherches des éléments de la même famille et/ou de la même valeur
            for (int i = 0; i < unJeu.Length; i++)
            {
                for (int j = 0; j < unJeu.Length; j++)
                {
                    // Compare et compte les cartes aux valeurs similaires
                    if (unJeu[i].valeur == unJeu[j].valeur)
                    {
                        similaire[i] += 1;
                    }
                    // Compte les cartes avec la même famille
                    if (unJeu[i].famille == unJeu[j].famille)
                    {
                        famille[i] += 1;
                    }
                }
            }
            
            for (int k = 0; k < 5; k++)
            {
                c = 0;

                // Test Paire
                // Le test verifie s'il y a un 2 dans le tableau similaire, s'il y en a un c'est qu'il y a une paire
                if (similaire[k] == 2)
                {
                    compte += 1;
                    result = combinaison.PAIRE;
                    p = true;
                }

                // Test Double-Paire
                // compte/2 nous donne le nombre de paire dans similaire pour verifier s'il y a double paire ou non
                
                if (compte / 2 == 2)
                {
                    result = combinaison.DOUBLE_PAIRE;
                }

                // Test Brelan
                if (similaire[k] == 3)
                {
                    result = combinaison.BRELAN;
                    b = true;
                }

                // Test Carre
                if (similaire[k] == 4)
                {
                    result = combinaison.CARRE;
                }

                // Quinte
                // avec c on commence par verifier si toutes les valeurs sont uniques et si elles le sont on a une quinte
                for (int l = 0; l < 5; l++)
                {
                    if (similaire[l] == 1)
                    {
                        c += 1; 
                    }
                   
                }

                if (c == 5)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        compte2 = 0;
                        for (int n = 0; n < 5; n++)
                        {
                            for (int p2 = 0; p2 < 5; p2++)
                            {
                                if (unJeu[n].valeur == quintes[m, p2])
                                {
                                    compte2++;
                                }

                                if (compte2 == 5)
                                {
                                    result = combinaison.QUINTE;
                                }
                            }
                            
                        }
                    }
                }

                // Test Full
                // Le Full reprend les resultat des test pour une paire et un brelan et verifie s'il y a les deux
                if (p && b)
                {
                    result = combinaison.FULL;
                }

                // Test Couleur
                // Le test vérifie que les 5 cartes ont la même famille mais sans quinte
                if (famille[k] == 5 && result != combinaison.QUINTE)
                {
                    result = combinaison.COULEUR;
                }

                // Test Quinte Flush
                // Le test vérifie que les 5 cartes ont la même famille et s'il y a eu une quinte
                if (famille[k] == 5 && result == combinaison.QUINTE)
                {
                    result = combinaison.QUINTE_FLUSH;
                }

            }
            return result;
        }

        // Echange des cartes
        // Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
        // On regarder chaque element dans e puis on modifie dans unJeu les cartes aux indices présent dans e
        private static void echangeCarte(ref carte[] unJeu, ref int[] e)
        {
            // on échange la carte à l'indice e dans unJeu par une autre que l'on créer avec la fonction tirage
            for (int i = 0; i < e.Length; i++)
            {
                do
                {
                    MonJeu[e[i]] = tirage();
                }
                while (carteUnique(MonJeu[i], MonJeu, i) == false);
            }
        }

        // Tirage d'un jeu de 5 cartes
        // Paramètre : le tableau de 5 cartes à remplir
        // On ajoute 5 cartes (de 0 à 5) dans unJeu
        private static void tirageDuJeu(ref carte[] unJeu)
        {
            // On appelle 5 fois la fonction tirage et on stock la carte crée dans unJeu
            for (int i = 0; i < 5; i++)
            {
                do
                {
                    MonJeu[i] = tirage();
                }
                while (carteUnique(MonJeu[i], MonJeu, i) == false);
            }
        }
        // Affiche à l'écran une carte {valeur;famille} en fournisant la colonne de départ
        private static void affichageCarte(ref carte uneCarte)
        {
            //----------------------------
            // TIRAGE D'UN JEU DE 5 CARTES
            //----------------------------
            int left = 0;
            int c = 1;
            // Tirage aléatoire de 5 cartes
            for (int i = 0; i < 5; i++)
            {
                
                // Tirage de la carte n°i (le jeu doit être sans doublons !)
                
                // Affichage de la carte
                if (MonJeu[i].famille == '\u2665' || MonJeu[i].famille == '\u2666')
                    SetConsoleTextAttribute(hConsole, 252);
                else
                    SetConsoleTextAttribute(hConsole, 240);
                Console.SetCursorPosition(left, 5);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 6);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 7);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 8);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 9);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 11);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 12);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 13);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 14);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 15);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 16);
                SetConsoleTextAttribute(hConsole, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", ' ', ' ', ' ', ' ', ' ', c, ' ', ' ', ' ', ' ', ' ');
                left = left + 15;
                c++;
            }

        }

        //--------------------
        // Fonction PRINCIPALE
        //--------------------
        static void Main(string[] args)
        {
            //---------------
            // BOUCLE DU JEU
            //---------------
            string reponse;

            Console.OutputEncoding = Encoding.GetEncoding(65001);

            SetConsoleTextAttribute(hConsole, 012);
            while (true)
            {
                // Positionnement et affichage
                Console.Clear();
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', 'P', 'O', 'K', 'E', 'R', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '1', ' ', 'J', 'o', 'u', 'e', 'r', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '2', ' ', 'S', 'c', 'o', 'r', 'e', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '3', ' ', 'F', 'i', 'n', ' ', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.WriteLine();
                // Lecture du choix


                do
                {
                    SetConsoleTextAttribute(hConsole, 014);
                    Console.Write("Votre choix : ");
                    reponse = Console.ReadLine();
                }
                while (reponse != "1" && reponse != "2" && reponse != "3");
                Console.Clear();
                SetConsoleTextAttribute(hConsole, 015);
                // Jouer au Poker
                if (reponse == "1")
                {
                    int i = 0;
                    tirageDuJeu(ref MonJeu);
                    affichageCarte(ref MonJeu[i]);

                    // Nombre de carte à échanger
                    try
                    {
                        int compteur = 0;
                        SetConsoleTextAttribute(hConsole, 012);
                        Console.Write("Nombre de cartes a echanger <0-5> ? : ");
                        compteur = int.Parse(Console.ReadLine());
                        int[] e = new int[compteur];
                        for (int j = 0; j < e.Length; j++)
                        {
                            Console.Write("Carte <1-5> : ");

                            e[j] = int.Parse(Console.ReadLine());
                            e[j] -= 1;
                        }

                        echangeCarte(ref MonJeu, ref e);

                    }
                    catch { }
                    //---------------------------------------
                    // CALCUL ET AFFICHAGE DU RESULTAT DU JEU
                    //---------------------------------------
                    Console.Clear();
                    affichageCarte(ref MonJeu[i]);
                    SetConsoleTextAttribute(hConsole, 012);
                    Console.Write("RESULTAT - Vous avez : ");
                    try
                    {
                        // Test de la combinaison
                        switch (cherche_combinaison(ref MonJeu))
                        {
                            case combinaison.RIEN:
                                Console.WriteLine("rien du tout... desole!"); break;
                            case combinaison.PAIRE:
                                Console.WriteLine("une simple paire..."); break;
                            case combinaison.DOUBLE_PAIRE:
                                Console.WriteLine("une double paire; on peut esperer..."); break;
                            case combinaison.BRELAN:
                                Console.WriteLine("un brelan; pas mal..."); break;
                            case combinaison.QUINTE:
                                Console.WriteLine("une quinte; bien!"); break;
                            case combinaison.FULL:
                                Console.WriteLine("un full; ouahh!"); break;
                            case combinaison.COULEUR:
                                Console.WriteLine("une couleur; bravo!"); break;
                            case combinaison.CARRE:
                                Console.WriteLine("un carre; champion!"); break;
                            case combinaison.QUINTE_FLUSH:
                                Console.WriteLine("une quinte-flush; royal!"); break;
                        }
                        ;
                    }
                    catch { }
                    Console.ReadKey();
                    char enregister = ' ';
                    string nom = "";
                    BinaryWriter f;
                    SetConsoleTextAttribute(hConsole, 014);
                    Console.Write("Enregistrer le Jeu ? (O/N) : ");
                    enregister = char.Parse(Console.ReadLine());
                    enregister = Char.ToUpper(enregister);

                    if (enregister == 'O')
                    {
                        const string fileName = "scores.txt";
                        Console.WriteLine("Vous pouvez saisir votre nom (ou pseudo) : ");
                        nom = Console.ReadLine();
                        using (f = new BinaryWriter(new FileStream("scores.txt", FileMode.Append, FileAccess.Write)))
                        {
                            f.Write(nom);
                            for (int a=0; a < 5; a++)
                            {
                                f.Write(MonJeu[a].valeur);
                                f.Write(MonJeu[a].famille);
                            }
                        }

                    }

                }
                if (reponse == "2")
                {
                    string articles;
                    char[] délimiteurs = { ';' };
                    carte UneCarte;
                    string nom;
                    Array a2;
                    char a1;
                    if (File.Exists("scores.txt"))
                    {
                        using (BinaryReader f = new BinaryReader(new FileStream("scores.txt", FileMode.Open, FileAccess.Read)))
                        {
                            nom = f.ReadString();
                            for (int k = 0; k < 5; k++)
                            {
                                MonJeu[k].valeur = f.ReadChar();
                                a1 = f.ReadChar();
                                if (Char.ToString(a1) == "e")
                                {
                                    MonJeu[k].famille = '\u2665';
                                }
                                else if (Char.ToString(a1) == "f")
                                {
                                    MonJeu[k].famille = '\u2666';
                                }
                                else if(Char.ToString(a1) == "c")
                                {
                                    MonJeu[k].famille = '\u2660';
                                }
                                else
                                {
                                    MonJeu[k].famille = '\u2663';
                                }
                                a2 = f.ReadChars(3);
                            }
                            
                        }
                        Console.WriteLine("Nom : " + nom);
                        affichageCarte(ref MonJeu[0]);
                        Console.ReadKey();
                    }
                }

                if (reponse == "3")
                    break;

            }
            Console.Clear();
            Console.ReadKey();
        }
    }
}

