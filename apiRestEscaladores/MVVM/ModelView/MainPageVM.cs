using apiRestEscaladores.MVVM.Modelo;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace apiRestEscaladores.MVVM.ModelView
{

    // aqui tendremos los commands para las llamadas
    public class MainPageVM : BindableObject
    {
        private readonly EscaladorServicio _servicio = new();
        public ObservableCollection<Escalador> Escaladores { get; set; } = new();


        public Escalador NuevoEscalador { get; set; }
        private Escalador _escaladorSeleccionado;

        public Escalador EscaladorSeleccionado
        {
            get => _escaladorSeleccionado;
            set
            {
                _escaladorSeleccionado = value;
                OnPropertyChanged(nameof(EscaladorSeleccionado));
                OnPropertyChanged(nameof(PuedeEliminar)); // Notificar si el bot�n debe habilitarse
            }
        }

        public bool PuedeEliminar => EscaladorSeleccionado != null;

  


        public ICommand CargarEscaladoresCommand { get; }
        public ICommand AgregarEscaladorCommand { get; }
        public ICommand EditarEscaladorCommand { get; }
        public ICommand EliminarEscaladorCommand { get; }
        public ICommand ResetearVistaCommand { get; }

        public MainPageVM()
        {
            //  Inicializaci�n de los comandos
            CargarEscaladoresCommand = new Command(async () => await CargarEscaladores());
            AgregarEscaladorCommand = new Command(async () => await AgregarEscalador());
            EditarEscaladorCommand = new Command<Escalador>(async (escalador) => await EditarEscalador(escalador));

            EliminarEscaladorCommand = new Command(async () => await EliminarEscalador());
            ResetearVistaCommand = new Command(() => EscaladorSeleccionado = null);
            NuevoEscalador = new Escalador();
        }

        // M�todo para cargar escaladores
        private async Task CargarEscaladores()
        {
            try
            {
                var lista = await _servicio.GetEscaladoresAsync();

                // Si la lista est� vac�a, no la agregamos
                if (lista != null && lista.Count > 0)
                {
                    Escaladores.Clear();
                    foreach (var item in lista)
                    {
                        Escaladores.Add(item);  // A�adimos los escaladores a la colecci�n
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar escaladores: {ex.Message}");
            }
        }

        //  M�todo para agregar un escalador


        private async Task AgregarEscalador()
        {
            //var nuevoEscalador = new Escalador
            //{
            ///  Name = "Ana Cachafeiro",
            //   Age = 45,
            //   MountainGroup = "Grupo Monta�a Ensidesa",
            //   Experience = 3,
            //    Federado = true
            // };

            //  bool exito = await _servicio.AddEscaladorAsync(nuevoEscalador);
            //  if (exito)
            //  {
            // Recargar los escaladores para actualizar la lista
            // await CargarEscaladores();
            // }
            // }
            if (NuevoEscalador != null)
            {
                bool exito = await _servicio.AddEscaladorAsync(NuevoEscalador);
                if (exito)
                {
                    // Recargar la lista de escaladores despu�s de agregar el nuevo escalador
                    await CargarEscaladores();

                    // Limpiar los campos despu�s de agregar el escalador
                    NuevoEscalador = new Escalador(); // Reiniciar la propiedad
                }
            }
        }

        //  M�todo para editar el escalador seleccionado
        //private async Task EditarEscalador(object sender)
         private async Task EditarEscalador(Escalador escalador)
        {
            //Button bt= (Button) sender;
            //bt.CommandParameter

            if (escalador == null)
            {
                Console.WriteLine("Error, escalador es nulo.");
                return;
            }

            // Ejemplo de edici�n
            //EscaladorSeleccionado.Name = "Elia Cachafeiro";

            bool exito = await _servicio.UpdateEscaladorAsync(escalador);
            if (exito)
            {
                Console.WriteLine($"Escalador {escalador.Name} acctualizado correctamente.");
                await CargarEscaladores();
            }
            else
            {
                Console.WriteLine("Error al actualizar el escalador.");
            }
        }

        //  M�todo para eliminar el escalador seleccionado
        private async Task EliminarEscalador()
        {
           // if (EscaladorSeleccionado == null)
           // {
           //     Console.WriteLine("No hay escalador seleccionado.");
           //     return;
          //  }

            try
            {
                bool exito = await _servicio.DeleteEscaladorAsync(EscaladorSeleccionado.Id);

                if (exito)
                {
                    Console.WriteLine($"Escalador {EscaladorSeleccionado.Name} eliminado exitosamente.");

                    // Eliminar del ObservableCollection
                    Escaladores.Remove(EscaladorSeleccionado);

                    // Limpiar la selecci�n
                    EscaladorSeleccionado = null;
                }
                else
                {
                    Console.WriteLine("Error al eliminar el escalador en el servicio.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepci�n al eliminar escalador: {ex.Message}");
            }
        }

    }
}