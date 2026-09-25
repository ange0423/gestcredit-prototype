-- ============================================================
-- GestCredit - Jour 14 : Vues, procédures, index, transactions
-- ============================================================

USE GestCredit;
GO

-- ============================================================
-- 1. VUE : vw_DemandesDetaillees
--    Regroupe une demande avec les infos client utiles, pour éviter
--    de réécrire la jointure Client/DemandeCredit dans chaque requête
--    ou chaque écran de l'application.
-- ============================================================
CREATE OR ALTER VIEW dbo.vw_DemandesDetaillees AS
SELECT
    d.Id            AS DemandeId,
    d.Montant,
    d.TauxAnnuel,
    d.DureeMois,
    d.Statut,
    d.DateCreation,
    c.Id            AS ClientId,
    c.Nom           AS ClientNom,
    c.Ville         AS ClientVille
FROM dbo.DemandeCredit d
INNER JOIN dbo.Client c ON c.Id = d.ClientId;
GO

-- Utilisation :
-- SELECT * FROM dbo.vw_DemandesDetaillees WHERE Statut = 'EnAnalyse';


-- ============================================================
-- 2. PROCÉDURE STOCKÉE : sp_SoumettreDemande
--    Crée une nouvelle demande directement au statut 'Soumise'.
--    Transactionnelle : si le client n'existe pas, ROLLBACK immédiat,
--    aucune ligne n'est insérée. Les contraintes CHECK du Jour 12
--    (Montant > 0, DureeMois entre 1 et 360) protègent déjà la donnée,
--    mais on vérifie ici explicitement l'existence du client, qui ne
--    peut pas être exprimée par une simple contrainte CHECK.
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.sp_SoumettreDemande
    @ClientId   INT,
    @Montant    DECIMAL(18,2),
    @TauxAnnuel DECIMAL(5,2),
    @DureeMois  INT,
    @NouvelId   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- toute erreur runtime annule automatiquement la transaction

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Client WHERE Id = @ClientId)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, 'Client introuvable : aucune demande créée.', 1;
        RETURN;
    END

    INSERT INTO dbo.DemandeCredit (ClientId, Montant, TauxAnnuel, DureeMois, Statut)
    VALUES (@ClientId, @Montant, @TauxAnnuel, @DureeMois, 'Soumise');

    SET @NouvelId = SCOPE_IDENTITY();

    COMMIT TRANSACTION;
END
GO

-- Test 1 : client existant (Id 1) -> doit réussir et renvoyer un nouvel Id
DECLARE @Id1 INT;
EXEC dbo.sp_SoumettreDemande
    @ClientId = 1, @Montant = 1500000, @TauxAnnuel = 11.5, @DureeMois = 24,
    @NouvelId = @Id1 OUTPUT;
SELECT @Id1 AS NouvelIdCree;
GO

-- Test 2 : client inexistant (Id 9999) -> doit lever une erreur, 0 ligne insérée
DECLARE @Id2 INT;
DECLARE @NbAvant INT = (SELECT COUNT(*) FROM dbo.DemandeCredit);

BEGIN TRY
    EXEC dbo.sp_SoumettreDemande
        @ClientId = 9999, @Montant = 1000000, @TauxAnnuel = 10, @DureeMois = 12,
        @NouvelId = @Id2 OUTPUT;
END TRY
BEGIN CATCH
    PRINT 'Erreur attendue : ' + ERROR_MESSAGE();
END CATCH

DECLARE @NbApres INT = (SELECT COUNT(*) FROM dbo.DemandeCredit);
SELECT @NbAvant AS AvantAppel, @NbApres AS ApresAppel; -- doivent être égaux
GO


-- ============================================================
-- 3. INDEX (justifiés)
-- ============================================================

-- Index sur DemandeCredit.ClientId : c'est la clé étrangère la plus
-- filtrée/jointe de tout le schéma (vw_DemandesDetaillees, requêtes du
-- Jour 13 "demandes par client", "clients sans demande"...). Sans index,
-- chaque jointure Client -> DemandeCredit fait un scan complet de la table.
-- Coût en écriture : faible, DemandeCredit reçoit des insertions mais peu
-- de mises à jour massives.
CREATE NONCLUSTERED INDEX IX_DemandeCredit_ClientId
    ON dbo.DemandeCredit (ClientId);
GO

-- Index sur DemandeCredit.Statut : utilisé dans quasiment tous les
-- rapports (encours par statut, dossiers en analyse > 30 jours, tableau
-- de bord). Cardinalité faible (5 valeurs), donc gain surtout utile
-- combiné à un filtre supplémentaire (ex. Statut + DateCreation) ;
-- on l'ajoute seul ici car c'est la colonne la plus filtrée isolément.
-- Coût en écriture : chaque ChangerStatut() (Jour 8 côté C#) met à jour
-- cette colonne, donc l'index a un coût de maintenance réel à surveiller
-- si le volume grossit beaucoup - acceptable au stade actuel du projet.
CREATE NONCLUSTERED INDEX IX_DemandeCredit_Statut
    ON dbo.DemandeCredit (Statut);
GO

PRINT 'Vue, procédure et index créés avec succès.';
GO
