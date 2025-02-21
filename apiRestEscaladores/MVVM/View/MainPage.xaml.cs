using apiRestEscaladores.MVVM.ModelView;


namespace apiRestEscaladores
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
        }

    }
}