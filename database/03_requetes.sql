-- ============================================================
-- GestCredit - Jour 13 : Requêtes DML (03_requetes.sql)
-- 12 requêtes commentées sur le schéma des Jours 11-12.
-- ============================================================

USE GestCredit;
GO

-- 1. Nombre de demandes par client, y compris les clients sans aucune demande.
--    LEFT JOIN indispensable : un INNER JOIN exclurait les clients à 0 demande.
SELECT
    c.Id,
    c.Nom,
    COUNT(d.Id) AS NombreDemandes
FROM dbo.Client c
LEFT JOIN dbo.DemandeCredit d ON d.ClientId = c.Id
GROUP BY c.Id, c.Nom
ORDER BY NombreDemandes DESC;
GO

-- 2. Clients sans aucune demande (variante de la requête précédente,
--    utile isolément pour une relance commerciale par exemple).
SELECT
    c.Id,
    c.Nom,
    c.Ville
FROM dbo.Client c
LEFT JOIN dbo.DemandeCredit d ON d.ClientId = c.Id
WHERE d.Id IS NULL;
GO

-- 3. Encours (somme des montants) par statut.
SELECT
    Statut,
    COUNT(*) AS NombreDemandes,
    SUM(Montant) AS Encours
FROM dbo.DemandeCredit
GROUP BY Statut
ORDER BY Encours DESC;
GO

-- 4. Les 5 plus gros dossiers, avec le nom du client (INNER JOIN :
--    une demande a toujours un client, pas besoin de LEFT JOIN ici).
SELECT TOP (5)
    d.Id AS DemandeId,
    c.Nom AS Client,
    d.Montant,
    d.Statut
FROM dbo.DemandeCredit d
INNER JOIN dbo.Client c ON c.Id = d.ClientId
ORDER BY d.Montant DESC;
GO

-- 5. Taux d'approbation global, sur les dossiers déjà tranchés
--    (Approuvee ou Rejetee ; on exclut Brouillon/Soumise/EnAnalyse, pas encore décidés).
SELECT
    CAST(
        100.0 * SUM(CASE WHEN Statut = 'Approuvee' THEN 1 ELSE 0 END)
        / NULLIF(SUM(CASE WHEN Statut IN ('Approuvee', 'Rejetee') THEN 1 ELSE 0 END), 0)
    AS DECIMAL(5,2)) AS TauxApprobationPourcent
FROM dbo.DemandeCredit;
GO

-- 6. Dossiers en analyse depuis plus de 30 jours (alerte de suivi).
SELECT
    d.Id,
    c.Nom AS Client,
    d.Montant,
    d.DateCreation,
    DATEDIFF(DAY, d.DateCreation, SYSUTCDATETIME()) AS JoursEnAnalyse
FROM dbo.DemandeCredit d
INNER JOIN dbo.Client c ON c.Id = d.ClientId
WHERE d.Statut = 'EnAnalyse'
  AND DATEDIFF(DAY, d.DateCreation, SYSUTCDATETIME()) > 30;
GO

-- 7. Montant moyen des demandes par ville (jointure + agrégation).
SELECT
    c.Ville,
    AVG(d.Montant) AS MontantMoyen,
    COUNT(d.Id) AS NombreDemandes
FROM dbo.Client c
INNER JOIN dbo.DemandeCredit d ON d.ClientId = c.Id
GROUP BY c.Ville
ORDER BY MontantMoyen DESC;
GO

-- 8. Liste des utilisateurs avec leurs rôles cumulés (illustre la relation N-N).
--    LEFT JOIN : un utilisateur sans rôle attribué doit quand même apparaître.
SELECT
    u.NomUtilisateur,
    STRING_AGG(r.Nom, ', ') AS Roles
FROM dbo.Utilisateur u
LEFT JOIN dbo.UtilisateurRole ur ON ur.UtilisateurId = u.Id
LEFT JOIN dbo.Role r ON r.Id = ur.RoleId
GROUP BY u.NomUtilisateur;
GO

-- 9. Clients ayant plus d'une demande (HAVING filtre après agrégation,
--    contrairement à WHERE qui filtre avant).
SELECT
    c.Nom,
    COUNT(d.Id) AS NombreDemandes
FROM dbo.Client c
INNER JOIN dbo.DemandeCredit d ON d.ClientId = c.Id
GROUP BY c.Nom
HAVING COUNT(d.Id) > 1
ORDER BY NombreDemandes DESC;
GO

-- 10. Détail complet d'une demande avec son client et ses documents éventuels.
--     LEFT JOIN sur Document : une demande peut n'avoir aucune pièce jointe.
SELECT
    d.Id AS DemandeId,
    c.Nom AS Client,
    d.Montant,
    d.Statut,
    doc.NomFichier,
    COALESCE(doc.DateAjout, d.DateCreation) AS DateReference -- exemple d'usage de COALESCE
FROM dbo.DemandeCredit d
INNER JOIN dbo.Client c ON c.Id = d.ClientId
LEFT JOIN dbo.Document doc ON doc.DemandeId = d.Id
ORDER BY d.Id;
GO

-- 11. Répartition des demandes par tranche de montant.
SELECT
    CASE
        WHEN Montant < 1000000 THEN 'Micro-crédit (< 1M)'
        WHEN Montant < 5000000 THEN 'Standard (1M - 5M)'
        ELSE 'Grand compte (>= 5M)'
    END AS Tranche,
    COUNT(*) AS NombreDemandes,
    SUM(Montant) AS EncoursTotal
FROM dbo.DemandeCredit
GROUP BY
    CASE
        WHEN Montant < 1000000 THEN 'Micro-crédit (< 1M)'
        WHEN Montant < 5000000 THEN 'Standard (1M - 5M)'
        ELSE 'Grand compte (>= 5M)'
    END;
GO

-- 12. Dernière demande de chaque client (sous-requête EXISTS :
--     ici, la demande dont la date de création est la plus récente pour ce client).
SELECT
    c.Nom,
    d.Id AS DemandeId,
    d.Montant,
    d.DateCreation
FROM dbo.DemandeCredit d
INNER JOIN dbo.Client c ON c.Id = d.ClientId
WHERE EXISTS (
    SELECT 1
    FROM dbo.DemandeCredit d2
    WHERE d2.ClientId = d.ClientId
    HAVING MAX(d2.DateCreation) = d.DateCreation
    -- Alternative sans EXISTS/HAVING : NOT EXISTS (SELECT 1 FROM DemandeCredit d3
    -- WHERE d3.ClientId = d.ClientId AND d3.DateCreation > d.DateCreation)
)
ORDER BY c.Nom;
GO

-- ============================================================
-- Rappel du danger d'un UPDATE/DELETE sans WHERE (pour mémoire,
-- ne pas exécuter) :
--   UPDATE dbo.DemandeCredit SET Statut = 'Rejetee';  -- modifie TOUTES les lignes
-- Toujours vérifier avec un SELECT utilisant le même WHERE avant
-- de transformer la requête en UPDATE ou DELETE.
-- ============================================================
