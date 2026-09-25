# GestCredit - Base de données

Base SQL Server du projet fil rouge GestCredit (Semaine 3 - modélisation et SQL).

## Prérequis

- SQL Server (local, LocalDB ou SQL Server Express) accessible depuis SSMS ou Azure Data Studio
- Un utilisateur disposant des droits de création/suppression de base de données

## Ordre d'exécution

Exécuter les scripts **dans cet ordre exact**, chacun en entier :

1. **`01_schema.sql`**
   Supprime la base `GestCredit` si elle existe déjà, la recrée, puis crée toutes les
   tables avec leurs contraintes (clés primaires, clés étrangères, `CHECK`, `UNIQUE`).
   Idempotent : peut être relancé à tout moment sans erreur.

2. **`02_seed.sql`**
   Vide les tables puis insère les données de test : 10 clients, 3 rôles, 3 utilisateurs,
   leurs attributions de rôles, et 25 demandes de crédit. Idempotent également
   (nettoyage avant insertion, IDs explicites via `IDENTITY_INSERT`).

3. **`03_requetes.sql`**
   12 requêtes de lecture commentées (pas de modification de données). Peut être
   exécuté à volonté pour explorer les données.

4. **`04_vues_procedures_index.sql`**
   Crée la vue `vw_DemandesDetaillees`, la procédure stockée `sp_SoumettreDemande`,
   et deux index. Inclut deux appels de test de la procédure (un cas valide, un cas
   client inexistant) qui affichent leur résultat dans l'onglet Messages/Résultats.

## Vérifier que tout fonctionne

Après le script 4, la sortie doit montrer :
- Un nouvel `Id` de demande pour le premier appel de test (client existant)
- Un message d'erreur pour le second appel (client inexistant)
- `AvantAppel` = `ApresAppel` dans le second test (aucune ligne insérée)

## Documentation du modèle

Voir `modele-donnees.md` pour le schéma conceptuel (diagramme Mermaid), le détail des
relations (dont la relation N-N Utilisateur/Role) et la justification de la 3e forme
normale.

## Reconstruire la base de zéro

Il suffit de relancer les 4 scripts dans l'ordre ci-dessus - `01_schema.sql` supprime
et recrée systématiquement la base, donc aucune étape manuelle de nettoyage n'est
nécessaire au préalable.
