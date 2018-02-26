using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using LegacyToXamarin.WinForms.Models;

namespace LegacyToXamarin.WinForms.Views
{

    public partial class SummaryForm : Form
    {
        //BindingList<PersonState> peopleDataList = new BindingList<PersonState>();

        public SummaryForm()
        {
            InitializeComponent();

            // Windows FormsのDataBinding
            //peopleList.DataSource = peopleDataList;
            //peopleList.DisplayMember = "DisplayString";
            //peopleList.ValueMember = "PersonValue";
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

            // Windows FormsのDataBinding
            //peopleList.Items.Clear();
            //foreach (var person in webPeople)
            //{
            //    peopleDataList.Add(
            //        new PersonState
            //        {
            //            Id = person.Id,
            //            Name = person.Name,
            //            Birthday = person.Birthday
            //        });
            //}

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

    }
}
