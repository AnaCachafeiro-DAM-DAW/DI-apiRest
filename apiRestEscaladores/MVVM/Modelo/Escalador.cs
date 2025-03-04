namespace apiRestEscaladores.MVVM.Modelo;


// tenemos que pegarlo del data del mockapi
public class Escalador
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string MountainGroup { get; set; }
    public int Experience { get; set; }
    public bool Federado { get; set; }
    public String Id { get; set; }

    public string FederadoTexto
    {
        get
        {
            return Federado ? "Sí" : "No";
        }
    }

    
}


