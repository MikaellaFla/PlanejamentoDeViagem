using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlanejamentoDeViagem
{
    public partial class EditarViagemPage : ContentPage
    {
        string caminhoBD;  //caminho do banco
        SQLiteConnection conexao;
        public Viagem viagemAtual;

        public EditarViagemPage(Viagem viagem)
        {
            InitializeComponent();
            caminhoBD = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "viagem.db3");

            conexao = new SQLiteConnection(caminhoBD);
            viagemAtual = viagem;

            CarregarDados();
        }

        private void CarregarDados() // Método que carrega todos os dados da viagem a ser editada
        {
            TxtDestino.Text = viagemAtual.Destino;
            DataIda.Date = viagemAtual.DataIda;
            DataVolta.Date = viagemAtual.DataVolta;
            PickerMotivo.SelectedItem = viagemAtual.Motivo;
            PickerTransporte.SelectedItem = viagemAtual.Transporte;
            TxtEstadia.Text = viagemAtual.Estadia;
            TxtCodigoPassagem.Text = viagemAtual.CodigoPassagem;
            TxtCodigoReserva.Text = viagemAtual.CodigoReserva;
            TxtAeroportoIda.Text = viagemAtual.AeroportoIda;
            TxtAeroportoChegada.Text = viagemAtual.AeroportoChegada;
            TxtCiaAerea.Text = viagemAtual.CiaAerea;
            AeroportoInfo.IsVisible = viagemAtual.Transporte == "Avião";
        }

        private void OnPickerTransporteSelectedIndexChanged(object sender, EventArgs e) // Método que torna visível os campos de informações do avião e
        {
            AeroportoInfo.IsVisible = PickerTransporte.SelectedItem.ToString() == "Avião";
        }

        private async void OnSalvarViagemClicked(object sender, EventArgs e)
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

            if (!Regex.IsMatch(codigoPassagem, @"^\d+$"))
            {
                await DisplayAlert("Erro", "O código da passagem deve conter apenas números.", "OK");
                return;
            }

            if (!Regex.IsMatch(codigoReserva, @"^\d+$"))
            {
                await DisplayAlert("Erro", "O código da reserva deve conter apenas números.", "OK");
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

            int usuarioId = Preferences.Get("UsuarioLogadoId", -1);

            viagemAtual.Destino = destino;
            viagemAtual.DataIda = dataIda;
            viagemAtual.DataVolta = dataVolta;
            viagemAtual.Motivo = motivo;
            viagemAtual.Transporte = transporte;
            viagemAtual.Estadia = estadia;
            viagemAtual.CodigoPassagem = codigoPassagem;
            viagemAtual.CodigoReserva = codigoReserva;
            viagemAtual.AeroportoIda = aeroportoIda;
            viagemAtual.AeroportoChegada = aeroportoChegada;
            viagemAtual.CiaAerea = ciaAerea;
            viagemAtual.UsuarioId = usuarioId; // Associe a viagem ao usuário logado

            conexao.Update(viagemAtual);

            await DisplayAlert("Sucesso", "Viagem atualizada com sucesso!", "OK");
            await Navigation.PopAsync();
        }
    }
}




