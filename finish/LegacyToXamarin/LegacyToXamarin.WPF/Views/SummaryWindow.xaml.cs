using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace LegacyToXamarin.WPF.Views
{
    using System.Windows.Controls;

    using WebClient.Core;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Person> PeopleList { get; set; } = new ObservableCollection<Person>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = PeopleList;
        }

        /// <summary>
        /// Windowが初期化されるときに呼ばれる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await UpdatePersonList();
        }

        /// <summary>
        /// リストの内容を表示する
        /// </summary>
        async Task UpdatePersonList()
        {
            var webPeople = await WebApiClient.Instance.GetPeopleAsync();

            PeopleList.Clear();
            foreach (var person in webPeople)
            {
                PeopleList.Add(person);
            }
        }

        /// <summary>
        /// 追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAddClick(object sender, RoutedEventArgs e)
        {
            if (new DetailWindow(null).ShowDialog() == true)
            {
                await UpdatePersonList();
            }
        }

        /// <summary>
        /// 全データを削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAllDataClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("データをすべて削除します\nよろしいですか？", "確認", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var people = await WebApiClient.Instance.GetPeopleAsync();
                foreach (var person in people)
                {
                    await WebApiClient.Instance.DeletePersonAsync(person);
                }

                await UpdatePersonList();
            }
        }

        /// <summary>
        /// リストを選択したら編集画面を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count>0)
            {
                var selectPerson = e.AddedItems[0] as Person;
                if (new DetailWindow(selectPerson).ShowDialog()==true)
                {
                    await UpdatePersonList();
                }
            }
        }
    }
}
