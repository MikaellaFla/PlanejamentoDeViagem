using SQLite;
namespace PlanejamentoDeViagem;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void BtnCadastrarViagem_Clicked(object sender, EventArgs e) // Ação do botão de cadastrar viagem, levando o usuario a pagina de formulario para cadastro 
    {
        await Navigation.PushAsync(new CadastroViagemPage());
    }

    private async void BtnMinhasViagens_Clicked(object sender, EventArgs e) // Ação do botão de minhas viagens, levando o usuário a pagina de de viagens cadastradas em seu login
    {
        await Navigation.PushAsync(new MinhasViagensPage());
    }

    private async void OnLogoutClicked(object sender, EventArgs e) // Ação do botão logout, desconectando-o da sessão atual, levando-o de volta à welcomepage
    {
        Preferences.Remove("UsuarioLogadoId");
        Application.Current.MainPage = new NavigationPage(new WelcomePage());
    }
}
