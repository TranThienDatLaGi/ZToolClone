using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZToolClone
{
    public partial class ZToolClone : Form
    {
        public ZToolClone()
        {
            InitializeComponent();
        }
        private List<Product> products = new List<Product>();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void buttonLoad_Click(object sender, EventArgs e)
        {
            // Lấy giá trị từ ComboBox và TextBox
            string selectedWebsite = comboBoxWebsite.SelectedItem?.ToString();
            string apiKey = textBoxAPI.Text.Trim();

            // Kiểm tra nếu không có API Key, hiển thị thông báo lỗi
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API Key!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xây dựng URL API dựa trên website được chọn
            string web = "";
            switch (selectedWebsite)
            {
                case "Clone8":
                    web = "clone8.com";
                    break;
                case "N2K":
                    web = "nguyenlieu68.com";
                    break;
                case "Clone30p":
                    web = "clone30p.com";
                    break;
                case "Thiên Phát":
                    web = "thienphatclone.com";
                    break;
                case "Khương FB":
                    web = "khuongfb.com";
                    break;
                case "Clone 72H":
                    web = "clone72h.com";
                    break;
                case "Clone 247":
                    web = "clone247.vn";
                    break;
                case "Full Clone":
                    web = "fullclone.net";
                    break;
                case "Clone 37":
                    web = "clone37.com";
                    break;
                case "ShopClone68":
                    web = "www.shopclone68vn.com";
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn một website hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // Gửi yêu cầu GET đến API để lấy thông tin người dùng
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // Đặt thời gian timeout cho yêu cầu HTTP

                    var response = await client.GetAsync($"https://{web}/api/profile.php?api_key={apiKey}"); // Gửi yêu cầu GET bất đồng bộ

                    if (response.IsSuccessStatusCode)
                    {
                        // Lấy dữ liệu trả về từ API
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Giả sử responseData là JSON với cấu trúc như bạn cung cấp
                        var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(responseData);

                        // Kiểm tra status, nếu là success thì tiếp tục xử lý
                        string status = jsonResponse["status"]?.ToString();
                        if (status == "success")
                        {
                            string username = jsonResponse["data"]?["username"]?.ToString();
                            string money = jsonResponse["data"]?["money"]?.ToString();

                            labelUsername.Text = "Username: " + username;
                            labelUsername.ForeColor = Color.MediumBlue;

                            labelMoney.Text = "Money: " + money + " VND";
                            labelMoney.ForeColor = Color.OrangeRed;

                            // Gửi yêu cầu GET đến API để lấy danh sách sản phẩm
                            try
                            {
                                var responseproduct = await client.GetAsync($"https://{web}/api/products.php?api_key={apiKey}");

                                if (responseproduct.IsSuccessStatusCode)
                                {
                                    // Lấy dữ liệu trả về từ API
                                    string responseDataProduct = await responseproduct.Content.ReadAsStringAsync();

                                    // Giả sử responseData là JSON với cấu trúc như bạn cung cấp
                                    var jsonResponseProduct = Newtonsoft.Json.Linq.JObject.Parse(responseDataProduct);

                                    // Kiểm tra status, nếu là success thì tiếp tục xử lý
                                    string productStatus = jsonResponseProduct["status"]?.ToString();
                                    if (productStatus == "success")
                                    {
                                        var categories = jsonResponseProduct["categories"];
                                        products.Clear(); // Xóa danh sách cũ
                                        comboBoxProductType.Items.Clear();

                                        foreach (var category in categories)
                                        {
                                            var productsInCategory = category["products"];
                                            foreach (var product in productsInCategory)
                                            {
                                                string productName = product["name"]?.ToString();
                                                string IDproduct = product["id"]?.ToString();
                                                string description = product["description"]?.ToString();
                                                string amount = product["amount"]?.ToString();
                                                string price = product["price"]?.ToString();

                                                if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(IDproduct))
                                                {
                                                    var prod = new Product
                                                    {
                                                        Id = IDproduct,
                                                        Name = productName,
                                                        Description = description,
                                                        Quantity = int.TryParse(amount, out int qty) ? qty : 0,
                                                        Price = double.TryParse(price, out double p) ? p : 0
                                                    };

                                                    products.Add(prod);
                                                    comboBoxProductType.Items.Add(prod); // Thêm object Product vào ComboBox
                                                }
                                            }
                                        }
                                        // Cập nhật ComboBox với sản phẩm
                                        comboBoxProductType.SelectedIndex = 0; // Chọn sản phẩm đầu tiên trong ComboBox (nếu có)
                                        MessageBox.Show("Load thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        // Nếu API trả về status không phải "success"
                                        MessageBox.Show("Dữ liệu không hợp lệ từ API.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    // Nếu có lỗi trong yêu cầu (ví dụ: API không phản hồi)
                                    MessageBox.Show("Lỗi khi lấy dữ liệu từ API. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Xử lý lỗi trong quá trình gửi yêu cầu HTTP (ví dụ: không có kết nối mạng)
                                MessageBox.Show("Đã xảy ra lỗi khi kết nối đến API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // Nếu API trả về status không phải "success"
                            MessageBox.Show("Dữ liệu không hợp lệ từ API.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // Nếu có lỗi trong yêu cầu (ví dụ: API không phản hồi)
                        MessageBox.Show("Lỗi khi lấy dữ liệu từ API. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi trong quá trình gửi yêu cầu HTTP (ví dụ: không có kết nối mạng)
                MessageBox.Show("Đã xảy ra lỗi khi kết nối đến API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void comboBoxIProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lấy sản phẩm được chọn từ comboBox
            var selectedProduct = comboBoxProductType.SelectedItem as Product;

            if (selectedProduct != null)
            {
                // Cập nhật các trường TextBox với thông tin của sản phẩm
                textBoxID.Text = selectedProduct.Id;
                textBoxDescription.Text = selectedProduct.Description;
                textBoxAmount.Text = selectedProduct.Quantity.ToString();
                textBoxPrice.Text = selectedProduct.Price.ToString("N0") + " VND";

                // Nếu bạn muốn thực hiện một hành động bất đồng bộ, có thể gọi API hoặc thực hiện công việc khác
                try
                {
                    // Ví dụ: Gửi một yêu cầu API để lấy thông tin chi tiết cho sản phẩm
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        var response = await client.GetAsync($"https://yourapi.com/api/productdetails?id={selectedProduct.Id}");

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            // Xử lý dữ liệu nhận về từ API (nếu cần)
                            // MessageBox.Show("Đã lấy thông tin chi tiết cho sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không thể lấy thông tin chi tiết sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có vấn đề với việc kết nối hoặc yêu cầu API
                    MessageBox.Show("Đã xảy ra lỗi khi lấy thông tin chi tiết sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        CancellationTokenSource cancellationTokenSource;
        private async void buttonStart_Click(object sender, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            btnStop.Enabled = true;
            // Lấy giá trị từ ComboBox và TextBox
            string selectedWebsite = comboBoxWebsite.SelectedItem?.ToString();
            string apiKey = textBoxAPI.Text.Trim();
            string productId = textBoxID.Text.Trim();
            int quantity = (int)numericQuantity.Value;
            int requestCount = (int)numericEnoughAmount.Value;  // Số lần request thành công cần có

            // Kiểm tra nếu không có API Key, hiển thị thông báo lỗi
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("Vui lòng nhập API Key!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(productId))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (quantity > requestCount)
            {
                MessageBox.Show("Vui lòng nhập số lần mua nhỏ hơn mức dừng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy đường dẫn lưu file
            string savePath = textBoxPath.Text.Trim();
            if (string.IsNullOrEmpty(savePath))
            {
                MessageBox.Show("Vui lòng nhập đường dẫn lưu file!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Xây dựng URL API dựa trên website được chọn
            string web = "";
            switch (selectedWebsite)
            {
                case "Clone8":
                    web = "clone8.com";
                    break;
                case "N2K":
                    web = "nguyenlieu68.com";
                    break;
                case "Clone30p":
                    web = "clone30p.com";
                    break;
                case "Thiên Phát":
                    web = "thienphatclone.com";
                    break;
                case "Khương FB":
                    web = "khuongfb.com";
                    break;
                case "Clone 72H":
                    web = "clone72h.com";
                    break;
                case "Clone 247":
                    web = "clone247.vn";
                    break;
                case "Full Clone":
                    web = "fullclone.net";
                    break;
                case "Clone 37":
                    web = "clone37.com";
                    break;
                case "ShopClone68":
                    web = "www.shopclone68vn.com";
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn một website hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // Xây dựng body form-data cho yêu cầu POST
            var formData = new MultipartFormDataContent
                {
                    { new StringContent("buyProduct"), "action" },
                    { new StringContent(productId), "id" },
                    { new StringContent(quantity.ToString()), "amount" },
                    { new StringContent(apiKey), "api_key" }
                };

            int successfulRequests = 0;
            int failedRequests = 0;

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    while (successfulRequests < requestCount)  // Lặp lại cho đến khi có đủ số lần success
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            break;  // Nếu đã hủy, thoát khỏi vòng lặp
                            return;
                        }
                        var response = await client.PostAsync($"https://{web}/api/buy_product", formData);

                        // Kiểm tra giá trị trong textBoxRequest trước khi chuyển đổi
                        int currentRequestCount = 0;
                        if (!int.TryParse(textBoxRequest.Text, out currentRequestCount))
                        {
                            currentRequestCount = 0;  // Nếu không thể chuyển đổi, gán giá trị mặc định là 0
                        }
                        textBoxRequest.Text = (currentRequestCount + 1).ToString();  // Cập nhật số lần request

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                            string status = jsonResponse["status"]?.ToString();

                            if (status == "success")
                            {
                                successfulRequests = successfulRequests + quantity;
                                int currentSuccessCount = 0;
                                if (!int.TryParse(textBoxGetOK.Text, out currentSuccessCount))
                                {
                                    currentSuccessCount = 0;  // Nếu không thể chuyển đổi, gán giá trị mặc định là 0
                                }
                                textBoxGetOK.Text = (currentSuccessCount + quantity).ToString();  // Cập nhật số lượng successful

                                // Ghi dữ liệu vào file
                                var dataArray = jsonResponse["data"]?.ToObject<System.Collections.Generic.List<string>>();
                                if (dataArray != null && dataArray.Count > 0)
                                {
                                    try
                                    {
                                        using (var writer = new System.IO.StreamWriter(savePath, true))
                                        {
                                            foreach (var item in dataArray)
                                            {
                                                writer.WriteLine(item);
                                            }
                                        }
                                        // MessageBox.Show("Dữ liệu đã được lưu vào file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        var response2 = await client.GetAsync($"https://{web}/api/profile.php?api_key={apiKey}");

                                        if (response2.IsSuccessStatusCode)
                                        {
                                            // Lấy dữ liệu trả về từ API
                                            string responseData2 = await response2.Content.ReadAsStringAsync();

                                            var jsonResponse2 = Newtonsoft.Json.Linq.JObject.Parse(responseData2);

                                            // Kiểm tra status, nếu là success thì tiếp tục xử lý
                                            string status2 = jsonResponse2["status"]?.ToString();
                                            if (status2 == "success")
                                            {
                                                string username = jsonResponse2["data"]?["username"]?.ToString();
                                                string money = jsonResponse2["data"]?["money"]?.ToString();

                                                labelUsername.Text = "Username: " + username;
                                                labelUsername.ForeColor = Color.MediumBlue;


                                                labelMoney.Text = "Money: " + money + " VND";
                                                labelMoney.ForeColor = Color.OrangeRed;
                                            }
                                        }
                                        else
                                        {
                                            // Nếu có lỗi trong yêu cầu (ví dụ: API không phản hồi)
                                            MessageBox.Show("Lỗi khi lấy dữ liệu từ API. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Đã xảy ra lỗi khi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                failedRequests++;
                                int currentErrorCount = 0;
                                if (!int.TryParse(textBoxError.Text, out currentErrorCount))
                                {
                                    currentErrorCount = 0;  // Nếu không thể chuyển đổi, gán giá trị mặc định là 0
                                }
                                textBoxError.Text = (currentErrorCount + 1).ToString();  // Cập nhật số lỗi
                            }
                        }
                        else
                        {
                            failedRequests++;
                            int currentErrorCount = 0;
                            if (!int.TryParse(textBoxError.Text, out currentErrorCount))
                            {
                                currentErrorCount = 0;  // Nếu không thể chuyển đổi, gán giá trị mặc định là 0
                            }
                            textBoxError.Text = (currentErrorCount + 1).ToString();  // Cập nhật số lỗi
                        }

                        // Chờ sau mỗi request theo thời gian delay
                    }

                    MessageBox.Show($"Hoàn thành với {successfulRequests} lần thành công và {failedRequests} lỗi.", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    try
                    {
                        var responseproduct = await client.GetAsync($"https://{web}/api/products.php?api_key={apiKey}");

                        if (responseproduct.IsSuccessStatusCode)
                        {
                            // Lấy dữ liệu trả về từ API
                            string responseDataProduct = await responseproduct.Content.ReadAsStringAsync();

                            var jsonResponseProduct = Newtonsoft.Json.Linq.JObject.Parse(responseDataProduct);

                            // Kiểm tra status, nếu là success thì tiếp tục xử lý
                            string productStatus = jsonResponseProduct["status"]?.ToString();
                            if (productStatus == "success")
                            {
                                if (cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    return;
                                }
                                var categories = jsonResponseProduct["categories"];
                                products.Clear(); // Xóa danh sách cũ
                                comboBoxProductType.Items.Clear();

                                foreach (var category in categories)
                                {
                                    var productsInCategory = category["products"];
                                    foreach (var product in productsInCategory)
                                    {
                                        string productName = product["name"]?.ToString();
                                        string IDproduct = product["id"]?.ToString();
                                        string description = product["description"]?.ToString();
                                        string amount = product["amount"]?.ToString();
                                        string price = product["price"]?.ToString();

                                        if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(IDproduct))
                                        {
                                            var prod = new Product
                                            {
                                                Id = IDproduct,
                                                Name = productName,
                                                Description = description,
                                                Quantity = int.TryParse(amount, out int qty) ? qty : 0,
                                                Price = double.TryParse(price, out double p) ? p : 0
                                            };

                                            products.Add(prod);
                                            comboBoxProductType.Items.Add(prod); // Thêm object Product vào ComboBox
                                        }
                                    }
                                }

                                // Cập nhật ComboBox với sản phẩm
                                comboBoxProductType.SelectedIndex = 0; // Chọn sản phẩm đầu tiên trong ComboBox (nếu có)
                                                                       // Đặt lại giá trị cho các NumericUpDown
                                numericQuantity.Value = 0;
                                numericEnoughAmount.Value = 0;

                                // Đặt lại giá trị cho các TextBox
                                textBoxEstimate.Text = "";
                                textBoxRequest.Text = "";
                                textBoxGetOK.Text = "";
                                textBoxError.Text = "";


                                MessageBox.Show("Load thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Nếu API trả về status không phải "success"
                                MessageBox.Show("Dữ liệu không hợp lệ từ API.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // Nếu có lỗi trong yêu cầu (ví dụ: API không phản hồi)
                            MessageBox.Show("Lỗi khi lấy dữ liệu từ API. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi trong quá trình gửi yêu cầu HTTP (ví dụ: không có kết nối mạng)
                        MessageBox.Show("Đã xảy ra lỗi khi kết nối đến API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi kết nối đến API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void buttonStop_Click(object sender, EventArgs e)
        {
            // Hủy yêu cầu đang chạy bằng cách gọi Cancel trên CancellationTokenSource
            cancellationTokenSource.Cancel();

            // Vô hiệu hóa nút startButton sau khi nhấn nút stop
            btnStart.Enabled = false;
            btnStop.Enabled = false;

            // Hiển thị thông báo
            MessageBox.Show("Hoạt động đã bị dừng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnStart.Enabled = true;
            btnStop.Enabled = true;

        }


        private void numericEnoughAmount_ValueChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu textBoxPrice không có giá trị hợp lệ
            if (textBoxAPI.Text.Trim() != "")
            {
                if (string.IsNullOrEmpty(textBoxPrice.Text))
                {
                    MessageBox.Show("Bạn phải chọn 1 loại sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy giá trị từ textBoxPrice và numericEnoughAmount
                double price = 0;
                if (!double.TryParse(textBoxPrice.Text.Replace(" VND", "").Replace(",", ""), out price))
                {
                    MessageBox.Show("Giá trị không hợp lệ trong textBoxPrice", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tính toán và gán giá trị vào textBoxEstimate
                double amount = (double)numericEnoughAmount.Value;
                double estimate = price * amount;

                // Cập nhật giá trị vào textBoxEstimate
                textBoxEstimate.Text = estimate.ToString("N0") + " VND"; // Định dạng với dấu phẩy ngăn cách phần nghìn 
            }

        }

        private void buttonPath_Click(object sender, EventArgs e)
        {
            // Mở cửa sổ chọn tệp (OpenFileDialog) chỉ cho phép chọn tệp TXT
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt"; // Chỉ cho phép chọn file .txt
                openFileDialog.Title = "Chọn tệp TXT";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Gán đường dẫn tệp vào TextBox
                    textBoxPath.Text = openFileDialog.FileName;
                }
            }
        }

    }


}
