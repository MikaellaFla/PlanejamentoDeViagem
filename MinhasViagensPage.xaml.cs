using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanejamentoDeViagem
{
    public partial class MinhasViagensPage : ContentPage
    {
        private SQLiteConnection conexao;
        string caminhoBD;  //caminho do banco
        private List<Viagem> todasViagens; // Lista de viagens cadastradas

        public MinhasViagensPage()
        {
            InitializeComponent();
            caminhoBD = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "viagem.db3");
    
            conexao = new SQLiteConnection(caminhoBD);
            conexao.CreateTable<Viagem>(); // Criação da tabela de viagens
            conexao.CreateTable<Itinerario>(); // Criação da tabela de itinerários
            ListarViagens(); // Chamada da função de listagem das viagens cadastradas
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListarViagens();
        }

        public void ListarViagens()
        {
            int usuarioId = Preferences.Get("UsuarioLogadoId", -1);
            if (usuarioId == -1)
            {
                DisplayAlert("Erro", "Usuário não logado", "OK");
                return;
            }
            var viagens = conexao.Table<Viagem>().Where(v => v.UsuarioId == usuarioId).ToList();

            // Carregar itinerários associados a cada viagem
            foreach (var viagem in viagens)
            {
                viagem.Itinerarios = conexao.Table<Itinerario>().Where(i => i.ViagemId == viagem.Id).ToList();
            }

            CollectionViewControl.ItemsSource = viagens;
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e) // Método da barra de pesquisa
        {
            var searchText = e.NewTextValue;
            var viagens = conexao.Table<Viagem>().ToList();

            // Filtrar viagens pelo destino
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                viagens = viagens.Where(v => v.Destino.ToLower().Contains(searchText.ToLower())).ToList();
            }

            // Carregar itinerários associados a cada viagem
            foreach (var viagem in viagens)
            {
                viagem.Itinerarios = conexao.Table<Itinerario>().Where(i => i.ViagemId == viagem.Id).ToList();
            }

            CollectionViewControl.ItemsSource = viagens;
        }

        private async void OnAdicionarItinerarioClicked(object sender, EventArgs e) // Método do botão de adicionar itinerário
        {
            var button = sender as Button;
            var viagem = button?.BindingContext as Viagem;

            if (viagem != null)
            {
                var novoItinerario = new Itinerario
                {
                    ViagemId = viagem.Id,
                    Titulo = "Novo Itinerário",
                    Data = DateTime.Now,
                    Hora = DateTime.Now.TimeOfDay,
                    Local = "Local"
                };

                viagem.Itinerarios.Add(novoItinerario);
                conexao.Insert(novoItinerario);
                ListarViagens(); // Recarrega e atualiza a viagem que recebeu um novo itinerário
            }
        }

        private async void OnEditarItinerarioClicked(object sender, EventArgs e) // Método chamada ao clicar no botão de editar do itinerário
        {
            var button = sender as Button;
            var itinerario = button?.BindingContext as Itinerario;

            if (itinerario != null)
            {
                // Abre um prompt para editar as informações do itinerário
                itinerario.Titulo = await DisplayPromptAsync("Editar Itinerário", "Título:", initialValue: itinerario.Titulo);
                itinerario.Local = await DisplayPromptAsync("Editar Itinerário", "Local:", initialValue: itinerario.Local);
                string dataString = await DisplayPromptAsync("Editar Itinerário", "Data (yyyy-MM-dd):", initialValue: itinerario.Data.ToString("yyyy-MM-dd"));
                string horaString = await DisplayPromptAsync("Editar Itinerário", "Hora (HH:mm):", initialValue: itinerario.Hora.ToString(@"hh\:mm"));

                if (DateTime.TryParse(dataString, out DateTime data))
                {
                    itinerario.Data = data;
                }

                if (TimeSpan.TryParse(horaString, out TimeSpan hora))
                {
                    itinerario.Hora = hora;
                }

                conexao.Update(itinerario);
                ListarViagens(); // Recarrega a página de minhas viagens
            }
        }

        private async void OnRemoverItinerarioClicked(object sender, EventArgs e) // Método do botão remover do itinerário
        {
            var button = sender as Button;
            var itinerario = button?.BindingContext as Itinerario;

            if (itinerario != null)
            {
                bool confirmar = await DisplayAlert("Remover Itinerário", "Deseja realmente remover este itinerário?", "Sim", "Não");
                if (confirmar)
                {
                    conexao.Delete(itinerario);
                    ListarViagens(); // Atualiza e recarrega a lista de viagens
                }
            }
        }
    

private async void Editar_Clicked(object sender, EventArgs e) // Método do botão de editar de uma viagem
        {
            var btn = (Button)sender;
            if (btn != null && btn.BindingContext is Viagem viagem)
            {
                // Carregar itinerários associados à viagem
                viagem.Itinerarios = conexao.Table<Itinerario>().Where(i => i.ViagemId == viagem.Id).ToList();

                await Navigation.PushAsync(new EditarViagemPage(viagem));
            }
        }

        private async void Excluir_Clicked(object sender, EventArgs e) // Método do botão de excluir de uma viagem
        {
            var btn = (Button)sender;
            if ((btn != null) && (btn.BindingContext is Viagem v))
            {
                bool res = await DisplayAlert("Excluir", "Deseja realmente excluir " +
                    "a viagem à " + v.Destino + " de " + v.Transporte + "?", "Sim", "Não");
                if (res)
                {
                    int id = Convert.ToInt32(v.Id);
                    conexao.Delete<Viagem>(id);
                    ListarViagens();
                }
            }
        }
    }
}
