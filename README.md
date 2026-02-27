# Poker
[![forthebadge](https://forthebadge.com/badges/made-with-c-sharp.svg)](https://forthebadge.com)

Le programme suit le cahier des charges fournit par l'entreprise Home Sweet Home afin de créer un jeu s'inspirant du Poker.


## Les Fonctions 

### Tirage
```c#
public static carte tirage()
{
    
}
```
La fonction ```tirage()``` permet de créer une seule des cartes de notre jeu.
Pour ce faire la fonction crée une carte avec une valeur et une famille séléctionnée de manière aléatoire.
### Carte Unique
```c#
public static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
{
    
}
```
La fonction ```carteUnique()``` permet de vérifier si une carte existe déjà dans le jeu à une position donnée.
Elle part du principe que notre carte est bien unique et stock cette donnée dans le bolléen résultat, la fonction compare ensuite les cartes et ne change la variable résultat que si elle rencontre une carte identique.


### Cherche Combinaison
```c#

    
```
La fonction ```cherche_combinaison()``` permet de vérifier si le jeu contient une combinaison gagnante.
Pour cela, elle commence par verifier le nombre d'occurence des valeurs et familles, puis les stockent dans les tableaux ```similaire = { 0, 0, 0, 0, 0 };``` et ```famille = { 0, 0, 0, 0, 0 };```.
Elle se décompose ensuite en 8 sous-parties qui testent les combinaisons.
```c#
```
#### Paire
```c#
   
```
Le test pour la paire verifie la présence ou non d'un 2 dans le tableau similaire.
Le test met également à jour un compteur ```compte``` pour chaque occurence du 2.
#### Double-Paire
```c#
       
```
Le test pour la Double-Paire utilise le compteur pour verifier le nombre de paires présentent dans le jeu.
#### Brelan
```c#
       
```
Le test pour le brelan verifie la présence ou non d'un 3 dans le tableau similaire.
#### Carre
```c#
       
```
Le test pour le carre, comme pour le brelan et la paire, verifie la présence ou non d'un 4 dans le tableau similaire.
#### Quinte
```c#
       
```
Le test pour la quinte commenece par verifier que tout les éléments sont bien différents puis les comparent au tableau ```quintes``` qui regroupe toutes les possibilités de quinte gagnante.
#### Full
```c#
       
```
Le test pour full verifie s'il y eu une paire et un brelan
#### Quinte Flush
```c#
       
```
Le test pour la Quinte Flush verifie que les 5 cartes ont la même famille puis qu'il y a une quinte
#### Couleur
```c#
        
```
Le test pour la Couleur verifie que les 5 cartes ont la même famille puis qu'il n'y a pas de quinte
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
