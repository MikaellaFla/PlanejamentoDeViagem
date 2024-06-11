using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private void CarregarDados()
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

        private void OnPickerTransporteSelectedIndexChanged(object sender, EventArgs e)
        {
            AeroportoInfo.IsVisible = PickerTransporte.SelectedItem.ToString() == "Avião";
        }


        private async void OnSalvarViagemClicked(object sender, EventArgs e)
        {
            viagemAtual.Destino = TxtDestino.Text;
            viagemAtual.DataIda = DataIda.Date;
            viagemAtual.DataVolta = DataVolta.Date;
            viagemAtual.Motivo = PickerMotivo.SelectedItem.ToString();
            viagemAtual.Transporte = PickerTransporte.SelectedItem.ToString();
            viagemAtual.Estadia = TxtEstadia.Text;
            viagemAtual.CodigoPassagem = TxtCodigoPassagem.Text;
            viagemAtual.CodigoReserva = TxtCodigoReserva.Text;
            viagemAtual.AeroportoIda = PickerTransporte.SelectedItem.ToString() == "Avião" ? TxtAeroportoIda.Text : null;
            viagemAtual.AeroportoChegada = PickerTransporte.SelectedItem.ToString() == "Avião" ? TxtAeroportoChegada.Text : null;
            viagemAtual.CiaAerea = PickerTransporte.SelectedItem.ToString() == "Avião" ? TxtCiaAerea.Text : null;

            conexao.Update(viagemAtual);

            await DisplayAlert("Sucesso", "Viagem atualizada com sucesso!", "OK");
            await Navigation.PopAsync();
        }
    }
}           
            

