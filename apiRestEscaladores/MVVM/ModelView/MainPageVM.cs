using apiRestEscaladores.MVVM.Modelo;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;

namespace apiRestEscaladores.MVVM.ModelView
{
    public class MainPageVM : BindableObject
    {
        private readonly EscaladorServicio _servicio = new();
        public ObservableCollection<Escalador> Escaladores { get; set; } = new();

        // Solo se usa para agregar nuevos escaladores
        public Escalador NuevoEscalador { get; set; }
        private Escalador _escaladorSeleccionado;

        public Escalador EscaladorSeleccionado
        {
            get => _escaladorSeleccionado;
            set
            {
                _escaladorSeleccionado = value;
                OnPropertyChanged(nameof(EscaladorSeleccionado));
                OnPropertyChanged(nameof(PuedeEliminar)); // Notifica cuando cambia la selección
            }
        }

        // Solo una vez: bool PuedeEliminar
        public bool PuedeEliminar => EscaladorSeleccionado != null;

        // Comandos
        public ICommand CargarEscaladoresCommand { get; }
        public ICommand AgregarOEditarEscaladorCommand { get; }
        public ICommand EliminarEscaladorCommand { get; }
        public ICommand SeleccionarEscaladorCommand { get; }
        public ICommand ResetearVistaCommand { get; }

        public string BotonTexto => EscaladorSeleccionado?.Id != null ? "Editar Escalador" : "Agregar Escalador";

        public MainPageVM()
        {
            // Inicialización de los comandos
            CargarEscaladoresCommand = new Command(async () => await CargarEscaladores());
            EliminarEscaladorCommand = new Command<Escalador>(async (escalador) => await EliminarEscalador(escalador));
            AgregarOEditarEscaladorCommand = new Command(async () => await AgregarOEditarEscalador());
            SeleccionarEscaladorCommand = new Command<Escalador>((escalador) =>
            {
                EscaladorSeleccionado = new Escalador
                {
                    Id = escalador.Id,
                    Name = escalador.Name,
                    Age = escalador.Age,
                    MountainGroup = escalador.MountainGroup,
                    Experience = escalador.Experience,
                    Federado = escalador.Federado
                };
            });
            ResetearVistaCommand = new Command(() => ResetearVista());

            // Inicialización de NuevoEscalador
            NuevoEscalador = new Escalador();
        }

        // Método para cargar escaladores
        private async Task CargarEscaladores()
        {
            try
            {
                // Obtener la lista actualizada desde el servicio MockAPI
                var lista = await _servicio.GetEscaladoresAsync();
                if (lista != null && lista.Count > 0)
                {
                    // Limpiar la lista local antes de agregar los nuevos datos
                    Escaladores.Clear();

                    // Agregar los escaladores de la respuesta del servidor
                    foreach (var item in lista)
                    {
                        Escaladores.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar escaladores: {ex.Message}");
            }
        }

        // Método para agregar un escalador
        private async Task AgregarEscalador()
        {
            if (NuevoEscalador != null)
            {
                bool exito = await _servicio.AddEscaladorAsync(NuevoEscalador);
                if (exito)
                {
                    // Si se agrega correctamente, recargamos la lista de escaladores
                    await CargarEscaladores();  // Recargamos la lista desde el servidor
                    NuevoEscalador = new Escalador(); // Limpiamos el formulario
                }
                else
                {
                    Console.WriteLine("Error al agregar escalador.");
                }
            }
        }

        // Método común para agregar o editar un escalador
        private async Task AgregarOEditarEscalador()
        {
            if (EscaladorSeleccionado != null)
            {
                if (!string.IsNullOrEmpty(EscaladorSeleccionado.Id))
                {
                    // Editar el escalador
                    var exito = await _servicio.UpdateEscaladorAsync(EscaladorSeleccionado);
                    if (exito)
                    {
                        // Recargamos la lista después de editar
                        await CargarEscaladores();
                        Console.WriteLine($"Escalador con ID {EscaladorSeleccionado.Id} editado exitosamente.");
                        ResetearVista(); // Limpiamos el formulario
                    }
                    else
                    {
                        Console.WriteLine("Error al editar escalador.");
                    }
                }
                else
                {
                    // Agregar un nuevo escalador
                    var exito = await _servicio.AddEscaladorAsync(EscaladorSeleccionado);
                    if (exito)
                    {
                        // Recargamos la lista después de agregar
                        await CargarEscaladores();
                        Console.WriteLine("Escalador agregado exitosamente.");
                        ResetearVista(); // Limpiamos el formulario
                    }
                    else
                    {
                        Console.WriteLine("Error al agregar escalador.");
                    }
                }
            }
        }

        // Método para eliminar el escalador seleccionado
        private async Task EliminarEscalador(Escalador escalador)
        {
            if (escalador == null) return;

            bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar", $"¿Eliminar a {escalador.Name}?", "Sí", "No");
            if (!confirmar) return;

            bool exito = await _servicio.DeleteEscaladorAsync(escalador.Id);
            if (exito)
            {
                // Recargamos la lista de escaladores después de la eliminación para asegurarnos de que todo esté sincronizado
                await CargarEscaladores();
            }
            else
            {
                Console.WriteLine("Error al eliminar escalador.");
            }
        }

        // Método para resetear el formulario
        private void ResetearVista()
        {
            // Limpiamos el formulario, ya sea con el objeto NuevoEscalador o EscaladorSeleccionado
            EscaladorSeleccionado = null; // Resetear la vista
            NuevoEscalador = new Escalador(); // Limpiamos los datos para agregar uno nuevo
        }
    }
}
