# Poker
[![forthebadge](https://forthebadge.com/badges/made-with-c-sharp.svg)](https://forthebadge.com)

Le programme suit le cahier des charges fournit par le service de direction des Systeme d'Information de l'entreprise Home Sweet Home afin de créer un jeu s'inspirant du Poker.

## Les Fonctions 

### Tirage

La fonction ```tirage()``` permet de créer une seule des cartes de notre jeu.
Pour ce faire la fonction crée une carte avec une valeur et une famille séléctionnée de manière aléatoire en utilisant la classe ```Random()```.

```c#
public static carte tirage()
{
    carte carte_tire;
    Random rnd = new Random();
    int v = rnd.Next(0, 13);
    int f = rnd.Next(0, 4);
    carte_tire.valeur = valeurs[v];
    carte_tire.famille = familles[f];
    return carte_tire;
}
```

### Carte Unique

La fonction ```carteUnique()``` permet de vérifier si une carte existe déjà dans le jeu afin d'éviter les doublons.
Elle part du principe que notre carte est bien unique et stock cette donnée dans le bolléen ```résultat```, la fonction compare ensuite les cartes et ne change la variable résultat que si elle rencontre une carte identique.

```c#
public static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
{
    bool resultat = true;
    for(int i = 0; i < 5; i++)
    {
        if (i != numero)
        {
            if (uneCarte.valeur == unJeu[i].valeur && uneCarte.famille == unJeu[i].famille)
            {
                resultat = false;
            }
        }
    }
    return resultat;
}
```

### Cherche Combinaison

La fonction ```cherche_combinaison()``` permet de vérifier si le jeu contient une combinaison gagnante.
Pour cela, elle commence par verifier le nombre d'occurence des valeurs et familles, puis les stockent dans les tableaux ```similaire = { 0, 0, 0, 0, 0 };``` et ```famille = { 0, 0, 0, 0, 0 };```.
Elle se décompose ensuite en 8 sous-parties qui testent les combinaisons possibles.

```c#
public static combinaison cherche_combinaison(ref carte[] unJeu)
{
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
    
```

#### Paire

Le test pour la paire verifie la présence ou non d'un 2 dans le tableau ```similaire```.
Le test met également à jour un compteur ```compte``` pour chaque occurence du 2.
Enfin, il met à jour une variable de type booléen p pour qu'elle soit égale à true.

```c#
if (similaire[k] == 2)
{
    compte += 1;
    result = combinaison.PAIRE;
    p = true;
}
```

#### Double-Paire

Le test pour la Double-Paire utilise le compteur ```compte```, incrémenté dans le test pour la paire, afien de verifier le nombre de paires présentent dans le jeu. Pour ce faire, le test divise par 2 le nombre de "2" trouvé par compte et vérifie si le résultat donne 2.

```c#
if (compte / 2 == 2)
{
    result = combinaison.DOUBLE_PAIRE;
}    
```

#### Brelan

Le test pour le brelan verifie la présence ou non de 3 dans le tableau similaire.

```c#
if (similaire[k] == 3)
{
    result = combinaison.BRELAN;
    b = true;
} 
```

#### Carre

Le test pour le carre, comme pour le brelan et la paire, verifie la présence ou non d'un 4 dans le tableau similaire.

```c#
if (similaire[k] == 4)
{
    result = combinaison.CARRE;
}    
```

#### Quinte

Le test pour la quinte décompose en deux parties, la première boucle vérifie que tout les élèments de ```similaire``` sont unique et stock ce résultat. Puis, la deuxième partie compare le jeu aux élèments present dans la variable ```char[,] quintes = { { 'X', 'V', 'D', 'R', 'A' }, { '9', 'X', 'V', 'D', 'R' }, { '8', '9', 'X', 'V', 'D' }, { '7', '8', '9', 'X', 'V' } };```.
Si tout les éléments correspondent à l'une des propositions, le resultat devient Quinte.

```c#
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
```

#### Full

Le test pour full ce sert des tests pour la paire et le brelan pour vérifier que le jeu contienne bien les deux.

```c#
if (p && b)
{
    result = combinaison.FULL;
} 
```

#### Couleur

Le test pour la Couleur verifie que les 5 cartes ont la même famille puis qu'il n'y a pas de quinte.

```c#
if (famille[k] == 5 && result != combinaison.QUINTE)
{
    result = combinaison.COULEUR;
}     
```

#### Quinte Flush

Le test pour la Quinte Flush verifie que les 5 cartes ont la même famille puis utilise le test de la quinte pour verifier s'il y en a une.

```c#
if (famille[k] == 5 && result == combinaison.QUINTE)
{
    result = combinaison.QUINTE_FLUSH;
}   
```

### Echange Carte

La fonction ```echangeCarte()``` permet d'échanger les cartes selectionner par le joueur et dont les index ont été stocké dans la variable e. Les cartes échangée sont toutes uniques.

```c#
private static void echangeCarte(ref carte[] unJeu, ref int[] e)
{
    for (int i = 0; i < e.Length; i++)
    {
        do
        {
            MonJeu[e[i]] = tirage();
        }
        while (carteUnique(MonJeu[i], MonJeu, i) == false);
    }
}
```

### Tirage du Jeu

La fonction ```tirageDuJeu()``` utilise la fonction ```tirage()``` pour créer un jeu de 5 cartes où toutes les cartes sont uniques.

```c#
private static void tirageDuJeu(ref carte[] unJeu)
{
    for (int i = 0; i < 5; i++)
    {
        do
        {
            MonJeu[i] = tirage();
        }
        while (carteUnique(MonJeu[i], MonJeu, i) == false);
    }
}
```

### Affichage Carte

La fonction ```affichageCarte()``` permet l'affichage des cartes, cette fonction gere donc la couleur et l'affichage des caractères spécifiques aux familles ♥, ♦, ♠ et ♣

```c#
private static void affichageCarte(ref carte uneCarte)
{
    int left = 0;
    int c = 1;
    // Tirage aléatoire de 5 cartes
    for (int i = 0; i < 5; i++)
    {
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
```
