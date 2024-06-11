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
        private List<Viagem> todasViagens;

        public MinhasViagensPage()
        {
            InitializeComponent();
            caminhoBD = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "viagem.db3");
    
            conexao = new SQLiteConnection(caminhoBD);
            conexao.CreateTable<Viagem>();
            conexao.CreateTable<Itinerario>();
            ListarViagens();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListarViagens();
        }

        public void ListarViagens()
        {
            var viagens = conexao.Table<Viagem>().ToList();

            // Carregar itinerários associados a cada viagem
            foreach (var viagem in viagens)
            {
                viagem.Itinerarios = conexao.Table<Itinerario>().Where(i => i.ViagemId == viagem.Id).ToList();
            }

            CollectionViewControl.ItemsSource = viagens;
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
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

        private async void OnAdicionarItinerarioClicked(object sender, EventArgs e)
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
                ListarViagens(); // Refresh the list
            }
        }

        private async void OnEditarItinerarioClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itinerario = button?.BindingContext as Itinerario;

            if (itinerario != null)
            {
                // Open a prompt to edit the itinerario details
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
                ListarViagens(); // Recrrega a página de minhas viagens
            }
        }

        private async void OnRemoverItinerarioClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itinerario = button?.BindingContext as Itinerario;

            if (itinerario != null)
            {
                bool confirmar = await DisplayAlert("Remover Itinerário", "Deseja realmente remover este itinerário?", "Sim", "Não");
                if (confirmar)
                {
                    conexao.Delete(itinerario);
                    ListarViagens(); // Refresh the list
                }
            }
        }
    

private async void Editar_Clicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn != null && btn.BindingContext is Viagem viagem)
            {
                // Carregar itinerários associados à viagem
                viagem.Itinerarios = conexao.Table<Itinerario>().Where(i => i.ViagemId == viagem.Id).ToList();

                await Navigation.PushAsync(new EditarViagemPage(viagem));
            }
        }

        private async void Excluir_Clicked(object sender, EventArgs e)
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
