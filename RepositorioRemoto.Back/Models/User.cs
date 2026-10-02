namespace RepositorioRemoto.Back.Models;

public record User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Website { get; set; } = null!;
    public Company Company { get; set; } = null!;
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
    public bool IsDeleted { get; set; }

    // 1. Constructor sin parámetros obligatorio para EF Core
    public User() { }

    // 2. Constructor explícito opcional (si tu código/tests lo usan directamente)
    public User(
        int id, 
        string name, 
        string userName, 
        string email, 
        Address address, 
        string phone, 
        string website, 
        Company company, 
        DateTime createAt, 
        DateTime updateAt, 
        DateTime? deleteAt = null, 
        bool isDeleted = false)
    {
        Id = id;
        Name = name;
        UserName = userName;
        Email = email;
        Address = address;
        Phone = phone;
        Website = website;
        Company = company;
        CreateAt = createAt;
        UpdateAt = updateAt;
        DeleteAt = deleteAt;
        IsDeleted = isDeleted;
    }
}