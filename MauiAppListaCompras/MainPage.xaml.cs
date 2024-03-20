﻿using System.Collections.ObjectModel;
using MauiAppListaCompras.Models;

namespace MauiAppListaCompras
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Produto> lista_produtos = 
            new ObservableCollection<Produto>();

        public MainPage()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista_produtos;
        }

        private void ToolbarItem_Clicked_Somar(object sender, EventArgs e)
        {
            double soma = lista_produtos.Sum(i => (i.Preco * i.Quantidade));
            string msg = $"O total é {soma:C}";
            DisplayAlert("Somatória", msg, "Fechar");
        }

        protected override void OnAppearing()
        {
            if (lista_produtos.Count == 0)
            {
                Task.Run(async () =>
                {
                    List<Produto> tmp = await App.Db.GetAll();
                    foreach (Produto p in tmp)
                    {
                        lista_produtos.Add(p);
                    }
                }); // Fecha Task
            } // Fecha if
        }

        private async void ToolbarItem_Clicked_Add(object sender, EventArgs e)
        {
            // await Shell.Current.GoToAsync("//NovoProduto");
            await Navigation.PushAsync(new Views.NovoProduto());
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue;
            lista_produtos.Clear();
            Task.Run(async() =>
            {
                List<Produto> tmp = await App.Db.Search(q);
                foreach (Produto p in tmp)
                {
                    lista_produtos.Add(p);
                }
            }); // Fecha Task
        } // Fecha método do evento

        private void ref_carregando_Refreshing(object sender, EventArgs e)
        {
            lista_produtos.Clear();
            Task.Run(async () =>
            {
                List<Produto> tmp = await App.Db.GetAll();
                foreach(Produto p in tmp)
                {
                    lista_produtos.Add(p);
                }

            }); // Fecha Task
            ref_carregando.IsRefreshing = false;
        } // Fecha método do Refreshing

        private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Produto? p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p
            });
        }

        private async void MenuItem_Clicked_Remover(object sender, EventArgs e)
        {
            try
            {
                MenuItem selecionado = (MenuItem)sender;
                
                Produto p = selecionado.BindingContext as Produto;
                
                bool confirm = await DisplayAlert(
                    "Tem certeza?", "Remover Produto?", 
                    "Sim", "Cancelar");

                if (confirm)
                {
                    await App.Db.Delete(p.Id);
                    await DisplayAlert("Sucesso!",
                        "Produto Removido", "OK");
                }

            } catch(Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    } // Fecha classe
}
