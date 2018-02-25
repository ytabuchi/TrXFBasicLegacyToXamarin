using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegacyToXamarin.WinForms
{
    using LegacyToXamarin.WinForms.Views;

    using WebClient.Core;

    public partial class SummaryForm : Form
    {
        public SummaryForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームが初期化されるときに呼ばれるので
        /// ここで一覧を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SummaryForm_Load(object sender, EventArgs e)
        {
            await UpdatePersonList();
        }

        /// <summary>
        /// リストの内容を表示する
        /// </summary>
        async Task UpdatePersonList()
        {
            var webPeople = await WebApiClient.Instance.GetPeopleAsync();

            peopleList.Items.Clear();
            foreach (var person in webPeople)
            {
                peopleList.Items.Add(person);
            }
        }

        /// <summary>
        /// 新規追加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addButton_Click(object sender, EventArgs e)
        {
            if (new DetailForm(null).ShowDialog() == DialogResult.OK)
            {
                await UpdatePersonList();
            }
        }

        /// <summary>
        /// リストを選択したら編集画面を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void peopleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (peopleList.SelectedItem != null)
            {
                var selectPerson = (Person)peopleList.SelectedItem;
                if (new DetailForm(selectPerson).ShowDialog() == DialogResult.OK)
                {
                    await UpdatePersonList();
                }
            }
        }

        /// <summary>
        /// データをすべて削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void clearButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("データをすべて削除します\nよろしいですか？", "確認", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var people = await WebApiClient.Instance.GetPeopleAsync();
                foreach (var person in people)
                {
                    await WebApiClient.Instance.DeletePersonAsync(person);
                }

                await UpdatePersonList();
            }
        }

        private void peopleList_MeasureItem(object sender, MeasureItemEventArgs e)
        {

        }
    }
}
