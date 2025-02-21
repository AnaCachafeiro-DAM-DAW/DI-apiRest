using apiRestEscaladores.MVVM.Modelo;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace apiRestEscaladores.MVVM.ModelView
{

    // aqui tendremos los commands para las llamadas
    public class MainPageVM : BindableObject
    {
        private readonly EscaladorServicio _servicio = new EscaladorServicio();

        public ObservableCollection<Escalador> Escaladores { get; set; } = new();
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

        public MainPageVM()
        {
            CargarEscaladoresCommand = new Command(async () => await CargarEscaladores());
            AgregarEscaladorCommand = new Command(async () => await AgregarEscalador());
            EditarEscaladorCommand = new Command(async () => await EditarEscalador());
            EliminarEscaladorCommand = new Command(async () => await EliminarEscalador());
        }

        private async Task CargarEscaladores()
        {
            var lista = await _servicio.GetEscaladoresAsync();
            Escaladores.Clear();
            foreach (var item in lista)
            {
                Escaladores.Add(item);
            }
        }

        private async Task AgregarEscalador()
        {
            var nuevoEscalador = new Escalador
            {
                Name = "Ana Cachafeiro",
                Age = 45,
                MountainGroup = "Grupo Montaña Ensidesa",
                Experience = 5,
                Federado = true
            };

            bool exito = await _servicio.AddEscaladorAsync(nuevoEscalador);
            if (exito)
            {
                await CargarEscaladores();
            }
        }

        private async Task EditarEscalador()
        {
            if (EscaladorSeleccionado == null) return;

            EscaladorSeleccionado.Name = "Elia Cachafeiro";  
            bool exito = await _servicio.UpdateEscaladorAsync(EscaladorSeleccionado);
            if (exito)
            {
                await CargarEscaladores();
            }
        }

        private async Task EliminarEscalador()
        {
            if (EscaladorSeleccionado == null) return;

            bool exito = await _servicio.DeleteEscaladorAsync(EscaladorSeleccionado.Id);  
            if (exito)
            {
                Escaladores.Remove(EscaladorSeleccionado);
            }
        }
    }
}
