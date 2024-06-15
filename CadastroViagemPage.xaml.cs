using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlanejamentoDeViagem
{
    public partial class CadastroViagemPage : ContentPage
    {
        private SQLiteConnection conexao;
        string caminhoBD;  //caminho do banco
        private List<Itinerario> itinerarios;

        public CadastroViagemPage()
        {
            InitializeComponent();
            caminhoBD = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "viagem.db3");
            conexao = new SQLiteConnection(caminhoBD);
            conexao.CreateTable<Viagem>();
            conexao.CreateTable<Itinerario>();
            itinerarios = new List<Itinerario>();
        }

        private void OnPickerTransporteSelectedIndexChanged(object sender, EventArgs e)
        {
            if (PickerTransporte.SelectedItem.ToString() == "Avião")
            {
                AeroportoInfo.IsVisible = true;
            }
            else
            {
                AeroportoInfo.IsVisible = false;
            }
        }

        private void OnAdicionarItinerarioClicked(object sender, EventArgs e)
        {
            var itineraryLayout = new StackLayout { Padding = 5 };

            var titleEntry = new Entry { Placeholder = "Título do Itinerário" };
            var datePicker = new DatePicker();
            var timePicker = new TimePicker();
            var locationEntry = new Entry { Placeholder = "Local" };

            itineraryLayout.Children.Add(titleEntry);
            itineraryLayout.Children.Add(datePicker);
            itineraryLayout.Children.Add(timePicker);
            itineraryLayout.Children.Add(locationEntry);

            ItineraryStackLayout.Children.Insert(ItineraryStackLayout.Children.Count - 1, itineraryLayout);
        }

        private async void OnCadastrarViagemClicked(object sender, EventArgs e)
        {
            string destino = TxtDestino.Text;
            DateTime dataIda = DataIda.Date;
            DateTime dataVolta = DataVolta.Date;
            string motivo = PickerMotivo.SelectedItem?.ToString();
            string transporte = PickerTransporte.SelectedItem?.ToString();
            string estadia = TxtEstadia.Text;
            string codigoPassagem = TxtCodigoPassagem.Text;
            string codigoReserva = TxtCodigoReserva.Text;

            if (string.IsNullOrWhiteSpace(destino) ||
                string.IsNullOrWhiteSpace(motivo) ||
                string.IsNullOrWhiteSpace(transporte) ||
                string.IsNullOrWhiteSpace(estadia) ||
                string.IsNullOrWhiteSpace(codigoPassagem) ||
                string.IsNullOrWhiteSpace(codigoReserva))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (!int.TryParse(codigoPassagem, out _) || !int.TryParse(codigoReserva, out _))
            {
                await DisplayAlert("Erro", "Código da passagem e Código da reserva devem conter apenas números.", "OK");
                return;
            }

            string aeroportoIda = null;
            string aeroportoChegada = null;
            string ciaAerea = null;

            if (transporte == "Avião")
            {
                aeroportoIda = TxtAeroportoIda.Text;
                aeroportoChegada = TxtAeroportoChegada.Text;
                ciaAerea = TxtCiaAerea.Text;

                if (string.IsNullOrWhiteSpace(aeroportoIda) ||
                    string.IsNullOrWhiteSpace(aeroportoChegada) ||
                    string.IsNullOrWhiteSpace(ciaAerea))
                {
                    await DisplayAlert("Erro", "Por favor, preencha todos os campos do avião.", "OK");
                    return;
                }
            }

            if (!Preferences.ContainsKey("UsuarioLogadoId"))
            {
                await DisplayAlert("Erro", "Usuário não está logado.", "OK");
                return;
            }

            int usuarioId = Preferences.Get("UsuarioLogadoId", -1);

            Viagem viagem = new Viagem
            {
                Destino = destino,
                DataIda = dataIda,
                DataVolta = dataVolta,
                Motivo = motivo,
                Transporte = transporte,
                Estadia = estadia,
                CodigoPassagem = codigoPassagem,
                CodigoReserva = codigoReserva,
                AeroportoIda = aeroportoIda,
                AeroportoChegada = aeroportoChegada,
                CiaAerea = ciaAerea,
                UsuarioId = usuarioId // Associe a viagem ao usuário logado
            };

            conexao.Insert(viagem);

            // Adicionar itinerários
            foreach (var child in ItineraryStackLayout.Children)
            {
                if (child is StackLayout layout)
                {
                    var titulo = ((Entry)layout.Children[0]).Text;
                    var data = ((DatePicker)layout.Children[1]).Date;
                    var hora = ((TimePicker)layout.Children[2]).Time;
                    var local = ((Entry)layout.Children[3]).Text;

                    if (string.IsNullOrWhiteSpace(titulo) ||
                        string.IsNullOrWhiteSpace(local))
                    {
                        await DisplayAlert("Erro", "Por favor, preencha todos os campos do itinerário.", "OK");
                        return;
                    }

                    Itinerario itinerario = new Itinerario
                    {
                        ViagemId = viagem.Id,
                        Titulo = titulo,
                        Data = data,
                        Hora = hora,
                        Local = local
                    };

                    conexao.Insert(itinerario);
                }
            }

            await DisplayAlert("Sucesso", "Viagem cadastrada com sucesso!", "OK");
            await Navigation.PopAsync(); // Voltar para a página principal
        }

        private async void OnCodigoTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            if (!string.IsNullOrEmpty(entry.Text) && !int.TryParse(entry.Text, out _))
            {
                await DisplayAlert("Erro", "Este campo aceita apenas números.", "OK");
                entry.Text = string.Empty;
            }
        }
    }
}

