using System.ComponentModel;

public class ReceiptItem : INotifyPropertyChanged
{
    private int _quantity;

    public string Brand { get; set; }
    public string Size { get; set; }
    public double Price { get; set; }
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Total));
            }
        }
    }
    public double Total => Price * Quantity;

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
