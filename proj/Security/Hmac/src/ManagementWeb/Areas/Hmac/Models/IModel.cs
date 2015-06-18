namespace ManagementWeb.Areas.Hmac.Models
{
    public interface IModel<TKey>
    {
        TKey Id { get; set; }
    }
}