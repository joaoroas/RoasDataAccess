using Dapper;
using Microsoft.Data.SqlClient;
using RoasDataAccess.Models;

const string connectionString = "Server=localhost;Database=Roas;Trusted_Connection=True;Encrypt=False";

using (var connection = new SqlConnection(connectionString))
{
    // UTILIZANDO DAPPER
    //CreateManyCategory(connection);
    //DeleteCategory(connection);
    //ListCategories(connection);
    //CreateCategory(connection);
    //UpdateCategory(connection);


    static void ListCategories(SqlConnection connection)
    {
        var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
        foreach (var item in categories)
        {
            Console.WriteLine($"{item.Id} - {item.Title}");
        }
    }

    static void CreateCategory(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços da AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;


        var insertSql = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

        var rows = connection.Execute(insertSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        });
        Console.WriteLine($"{rows} Linhas inseridas");
    }

    static void UpdateCategory(SqlConnection connection)
    {
        var updateQuery = "UPDATE [Category] SET [Title]=@Title WHERE [Id]=@Id";

        var rows = connection.Execute(updateQuery, new
        {
            Id = "af3407aa-11ae-4621-a2ef-2028b85507c4",
            Title = "Frontend"
        });
        Console.WriteLine($"{rows} Linhas alteradas");
    }

    static void DeleteCategory(SqlConnection connection)
    {
        var deleteQuery = "DELETE FROM [Category] WHERE [Id]=@Id";
        var rows = connection.Execute(deleteQuery, new
        {
            Id = "1bc15f6f-c052-4d52-8c9d-d844b808780a"
        });
        Console.WriteLine($"{rows} Registros deletados.");
    }

    static void CreateManyCategory(SqlConnection connection)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Title = "Amazon AWS";
        category.Url = "amazon";
        category.Description = "Categoria destinada a serviços da AWS";
        category.Order = 8;
        category.Summary = "AWS Cloud";
        category.Featured = false;

        var category2 = new Category();
        category2.Id = Guid.NewGuid();
        category2.Title = "Categoria nova";
        category2.Url = "categoria-nova";
        category2.Description = "Categoria nova";
        category2.Order = 9;
        category2.Summary = "Categoria";
        category2.Featured = true;


        var insertSql = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

        var rows = connection.Execute(insertSql, new[] {
            new
            {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
            },
            new
            {
            category2.Id,
            category2.Title,
            category2.Url,
            category2.Summary,
            category2.Order,
            category2.Description,
            category2.Featured
            }
        });
        Console.WriteLine($"{rows} Linhas inseridas");
    }
}
// UTILIZANDO ADO.NET
/* Console.WriteLine("Conectado ao banco");
connection.Open();

using (var command = new SqlCommand())
{
    command.Connection = connection;
    command.CommandType = System.Data.CommandType.Text;
    command.CommandText = "SELECT [Id], [Title] FROM [Category]";

    var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
    }

} */
