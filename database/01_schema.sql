-- ============================================================
-- GestCredit - Jour 12 : Script de création du schéma (DDL)
-- Idempotent : recrée la base entièrement à chaque exécution,
-- ce qui évite tout problème d'ordre de suppression lié aux FK.
-- ============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'GestCredit')
BEGIN
    -- Force la déconnexion de toute session active sur la base avant de la supprimer
    ALTER DATABASE GestCredit SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GestCredit;
END
GO

CREATE DATABASE GestCredit;
GO

USE GestCredit;
GO

-- ============================================================
-- Création des tables dans l'ordre des dépendances
-- (tables parentes d'abord, tables enfants ensuite)
-- ============================================================

-- Table Client
CREATE TABLE dbo.Client (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nom     NVARCHAR(150) NOT NULL,
    Ville   NVARCHAR(100) NOT NULL,
    Email   NVARCHAR(200) NULL,

    CONSTRAINT UQ_Client_Email UNIQUE (Email)
);
GO

-- Table Utilisateur
CREATE TABLE dbo.Utilisateur (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NomUtilisateur  NVARCHAR(100) NOT NULL,
    MotDePasseHash  NVARCHAR(300) NOT NULL,

    CONSTRAINT UQ_Utilisateur_Nom UNIQUE (NomUtilisateur)
);
GO

-- Table Role
CREATE TABLE dbo.Role (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Nom  NVARCHAR(50) NOT NULL,

    CONSTRAINT UQ_Role_Nom UNIQUE (Nom)
);
GO

-- Table de jonction UtilisateurRole (relation N-N)
CREATE TABLE dbo.UtilisateurRole (
    UtilisateurId  INT NOT NULL,
    RoleId         INT NOT NULL,

    CONSTRAINT PK_UtilisateurRole PRIMARY KEY (UtilisateurId, RoleId),

    CONSTRAINT FK_UtilisateurRole_Utilisateur
        FOREIGN KEY (UtilisateurId) REFERENCES dbo.Utilisateur(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_UtilisateurRole_Role
        FOREIGN KEY (RoleId) REFERENCES dbo.Role(Id)
        ON DELETE CASCADE
);
GO

-- Table DemandeCredit
CREATE TABLE dbo.DemandeCredit (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    ClientId       INT NOT NULL,
    Montant        DECIMAL(18,2) NOT NULL,
    TauxAnnuel     DECIMAL(5,2)  NOT NULL,
    DureeMois      INT NOT NULL,
    Statut         NVARCHAR(20) NOT NULL DEFAULT 'Brouillon',
    DateCreation   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_DemandeCredit_Client
        FOREIGN KEY (ClientId) REFERENCES dbo.Client(Id),

    CONSTRAINT CK_DemandeCredit_Montant CHECK (Montant > 0),
    CONSTRAINT CK_DemandeCredit_Duree CHECK (DureeMois BETWEEN 1 AND 360),
    CONSTRAINT CK_DemandeCredit_Statut CHECK (
        Statut IN ('Brouillon', 'Soumise', 'EnAnalyse', 'Approuvee', 'Rejetee')
    )
);
GO

-- Table Document
CREATE TABLE dbo.Document (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    DemandeId       INT NOT NULL,
    NomFichier      NVARCHAR(255) NOT NULL,
    CheminStockage  NVARCHAR(500) NOT NULL,
    DateAjout       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Document_DemandeCredit
        FOREIGN KEY (DemandeId) REFERENCES dbo.DemandeCredit(Id)
        ON DELETE CASCADE
);
GO

PRINT 'Schema GestCredit cree avec succes.';
GO
