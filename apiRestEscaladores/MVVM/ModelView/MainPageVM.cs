// Importaciones necesarias
using apiRestEscaladores.MVVM.Modelo; // Referencia al modelo Escalador
using System.Collections.ObjectModel; // Para usar ObservableCollection
using System.Windows.Input; // Para los comandos (ICommand)

namespace apiRestEscaladores.MVVM.ModelView
{
    public class MainPageVM : BindableObject
    {
        // Servicio para interactuar con la API (GET, POST, PUT, DELETE)
        private readonly EscaladorServicio _servicio = new();

        // Lista observable de escaladores que se mostrará en la interfaz
        public ObservableCollection<Escalador> Escaladores { get; set; } = new();

        // Objeto para el formulario de agregar/editar escalador
        private Escalador _escaladorEditable = new();
        public Escalador EscaladorEditable
        {
            get => _escaladorEditable;
            set
            {
                _escaladorEditable = value;
                OnPropertyChanged(nameof(EscaladorEditable)); // Notifica a la vista que el objeto ha cambiado
                OnPropertyChanged(nameof(BotonTexto)); // Actualiza el texto del botón dinámicamente
            }
        }

        // Escalador seleccionado en la lista
        private Escalador _escaladorSeleccionado;
        public Escalador EscaladorSeleccionado
        {
            get => _escaladorSeleccionado;
            set
            {
                _escaladorSeleccionado = value;
                OnPropertyChanged(nameof(EscaladorSeleccionado)); // Notifica a la vista que cambió la selección
                OnPropertyChanged(nameof(PuedeEliminar)); // Actualiza la propiedad que habilita/deshabilita el botón de eliminar

                // Si se selecciona un escalador, copia sus datos al formulario (EscaladorEditable)
                EscaladorEditable = _escaladorSeleccionado != null
                    ? new Escalador
                    {
                        Id = _escaladorSeleccionado.Id,
                        Name = _escaladorSeleccionado.Name,
                        Age = _escaladorSeleccionado.Age,
                        MountainGroup = _escaladorSeleccionado.MountainGroup,
                        Experience = _escaladorSeleccionado.Experience,
                        Federado = _escaladorSeleccionado.Federado
                    }
                    : new Escalador(); // Si no hay selección, limpia el formulario
            }
        }

        // Propiedad que indica si el botón de eliminar debe estar habilitado
        public bool PuedeEliminar => EscaladorSeleccionado != null;

        // Comandos (acciones que se pueden ejecutar desde la interfaz)
        public ICommand CargarEscaladoresCommand { get; }
        public ICommand AgregarOEditarEscaladorCommand { get; }
        public ICommand EliminarEscaladorCommand { get; }
        public ICommand ResetearVistaCommand { get; }

        // Texto dinámico para el botón (cambia entre "Agregar" y "Editar" según el contexto)
        public string BotonTexto => !string.IsNullOrEmpty(EscaladorEditable?.Id) ? "Editar Escalador" : "Agregar Escalador";

        // Constructor: inicializa los comandos y carga la lista de escaladores
        public MainPageVM()
        {
            // Asigna las acciones a los comandos
            CargarEscaladoresCommand = new Command(async () => await CargarEscaladores());
            AgregarOEditarEscaladorCommand = new Command(async () => await AgregarOEditarEscalador());
            EliminarEscaladorCommand = new Command<Escalador>(async (escalador) => await EliminarEscalador(escalador));
            ResetearVistaCommand = new Command(() => ResetearVista());

            // Carga inicial de los escaladores (se ejecuta en segundo plano)
            Task.Run(async () => await CargarEscaladores());
        }

        // Método para cargar la lista de escaladores desde la API
        private async Task CargarEscaladores()
        {
            try
            {
                // Llama al servicio para obtener los escaladores
                var lista = await _servicio.GetEscaladoresAsync();

                // Limpia la colección actual para evitar duplicados
                Escaladores.Clear();

                // Si la lista no es nula, agrega cada escalador a la colección observable
                if (lista != null)
                {
                    foreach (var item in lista)
                        Escaladores.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje en consola si hay un error
                Console.WriteLine($"Error al cargar escaladores: {ex.Message}");
            }
        }

        // Método para agregar o editar un escalador
        private async Task AgregarOEditarEscalador()
        {
            // Validación básica del formulario
            if (string.IsNullOrWhiteSpace(EscaladorEditable.Name) || EscaladorEditable.Age <= 0)
            {
                Console.WriteLine("Por favor, completa todos los campos.");
                return;
            }

            bool exito; // Variable para saber si la operación fue exitosa

            if (string.IsNullOrEmpty(EscaladorEditable.Id))
            {
                // Si no tiene un ID, es un nuevo escalador (Agregar)
                exito = await _servicio.AddEscaladorAsync(EscaladorEditable);
            }
            else
            {
                // Si tiene un ID, es una edición (Actualizar)
                exito = await _servicio.UpdateEscaladorAsync(EscaladorEditable);
            }

            if (exito)
            {
                // Si la operación fue exitosa:
                Console.WriteLine("Operación exitosa.");
                await CargarEscaladores(); // Recarga la lista de escaladores
                ResetearVista(); // Limpia el formulario
            }
            else
            {
                Console.WriteLine("Error en la operación."); // Si falla, muestra un mensaje
            }
        }

        // Método para eliminar un escalador
        private async Task EliminarEscalador(Escalador escalador)
        {
            // Si no hay un escalador seleccionado, no hace nada
            if (escalador == null) return;

            // Muestra un cuadro de confirmación antes de eliminar
            bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar", $"¿Eliminar a {escalador.Name}?", "Sí", "No");
            if (!confirmar) return; // Si el usuario cancela, no se hace nada

            // Llama al servicio para eliminar el escalador por ID
            bool exito = await _servicio.DeleteEscaladorAsync(escalador.Id);

            if (exito)
            {
                // Si la eliminación es exitosa:
                await CargarEscaladores(); // Recarga la lista
                ResetearVista(); // Limpia la selección y el formulario
            }
            else
            {
                Console.WriteLine("Error al eliminar escalador."); // Si falla, muestra un mensaje
            }
        }

        // Método para limpiar el formulario y la selección actual
        private void ResetearVista()
        {
            EscaladorSeleccionado = null; // Quita el escalador seleccionado
            EscaladorEditable = new Escalador(); // Limpia el formulario
        }
    }
}
