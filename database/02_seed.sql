-- ============================================================
-- GestCredit - Jour 12 : Script de données de test (seed)
-- Idempotent : nettoie puis réinsère des IDs explicites à
-- chaque exécution (via IDENTITY_INSERT), donc 100% déterministe.
-- ============================================================

USE GestCredit;
GO

-- Nettoyage dans l'ordre inverse des dépendances (FK)
DELETE FROM dbo.Document;
DELETE FROM dbo.DemandeCredit;
DELETE FROM dbo.UtilisateurRole;
DELETE FROM dbo.Role;
DELETE FROM dbo.Utilisateur;
DELETE FROM dbo.Client;
GO

-- ============================================================
-- 10 clients (IDs explicites 1 à 10)
-- ============================================================
SET IDENTITY_INSERT dbo.Client ON;

INSERT INTO dbo.Client (Id, Nom, Ville, Email) VALUES
    (1,  'Amadou Diallo',   'Dakar',       'amadou.diallo@mail.com'),
    (2,  'Fatou Ndiaye',    'Thies',       'fatou.ndiaye@mail.com'),
    (3,  'Moussa Kane',     'Dakar',       'moussa.kane@mail.com'),
    (4,  'Aissatou Ba',     'Saint-Louis', 'aissatou.ba@mail.com'),
    (5,  'Ibrahima Sow',    'Dakar',       'ibrahima.sow@mail.com'),
    (6,  'Mariam Cisse',    'Ziguinchor',  'mariam.cisse@mail.com'),
    (7,  'Ousmane Fall',    'Kaolack',     'ousmane.fall@mail.com'),
    (8,  'Aminata Diop',    'Thies',       'aminata.diop@mail.com'),
    (9,  'Cheikh Gueye',    'Dakar',       'cheikh.gueye@mail.com'),
    (10, 'Khady Sarr',      'Saint-Louis', 'khady.sarr@mail.com');

SET IDENTITY_INSERT dbo.Client OFF;
GO

-- ============================================================
-- Rôles (IDs explicites 1 à 3)
-- ============================================================
SET IDENTITY_INSERT dbo.Role ON;

INSERT INTO dbo.Role (Id, Nom) VALUES
    (1, 'Agent'),
    (2, 'Analyste'),
    (3, 'Administrateur');

SET IDENTITY_INSERT dbo.Role OFF;
GO

-- ============================================================
-- Utilisateurs (IDs explicites 1 à 3)
-- ============================================================
SET IDENTITY_INSERT dbo.Utilisateur ON;

INSERT INTO dbo.Utilisateur (Id, NomUtilisateur, MotDePasseHash) VALUES
    (1, 'agent1',    'HASH_FICTIF_1'),
    (2, 'analyste1', 'HASH_FICTIF_2'),
    (3, 'admin1',    'HASH_FICTIF_3');

SET IDENTITY_INSERT dbo.Utilisateur OFF;
GO

-- UtilisateurRole n'a pas de colonne IDENTITY (clé composite) : rien de spécial ici
-- admin1 cumule Agent + Administrateur (illustre la relation N-N)
INSERT INTO dbo.UtilisateurRole (UtilisateurId, RoleId) VALUES
    (1, 1),  -- agent1     -> Agent
    (2, 2),  -- analyste1  -> Analyste
    (3, 1),  -- admin1     -> Agent
    (3, 3);  -- admin1     -> Administrateur
GO

-- ============================================================
-- 25 demandes de crédit, réparties sur les 10 clients
-- ============================================================
INSERT INTO dbo.DemandeCredit (ClientId, Montant, TauxAnnuel, DureeMois, Statut) VALUES
    (1, 2000000, 12.0, 24, 'Approuvee'),
    (1, 500000,  10.5, 12, 'Rejetee'),
    (2, 3500000, 11.0, 36, 'Soumise'),
    (2, 1200000, 13.0, 18, 'EnAnalyse'),
    (3, 5000000, 12.0, 24, 'Approuvee'),
    (3, 800000,  9.5,  12, 'Brouillon'),
    (4, 4200000, 11.5, 48, 'Soumise'),
    (4, 1500000, 10.0, 24, 'Approuvee'),
    (5, 9000000, 13.5, 60, 'EnAnalyse'),
    (5, 700000,  9.0,  12, 'Rejetee'),
    (6, 2300000, 12.5, 24, 'Approuvee'),
    (6, 3100000, 11.0, 36, 'Soumise'),
    (7, 600000,  9.5,  12, 'Brouillon'),
    (7, 1800000, 10.5, 24, 'Approuvee'),
    (8, 4600000, 12.0, 36, 'EnAnalyse'),
    (8, 900000,  9.0,  12, 'Rejetee'),
    (9, 7200000, 13.0, 48, 'Soumise'),
    (9, 2100000, 11.5, 24, 'Approuvee'),
    (10,1000000, 10.0, 18, 'Brouillon'),
    (10,3300000, 12.0, 36, 'Approuvee'),
    (2, 5500000, 12.5, 48, 'Soumise'),
    (3, 1600000, 10.5, 24, 'Approuvee'),
    (5, 2900000, 11.0, 24, 'EnAnalyse'),
    (7, 4000000, 12.0, 36, 'Approuvee'),
    (9, 1100000, 9.5,  12, 'Rejetee');
GO

PRINT 'Donnees de test inserees avec succes : 10 clients, 25 demandes.';
GO
