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
            string motivo = PickerMotivo.SelectedItem.ToString();
            string transporte = PickerTransporte.SelectedItem.ToString();
            string estadia = TxtEstadia.Text;
            string codigoPassagem = TxtCodigoPassagem.Text;
            string codigoReserva = TxtCodigoReserva.Text;

            string aeroportoIda = null;
            string aeroportoChegada = null;
            string ciaAerea = null;

            if (transporte == "Avião")
            {
                aeroportoIda = TxtAeroportoIda.Text;
                aeroportoChegada = TxtAeroportoChegada.Text;
                ciaAerea = TxtCiaAerea.Text;
            }

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
                CiaAerea = ciaAerea
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
    }
}
