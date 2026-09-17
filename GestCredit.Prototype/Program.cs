// GestCredit.Prototype - Jour 6
// Classes Client et DemandeCredit avec encapsulation

var client = new Client("Amadou Diallo", "Dakar");
Console.WriteLine($"Client créé : {client}");

try
{
    var demande = new DemandeCredit(client, montant: 5_000_000m, tauxAnnuel: 12m, dureeMois: 24);
    Console.WriteLine($"Mensualité : {demande.Mensualite:N2} FCFA");
    Console.WriteLine($"Coût total : {demande.CoutTotal:N2} FCFA");
    Console.WriteLine($"Intérêts   : {demande.Interets:N2} FCFA");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Erreur : {ex.Message}");
}

// Test volontaire d'un montant invalide pour vérifier que le constructeur protège bien la classe
try
{
    var demandeInvalide = new DemandeCredit(client, montant: -1000m, tauxAnnuel: 12m, dureeMois: 24);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Erreur attendue : {ex.Message}");
}


// ===================== Classes =====================

class Client
{
    // ID généré automatiquement, exposé en lecture seule
    private static int _prochainId = 1;

    public int Id { get; }
    public string Nom { get; }
    public string Ville { get; }

    public Client(string nom, string ville)
    {
        if (string.IsNullOrWhiteSpace(nom))
            throw new ArgumentException("Le nom du client est obligatoire.");

        Id = _prochainId++;
        Nom = nom;
        Ville = ville;
    }

    public override string ToString() => $"[{Id}] {Nom} ({Ville})";
}

class DemandeCredit
{
    public Client Client { get; }
    public decimal Montant { get; }
    public decimal TauxAnnuel { get; }
    public int DureeMois { get; }

    // Propriétés calculées : PAS de champ stocké, PAS de set public.
    // Elles se recalculent à chaque lecture à partir de Montant, TauxAnnuel, DureeMois.
    public decimal Mensualite
    {
        get
        {
            decimal tauxMensuel = TauxAnnuel / 12 / 100;
            double facteur = 1 - Math.Pow(1 + (double)tauxMensuel, -DureeMois);
            return Montant * tauxMensuel / (decimal)facteur;
        }
    }

    public decimal CoutTotal => Mensualite * DureeMois; // syntaxe expression-bodied, équivalente à un get { }

    public decimal Interets => CoutTotal - Montant;

    public DemandeCredit(Client client, decimal montant, decimal tauxAnnuel, int dureeMois)
    {
        if (montant <= 0)
            throw new ArgumentException("Le montant doit être positif.", nameof(montant));

        if (tauxAnnuel <= 0)
            throw new ArgumentException("Le taux doit être positif.", nameof(tauxAnnuel));

        if (dureeMois <= 0)
            throw new ArgumentException("La durée doit être positive.", nameof(dureeMois));

        Client = client;
        Montant = montant;
        TauxAnnuel = tauxAnnuel;
        DureeMois = dureeMois;
    }
}