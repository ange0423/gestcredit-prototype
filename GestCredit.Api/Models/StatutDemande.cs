namespace GestCredit.Api.Models;

// Reprend le workflow du Jour 8 (C#) et la contrainte CHECK du Jour 12 (SQL) :
// Brouillon -> Soumise -> EnAnalyse -> Approuvee / Rejetee
public enum StatutDemande
{
    Brouillon,
    Soumise,
    EnAnalyse,
    Approuvee,
    Rejetee
}