public class Product
{
    public string Id { get; set; }
    public string Name { get; set; } // Tên hiển thị trên ComboBox
    public string Description { get; set; }
    public int Quantity { get; set; } // hoặc kiểu khác nếu cần
    public double Price { get; set; } // hoặc kiểu khác nếu cần


    public override string ToString()
    {
        return Name; // Hiển thị tên trong ComboBox
    }
}