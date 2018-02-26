using System;
using System.Windows;
using LegacyToXamarin.WPF.Models;

namespace LegacyToXamarin.WPF.Views
{
    /// <summary>
    /// DetailWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DetailWindow : Window
    {
        public DetailWindow(Person person)
        {
            InitializeComponent();

            if (person != null)
                DataContext = person;
            else
                DataContext = new Person
                {
                    Birthday = DateTime.Now
                };
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            var currentPerson = DataContext as Person;
            if (currentPerson.Id == 0)
            {
                // 新規追加
                await WebApiClient.Instance.PostPersonAsync(currentPerson);
            }
            else
            {
                // 更新
                await WebApiClient.Instance.UpdatePersonAsync(currentPerson);
            }

            DialogResult = true;
        }

        /// <summary>
        /// 削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDeleteClick(object sender, RoutedEventArgs e)
        {
            var currentPerson = DataContext as Person;
            await WebApiClient.Instance.DeletePersonAsync(currentPerson);

            DialogResult = true;
        }

        /// <summary>
        /// Windowロード時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var currentPerson = DataContext as Person;
            edtId.IsEnabled = false;
            btnDelete.IsEnabled = (currentPerson.Id == 0) ? false : true;
        }
    }
}
