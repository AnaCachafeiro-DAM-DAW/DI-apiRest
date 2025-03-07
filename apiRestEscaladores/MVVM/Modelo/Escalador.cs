using System.Text.Json.Serialization;

namespace apiRestEscaladores.MVVM.Modelo;


// tenemos que pegarlo del data del mockapi
public class Escalador
{

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
  
    [JsonPropertyName("mountainGroup")]
    public string MountainGroup { get; set; }
    
    [JsonPropertyName("experience")]
    public int Experience { get; set; }
   
    [JsonPropertyName("federado")]
    public bool Federado { get; set; }
    
    [JsonPropertyName("id")]
    public String Id { get; set; }

    public string FederadoTexto
    {
        get
        {
            return Federado ? "Sí" : "No";
        }
    }





}


