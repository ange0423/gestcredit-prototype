namespace GestCredit.Prototype.Modeles;

public class Client
{
    public int Id { get; }
    public string Nom { get; }
    public string Ville { get; }

    // Constructeur reconnu automatiquement par System.Text.Json (paramètres = noms de propriétés)
    public Client(int id, string nom, string ville)
    {
        if (string.IsNullOrWhiteSpace(nom))
            throw new ArgumentException("Le nom du client est obligatoire.");

        Id = id;
        Nom = nom;
        Ville = ville;
    }

    public override string ToString() => $"[{Id}] {Nom} ({Ville})";
}