using SQLite;
namespace PlanejamentoDeViagem;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void BtnCadastrarViagem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroViagemPage());
    }

    private async void BtnMinhasViagens_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MinhasViagensPage());
    }
}
