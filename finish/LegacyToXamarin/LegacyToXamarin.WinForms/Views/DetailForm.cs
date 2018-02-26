using LegacyToXamarin.WinForms.Models;
using System;
using System.Windows.Forms;

namespace LegacyToXamarin.WinForms.Views
{


    public partial class DetailForm : Form
    {
        private Person Current { get; set; }

        public DetailForm(Person person)
        {
            InitializeComponent();
            Current = person;
        }

        /// <summary>
        /// フォームの初期化時に値をセットする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailForm_Load(object sender, EventArgs e)
        {
            edtId.Enabled = false;
            if (Current != null)
            {
                edtId.Text = Current.Id.ToString();
                edtName.Text = Current.Name;
                dateTimeBirthday.Value = Current.Birthday.DateTime;
            }
            else
            {
                edtId.Text = "0";
                btnDelete.Enabled = false;
            }
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSave_Click(object sender, EventArgs e)
        {
            var newPerson = new Person
            {
                Name = edtName.Text,
                Birthday = dateTimeBirthday.Value
            };
            if (Current==null)
            {
                // 新規追加
                await WebApiClient.Instance.PostPersonAsync(newPerson);
            }
            else
            {
                // 更新
                await WebApiClient.Instance.UpdatePersonAsync(newPerson);
            }

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 削除する TEST issue2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await WebApiClient.Instance.DeletePersonAsync(Current);
            DialogResult = DialogResult.OK;
        }

    }
}
