# Poker
[![forthebadge](https://forthebadge.com/badges/made-with-c-sharp.svg)](https://forthebadge.com)

Le programme suit le cahier des charges fournit par l'entreprise Home Sweet Home afin de créer un jeu s'inspirant du Poker.

## Les Fonctions 

### Tirage

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
La fonction ```tirage()``` permet de créer une seule des cartes de notre jeu.
Pour ce faire la fonction crée une carte avec une valeur et une famille séléctionnée de manière aléatoire en utilisant la classe ```Random()```.

### Carte Unique

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
La fonction ```carteUnique()``` permet de vérifier si une carte existe déjà dans le jeu afin d'éviter les doublons.
Elle part du principe que notre carte est bien unique et stock cette donnée dans le bolléen ```résultat```, la fonction compare ensuite les cartes et ne change la variable résultat que si elle rencontre une carte identique.

### Cherche Combinaison

```c#
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
    
```
La fonction ```cherche_combinaison()``` permet de vérifier si le jeu contient une combinaison gagnante.
Pour cela, elle commence par verifier le nombre d'occurence des valeurs et familles, puis les stockent dans les tableaux ```similaire = { 0, 0, 0, 0, 0 };``` et ```famille = { 0, 0, 0, 0, 0 };```.
Elle se décompose ensuite en 8 sous-parties qui testent les combinaisons possibles.

#### Paire

```c#
if (similaire[k] == 2)
{
    compte += 1;
    result = combinaison.PAIRE;
    p = true;
}
```
Le test pour la paire verifie la présence ou non d'un 2 dans le tableau ```similaire```.
Le test met également à jour un compteur ```compte``` pour chaque occurence du 2.
Enfin, il met à jour une variable de type booléen p pour qu'elle soit égale à true.

#### Double-Paire

```c#
if (compte / 2 == 2)
{
    result = combinaison.DOUBLE_PAIRE;
}    
```
Le test pour la Double-Paire utilise le compteur ```compte```, incrémenté dans le test pour la paire, afien de verifier le nombre de paires présentent dans le jeu. Pour ce faire, le test divise par 2 le nombre de "2" trouvé par compte et vérifie si le résultat donne 2.

#### Brelan

```c#
if (similaire[k] == 3)
{
    result = combinaison.BRELAN;
    b = true;
} 
```
Le test pour le brelan verifie la présence ou non de 3 dans le tableau similaire.

#### Carre

```c#
if (similaire[k] == 4)
{
    result = combinaison.CARRE;
}    
```
Le test pour le carre, comme pour le brelan et la paire, verifie la présence ou non d'un 4 dans le tableau similaire.

#### Quinte

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
Le test pour la quinte décompose en deux parties, la première boucle vérifie que tout les élèments de ```similaire``` sont unique et stock ce résultat. Puis, la deuxième partie compare le jeu aux élèments present dans la variable ```char[,] quintes = { { 'X', 'V', 'D', 'R', 'A' }, { '9', 'X', 'V', 'D', 'R' }, { '8', '9', 'X', 'V', 'D' }, { '7', '8', '9', 'X', 'V' } };```.
Si tout les éléments correspondent à l'une des propositions, le resultat devient Quinte.
#### Full

```c#
if (p && b)
{
    result = combinaison.FULL;
} 
```
Le test pour full ce sert des tests pour la paire et le brelan pour vérifier que le jeu contienne bien les deux.

#### Couleur

```c#
if (famille[k] == 5 && result != combinaison.QUINTE)
{
    result = combinaison.COULEUR;
}     
```
Le test pour la Couleur verifie que les 5 cartes ont la même famille puis qu'il n'y a pas de quinte.

#### Quinte Flush

```c#
if (famille[k] == 5 && result == combinaison.QUINTE)
{
    result = combinaison.QUINTE_FLUSH;
}   
```
Le test pour la Quinte Flush verifie que les 5 cartes ont la même famille puis utilise le test de la quinte pour verifier s'il y en a une.


### Echange Carte

```c#

```
La fonction ```echangeCarte()``` verifie si une carte existe déjà à un emplacement donné.

### Tirage du Jeu

```c#

```
La fonction ```tirageDuJeu()``` utilise la fonction ```tirage()``` pour créer un jeu de 5 cartes.

### Affichage Carte

```c#

```
La fonction ```affichageCarte()``` permet l'affichage des cartes
