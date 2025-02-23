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
                OnPropertyChanged();
            }
        }

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
            EditarEscaladorCommand = new Command(async () => await EditarEscalador());
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
            ///{
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
        private async Task EditarEscalador()
        {
            if (EscaladorSeleccionado == null) return;

            EscaladorSeleccionado.Name = "Elia Cachafeiro";
            bool exito = await _servicio.UpdateEscaladorAsync(EscaladorSeleccionado);
            if (exito)
            {
                Console.WriteLine("Escalador editado exitosamente.");
                await CargarEscaladores();
            }
        }

        //  M�todo para eliminar el escalador seleccionado
        private async Task EliminarEscalador()
        {
            if (EscaladorSeleccionado == null) return;

            bool exito = await _servicio.DeleteEscaladorAsync(EscaladorSeleccionado.Id);
            if (exito)
            {
                Console.WriteLine("Escalador eliminado exitosamente.");
                Escaladores.Remove(EscaladorSeleccionado);
            }
        }
    }
}