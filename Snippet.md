# 張り付け用のスニペットです。

## Windows Forms

Windows Forms プロジェクト用

### Models/Person.cs

```csharp
// Personクラス内

[JsonProperty(PropertyName = "id")]
public int Id { get; set; }
[JsonProperty(PropertyName = "name")]
public string Name { get; set; }
[JsonProperty(PropertyName = "birthday")]
public DateTimeOffset Birthday { get; set; }

/// <summary>
/// ListBoxに表示される文字列
/// </summary>
/// <returns></returns>
public override string ToString()
{
    return $"{Id}:{Name} {Birthday.Year}/{Birthday.Month}/{Birthday.Day}";
}
```

### Models/WebClient.cs

```csharp
class AuthResult
{
    [JsonProperty(PropertyName = "access_token")]
    public string AccessToken { get; set; }
}
```


```csharp
public static WebApiClient Instance { get; set; } = new WebApiClient();

private Uri baseAddress = ApiKeys.BaseAddress;
private string Token = "";
private object locker = new object();

private readonly string _name = "admin";
private readonly string _password = "p@ssw0rd";

private WebApiClient()
{
}
```


```csharp
private void Initialize(string name, string password)
{
    lock (locker)
    {
        if (string.IsNullOrEmpty(Token))
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = baseAddress;

                    var authContent = new StringContent($"grant_type=password&username={name}&password={password}");
                    authContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    var authResponse = client.PostAsync("/Token", authContent).Result;
                    authResponse.EnsureSuccessStatusCode();
                    var authResult = authResponse.Content.ReadAsStringAsync().Result;

                    Token = JsonConvert.DeserializeObject<AuthResult>(authResult).AccessToken;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"【InitializeError】{ex.Source},{ex.Message},{ex.InnerException}");
            }
        }
    }
}
```


```csharp
public async Task<ObservableCollection<Person>> GetPeopleAsync()
{
    Initialize(_name, _password);

    using (var client = new HttpClient())
    {
        client.BaseAddress = baseAddress;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {
            var response = await client.GetAsync("api/People");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ObservableCollection<Person>>(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"【GetError】{ex.Source},{ex.Message},{ex.InnerException}");

            return null;
        }
    }
}
```


```csharp
public async Task<int> PostPersonAsync(Person person)
{
    Initialize(_name, _password);

    using (var client = new HttpClient())
    {
        client.BaseAddress = baseAddress;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(person));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync("api/People", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var id = JsonConvert.DeserializeObject<Person>(result).Id;

            return id;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"【PostError】{ex.Source},{ex.Message},{ex.InnerException}");
            throw;
        }
    }
}
```


```csharp
public async Task<bool> UpdatePersonAsync(Person person)
{
    Initialize(_name, _password);

    using (var client = new HttpClient())
    {
        client.BaseAddress = baseAddress;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(person));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync($"api/People/{person.Id}", content);
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"【UpdateError】{ex.Source},{ex.Message},{ex.InnerException}");

            return false;
        }
    }
}
```


```csharp
public async Task<bool> DeletePersonAsync(Person person)
{
    Initialize(_name, _password);

    using (var client = new HttpClient())
    {
        client.BaseAddress = baseAddress;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        try
        {
            var response = await client.DeleteAsync($"api/People/{person.Id}");
            response.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"【DeleteError】{ex.Source},{ex.Message},{ex.InnerException}");

            return false;
        }
    }
}
```



### Views/SummaryForm

<img src="images/WinForms001.png" width="750" />

- ListBox(peopleList)
    - Name: peopleList
    - Dock: Fill
- TableLayoutPanel
    - Dock: Bottom
    - ColumnCount: 2
- Button(btnAdd／btnDelete)
    - Dock: Fill

#### Load Event

```csharp
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
```


```csharp
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
```


#### ListBox (SelectedIndexChanged)

```csharp
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
```


#### Button(Click イベント)

```csharp
/// <summary>
/// 新規追加ボタン
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private async void btnAdd_Click(object sender, EventArgs e)
{
    if (new DetailForm(null).ShowDialog() == DialogResult.OK)
    {
        await UpdatePersonList();
    }
}
```


```csharp
/// <summary>
/// データをすべて削除する
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private async void btnDelete_Click(object sender, EventArgs e)
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
```


### Views/DetailForm

<img src="images/WinForms002.png" width="750">

- Label
- TextBox (edtId／edtName／dateTimeBirthday)
- Button (btnSave／btnDelete)


#### コンストラクター

```csharp
private Person Current { get; set; }

public DetailForm(Person person)
{
    InitializeComponent();
    Current = person;
}
```


#### Load イベント

```csharp
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
```


#### Button (Click イベント)

```csharp
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
```


```csharp
/// <summary>
/// 削除する
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private async void btnDelete_Click(object sender, EventArgs e)
{
    await WebApiClient.Instance.DeletePersonAsync(Current);
    DialogResult = DialogResult.OK;
}
```




## WPF

WPF 用




